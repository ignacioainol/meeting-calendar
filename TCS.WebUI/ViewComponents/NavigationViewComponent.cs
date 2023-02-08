using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TCS.WebUI.Models;

namespace TCS.WebUI.ViewComponents
{
    public class NavigationViewComponent : ViewComponent
    {
        public static IEnumerable<Claim> claims;
        public IViewComponentResult Invoke()
        {
            var identity = (ClaimsIdentity)User.Identity;
            IEnumerable<Claim> claim = identity.Claims;
            claims = claim.Where(c => c.Type == ClaimTypes.Role);

            var items = NavigationModel.Full;


            return View(items);
        }
    }
}
