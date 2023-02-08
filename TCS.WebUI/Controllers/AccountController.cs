using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MimeKit.Encodings;
using Newtonsoft.Json;
using SOFTLAND.EF;
using TCS.EF;
using TCS.EF.Entidades;
using TCS.WebUI.Helpers;
using TCS.WebUI.Models;

namespace TCS.WebUI.Controllers
{
    public class AccountController : Controller
    {
        private readonly TCSContext _context;
        private readonly SOFTLANDContext _contextSoftland;
        private readonly Helpers.IAuthenticationService _authenticationService;
        public AccountController(TCSContext context, SOFTLANDContext contextSoftland, Helpers.IAuthenticationService authenticationService)
        {
            //CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("es-CL");
            _context = context;
            _contextSoftland = contextSoftland;
            _authenticationService = authenticationService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            //ViewBag.Domain = new SelectList(Domains.DomainList, "DomainRute", "DomainName");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "http://192.168.24.4/auth/api/v1/auth/tcs");
            string jsonsend = JsonConvert.SerializeObject(model);
            request.Content = new StringContent(jsonsend, Encoding.UTF8, "application/json");
            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("APIKEY", "test123");
            HttpResponseMessage response = await httpClient.SendAsync(request).ConfigureAwait(false);

            var jsonget = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            UserAdModel result = JsonConvert.DeserializeObject<UserAdModel>(jsonget);


            if (!result.Succeeded)
            {
                ModelState.AddModelError("LogOnError", "El usuario o contraseña son incorrectos.");
                ViewBag.logginError = "Error";
            }
            else if (result.Data.IsLocked && model.Username.Contains("CRS"))
            {
                ModelState.AddModelError("LogOnError", "El usuario esta bloqueado.");
                ViewBag.logginError = "Error";
            }
            else
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, model.Username),
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                var usuario = await _context.Users
                        .Where(u => u.UserName == model.Username)
                        .FirstOrDefaultAsync()
                        .ConfigureAwait(false);

                if (usuario == null && result.Succeeded)
                {
                    usuario = new User();

                    usuario.UserName = model.Username.ToUpper();
                    if (usuario.UserName.Contains("CRS"))
                    {
                        usuario.Culture = "CL";
                        usuario.Language = "ES";
                    }
                    else
                    {
                        usuario.Culture = "US";
                        usuario.Language = "EN";
                    }
                    usuario.FirstName = result.Data.FirstName;
                    usuario.LastName = result.Data.LastName;
                    usuario.Email = result.Data.Email;
                    usuario.Department = result.Data.Department;
                    usuario.LastLogin = DateTime.Now;
                    usuario.Rut = result.Data.Dni;

                    _context.Add(usuario);
                }
                else
                {
                    if (!string.IsNullOrEmpty(result.Data.Dni))
                    {
                        HttpRequestMessage requestSoftlandUser = new HttpRequestMessage(HttpMethod.Get, $"http://192.168.24.4/softland/api/v1/Persona/{result.Data.Dni}");
                        HttpClient httpClientSoftlandUser = new HttpClient();
                        httpClientSoftlandUser.DefaultRequestHeaders.Add("APIKEY", "test123");
                        HttpResponseMessage responseSoftlandUser = await httpClientSoftlandUser.SendAsync(requestSoftlandUser).ConfigureAwait(false);

                        string jsongetSoftlandUser = await responseSoftlandUser.Content.ReadAsStringAsync().ConfigureAwait(false);
                        UserSoftlandModel resultSoftland = JsonConvert.DeserializeObject<UserSoftlandModel>(jsongetSoftlandUser);

                        string[] nombres = resultSoftland.Data.nombre.Split(" ");
                        usuario.FirstName = (nombres.Length > 0) ? nombres[0] : "";
                        usuario.MiddleName = (nombres.Length > 1) ? nombres[1] : "";
                        usuario.LastName = resultSoftland.Data.appaterno;
                        usuario.SecondLastName = resultSoftland.Data.apmaterno;
                        usuario.Ficha = resultSoftland.Data.ficha;
                        usuario.Department = resultSoftland.Data.descCC;
                        usuario.Address = resultSoftland.Data.direccion;
                    }

                    usuario.LastLogin = DateTime.Now;
                    _context.Update(usuario);
                }
                await _context.SaveChangesAsync();

                ClaimsIdentity identity = new ClaimsIdentity(this.GetUserClaims(usuario), CookieAuthenticationDefaults.AuthenticationScheme);
                ClaimsPrincipal principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                Response.Cookies.Append(CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(usuario.Language)),
                new CookieOptions { Expires = DateTimeOffset.Now.AddDays(30) });

                if (model.Username.ToUpper().Contains("HS"))
                {
                    return RedirectToAction("order", "hollandsherry");
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
                //else if(string.IsNullOrEmpty(usuario.Rut) && usuario.UserName.Contains("CRS"))
                //{
                //    return RedirectToAction("RequestRut", "Account");
                //}

            }

            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction(nameof(Login));
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        public IActionResult RequestRut()
        {
            return View();
        }

        public async Task<IActionResult> UpdateRut(string rut)
        {
            string userid = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var usuario = await _context.Users
            .Where(u => u.UserId.ToString() == userid)
            .FirstOrDefaultAsync();

            usuario.Rut = rut;
            _context.Update(usuario);
            await _context.SaveChangesAsync();

            return Ok();
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
                claims.Add(new Claim(ClaimTypes.Role, string.Format("{0}-{1}",item.Module.Description,item.Permission.Description)));
            }

            return claims;
        }
    }
}
