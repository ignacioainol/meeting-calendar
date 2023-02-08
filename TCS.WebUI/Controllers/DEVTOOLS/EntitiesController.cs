using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DEVTOOLS.EF;
using DEVTOOLS.EF.Entidades;

namespace TCS.WebUI.Controllers.DEVTOOLS
{
    public class EntitiesController : Controller
    {
        private readonly DEVTOOLSContext _context;

        public EntitiesController(DEVTOOLSContext context)
        {
            _context = context;
        }

        // GET: Entities
        public async Task<IActionResult> Index()
        {
            var dEVTOOLSContext = _context.Entity.Include(e => e.EntityProjectDatabaseSource).Include(e => e.Project);
            return View(await dEVTOOLSContext.ToListAsync());
        }

        // GET: Entities/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Entity == null)
            {
                return NotFound();
            }

            var entity = await _context.Entity
                .Include(e => e.EntityProjectDatabaseSource)
                .Include(e => e.Project)
                .FirstOrDefaultAsync(m => m.EntityId == id);
            if (entity == null)
            {
                return NotFound();
            }

            return View(entity);
        }

        // GET: Entities/Create
        public IActionResult Create(int ProjectId)
        {
            ViewData["EntityProjectDatabaseSourceId"] = new SelectList(_context.DatabaseSource, "DatabaseSourceId", "DatabaseSourceName");
            ViewData["ProjectId"] = new SelectList(_context.Project.Where(x=>x.ProjectId == ProjectId), "ProjectId", "ProjectName", ProjectId);
            return View();
        }

        // POST: Entities/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EntityId,ProjectId,EntityName,EntityTableName,EntityProjectDatabaseSourceId,Active")] Entity entity)
        {
            if (ModelState.IsValid)
            {
                _context.Add(entity);
                await _context.SaveChangesAsync();
                return RedirectToAction("Edit", "Projects", new { id = entity.ProjectId });
            }
            ViewData["EntityProjectDatabaseSourceId"] = new SelectList(_context.DatabaseSource, "DatabaseSourceId", "DatabaseSourceName", entity.EntityProjectDatabaseSourceId);
            ViewData["ProjectId"] = new SelectList(_context.Project.Where(x => x.ProjectId == entity.ProjectId), "ProjectId", "ProjectName", entity.ProjectId);
            return View(entity);
        }

        // GET: Entities/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Entity == null)
            {
                return NotFound();
            }

            var entity = await _context.Entity
                .Include(x => x.EntityColumns)
                .Include(x => x.EntityTemplates)
                .FirstOrDefaultAsync(x => x.EntityId == id);

            if (entity == null)
            {
                return NotFound();
            }

            List<int> fkList = await _context.Column
                .Where(x => x.ColumnParentForeignKey != 0 && x.EntityId == id)
                .Select(x => x.ColumnParentForeignKey)
                .ToListAsync();

            List<Column> fkCols = await _context.Column
            .Include(x => x.Entity)
            .Where(x => fkList.Contains(x.ColumnId)).ToListAsync();


            ViewData["EntityProjectDatabaseSourceId"] = new SelectList(_context.DatabaseSource, "DatabaseSourceId", "DatabaseSourceName", entity.EntityProjectDatabaseSourceId);
            ViewData["ProjectId"] = new SelectList(_context.Project.Where(x => x.ProjectId == entity.ProjectId), "ProjectId", "ProjectName", entity.ProjectId);
            ViewData["ParentList"] = fkCols;
            //ViewData["ChildList"] = await _context.Column
            //    .Include(x => x.Entity)
            //    .Where(x => x.ColumnParentForeignKey != 0 && x.EntityId == id).ToListAsync();

            return View(entity);
        }

        // POST: Entities/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int EntityId, [Bind("EntityId,ProjectId,EntityName,EntityTableName,EntityProjectDatabaseSourceId,Active")] Entity entity)
        {
            if (EntityId != entity.EntityId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(entity);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EntityExists(entity.EntityId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Edit", "Projects", new { id = entity.ProjectId });
            }
            ViewData["EntityProjectDatabaseSourceId"] = new SelectList(_context.DatabaseSource, "DatabaseSourceId", "DatabaseSourceName", entity.EntityProjectDatabaseSourceId);
            ViewData["ProjectId"] = new SelectList(_context.Project.Where(x => x.ProjectId == entity.ProjectId), "ProjectId", "ProjectName", entity.ProjectId);
            return View(entity);
        }

        // GET: Entities/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Entity == null)
            {
                return NotFound();
            }

            var entity = await _context.Entity
                .Include(e => e.EntityProjectDatabaseSource)
                .Include(e => e.Project)
                .FirstOrDefaultAsync(m => m.EntityId == id);
            if (entity == null)
            {
                return NotFound();
            }

            return View(entity);
        }

        // POST: Entities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int EntityId)
        {
            if (_context.Entity == null)
            {
                return Problem("Entity set 'DEVTOOLSContext.Entity'  is null.");
            }
            var entity = await _context.Entity.FindAsync(EntityId);
            if (entity != null)
            {
                _context.Entity.Remove(entity);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Edit", "Projects", new { id = entity.ProjectId });
        }

        private bool EntityExists(int id)
        {
          return _context.Entity.Any(e => e.EntityId == id);
        }
    }
}
