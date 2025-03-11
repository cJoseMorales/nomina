using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using nomina.Context;
using nomina.Entities;

namespace nomina.Controllers;

public class DepartamentoController(NominaContext context) : Controller
{
    // GET: Departamento
    public async Task<IActionResult> Index()
    {
        return View(await context.Departamento.ToListAsync());
    }

    // GET: Departamento/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var departamento = await context.Departamento
            .FirstOrDefaultAsync(m => m.Id == id);
        if (departamento == null)
        {
            return NotFound();
        }

        return View(departamento);
    }

    // GET: Departamento/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Departamento/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Nombre,Descripcion")] Departamento departamento)
    {
        if (ModelState.IsValid)
        {
            context.Add(departamento);
            await context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(departamento);
    }

    // GET: Departamento/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var departamento = await context.Departamento.FindAsync(id);
        if (departamento == null)
        {
            return NotFound();
        }
        return View(departamento);
    }

    // POST: Departamento/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,Descripcion")] Departamento departamento)
    {
        if (id != departamento.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                context.Update(departamento);
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DepartamentoExists(departamento.Id))
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
        return View(departamento);
    }

    // GET: Departamento/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var departamento = await context.Departamento
            .FirstOrDefaultAsync(m => m.Id == id);
        if (departamento == null)
        {
            return NotFound();
        }

        return View(departamento);
    }

    // POST: Departamento/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var departamento = await context.Departamento.FindAsync(id);
        if (departamento != null)
        {
            context.Departamento.Remove(departamento);
        }

        await context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool DepartamentoExists(int id)
    {
        return context.Departamento.Any(e => e.Id == id);
    }
}