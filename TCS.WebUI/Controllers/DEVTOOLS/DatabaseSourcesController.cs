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
    public class DatabaseSourcesController : Controller
    {
        private readonly DEVTOOLSContext _context;

        public DatabaseSourcesController(DEVTOOLSContext context)
        {
            _context = context;
        }

        // GET: DatabaseSources
        public async Task<IActionResult> Index()
        {
            var dEVTOOLSContext = _context.DatabaseSource.Include(d => d.Project);
            return View(await dEVTOOLSContext.ToListAsync());
        }

        // GET: DatabaseSources/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.DatabaseSource == null)
            {
                return NotFound();
            }

            var databaseSource = await _context.DatabaseSource
                .Include(d => d.Project)
                .FirstOrDefaultAsync(m => m.DatabaseSourceId == id);
            if (databaseSource == null)
            {
                return NotFound();
            }

            return View(databaseSource);
        }

        // GET: DatabaseSources/Create
        public IActionResult Create(int ProjectId)
        {
            ViewData["ProjectId"] = new SelectList(_context.Project, "ProjectId", "ProjectName", ProjectId);
            ViewData["DatabaseSourceType"] = new SelectList(EnumFunctions.EnumToObjectList<DatabaseType>(), "Value", "Description");

            return View();
        }

        // POST: DatabaseSources/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DatabaseSourceId,ProjectId,DatabaseSourceName,DatabaseSourceType,DatabaseSourceDNS")] DatabaseSource databaseSource)
        {
            if (ModelState.IsValid)
            {
                _context.Add(databaseSource);
                await _context.SaveChangesAsync();
                return RedirectToAction("Edit", "Projects", new { id = databaseSource.ProjectId });
            }
            ViewData["ProjectId"] = new SelectList(_context.Project, "ProjectId", "ProjectName", databaseSource.ProjectId);
            ViewData["DatabaseSourceType"] = new SelectList(EnumFunctions.EnumToObjectList<DatabaseType>(), "Value", "Description", databaseSource.DatabaseSourceType);
            return View(databaseSource);
        }

        // GET: DatabaseSources/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.DatabaseSource == null)
            {
                return NotFound();
            }

            var databaseSource = await _context.DatabaseSource.FindAsync(id);
            if (databaseSource == null)
            {
                return NotFound();
            }

            ViewData["ProjectId"] = new SelectList(_context.Project, "ProjectId", "ProjectName", databaseSource.ProjectId);
            ViewData["DatabaseSourceType"] = new SelectList(EnumFunctions.EnumToObjectList<DatabaseType>(), "Value", "Description", databaseSource.DatabaseSourceType);
            return View(databaseSource);
        }

        // POST: DatabaseSources/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int DatabaseSourceId, [Bind("DatabaseSourceId,ProjectId,DatabaseSourceName,DatabaseSourceType,DatabaseSourceDNS")] DatabaseSource databaseSource)
        {
            if (DatabaseSourceId != databaseSource.DatabaseSourceId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(databaseSource);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DatabaseSourceExists(databaseSource.DatabaseSourceId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Edit", "Projects", new { id = databaseSource.ProjectId });
            }
            ViewData["ProjectId"] = new SelectList(_context.Project, "ProjectId", "ProjectName", databaseSource.ProjectId);
            ViewData["DatabaseSourceType"] = new SelectList(EnumFunctions.EnumToObjectList<DatabaseType>(), "Value", "Description", databaseSource.DatabaseSourceType);
            return View(databaseSource);
        }

        // GET: DatabaseSources/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.DatabaseSource == null)
            {
                return NotFound();
            }

            var databaseSource = await _context.DatabaseSource
                .Include(d => d.Project)
                .FirstOrDefaultAsync(m => m.DatabaseSourceId == id);
            if (databaseSource == null)
            {
                return NotFound();
            }

            return View(databaseSource);
        }

        // POST: DatabaseSources/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int DatabaseSourceId)
        {
            if (_context.DatabaseSource == null)
            {
                return Problem("Entity set 'DEVTOOLSContext.DatabaseSource'  is null.");
            }
            var databaseSource = await _context.DatabaseSource.FindAsync(DatabaseSourceId);
            if (databaseSource != null)
            {
                _context.DatabaseSource.Remove(databaseSource);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction("Edit", "Projects", new { id = databaseSource.ProjectId });
        }

        private bool DatabaseSourceExists(int id)
        {
          return _context.DatabaseSource.Any(e => e.DatabaseSourceId == id);
        }
    }
}
