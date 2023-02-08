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
    public class ProjectTemplatesController : Controller
    {
        private readonly DEVTOOLSContext _context;

        public ProjectTemplatesController(DEVTOOLSContext context)
        {
            _context = context;
        }

        // GET: ProjectTemplates
        public async Task<IActionResult> Index()
        {
            var dEVTOOLSContext = _context.ProjectTemplate.Include(p => p.Project);
            return View(await dEVTOOLSContext.ToListAsync());
        }

        // GET: ProjectTemplates/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.ProjectTemplate == null)
            {
                return NotFound();
            }

            var projectTemplate = await _context.ProjectTemplate
                .Include(p => p.Project)
                .FirstOrDefaultAsync(m => m.ProjectTemplateId == id);
            if (projectTemplate == null)
            {
                return NotFound();
            }

            return View(projectTemplate);
        }

        // GET: ProjectTemplates/Create
        public IActionResult Create(int ProjectId)
        {
            ViewData["ProjectId"] = new SelectList(_context.Project, "ProjectId", "ProjectName", ProjectId);
            ViewData["ProjectTemplateSourceCodeType"] = new SelectList(EnumFunctions.EnumToObjectList<SourceCodeType>(), "Value", "Description");
            return View();
        }

        // POST: ProjectTemplates/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProjectTemplateId,ProjectId,ProjectTemplateName,ProjectTemplateObservations,ProjectTemplateSourceCodeType,ProjectTemplateContent")] ProjectTemplate projectTemplate)
        {
            if (ModelState.IsValid)
            {
                _context.Add(projectTemplate);
                await _context.SaveChangesAsync();
                return RedirectToAction("Edit", "Projects", new { id = projectTemplate.ProjectId });
            }
            ViewData["ProjectId"] = new SelectList(_context.Project, "ProjectId", "ProjectName", projectTemplate.ProjectId);
            ViewData["ProjectTemplateSourceCodeType"] = new SelectList(EnumFunctions.EnumToObjectList<SourceCodeType>(), "Value", "Description", projectTemplate.ProjectTemplateSourceCodeType);
            return View(projectTemplate);
        }

        // GET: ProjectTemplates/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.ProjectTemplate == null)
            {
                return NotFound();
            }

            var projectTemplate = await _context.ProjectTemplate.FindAsync(id);
            if (projectTemplate == null)
            {
                return NotFound();
            }
            ViewData["ProjectId"] = new SelectList(_context.Project, "ProjectId", "ProjectName", projectTemplate.ProjectId);
            ViewData["ProjectTemplateSourceCodeType"] = new SelectList(EnumFunctions.EnumToObjectList<SourceCodeType>(), "Value", "Description", projectTemplate.ProjectTemplateSourceCodeType);
            return View(projectTemplate);
        }

        // POST: ProjectTemplates/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int ProjectTemplateId, [Bind("ProjectTemplateId,ProjectId,ProjectTemplateName,ProjectTemplateObservations,ProjectTemplateSourceCodeType,ProjectTemplateContent")] ProjectTemplate projectTemplate)
        {
            if (ProjectTemplateId != projectTemplate.ProjectTemplateId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(projectTemplate);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProjectTemplateExists(projectTemplate.ProjectTemplateId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Edit", "Projects", new { id = projectTemplate.ProjectId });
            }
            ViewData["ProjectId"] = new SelectList(_context.Project, "ProjectId", "ProjectName", projectTemplate.ProjectId);
            ViewData["ProjectTemplateSourceCodeType"] = new SelectList(EnumFunctions.EnumToObjectList<SourceCodeType>(), "Value", "Description", projectTemplate.ProjectTemplateSourceCodeType);
            return View(projectTemplate);
        }

        // GET: ProjectTemplates/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.ProjectTemplate == null)
            {
                return NotFound();
            }

            var projectTemplate = await _context.ProjectTemplate
                .Include(p => p.Project)
                .FirstOrDefaultAsync(m => m.ProjectTemplateId == id);
            if (projectTemplate == null)
            {
                return NotFound();
            }

            return View(projectTemplate);
        }

        // POST: ProjectTemplates/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int RelationShipId)
        {
            if (_context.ProjectTemplate == null)
            {
                return Problem("Entity set 'DEVTOOLSContext.ProjectTemplate'  is null.");
            }
            var projectTemplate = await _context.ProjectTemplate.FindAsync(RelationShipId);
            if (projectTemplate != null)
            {
                _context.ProjectTemplate.Remove(projectTemplate);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Edit", "Projects", new { id = projectTemplate.ProjectId });
        }

        private bool ProjectTemplateExists(int id)
        {
          return _context.ProjectTemplate.Any(e => e.ProjectTemplateId == id);
        }
    }
}
