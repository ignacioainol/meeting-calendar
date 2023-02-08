using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace TCS.WebUI.Controllers
{
    public class SpinningController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
