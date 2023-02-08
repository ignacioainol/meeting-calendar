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
    public class ColumnsController : Controller
    {
        private readonly DEVTOOLSContext _context;

        public ColumnsController(DEVTOOLSContext context)
        {
            _context = context;
        }

        // GET: Columns
        public async Task<IActionResult> Index()
        {
            var dEVTOOLSContext = _context.Column.Include(c => c.Entity);
            return View(await dEVTOOLSContext.ToListAsync());
        }

        // GET: Columns/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Column == null)
            {
                return NotFound();
            }

            var column = await _context.Column
                .Include(c => c.Entity)
                .FirstOrDefaultAsync(m => m.ColumnId == id);
            if (column == null)
            {
                return NotFound();
            }

            return View(column);
        }

        // GET: Columns/Create
        public IActionResult Create(int EntityId)
        {
            ViewData["EntityVal"] = EntityId;
            ViewData["EntityId"] = new SelectList(_context.Entity.Where(x => x.EntityId == EntityId), "EntityId", "EntityName", EntityId);
            ViewData["ColumnType"] = new SelectList(EnumFunctions.EnumToObjectList<ColType>(), "Value", "Description");
            return View();
        }

        // POST: Columns/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ColumnId,ColumnIndex,EntityId,Active,ColumnPrimaryKey,ColumnForeignKey,ColumnParentForeignKey,ColumnName,ColumnDescription,ColumnType")] Column column)
        {
            if (column.ColumnForeignKey == false)
            {
                column.ColumnParentForeignKey = 0;
            }

            if (ModelState.IsValid)
            {
                _context.Add(column);
                await _context.SaveChangesAsync();
                return RedirectToAction("Edit", "Entities", new { id = column.EntityId });
            }
            ViewData["EntityId"] = new SelectList(_context.Entity.Where(x => x.EntityId == column.EntityId), "EntityId", "EntityName", column.EntityId);
            ViewData["ColumnType"] = new SelectList(EnumFunctions.EnumToObjectList<ColType>(), "Value", "Description", column.ColumnType);

            return View(column);
        }

        // GET: Columns/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Column == null)
            {
                return NotFound();
            }

            var column = await _context.Column
                .Include(x => x.ColumnTemplates)
                .FirstOrDefaultAsync(x => x.ColumnId == id);

            if (column == null)
            {
                return NotFound();
            }
            ViewData["EntityId"] = new SelectList(_context.Entity.Where(x => x.EntityId == column.EntityId), "EntityId", "EntityName", column.EntityId);
            ViewData["ColumnType"] = new SelectList(EnumFunctions.EnumToObjectList<ColType>(), "Value", "Description", column.ColumnType);
            ViewData["EntityVal"] = column.EntityId;
            ViewData["EntityParent"] = (column.ColumnParentForeignKey != 0) ? _context.Column.FirstOrDefault(x => x.ColumnId == column.ColumnParentForeignKey).EntityId:0;
            ViewData["ColumnParent"] = column.ColumnParentForeignKey;

            return View(column);
        }

        // POST: Columns/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int ColumnId, [Bind("ColumnId,ColumnIndex,EntityId,Active,ColumnPrimaryKey,ColumnForeignKey,ColumnParentForeignKey,ColumnName,ColumnDescription,ColumnType")] Column column)
        {
            if (ColumnId != column.ColumnId)
            {
                return NotFound();
            }

            if(column.ColumnForeignKey == false)
            {
                column.ColumnParentForeignKey = 0;
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(column);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ColumnExists(column.ColumnId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Edit", "Entities", new { id = column.EntityId });
            }
            ViewData["EntityId"] = new SelectList(_context.Entity.Where(x => x.EntityId == column.EntityId), "EntityId", "EntityName", column.EntityId);
            ViewData["ColumnType"] = new SelectList(EnumFunctions.EnumToObjectList<ColType>(), "Value", "Description", column.ColumnType);

            return View(column);
        }

        // GET: Columns/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Column == null)
            {
                return NotFound();
            }

            var column = await _context.Column
                .Include(c => c.Entity)
                .FirstOrDefaultAsync(m => m.ColumnId == id);
            if (column == null)
            {
                return NotFound();
            }

            return View(column);
        }

        // POST: Columns/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int ColumnId)
        {
            if (_context.Column == null)
            {
                return Problem("Entity set 'DEVTOOLSContext.Column'  is null.");
            }
            var column = await _context.Column.FindAsync(ColumnId);
            if (column != null)
            {
                _context.Column.Remove(column);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction("Edit", "Entities", new { id = column.EntityId });
        }

        private bool ColumnExists(int id)
        {
          return _context.Column.Any(e => e.ColumnId == id);
        }

        public async Task<IActionResult> GetColumns(int EntityId)
        {
            var columnList = await _context.Column.Where(x => x.EntityId == EntityId).ToListAsync();

            return Json(columnList);
        }

        public async Task<IActionResult> GetEntities(int EntityId)
        {
            Entity entity = await _context.Entity.FirstOrDefaultAsync(x => x.EntityId == EntityId);

            var entityList = await _context.Entity
                .Where(x => x.ProjectId == entity.ProjectId)
                .Where(x => x.EntityId != EntityId).ToListAsync();

            return Json(entityList);
        }
    }
}
