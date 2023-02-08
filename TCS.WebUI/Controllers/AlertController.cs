using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SOFTLAND.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TCS.EF;
using TCS.EF.Entidades;
using TCS.WebUI.Models;

namespace TCS.WebUI.Controllers
{
    [Authorize(Roles = "Alert-Admin, Alert-Private, Alert-Public")]
    public class AlertController : Controller
    {
        private readonly TCSContext _context;
        private readonly ALARMContext _contextAlarm;
        private readonly SOFTLANDContext _contextSoftland;

        public AlertController(TCSContext context, ALARMContext contextAlarm, SOFTLANDContext contextSoftland)
        {
            _context = context;
            _contextAlarm = contextAlarm;
            _contextSoftland = contextSoftland;
        }

        public async Task<IActionResult> AlertPermission()
        {
            var enroll = await _context.Enroll.ToListAsync();
            var enrollUser = enroll.Select(x => new EnrollModel()
            {
                FichaCRS = x.FichaCRS,
                Ficha = x.Ficha,
                Enabled = x.Enabled,
                EnrollId = x.EnrollId,
                PhoneNumber = x.PhoneNumber,
                FullName = (!string.IsNullOrEmpty(x.FichaCRS)) ?
                                                    _context.Users.FirstOrDefault(z => z.UserName.ToUpper() == x.FichaCRS)?.FullName
                                                    : _contextSoftland.sw_personal.Where(z => z.ficha == x.Ficha).Select(y=> $"{y.nombre} {y.appaterno} {y.apmaterno}").FirstOrDefault()
            })
                                                .OrderBy(x => x.Enabled);
            return View(enrollUser);
        }

        public async Task<JsonResult> ChangeAuthorization(string fichaCRS, string ficha, bool isEnabled)
        {
            Enroll enroll = new();
            if (fichaCRS != null)
            {
              enroll =  await _context.Enroll.FirstOrDefaultAsync(x => x.FichaCRS == fichaCRS);
            }
            else
            {
                enroll = await _context.Enroll.FirstOrDefaultAsync(x => x.Ficha == ficha);
            }

            enroll.Enabled = isEnabled;
            _context.Update(enroll);

            await _context.SaveChangesAsync();
            if (isEnabled)
            {
                User user = new();
                if (!string.IsNullOrEmpty(fichaCRS))
                {
                   user = await _context.Users.FirstOrDefaultAsync(z => z.UserName.ToUpper() == fichaCRS);
                }
                else
                {
                    string nombres = await _contextSoftland.sw_personal
                                        .Where(z => z.ficha.ToUpper() == ficha)
                                        .Select(x=>x.nombre)
                                        .FirstOrDefaultAsync();
                    string[] arrNombres = nombres.Split(" ");

                    user = await _contextSoftland.sw_personal
                                  .Include(x => x.ccostopers)
                                    .ThenInclude(x => x.cwtcco)
                                  .Where(z => z.ficha.ToUpper() == ficha)
                                  .Select(y =>
                                      new User
                                      {
                                          Email = y.Email,
                                          FirstName = (arrNombres.Length > 0) ? arrNombres[0] : "",
                                          MiddleName = (arrNombres.Length > 1) ? arrNombres[1] : "",
                                          LastName = y.appaterno,
                                          SecondLastName = y.apmaterno,
                                          Ficha = y.ficha,
                                          Department = y.ccostopers.FirstOrDefault().cwtcco.DescCC,
                                          Address = y.direccion,
                                          Rut = y.rut
                                      })
                                  .FirstOrDefaultAsync();
                }

                int portadorId = 0;

                Portador portador = new()
                {
                    Correo = user.Email,
                    Nombres = $"{user.FirstName} {user.MiddleName}",
                    Apellidos = $"{user.LastName} {user.SecondLastName}",
                    Observacion = "",
                    AddUserId = user.UserId,
                    ChgUserId = user.UserId,
                    ChgDate = DateTime.Now,
                    EmpresaId = 1,
                    Ubicacion = user.Department,
                    Direccion = user.Address,
                    Rut = user.Rut.Replace(".", "").TrimStart('0'),
                    Activo = true
                };

                List<Portador> portadoresOld = await _contextAlarm.Portadors.ToListAsync();

                Portador portadorOld = portadoresOld.Find(x => x.Rut.Replace(".", "").TrimStart('0') == user.Rut.Replace(".", "").TrimStart('0'));

                if (portadorOld == null)
                {
                    _contextAlarm.Portadors.Add(portador);
                    await _contextAlarm.SaveChangesAsync();

                    portadorId = portador.PortadorId;
                }
                else
                {
                    portadorId = portadorOld.PortadorId;
                }

                Dispositivo dispositivo = await _contextAlarm.Dispositivos.FirstOrDefaultAsync(x => x.NumeroTelefono == Convert.ToInt32(enroll.PhoneNumber));
                if (dispositivo == null)
                {
                    dispositivo = new()
                    {
                        PortadorId = portadorId,
                        Status = "",
                        AddUserId = user.UserId,
                        ChgUserId = user.UserId,
                        ChgDate = DateTime.Now,
                        EmpresaId = 1,
                        NumeroTelefono = Convert.ToInt32(enroll.PhoneNumber),
                        Activo = true
                    };

                    _contextAlarm.Dispositivos.Add(dispositivo);

                    await _contextAlarm.SaveChangesAsync();
                }
                else
                {
                    dispositivo.Activo = true;
                    _contextAlarm.Dispositivos.Update(dispositivo);

                    await _contextAlarm.SaveChangesAsync();
                }
            }
            else
            {
                Dispositivo dispositivo = await _contextAlarm.Dispositivos.FirstOrDefaultAsync(x => x.NumeroTelefono == Convert.ToInt32(enroll.PhoneNumber));
                dispositivo.Activo = false;
                _contextAlarm.Dispositivos.Update(dispositivo);

                await _contextAlarm.SaveChangesAsync();
            }

            return Json(fichaCRS);
        }
    }
}
