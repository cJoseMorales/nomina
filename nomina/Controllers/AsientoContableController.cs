using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using nomina.Context;
using nomina.Entities;

namespace nomina.Controllers;

public class AsientoContableController(NominaContext context) : Controller
{
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
    public async Task<IActionResult> Create([Bind("Id,Descripcion,Cuenta,TipoDeMovimiento,Periodo,Monto,Estado")] AsientoContable asientoContable)
    {
        if (ModelState.IsValid)
        {
            context.Add(asientoContable);
            await context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(asientoContable);
    }

    // GET: AsientoContable/Edit/5
    public async Task<IActionResult> Edit(int? id)
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
        return View(asientoContable);
    }

    // POST: AsientoContable/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Descripcion,Cuenta,TipoDeMovimiento,Periodo,Monto,Estado")] AsientoContable asientoContable)
    {
        if (id != asientoContable.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                context.Update(asientoContable);
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AsientoContableExists(asientoContable.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
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

    private bool AsientoContableExists(int id)
    {
        return context.AsientoContable.Any(e => e.Id == id);
    }
}