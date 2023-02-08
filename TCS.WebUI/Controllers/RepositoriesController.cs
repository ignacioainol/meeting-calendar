using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using REPOSITORY.EF;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace TCS.WebUI.Controllers
{
    public class RepositoriesController : Controller
    {
        private readonly REPOSITORYContext _context;


        public RepositoriesController(REPOSITORYContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {

            var crs = User.FindFirst(ClaimTypes.WindowsAccountName).Value;

            var consultas = _context.Share.Where(x => x.User_Share == crs);

            var repositories = _context.Query.Include(x => x.Shares).Include(x => x.Parameters).AsQueryable();

            repositories = repositories.Where(x => consultas.Any(x2 => x2.ID_Query == x.ID_Query) || x.ID_Owner == crs);

            

            return View(await repositories.ToListAsync());
        }

        public async Task<IActionResult> ExportData()
        {

            var crs = User.FindFirst(ClaimTypes.WindowsAccountName).Value;

            var consultas = _context.Share.Where(x => x.User_Share == crs);

            var repositories = _context.Query.Include(x => x.Shares).AsQueryable();

            repositories = repositories.Where(x => consultas.Any(x2 => x2.ID_Query == x.ID_Query) || x.ID_Owner == crs);



            return View(await repositories.ToListAsync());
        }
    }
}
