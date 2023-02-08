using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TCS.EF;
using TCS.EF.Entidades;

namespace TCS.WebUI.ViewComponents
{
    public class WarpingViewComponent : ViewComponent
    {
        private readonly TCSContext _context;
        public WarpingViewComponent(TCSContext context)
        {
            _context = context;
        }
        public IViewComponentResult Invoke(int? noWarps)
        {

            ViewBag.noWarps = noWarps;
            return View();
        }

    }
}
