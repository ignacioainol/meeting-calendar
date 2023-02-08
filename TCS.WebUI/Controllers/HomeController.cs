using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using INVENTORY.EF;
using INVENTORY.EF.Entidades;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using MimeKit;
using MimeKit.Text;
using TCS.EF;
using TCS.EF.Entidades;
using MailKit.Net.Smtp;


namespace TCS.WebUI.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly TCSContext _context;
        private readonly INVENTORYRContext _contextInventory;
        private readonly IAuthenticationService _authenticationService;
        private readonly IStringLocalizer<SharedResource> _localizer;
        public HomeController(TCSContext context, INVENTORYRContext contextInventory, IAuthenticationService authenticationService, IStringLocalizer<SharedResource> localizer)
        {
            //CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("es-CL");
            _context = context;
            _contextInventory = contextInventory;
            _authenticationService = authenticationService;
            _localizer = localizer;
        }
        public async Task<IActionResult> Index()
        {
            string userid = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var usuario = await _context.Users
            .Where(u => u.UserId.ToString() == userid)
            .FirstOrDefaultAsync();

            var usuarioCrs = await _context.Users
            .Where(u => u.UserId.ToString() == userid && u.UserName.StartsWith("C"))
            .FirstOrDefaultAsync();

            if (usuarioCrs != null)
            {
                var covid = await _context.Covid.Where(x => x.UserId == usuarioCrs.UserId && x.FechaConfirmacion.Date == DateTime.Now.Date).FirstOrDefaultAsync();
                ViewBag.covid = covid;
                ViewBag.cl = "chilean";
            }

            ClaimsIdentity identity = new ClaimsIdentity(this.GetUserClaims(usuario), CookieAuthenticationDefaults.AuthenticationScheme);
            ClaimsPrincipal principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            Inventory currentInventory = _contextInventory.Inventory.Where(x => x.InventoryId == _contextInventory.Inventory.Max(xs => xs.InventoryId)).ToList().First();
            ViewBag.OpenOrClose = InventarioAbierto();
            if (ViewBag.OpenOrClose && _contextInventory.TmsPiece.Any())
            {
                var inventoryInfo = await _contextInventory.Inventory
                                    .Include(x => x.TmsPiece)
                                    .OrderByDescending(x => x.StartDate)
                                    .Select(x => new InventoryData
                                    {
                                        StartDate = x.StartDate,
                                        CreatorOwner = x.CreatorOwner,
                                        EncontradaGeneral = x.TmsPiece.Count(x => x.StatusId != 0 && x.ScanDate <= DateTime.Now),
                                        TotalGeneral = x.TmsPiece.Count,
                                        PorcentajeGeneral = Math.Round(Convert.ToDouble(x.TmsPiece.Where(x => x.StatusId != 0 && x.ScanDate <= DateTime.Now).Count()) * 100 / Convert.ToDouble(x.TmsPiece.Count()), 1, MidpointRounding.AwayFromZero),
                                        PrimerScannGeneral = (x.TmsPiece.OrderByDescending(s => s.ScanDate).FirstOrDefault().ScanDate == null) ? x.StartDate : (x.TmsPiece.OrderBy(s => s.ScanDate).FirstOrDefault(x => x.ScanDate != null).ScanDate),
                                        TmsPiece = x.TmsPiece.ToList()
                                    })
                                    .FirstOrDefaultAsync();

                ViewBag.Porcentaje = inventoryInfo.PorcentajeGeneral;
                ViewBag.CreatorOwner = inventoryInfo.CreatorOwner;
                ViewBag.Total = inventoryInfo.TotalGeneral;
                ViewBag.Encontradas = inventoryInfo.EncontradaGeneral;
            }
            else
            {
                ViewBag.CreatorOwner = "Piezas con problemas, por favor ver integridad de datos en TMS";
            }

            ViewBag.Fecha = currentInventory.StartDate.ToShortDateString();

            var GetCurrentShift = await _context.CurrentProd
                        .Include(x => x.Loom)
                        .ThenInclude(x => x.LoomType)
                        .Where(x => x.Loom.Status == true && x.Loom.IsCalculated == true)
                        .ToListAsync();
            ViewBag.GetRunningEfficiencyCurrentShiftAvg = GetCurrentShift.Average(x => x.ActualEff);
            ViewBag.GetStandardEfficiencyCurrentShiftAvg = GetCurrentShift.Average(x => x.StdEff);
            return View();
        }

        //[HttpPost]
        public async Task<IActionResult> CultureManagement(string culture, string returnUrl)
        {
            Response.Cookies.Append(CookieRequestCultureProvider.DefaultCookieName, 
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires =  DateTimeOffset.Now.AddDays(30)});

            string userid = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var usuario = await _context.Users
            .FirstOrDefaultAsync(u => u.UserId.ToString() == userid);

            if (culture == "es")
            {
                usuario.Culture = "CL";
                usuario.Language = "ES";
            }
            else
            {
                usuario.Culture = "US";
                usuario.Language = "EN";
            }
            _context.Update(usuario);
            await _context.SaveChangesAsync();
            return LocalRedirect(returnUrl);
        }

        public IActionResult Directory()
        {
            return View();
        }

        private IEnumerable<Claim> GetUserClaims(User user)
        {
            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()));
            claims.Add(new Claim(ClaimTypes.Name, user.FullName ?? ""));
            claims.Add(new Claim(ClaimTypes.WindowsAccountName, user.UserName ?? ""));
            claims.Add(new Claim(ClaimTypes.Email, user.Email ?? ""));
            claims.Add(new Claim(ClaimTypes.Uri, user.Photo ?? ""));
            claims.AddRange(this.GetUserRoleClaims(user));
            return claims;
        }

        private IEnumerable<Claim> GetUserRoleClaims(User user)
        {
            List<Claim> claims = new List<Claim>();
            var permisos = _context.UserPermissions
                            .Include(x => x.Permission)
                            .Include(x => x.Module)
                            .Include(x => x.User)
                            .Where(x => x.User.UserName == user.UserName);

            foreach (UserPermission item in permisos)
            {
                claims.Add(new Claim(ClaimTypes.Role, string.Format("{0}-{1}", item.Module.Description, item.Permission.Description)));
            }

            return claims;
        }

        public bool InventarioAbierto()
        {
            List<Inventory> inventario = _contextInventory.Inventory.ToList();
            inventario = inventario.Where(x => x.InventoryId == inventario.Max(xs => xs.InventoryId)).ToList();
            if (inventario.Count == 0)
            {
                return false;
            }

            Inventory ultimo_inventario = inventario.First();

            DateTime fecha_termino = ultimo_inventario.EndDate ?? default;

            if (fecha_termino == DateTime.MinValue) //Si la fecha de termino es nula (o MinValue) significa que hay un inventario abierto
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<ActionResult<object>> Covid(string respuesta)
        {

            string userid = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var usuario = await _context.Users
            .Where(u => u.UserId.ToString() == userid && u.UserName.StartsWith("C"))
            .FirstOrDefaultAsync();


            Covid covid = new Covid();

            covid.UserId = usuario.UserId;
            if (respuesta == "no")
            {
                covid.Confirmacion = true;

            }
            else
            {
                covid.Confirmacion = false;
            }
            covid.FechaConfirmacion = DateTime.Now;
            _context.Add(covid);
            await _context.SaveChangesAsync();

            if (respuesta == "no")
            {
                await Email(usuario, covid);
            }
            return new RedirectResult(Url.Action("Index", "Home"));

        }

        public async Task<ActionResult<object>> Email(User user, Covid covid)
        {

                try
                {
                    //string StrContent = "cheee hay covid";
                    
                   
                    var message = new MimeMessage();
                    //Setting the To e-mail address
                    message.To.Add(new MailboxAddress("Ximena Molina", "ximolina@crossvillefabric.com"));
                    message.Cc.Add(new MailboxAddress("Erwin Cabrera", "ercabrera@crossvillefabric.com"));
                    message.Cc.Add(new MailboxAddress("Lilian Medel", "asistente.prevencion@crossvillefabric.com"));
                //Setting the From e-mail address
                message.From.Add(new MailboxAddress($"Aviso de caso contacto estrecho", "support@crossvillefabric.com"));
                    //E-mail subject 
                    //message.Subject = $"lo envio {abogado.NombreCompleto}";
                    message.Subject = $"Mensaje Automático Asistencia";

                    //E-mail message bodys
                    //var key = Guid.Parse("a392ef91-db60-4a3c-918d-7bb30187e21a").ToString("N");
                    //var encrypted = CryptographyHelper.Encrypt(data.Correo, key);

                    message.Body = new TextPart(TextFormat.Html)
                    {
                        Text = $"Estimados: El Sr(a) {user.FullName}, usuario {user.UserName},ha confirmado en TCS a las { string.Format("{0:HH}:{0:mm}", covid.FechaConfirmacion)} que podría tener síntoma de covid o haber sido contacto estrecho, se le notificó dirigirse de inmediato al departamento de Prevención de Riesgos.<br/> No conteste este correo ya que fue generado de forma automática desde TCS el día { string.Format("{0:dd}-{0:MM}-{0:yyyy}", covid.FechaConfirmacion)} a las {string.Format("{0:HH}:{0:mm}", covid.FechaConfirmacion)} ."
                    };
                    //Configure the e-mail
                    using (var emailClient = new SmtpClient())
                    {
                    emailClient.Connect("secure.emailsrvr.com", 25, false);
                    emailClient.Authenticate("support@crossvillefabric.com", "O50m47z2");
                    await emailClient.SendAsync(message);
                    emailClient.Disconnect(true);
                }
                    return "ok";
                }
                catch (Exception ex)
                {
                    return ex.Message;
                }
            }
    }
}
