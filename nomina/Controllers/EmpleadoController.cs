using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using nomina.Context;
using nomina.Entities;

namespace nomina.Controllers;

public class EmpleadoController(NominaContext context) : Controller
{
    // GET: Empleado
    public async Task<IActionResult> Index()
    {
        return View(await context.Empleado
            .Include(empleado => empleado.Departamento)
            .Include(empleado => empleado.Puesto)
            .ToListAsync());
    }

    // GET: Empleado/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var empleado = await context.Empleado
            .Include(empleado => empleado.Departamento)
            .Include(empleado => empleado.Puesto)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (empleado == null)
        {
            return NotFound();
        }

        return View(empleado);
    }

    // GET: Empleado/Create
    public IActionResult Create()
    {
        ViewData["Puestos"] = new SelectList(context.Puesto, "Id", "Nombre");
        ViewData["Departamentos"] = new SelectList(context.Departamento, "Id", "Nombre");
        return View();
    }

    // POST: Empleado/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Cedula,Nombre,DepartamentoId,PuestoId,SalarioMensual")] Empleado empleado)
    {
        if (ModelState.IsValid)
        {
            context.Add(empleado);
            await context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        
        ViewData["Puestos"] = new SelectList(context.Puesto, "Id", "Nombre");
        ViewData["Departamentos"] = new SelectList(context.Departamento, "Id", "Nombre");
        return View(empleado);
    }

    // GET: Empleado/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var empleado = await context.Empleado.FindAsync(id);
        if (empleado == null)
        {
            return NotFound();
        }
        
        ViewData["Puestos"] = new SelectList(context.Puesto, "Id", "Nombre");
        ViewData["Departamentos"] = new SelectList(context.Departamento, "Id", "Nombre");
        return View(empleado);
    }

    // POST: Empleado/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Cedula,Nombre,DepartamentoId,PuestoId,SalarioMensual")] Empleado empleado)
    {
        if (id != empleado.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                context.Update(empleado);
                await context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmpleadoExists(empleado.Id))
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
        
        ViewData["Puestos"] = new SelectList(context.Puesto, "Id", "Nombre");
        ViewData["Departamentos"] = new SelectList(context.Departamento, "Id", "Nombre");
        
        return View(empleado);
    }

    // GET: Empleado/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var empleado = await context.Empleado
            .Include(m => m.Departamento)
            .Include(m => m.Puesto)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (empleado == null)
        {
            return NotFound();
        }

        return View(empleado);
    }

    // POST: Empleado/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var empleado = await context.Empleado.FindAsync(id);
        if (empleado != null)
        {
            context.Empleado.Remove(empleado);
        }

        await context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool EmpleadoExists(int id)
    {
        return context.Empleado.Any(e => e.Id == id);
    }
}