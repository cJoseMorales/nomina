using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using nomina.Context;
using nomina.Entities;

namespace nomina.Controllers;

public class AsientoContableController : Controller
{
    private const int AuxiliarId = 2;

    private const int CrAccountId = 70;
    private const int DbAccountId = 71;

    private const string DescriptionFormat = "Asiento de Nominas correspondiente al periodo {0}";

    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

    private readonly NominaContext _context;
    private readonly HttpClient _client;
    private readonly IConfiguration _configuration;

    public AsientoContableController(NominaContext context, IConfiguration configuration, HttpClient client)
    {
        _context = context;
        _client = client;
        _configuration = configuration;

        _client.BaseAddress = new Uri("https://iso810-contabilidad.azurewebsites.net");
    }

    // GET: AsientoContable
    public async Task<IActionResult> Index()
    {
        return View(await _context.AsientoContable.ToListAsync());
    }

    // GET: AsientoContable/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var asientoContable = await _context.AsientoContable
            .FirstOrDefaultAsync(m => m.Id == id);
        if (asientoContable == null)
        {
            return NotFound();
        }

        return View(asientoContable);
    }

    // GET: AsientoContable/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: AsientoContable/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        [Bind("Id,Descripcion,Periodo,Monto,Estado")]
        AsientoContable asientoContable)
    {
        if (ModelState.IsValid)
        {
            asientoContable.Descripcion = string.Format(DescriptionFormat, asientoContable.Periodo);

            var parts = asientoContable.Periodo.Split("-");
            var year = int.Parse(parts[0]);
            var month = int.Parse(parts[1]);

            var transaccionesDelPeriodo = _context.Transaccion
                .Where(t => t.IdAsiento == null && t.Fecha.Year == year && t.Fecha.Month == month)
                .ToList();
            if (transaccionesDelPeriodo.Count == 0)
            {
                return BadRequest("No se han encontrado transacciones en el período especificado (" + year + "-" +
                                  month + ").");
            }

            asientoContable.Monto = transaccionesDelPeriodo.Sum(t => t.Monto);

            _context.Add(asientoContable);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        return View(asientoContable);
    }

    // GET: AsientoContable/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var asientoContable = await _context.AsientoContable
            .FirstOrDefaultAsync(m => m.Id == id);
        if (asientoContable == null)
        {
            return NotFound();
        }

        return View(asientoContable);
    }

    // POST: AsientoContable/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var asientoContable = await _context.AsientoContable.FindAsync(id);
        if (asientoContable != null)
        {
            _context.AsientoContable.Remove(asientoContable);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Update(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var asientoContable = await _context.AsientoContable.FindAsync(id);
        if (asientoContable == null)
        {
            return NotFound();
        }

        var parts = asientoContable.Periodo.Split("-");
        var year = int.Parse(parts[0]);
        var month = int.Parse(parts[1]);

        var transaccionesDelPeriodo = _context.Transaccion
            .Where(t => t.IdAsiento == null && t.Fecha.Year == year && t.Fecha.Month == month)
            .ToList();
        if (transaccionesDelPeriodo.Count == 0) // no transactions so there's no need to keep this entry
        {
            _context.AsientoContable.Remove(asientoContable);
            await _context.SaveChangesAsync();
            
            return RedirectToAction(nameof(Index));
        }

        asientoContable.Monto = transaccionesDelPeriodo.Sum(t => t.Monto);

        _context.AsientoContable.Update(asientoContable);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SendToAccounting(int id)
    {
        var asientoContable = await _context.AsientoContable.FindAsync(id);
        if (asientoContable == null)
        {
            return NotFound();
        }

        var parts = asientoContable.Periodo.Split("-");
        var year = int.Parse(parts[0]);
        var month = int.Parse(parts[1]);

        var total = asientoContable.Monto;
        var data = new
        {
            descripcion = string.Format(DescriptionFormat, asientoContable.Periodo),
            sistemaAuxiliarId = AuxiliarId,
            fechaAsiento = new DateOnly(year, month, 1),
            detalles = new List<object>
            {
                new
                {
                    cuentaId = CrAccountId,
                    montoAsiento = total,
                    tipoMovimiento = "CR"
                },
                new
                {
                    cuentaId = DbAccountId,
                    montoAsiento = total,
                    tipoMovimiento = "DB"
                }
            }
        };

        return await SendDataToAccounting(JsonSerializer.Serialize(
            data,
            JsonOptions
        ), []);
    }

    private async Task<IActionResult> SendDataToAccounting(string json, List<Transaccion> transacciones)
    {
        Console.WriteLine(json);

        HttpResponseMessage? response;
        try
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                _configuration["ContabilidadToken"]
            );

            response = await _client.PostAsync(
                "api/EntradaContable",
                new StringContent(
                    json,
                    Encoding.UTF8,
                    "application/json"
                )
            );
        }
        catch (Exception e)
        {
            return BadRequest("Ha ocurrido un error al enviar los datos al departamento de contabilidad. " + e);
        }

        var result = await response.Content.ReadAsStringAsync();
        if (response is not { IsSuccessStatusCode: true })
        {
            return BadRequest("Ha ocurrido un error (" + response.StatusCode +
                              ") enviando los datos de la nómina al departamento de contabilidad. " + result);
        }

        Console.WriteLine("response " + result);

        JsonDocument? document;
        try
        {
            document = JsonDocument.Parse(result);
        }
        catch (Exception e)
        {
            return BadRequest(
                "Ha ocurrido un error al intentar decodificar la respuesta de la solicitud al departamento de Contabilidad. " +
                e
            );
        }

        // todo: determine if we need to do this to our transactions as we are not the same as other groups
        // that only need to record transactions, our transactions modify employees base salary
        if (transacciones.Count != 0 && document.RootElement.TryGetProperty("id", out var element))
        {
            foreach (var t in transacciones)
            {
                t.IdAsiento = element.GetInt32();
                _context.Transaccion.Update(t);
            }
        }

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Update2(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var asientoContable = await _context.AsientoContable.FindAsync(id);
        if (asientoContable == null)
        {
            return NotFound();
        }

        asientoContable.Monto = (decimal)await CalculateTotal();

        _context.AsientoContable.Update(asientoContable);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SendToAccounting2(int id)
    {
        var asientoContable = await _context.AsientoContable.FindAsync(id);
        if (asientoContable == null)
        {
            return NotFound();
        }

        var parts = asientoContable.Periodo.Split("-");
        var year = int.Parse(parts[0]);
        var month = int.Parse(parts[1]);

        var total = asientoContable.Monto;
        var data = new
        {
            descripcion = string.Format(DescriptionFormat, asientoContable.Descripcion),
            sistemaAuxiliarId = AuxiliarId,
            fechaAsiento = new DateOnly(year, month, 1),
            detalles = new List<object>
            {
                new
                {
                    cuentaId = CrAccountId,
                    montoAsiento = total,
                    tipoMovimiento = "CR"
                },
                new
                {
                    cuentaId = DbAccountId,
                    montoAsiento = total,
                    tipoMovimiento = "DB"
                }
            }
        };

        return await SendDataToAccounting2(JsonSerializer.Serialize(
            data,
            JsonOptions
        ));
    }

    private async Task<double> CalculateTotal()
    {
        var empleados = await _context.Empleado
            .ToListAsync();
        if (empleados.Count == 0)
        {
            return 0;
        }

        double total = 0;
        foreach (var empleado in empleados)
        {
            var salarioBase = empleado.SalarioMensual;
            var transacciones = await _context.Transaccion
                .Where(t => t.Estado == "ACTIVO" && t.EmpleadoId == empleado.Id)
                .Include(transaccion => transaccion.TipoDeTransaccion)
                .ToListAsync();
            if (transacciones.Count == 0)
            {
                total += (double)salarioBase;
                continue;
            }

            var salario = empleado.SalarioMensual;
            foreach (var transaccion in transacciones)
            {
                var tipo = transaccion.TipoDeTransaccion;
                if (tipo?.Estado != "ACTIVO")
                {
                    continue;
                }

                var monto = NormalizeAmount(transaccion.Monto);
                var depende = tipo.DependeDeSalario;
                switch (tipo.Operacion)
                {
                    case "INGRESO":
                    {
                        if (depende)
                        {
                            salario += salarioBase * monto; // monto is a percentage as it depends on the salary
                        }
                        else
                        {
                            salario += monto;
                        }

                        break;
                    }

                    case "DEDUCCION":
                    {
                        if (depende)
                        {
                            salario -= salarioBase * monto;
                        }
                        else
                        {
                            salario -= monto;
                        }

                        break;
                    }
                }

                Console.WriteLine($"El salario mensual de {empleado.Nombre} es de {salario}.");
            }

            if (salario < 0)
            {
                Console.WriteLine("Al parecer el empleado '" + empleado.Nombre +
                                  " está recibiendo un saldo negativo luego de aplicar todos los modificadores a su nombre.");
            }

            total += (double)(salario < 0 ? 0 : salario);
        }

        return total;
    }

    private async Task<IActionResult> SendDataToAccounting2(string json)
    {
        Console.WriteLine(json);

        HttpResponseMessage? response;
        try
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                _configuration["ContabilidadToken"]
            );

            response = await _client.PostAsync(
                "api/EntradaContable",
                new StringContent(
                    json,
                    Encoding.UTF8,
                    "application/json"
                )
            );
        }
        catch (Exception e)
        {
            return BadRequest("Ha ocurrido un error al enviar los datos al departamento de contabilidad. " + e);
        }

        var result = await response.Content.ReadAsStringAsync();
        if (response is not { IsSuccessStatusCode: true })
        {
            return BadRequest("Ha ocurrido un error (" + response.StatusCode +
                              ") enviando los datos de la nómina al departamento de contabilidad. " + result);
        }

        Console.WriteLine("response " + result);
        return RedirectToAction(nameof(Index));
    }

    private static decimal NormalizeAmount(decimal? amount)
    {
        if (!amount.HasValue)
        {
            return 1;
        }

        var value = amount.Value;

        switch (amount)
        {
            // probably on the scale 1-100
            case > 1:
            {
                var percentage = value / 100;
                return percentage > 1 ? 1 : percentage;
            }
            // we dont want to multiply by 0
            case 0:
                return 1;
            // already a percentage
            default:
                return value;
        }
    }
}