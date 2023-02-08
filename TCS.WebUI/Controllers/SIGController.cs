using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TCS.WebUI.Controllers
{
    public class SIGController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
