using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TCS.EF;
using TCS.EF.Entidades;

namespace TCS.WebUI.Controllers
{
    public class YarnsController : Controller
    {
        private readonly TCSContext _context;

        public YarnsController(TCSContext context)
        {
            _context = context;
        }

        // GET: Yarns
        public async Task<IActionResult> Index()
        {
            return View(await _context.Yarn.ToListAsync());
        }

        // GET: Yarns/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var yarn = await _context.Yarn
                .FirstOrDefaultAsync(m => m.YarnId == id);
            if (yarn == null)
            {
                return NotFound();
            }

            return View(yarn);
        }

        // GET: Yarns/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Yarns/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("YarnId,YarnCount,Shade")] Yarn yarn)
        {
            if (ModelState.IsValid)
            {
                _context.Add(yarn);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(yarn);
        }

        // GET: Yarns/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var yarn = await _context.Yarn.FindAsync(id);
            if (yarn == null)
            {
                return NotFound();
            }
            return View(yarn);
        }

        // POST: Yarns/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("YarnId,YarnCount,Shade")] Yarn yarn)
        {
            if (id != yarn.YarnId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(yarn);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!YarnExists(yarn.YarnId))
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
            return View(yarn);
        }

        // GET: Yarns/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var yarn = await _context.Yarn
                .FirstOrDefaultAsync(m => m.YarnId == id);
            if (yarn == null)
            {
                return NotFound();
            }

            return View(yarn);
        }

        // POST: Yarns/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var yarn = await _context.Yarn.FindAsync(id);
            _context.Yarn.Remove(yarn);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool YarnExists(int id)
        {
            return _context.Yarn.Any(e => e.YarnId == id);
        }


        public async Task<ActionResult> GetYarn()
        {
            var yarn = await _context.Yarn
                        .AsNoTracking()
                        .ToListAsync();
            return Json(yarn);
        }

    }
}
