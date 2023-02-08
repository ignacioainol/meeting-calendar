using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TCS.EF;
using TCS.EF.Entidades;

namespace TCS.WebUI.Controllers
{
    [Authorize(Roles = "Permission Assignment-Admin, Permission Assignment-Private, Permission Assignment-Public")]
    public class PermissionAssignmentController : Controller
    {
        private readonly TCSContext _context;

        public PermissionAssignmentController(TCSContext context)
        {
            _context = context;
        }

        // GET: AssignPermissions
        public async Task<IActionResult> Index()
        {
            var tCSContext = _context.UserPermissions.Include(u => u.Module).Include(u => u.Permission).Include(u => u.User);
            return View(await tCSContext.ToListAsync());
        }

        // GET: AssignPermissions/Details/5
        public async Task<IActionResult> Details(int? UserId, int? PermissionId, int? ModuleId)
        {
            if (UserId == null || PermissionId == null || ModuleId == null)
            {
                return NotFound();
            }

            var userPermission = await _context.UserPermissions
                .Include(u => u.Module)
                .Include(u => u.Permission)
                .Include(u => u.User)
                .FirstOrDefaultAsync(m => m.UserId == UserId && m.PermissionId == PermissionId && m.ModuleId == ModuleId);
            if (userPermission == null)
            {
                return NotFound();
            }

            return View(userPermission);
        }

        // GET: AssignPermissions/Create
        public IActionResult Create()
        {
            //ViewBag.userPermissionRep = "";
            ViewData["ModuleId"] = new SelectList(_context.Modules, "ModuleId", "Description");
            ViewData["PermissionId"] = new SelectList(_context.Permissions, "PermissionId", "Description");
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "FullNamePlusUserName");
            return View();
        }

        // POST: AssignPermissions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserId,PermissionId,ModuleId")] UserPermission userPermission)
        {
            if (ModelState.IsValid)
            {
                if (_context.UserPermissions.Any(x=>x.PermissionId == userPermission.PermissionId && x.ModuleId == userPermission.ModuleId && x.UserId == userPermission.UserId))
                {
                    ViewBag.userPermissionRep = "userPermissionRepeated";
                }
                else
                {
                    _context.Add(userPermission);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            ViewData["ModuleId"] = new SelectList(_context.Modules, "ModuleId", "Description", userPermission.ModuleId);
            ViewData["PermissionId"] = new SelectList(_context.Permissions, "PermissionId", "Description", userPermission.PermissionId);
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "FullNamePlusUserName", userPermission.UserId);
            return View(userPermission);
        }

        // GET: AssignPermissions/Edit/5
        public async Task<IActionResult> Edit(int? UserId, int? PermissionId, int? ModuleId)
        {
            if (UserId == null || PermissionId == null || ModuleId == null)
            {
                return NotFound();
            }

            var userPermission = await _context.UserPermissions.FindAsync(UserId, PermissionId,  ModuleId);
            if (userPermission == null)
            {
                return NotFound();
            }
            ViewData["ModuleId"] = new SelectList(_context.Modules, "ModuleId", "Description", userPermission.ModuleId);
            ViewData["PermissionId"] = new SelectList(_context.Permissions, "PermissionId", "Description", userPermission.PermissionId);
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserName", userPermission.UserId);
            return View(userPermission);
        }

        // POST: AssignPermissions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int UserId, int PermissionId, int ModuleId, [Bind("UserId,PermissionId,ModuleId")] UserPermission userPermission)
        {
            if (UserId != userPermission.UserId && PermissionId != userPermission.PermissionId && ModuleId != userPermission.ModuleId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(userPermission);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserPermissionExists(userPermission.UserId,userPermission.PermissionId,userPermission.ModuleId))
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
            ViewData["ModuleId"] = new SelectList(_context.Modules, "ModuleId", "Description", userPermission.ModuleId);
            ViewData["PermissionId"] = new SelectList(_context.Permissions, "PermissionId", "Description", userPermission.PermissionId);
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserName", userPermission.UserId);
            return View(userPermission);
        }

        // GET: AssignPermissions/Delete/5
        public async Task<IActionResult> Delete(int? UserId, int? PermissionId, int? ModuleId)
        {
            if (UserId == null || PermissionId == null || ModuleId == null)
            {
                return NotFound();
            }

            var userPermission = await _context.UserPermissions
                .Include(u => u.Module)
                .Include(u => u.Permission)
                .Include(u => u.User)
                .FirstOrDefaultAsync(m => m.UserId == UserId && m.PermissionId == PermissionId && m.ModuleId == ModuleId);
            if (userPermission == null)
            {
                return NotFound();
            }

            return View(userPermission);
        }

        // POST: AssignPermissions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int UserId, int PermissionId, int ModuleId)
        {
            var userPermission = await _context.UserPermissions.FindAsync(UserId, PermissionId, ModuleId);
            _context.UserPermissions.Remove(userPermission);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserPermissionExists(int UserId, int PermissionId, int ModuleId)
        {
            return _context.UserPermissions.Any(m => m.UserId == UserId && m.PermissionId == PermissionId && m.ModuleId == ModuleId);
        }
    }
}
