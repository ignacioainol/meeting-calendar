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
    public class EntityTemplatesController : Controller
    {
        private readonly DEVTOOLSContext _context;

        public EntityTemplatesController(DEVTOOLSContext context)
        {
            _context = context;
        }

        // GET: EntityTemplates
        public async Task<IActionResult> Index()
        {
            var dEVTOOLSContext = _context.EntityTemplate.Include(e => e.Entity);
            return View(await dEVTOOLSContext.ToListAsync());
        }

        // GET: EntityTemplates/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.EntityTemplate == null)
            {
                return NotFound();
            }

            var entityTemplate = await _context.EntityTemplate
                .Include(e => e.Entity)
                .FirstOrDefaultAsync(m => m.EntityTemplateId == id);
            if (entityTemplate == null)
            {
                return NotFound();
            }

            return View(entityTemplate);
        }

        // GET: EntityTemplates/Create
        public IActionResult Create(int EntityId)
        {
            ViewData["EntityId"] = new SelectList(_context.Entity, "EntityId", "EntityName", EntityId);
            ViewData["EntityTemplateSourceCodeType"] = new SelectList(EnumFunctions.EnumToObjectList<SourceCodeType>(), "Value", "Description");

            return View();
        }

        // POST: EntityTemplates/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EntityTemplateId,EntityId,EntityTemplateName,EntityTemplateObservations,EntityTemplateSourceCodeType,EntityTemplateContent")] EntityTemplate entityTemplate)
        {
            if (ModelState.IsValid)
            {
                _context.Add(entityTemplate);
                await _context.SaveChangesAsync();
                return RedirectToAction("Edit", "Entities", new { id = entityTemplate.EntityId });
            }
            ViewData["EntityId"] = new SelectList(_context.Entity, "EntityId", "EntityName", entityTemplate.EntityId);
            ViewData["EntityTemplateSourceCodeType"] = new SelectList(EnumFunctions.EnumToObjectList<SourceCodeType>(), "Value", "Description", entityTemplate.EntityTemplateSourceCodeType);

            return View(entityTemplate);
        }

        // GET: EntityTemplates/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.EntityTemplate == null)
            {
                return NotFound();
            }

            var entityTemplate = await _context.EntityTemplate.FindAsync(id);
            if (entityTemplate == null)
            {
                return NotFound();
            }
            ViewData["EntityId"] = new SelectList(_context.Entity, "EntityId", "EntityName", entityTemplate.EntityId);
            ViewData["EntityTemplateSourceCodeType"] = new SelectList(EnumFunctions.EnumToObjectList<SourceCodeType>(), "Value", "Description", entityTemplate.EntityTemplateSourceCodeType);

            return View(entityTemplate);
        }

        // POST: EntityTemplates/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int EntityTemplateId, [Bind("EntityTemplateId,EntityId,EntityTemplateName,EntityTemplateObservations,EntityTemplateSourceCodeType,EntityTemplateContent")] EntityTemplate entityTemplate)
        {
            if (EntityTemplateId != entityTemplate.EntityTemplateId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(entityTemplate);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EntityTemplateExists(entityTemplate.EntityTemplateId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Edit", "Entities", new { id = entityTemplate.EntityId });
            }
            ViewData["EntityId"] = new SelectList(_context.Entity, "EntityId", "EntityName", entityTemplate.EntityId);
            ViewData["EntityTemplateSourceCodeType"] = new SelectList(EnumFunctions.EnumToObjectList<SourceCodeType>(), "Value", "Description", entityTemplate.EntityTemplateSourceCodeType);

            return View(entityTemplate);
        }

        // GET: EntityTemplates/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.EntityTemplate == null)
            {
                return NotFound();
            }

            var entityTemplate = await _context.EntityTemplate
                .Include(e => e.Entity)
                .FirstOrDefaultAsync(m => m.EntityTemplateId == id);
            if (entityTemplate == null)
            {
                return NotFound();
            }

            return View(entityTemplate);
        }

        // POST: EntityTemplates/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int EntityTemplateId)
        {
            if (_context.EntityTemplate == null)
            {
                return Problem("Entity set 'DEVTOOLSContext.EntityTemplate'  is null.");
            }
            var entityTemplate = await _context.EntityTemplate.FindAsync(EntityTemplateId);
            if (entityTemplate != null)
            {
                _context.EntityTemplate.Remove(entityTemplate);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Edit", "Entities", new { id = entityTemplate.EntityId });
        }

        private bool EntityTemplateExists(int id)
        {
          return _context.EntityTemplate.Any(e => e.EntityTemplateId == id);
        }
    }
}
