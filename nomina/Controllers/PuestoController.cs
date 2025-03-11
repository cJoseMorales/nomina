using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using nomina.Context;
using nomina.Entities;

namespace nomina.Controllers;

public class PuestoController(NominaContext context) : Controller
{
    // GET: Puesto
    public async Task<IActionResult> Index()
    {
        return View(await context.Puesto.ToListAsync());
    }

    // GET: Puesto/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var puesto = await context.Puesto
            .FirstOrDefaultAsync(m => m.Id == id);
        if (puesto == null)
        {
            return NotFound();
        }

        return View(puesto);
    }

    // GET: Puesto/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Puesto/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Nombre,Descripcion")] Puesto puesto)
    {
        if (ModelState.IsValid)
        {
            context.Add(puesto);
            await context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(puesto);
    }

    // GET: Puesto/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var puesto = await context.Puesto.FindAsync(id);
        if (puesto == null)
        {
            return NotFound();
        }
        return View(puesto);
    }

    // POST: Puesto/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,Descripcion")] Puesto puesto)
    {
        if (id != puesto.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                context.Update(puesto);
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PuestoExists(puesto.Id))
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
        return View(puesto);
    }

    // GET: Puesto/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var puesto = await context.Puesto
            .FirstOrDefaultAsync(m => m.Id == id);
        if (puesto == null)
        {
            return NotFound();
        }

        return View(puesto);
    }

    // POST: Puesto/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var puesto = await context.Puesto.FindAsync(id);
        if (puesto != null)
        {
            context.Puesto.Remove(puesto);
        }

        await context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool PuestoExists(int id)
    {
        return context.Puesto.Any(e => e.Id == id);
    }
}