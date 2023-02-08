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
    public class DesignYarnsController : Controller
    {
        private readonly TCSContext _context;

        public DesignYarnsController(TCSContext context)
        {
            _context = context;
        }

        // GET: DesignYarns
        public async Task<IActionResult> Index()
        {
            var tCSContext = _context.DesignYarn.Include(d => d.Design).Include(d => d.Yarn);
            return View(await tCSContext.ToListAsync());
        }

        // GET: DesignYarns/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var designYarn = await _context.DesignYarn
                .Include(d => d.Design)
                .Include(d => d.Yarn)
                .FirstOrDefaultAsync(m => m.DesignYarnId == id);
            if (designYarn == null)
            {
                return NotFound();
            }

            return View(designYarn);
        }

        // GET: DesignYarns/Create
        public IActionResult Create()
        {
            ViewData["DesignId"] = new SelectList(_context.Design, "DesignId", "DesignId");
            ViewData["YarnId"] = new SelectList(_context.Yarn, "YarnId", "YarnId");
            return View();
        }

        // POST: DesignYarns/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DesignYarnId,DesignId,YarnId,Tipo,Linea")] DesignYarn designYarn)
        {
            if (ModelState.IsValid)
            {
                _context.Add(designYarn);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DesignId"] = new SelectList(_context.Design, "DesignId", "DesignId", designYarn.DesignId);
            ViewData["YarnId"] = new SelectList(_context.Yarn, "YarnId", "YarnId", designYarn.YarnId);
            return View(designYarn);
        }

        // GET: DesignYarns/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var designYarn = await _context.DesignYarn.FindAsync(id);
            if (designYarn == null)
            {
                return NotFound();
            }
            ViewData["DesignId"] = new SelectList(_context.Design, "DesignId", "DesignId", designYarn.DesignId);
            ViewData["YarnId"] = new SelectList(_context.Yarn, "YarnId", "YarnId", designYarn.YarnId);
            return View(designYarn);
        }

        // POST: DesignYarns/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("DesignYarnId,DesignId,YarnId,Tipo,Linea")] DesignYarn designYarn)
        {
            if (id != designYarn.DesignYarnId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(designYarn);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DesignYarnExists(designYarn.DesignYarnId))
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
            ViewData["DesignId"] = new SelectList(_context.Design, "DesignId", "DesignId", designYarn.DesignId);
            ViewData["YarnId"] = new SelectList(_context.Yarn, "YarnId", "YarnId", designYarn.YarnId);
            return View(designYarn);
        }

        // GET: DesignYarns/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var designYarn = await _context.DesignYarn
                .Include(d => d.Design)
                .Include(d => d.Yarn)
                .FirstOrDefaultAsync(m => m.DesignYarnId == id);
            if (designYarn == null)
            {
                return NotFound();
            }

            return View(designYarn);
        }

        // POST: DesignYarns/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var designYarn = await _context.DesignYarn.FindAsync(id);
            _context.DesignYarn.Remove(designYarn);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DesignYarnExists(int id)
        {
            return _context.DesignYarn.Any(e => e.DesignYarnId == id);
        }
    }
}
