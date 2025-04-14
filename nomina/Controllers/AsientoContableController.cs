using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using nomina.Context;
using nomina.Entities;

namespace nomina.Controllers;

public class AsientoContableController(NominaContext context, HttpClient client) : Controller
{
    private static readonly string ApiKey = "";

    private static readonly int AuxiliarId = 2;

    private static readonly int CrAccountId = 70;
    private static readonly int DbAccountId = 71;

    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

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

        var total = asientoContable.Monto;
        var data = new
        {
            descripcion = "Asiento de Nominas correspondiente al periodo " + asientoContable.Periodo,
            id_auxiliar = AuxiliarId,
            cuenta_cr = new
            {
                cuenta_id = CrAccountId,
                monto = total
            },
            cuenta_db = new
            {
                cuenta_id = DbAccountId,
                monto = total
            }
        };

        return await SendDataToAccounting(JsonSerializer.Serialize(
            data,
            JsonOptions
        ));
    }

    public async Task<IActionResult> SendCurrentPeriodToAccounting()
    {
        var now = DateTime.Now;

        var year = now.Year;
        var month = now.Month;

        var transaccionesDelPeriodo = context.Transaccion
            .Where(t => t.Fecha.Year == year && t.Fecha.Month == month)
            .ToList();
        if (transaccionesDelPeriodo.Count == 0)
        {
            return NotFound();
        }

        var total = transaccionesDelPeriodo.Sum(t => t.Monto);
        var data = new
        {
            descripcion = "Asiento de Nominas correspondiente al periodo " + now.ToString("yyyy-MM"),
            id_auxiliar = AuxiliarId,
            cuenta_cr = new
            {
                cuenta_id = CrAccountId,
                monto = total
            },
            cuenta_db = new
            {
                cuenta_id = DbAccountId,
                monto = total
            }
        };

        var json = JsonSerializer.Serialize(
            data,
            JsonOptions
        );

        return await SendDataToAccounting(json);
    }

    private async Task<IActionResult> SendDataToAccounting(string json)
    {
        Console.WriteLine(json);

        HttpResponseMessage? response;
        try
        {
            response = await client.PostAsync(
                ApiKey,
                new StringContent(
                    json,
                    Encoding.UTF8,
                    "application/json"
                )
            );
        }
        catch (Exception e)
        {
            Console.WriteLine("Ha ocurrido un error al enviar los datos a '" + ApiKey + "'. " + e);
            return NotFound();
        }

        if (response is not { IsSuccessStatusCode: true })
        {
            Console.Write("Ha ocurrido un error enviando los datos de la nómina al departamento de contabilidad.");
            return NotFound();
        }

        Console.WriteLine("Enviado satisfactoriamente.");

        return RedirectToAction(nameof(Index));
    }
}