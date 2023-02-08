using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DEVTOOLS.EF;
using DEVTOOLS.EF.Entidades;
using DEVTOOLS.EF.Enums;
using TCS.WebUI.Helpers;

namespace TCS.WebUI.Controllers.DEVTOOLS
{
    public class ColumnTemplatesController : Controller
    {
        private readonly DEVTOOLSContext _context;

        public ColumnTemplatesController(DEVTOOLSContext context)
        {
            _context = context;
        }

        // GET: ColumnTemplates
        public async Task<IActionResult> Index()
        {
            var dEVTOOLSContext = _context.ColumnTemplate.Include(c => c.Column);
            return View(await dEVTOOLSContext.ToListAsync());
        }

        // GET: ColumnTemplates/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.ColumnTemplate == null)
            {
                return NotFound();
            }

            var columnTemplate = await _context.ColumnTemplate
                .Include(c => c.Column)
                .FirstOrDefaultAsync(m => m.ColumnTemplateId == id);
            if (columnTemplate == null)
            {
                return NotFound();
            }

            return View(columnTemplate);
        }

        // GET: ColumnTemplates/Create
        public IActionResult Create(int ColumnId)
        {
            ViewData["ColumnId"] = new SelectList(_context.Column, "ColumnId", "ColumnDescription", ColumnId);
            ViewData["ColumnTemplateSourceCodeType"] = new SelectList(EnumFunctions.EnumToObjectList<SourceCodeType>(), "Value", "Description");

            return View();
        }

        // POST: ColumnTemplates/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ColumnTemplateId,ColumnId,ColumnTemplateSourceCodeType,ColumnTemplateName,ColumnTemplateObservations,ColumnTemplateContent")] ColumnTemplate columnTemplate)
        {
            if (ModelState.IsValid)
            {
                _context.Add(columnTemplate);
                await _context.SaveChangesAsync();
                return RedirectToAction("Edit", "Columns", new { id = columnTemplate.ColumnId });
            }
            ViewData["ColumnId"] = new SelectList(_context.Column, "ColumnId", "ColumnDescription", columnTemplate.ColumnId);
            ViewData["ColumnTemplateSourceCodeType"] = new SelectList(EnumFunctions.EnumToObjectList<SourceCodeType>(), "Value", "Description", columnTemplate.ColumnTemplateSourceCodeType);

            return View(columnTemplate);
        }

        // GET: ColumnTemplates/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.ColumnTemplate == null)
            {
                return NotFound();
            }

            var columnTemplate = await _context.ColumnTemplate.FindAsync(id);
            if (columnTemplate == null)
            {
                return NotFound();
            }
            ViewData["ColumnId"] = new SelectList(_context.Column, "ColumnId", "ColumnDescription", columnTemplate.ColumnId);
            ViewData["ColumnTemplateSourceCodeType"] = new SelectList(EnumFunctions.EnumToObjectList<SourceCodeType>(), "Value", "Description", columnTemplate.ColumnTemplateSourceCodeType);

            return View(columnTemplate);
        }

        // POST: ColumnTemplates/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int ColumnTemplateId, [Bind("ColumnTemplateId,ColumnId,ColumnTemplateSourceCodeType,ColumnTemplateName,ColumnTemplateObservations,ColumnTemplateContent")] ColumnTemplate columnTemplate)
        {
            if (ColumnTemplateId != columnTemplate.ColumnTemplateId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(columnTemplate);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ColumnTemplateExists(columnTemplate.ColumnTemplateId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Edit", "Columns", new { id = columnTemplate.ColumnId });
            }
            ViewData["ColumnId"] = new SelectList(_context.Column, "ColumnId", "ColumnDescription", columnTemplate.ColumnId);
            ViewData["ColumnTemplateSourceCodeType"] = new SelectList(EnumFunctions.EnumToObjectList<SourceCodeType>(), "Value", "Description", columnTemplate.ColumnTemplateSourceCodeType);

            return View(columnTemplate);
        }

        // GET: ColumnTemplates/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.ColumnTemplate == null)
            {
                return NotFound();
            }

            var columnTemplate = await _context.ColumnTemplate
                .Include(c => c.Column)
                .FirstOrDefaultAsync(m => m.ColumnTemplateId == id);
            if (columnTemplate == null)
            {
                return NotFound();
            }

            return View(columnTemplate);
        }

        // POST: ColumnTemplates/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int ColumnTemplateId)
        {
            if (_context.ColumnTemplate == null)
            {
                return Problem("Entity set 'DEVTOOLSContext.ColumnTemplate'  is null.");
            }
            var columnTemplate = await _context.ColumnTemplate.FindAsync(ColumnTemplateId);
            if (columnTemplate != null)
            {
                _context.ColumnTemplate.Remove(columnTemplate);
            }
            await _context.SaveChangesAsync();

            return RedirectToAction("Edit", "Columns", new { id = columnTemplate.ColumnId });
        }

        private bool ColumnTemplateExists(int id)
        {
          return _context.ColumnTemplate.Any(e => e.ColumnTemplateId == id);
        }
    }
}
