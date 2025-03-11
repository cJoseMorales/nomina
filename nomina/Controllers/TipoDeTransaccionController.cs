using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using nomina.Context;
using nomina.Entities;

namespace nomina.Controllers
{
    public class TipoDeTransaccionController(NominaContext context) : Controller
    {
        // GET: TipoDeTransaccion
        public async Task<IActionResult> Index()
        {
            return View(await context.TipoDeTransaccion.ToListAsync());
        }

        // GET: TipoDeTransaccion/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tipoDeTransaccion = await context.TipoDeTransaccion
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tipoDeTransaccion == null)
            {
                return NotFound();
            }

            return View(tipoDeTransaccion);
        }

        // GET: TipoDeTransaccion/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TipoDeTransaccion/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nombre,Operacion,DependeDeSalario,Estado")] TipoDeTransaccion tipoDeTransaccion)
        {
            if (ModelState.IsValid)
            {
                context.Add(tipoDeTransaccion);
                await context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tipoDeTransaccion);
        }

        // GET: TipoDeTransaccion/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tipoDeTransaccion = await context.TipoDeTransaccion.FindAsync(id);
            if (tipoDeTransaccion == null)
            {
                return NotFound();
            }
            return View(tipoDeTransaccion);
        }

        // POST: TipoDeTransaccion/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,Operacion,DependeDeSalario,Estado")] TipoDeTransaccion tipoDeTransaccion)
        {
            if (id != tipoDeTransaccion.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    context.Update(tipoDeTransaccion);
                    await context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TipoDeTransaccionExists(tipoDeTransaccion.Id))
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
            return View(tipoDeTransaccion);
        }

        // GET: TipoDeTransaccion/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tipoDeTransaccion = await context.TipoDeTransaccion
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tipoDeTransaccion == null)
            {
                return NotFound();
            }

            return View(tipoDeTransaccion);
        }

        // POST: TipoDeTransaccion/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tipoDeTransaccion = await context.TipoDeTransaccion.FindAsync(id);
            if (tipoDeTransaccion != null)
            {
                context.TipoDeTransaccion.Remove(tipoDeTransaccion);
            }

            await context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TipoDeTransaccionExists(int id)
        {
            return context.TipoDeTransaccion.Any(e => e.Id == id);
        }
    }
}
