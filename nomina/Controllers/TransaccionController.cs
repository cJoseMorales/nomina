using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using nomina.Context;
using nomina.Entities;

namespace nomina.Controllers;

public class TransaccionController(NominaContext context) : Controller
{
    // GET: Transaccion
    public async Task<IActionResult> Index(
        string? filter,
        string? sorting,
        DateTime? since,
        DateTime? until
    )
    {
        var transacciones = await context.Transaccion
            .Include(t => t.Empleado)
            .Include(t => t.TipoDeTransaccion)
            .ToListAsync();
        var type = sorting == null ? "TIPO_DE_TRANSACCION" : sorting.ToUpper();

        if (transacciones.Count != 0)
        {
            if (!string.IsNullOrWhiteSpace(filter))
            {
                transacciones = type switch
                {
                    "TIPO_DE_TRANSACCION" => transacciones.Where(t =>
                            t.TipoDeTransaccion!.Nombre.Contains(filter, StringComparison.CurrentCultureIgnoreCase))
                        .ToList(),
                    "EMPLEADO" => transacciones.Where(t =>
                            t.Empleado!.Nombre.Contains(filter, StringComparison.CurrentCultureIgnoreCase))
                        .ToList(),
                    _ => transacciones
                };
            }

            if (since != null)
            {
                transacciones = transacciones.Where(t => t.Fecha >= since)
                    .ToList();
            }

            if (until != null)
            {
                transacciones = transacciones.Where(t => t.Fecha <= until)
                    .ToList();
            }
        }

        ViewData["filter"] = filter;
        ViewData["sorting"] = type;
        ViewData["since"] = since;
        ViewData["until"] = until;
        return View(transacciones);
    }

    // GET: Transaccion/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var transaccion = await context.Transaccion
            .Include(m => m.Empleado)
            .Include(m => m.TipoDeTransaccion)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (transaccion == null)
        {
            return NotFound();
        }

        return View(transaccion);
    }

    // GET: Transaccion/Create
    public IActionResult Create()
    {
        ViewData["Empleados"] = new SelectList(context.Empleado, "Id", "Nombre");
        ViewData["TiposTransaccion"] = new SelectList(context.TipoDeTransaccion, "Id", "Nombre");
        return View();
    }

    // POST: Transaccion/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        [Bind("Id,EmpleadoId,TipoDeTransaccionId,Fecha,Monto,Estado")]
        Transaccion transaccion)
    {
        if (ModelState.IsValid)
        {
            context.Add(transaccion);
            await context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        ViewData["Empleados"] = new SelectList(context.Empleado, "Id", "Nombre");
        ViewData["TiposTransaccion"] = new SelectList(context.TipoDeTransaccion, "Id", "Nombre");
        return View(transaccion);
    }

    // GET: Transaccion/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var transaccion = await context.Transaccion
            .FindAsync(id);
        if (transaccion == null)
        {
            return NotFound();
        }

        ViewData["Empleados"] = new SelectList(context.Empleado, "Id", "Nombre");
        ViewData["TiposTransaccion"] = new SelectList(context.TipoDeTransaccion, "Id", "Nombre");
        return View(transaccion);
    }

    // POST: Transaccion/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id,
        [Bind("Id,EmpleadoId,TipoDeTransaccionId,Fecha,Monto,Estado")]
        Transaccion transaccion)
    {
        if (id != transaccion.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                context.Update(transaccion);
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TransaccionExists(transaccion.Id))
                {
                    return NotFound();
                }

                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        ViewData["Empleados"] = new SelectList(context.Empleado, "Id", "Nombre");
        ViewData["TiposTransaccion"] = new SelectList(context.TipoDeTransaccion, "Id", "Nombre");
        return View(transaccion);
    }

    // GET: Transaccion/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var transaccion = await context.Transaccion
            .Include(m => m.Empleado)
            .Include(m => m.TipoDeTransaccion)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (transaccion == null)
        {
            return NotFound();
        }

        return View(transaccion);
    }

    // POST: Transaccion/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var transaccion = await context.Transaccion.FindAsync(id);
        if (transaccion != null)
        {
            context.Transaccion.Remove(transaccion);
        }

        await context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool TransaccionExists(int id)
    {
        return context.Transaccion.Any(e => e.Id == id);
    }
}