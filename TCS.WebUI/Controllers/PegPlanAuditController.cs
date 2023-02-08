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
    public class PegPlanAuditController : Controller
    {
        private readonly TCSContext _context;

        public PegPlanAuditController(TCSContext context)
        {
            _context = context;
        }

        // GET: DraftsAudits
        public async Task<IActionResult> Index()
        {
            var tCSContext = _context.PegPlanAudit.Include(d => d.Draft).Include(d => d.User);
            return View(await tCSContext.ToListAsync());
        }

        // GET: DraftsAudits/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pegPlanAudit = await _context.PegPlanAudit
                .Include(d => d.Draft)
                .Include(d => d.User)
                .FirstOrDefaultAsync(m => m.PegPlanAuditId == id);
            if (pegPlanAudit == null)
            {
                return NotFound();
            }

            return View(pegPlanAudit);
        }

        // GET: DraftsAudits/Create
        public IActionResult Create()
        {
            ViewData["PegPlanId"] = new SelectList(_context.PegPlan, "PegPlanId", "Code");
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Culture");
            return View();
        }

        // POST: DraftsAudits/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PegPlanAuditId,PegPlanId,UserId,Code,Shafts,Ends,DraftPlan,Description,Authorized,RepeatStart,RepeatEnd,RepeatCount,Process,ByUser,OnDay,AuthorizedBy")] PegPlanAudit pegPlanAudit)
        {
            if (ModelState.IsValid)
            {
                _context.Add(pegPlanAudit);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PegPlanId"] = new SelectList(_context.PegPlan, "PegPlanId", "Code", pegPlanAudit.PegPlanId);
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Culture", pegPlanAudit.UserId);
            return View(pegPlanAudit);
        }

        // GET: DraftsAudits/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pegPlanAudit = await _context.PegPlanAudit.FindAsync(id);
            if (pegPlanAudit == null)
            {
                return NotFound();
            }
            ViewData["PegPlanId"] = new SelectList(_context.PegPlan, "PegPlanId", "Code", pegPlanAudit.PegPlanId);
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Culture", pegPlanAudit.UserId);
            return View(pegPlanAudit);
        }

        // POST: DraftsAudits/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PegPlanAuditId,PegPlanId,UserId,Code,Shafts,Ends,DraftPlan,Description,Authorized,RepeatStart,RepeatEnd,RepeatCount,Process,ByUser,OnDay,AuthorizedBy")] PegPlanAudit pegPlanAudit)
        {
            if (id != pegPlanAudit.PegPlanAuditId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(pegPlanAudit);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PegPlanAuditExists(pegPlanAudit.PegPlanAuditId))
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
            ViewData["PegPlanId"] = new SelectList(_context.PegPlan, "PegPlanId", "Code", pegPlanAudit.PegPlanId);
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Culture", pegPlanAudit.UserId);
            return View(pegPlanAudit);
        }

        // GET: DraftsAudits/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pegPlanAudit = await _context.PegPlanAudit
                .Include(d => d.Draft)
                .Include(d => d.User)
                .FirstOrDefaultAsync(m => m.PegPlanAuditId == id);
            if (pegPlanAudit == null)
            {
                return NotFound();
            }

            return View(pegPlanAudit);
        }

        // POST: DraftsAudits/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var pegPlanAudit = await _context.PegPlanAudit.FindAsync(id);
            _context.PegPlanAudit.Remove(pegPlanAudit);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PegPlanAuditExists(int id)
        {
            return _context.PegPlanAudit.Any(e => e.PegPlanAuditId == id);
        }
    }
}
