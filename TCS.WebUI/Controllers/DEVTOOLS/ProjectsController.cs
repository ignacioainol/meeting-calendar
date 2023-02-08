using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DEVTOOLS.EF;
using DEVTOOLS.EF.Entidades;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Data.SqlClient;
using System.Data;
using Newtonsoft.Json;
using DEVTOOLS.EF.Enums;

namespace TCS.WebUI.Controllers.DEVTOOLS
{
    [Authorize(Roles = "DevTools-Admin, DevTools-Private, DevTools-Public")]
    public class ProjectsController : Controller
    {
        private readonly DEVTOOLSContext _context;

        public ProjectsController(DEVTOOLSContext context)
        {
            _context = context;
        }

        // GET: Projects
        public async Task<IActionResult> Index()
        {
              return View(await _context.Project.ToListAsync());
        }

        // GET: Projects/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Project == null)
            {
                return NotFound();
            }

            var project = await _context.Project
                .FirstOrDefaultAsync(m => m.ProjectId == id);
            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        // GET: Projects/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Projects/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProjectId,ProjectName")] Project project)
        {
            if (ModelState.IsValid)
            {
                _context.Add(project);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(project);
        }

        // GET: Projects/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Project == null)
            {
                return NotFound();
            }

            var project = await _context.Project
                .Include(x=>x.ProjectDatabaseSources)
                .Include(x => x.ProjectEntities)
                .Include(x => x.ProjectTemplates)
                .FirstOrDefaultAsync(x=>x.ProjectId == id);
            if (project == null)
            {
                return NotFound();
            }
            return View(project);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int ProjectId, [Bind("ProjectId,ProjectName")] Project project)
        {
            if (ProjectId != project.ProjectId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(project);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProjectExists(project.ProjectId))
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
            return View(project);
        }

        // GET: Projects/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Project == null)
            {
                return NotFound();
            }

            var project = await _context.Project
                .FirstOrDefaultAsync(m => m.ProjectId == id);
            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int ProjectId)
        {
            if (_context.Project == null)
            {
                return Problem("Entity set 'DEVTOOLSContext.Project'  is null.");
            }
            var project = await _context.Project.FindAsync(ProjectId);
            if (project != null)
            {
                _context.Project.Remove(project);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProjectExists(int id)
        {
          return _context.Project.Any(e => e.ProjectId == id);
        }

        public async Task<ActionResult> GetDataSources(int ProjectId)
        {
            var datasources = await _context.DatabaseSource
                .AsNoTracking()
                .Where(x => x.ProjectId == ProjectId)
                .Select(x=> new { x.DatabaseSourceId, x.DatabaseSourceName })
                .ToListAsync();
            return Json(datasources);
        }

        public async Task<ActionResult> GetDatabaseEntities(int DatabaseSourceId)
        {
            var datasource = await _context.DatabaseSource
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.DatabaseSourceId == DatabaseSourceId);

            string test = $"Server = 192.168.24.11; Database=TCS;User Id = sa; Password = Cr0ssv1ll3; Connection Timeout = 0;";
            SqlConnection conn = new SqlConnection(test);
            conn.Open();

            DataTable schema = conn.GetSchema("Tables");

            List<string> TableNames = new List<string>();
            foreach (DataRow row in schema.Rows)
            {
                TableNames.Add(row[1].ToString() +"." + row[2].ToString());
            }

            conn.Close();

            return Json(TableNames);
        }

        [HttpPost]
        public async Task<ActionResult> InsertTableList([FromBody] string tableJson, int projectId, int dataSourceId)
        {
            List<string> tableList = JsonConvert.DeserializeObject<List<string>>(tableJson);

            string test = $"Server = 192.168.24.11; Database=TCS;User Id = sa; Password = Cr0ssv1ll3; Connection Timeout = 0;";
            
            foreach (var tab in tableList) 
            {
                Entity entidad = new();
                entidad.EntityName = tab;
                entidad.ProjectId = projectId;
                entidad.EntityProjectDatabaseSourceId = dataSourceId;
                _context.Add(entidad);
                await _context.SaveChangesAsync();

                List<Column> listacolumnas = new();

                using (SqlConnection connection = new SqlConnection(test))
                using (SqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = @$"SELECT TOP 0 * FROM {tab}";
                    connection.Open();

                    using (var reader = command.ExecuteReader(CommandBehavior.SchemaOnly))
                    {
                        reader.Read();

                        var table = reader.GetSchemaTable();

                        foreach (DataRow item in table.Rows)
                        {
                            listacolumnas.Add(new Column()
                            {
                                ColumnName = item[0].ToString(),
                                ColumnType = GetColType(item[12].ToString()),
                                EntityId = entidad.EntityId
                            });
                        }
                    }
                    _context.AddRange(listacolumnas);
                    await _context.SaveChangesAsync();
                }
            }
            return Ok();
        }

        private ColType GetColType(string val)
        {
            switch(val)
            {
                case string i when i.Contains("Int32"):
                    return ColType.Integer;
                case string i when i.Contains("String"):
                    return ColType.String;
                case string i when i.Contains("DateTime"):
                    return ColType.DateTime;
                case string i when i.Contains("Decimal"):
                    return ColType.Decimal;
                case string i when i.Contains("Boolean"):
                    return ColType.Boolean;
                default:
                    return ColType.String;
            }
        }
    }
}
