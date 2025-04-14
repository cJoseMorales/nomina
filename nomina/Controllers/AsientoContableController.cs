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

    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

    private readonly NominaContext context;
    private readonly HttpClient client;

    public AsientoContableController(NominaContext context, HttpClient client)
    {
        this.context = context;
        this.client = client;

        this.client.BaseAddress = new Uri("https://iso810-contabilidad.azurewebsites.net");
    }

    // GET: AsientoContable
    public async Task<IActionResult> Index()
    {
        return View(await context.AsientoContable.ToListAsync());
    }

    // GET: AsientoContable/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var asientoContable = await context.AsientoContable
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
            context.Add(asientoContable);
            await context.SaveChangesAsync();
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

        var asientoContable = await context.AsientoContable
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
        var asientoContable = await context.AsientoContable.FindAsync(id);
        if (asientoContable != null)
        {
            context.AsientoContable.Remove(asientoContable);
        }

        await context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Update(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var asientoContable = await context.AsientoContable.FindAsync(id);
        if (asientoContable == null)
        {
            return NotFound();
        }

        var parts = asientoContable.Periodo.Split("-");
        var year = int.Parse(parts[0]);
        var month = int.Parse(parts[1]);

        var transaccionesDelPeriodo = context.Transaccion
            .Where(t => t.Fecha.Year == year && t.Fecha.Month == month)
            .ToList();
        if (transaccionesDelPeriodo.Count == 0)
        {
            return NotFound();
        }

        var total = transaccionesDelPeriodo.Sum(t => t.Monto);
        asientoContable.Monto = total;

        context.AsientoContable.Update(asientoContable);
        await context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SendToAccounting(int id)
    {
        var asientoContable = await context.AsientoContable.FindAsync(id);
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
            dto = new
            {
                descripcion = "Asiento de Nominas correspondiente al periodo " + asientoContable.Periodo,
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
            }
        };

        return await SendDataToAccounting(JsonSerializer.Serialize(
            data,
            JsonOptions
        ), []);
    }

    public async Task<IActionResult> SendCurrentPeriodToAccounting()
    {
        var now = DateTime.Now;

        var year = now.Year;
        var month = now.Month;

        var transaccionesDelPeriodo = context.Transaccion
            .Where(t => t.IdAsiento == null && t.Fecha.Year == year && t.Fecha.Month == month)
            .ToList();
        if (transaccionesDelPeriodo.Count == 0)
        {
            return BadRequest("No se han encontrado transacciones en el período actual.");
        }

        var total = transaccionesDelPeriodo.Sum(t => t.Monto);
        var data = new
        {
            dto = new
            {
                descripcion = "Asiento de Nominas correspondiente al periodo " + now.ToString("yyyy-MM"),
                sistemaAuxiliarId = AuxiliarId,
                fechaAsiento = new DateTime(year, month, 1),
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
            }
        };

        var json = JsonSerializer.Serialize(
            data,
            JsonOptions
        );

        return await SendDataToAccounting(json, transaccionesDelPeriodo);
    }

    private async Task<IActionResult> SendDataToAccounting(string json, List<Transaccion> transacciones)
    {
        Console.WriteLine(json);

        HttpResponseMessage? response;
        try
        {
            /*if (HttpContext.Request.Headers.ContainsKey("Authorization"))
            {
                var token = HttpContext.Request.Headers.Authorization.ToString().Replace("Bearer ", "");

                if (string.IsNullOrEmpty(token))
                {
                    throw new Exception("Token de autenticación no válido o vacío.");
                }

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            else
            {
                return BadRequest("No se ha encontrado el token de autenticación.");
            }*/

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "el pepe");

            response = await client.PostAsync(
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

        if (response is not { IsSuccessStatusCode: true })
        {
            return BadRequest("Ha ocurrido un error enviando los datos de la nómina al departamento de contabilidad. " +
                              await response.Content.ReadAsStringAsync());
        }

        var result = await response.Content.ReadAsStringAsync();
        Console.WriteLine("result " + result);

        // todo: extract IdAsiento from response
        if (transacciones.Count != 0)
        {
            foreach (var t in transacciones)
            {
                t.IdAsiento = int.Parse(result);
                context.Transaccion.Update(t);
            }

            await context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }
}