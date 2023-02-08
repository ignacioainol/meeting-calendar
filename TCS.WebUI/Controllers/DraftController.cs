using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using TCS.EF;
using TCS.EF.Entidades;
using TCS.WebUI.Helpers;
using TCS.WebUI.Models;
using TCS.WebUI.ViewComponents;
using Wkhtmltopdf.NetCore;

namespace TCS.WebUI.Controllers
{
    public class DraftController : Controller
    {
        private readonly TCSContext _context;
        readonly IGeneratePdf _generarPDF;
        public DraftController(TCSContext context, IGeneratePdf generatePdf)
        {
            _context = context;
            _generarPDF = generatePdf;
            excelWorkbook = new HSSFWorkbook();
        }

        // GET: Sleys
        public async Task<IActionResult> Index(int? shaft,int? length, int? end, int? draftId)
        {
            var sley = _context.Draft.Include(d => d.User).AsQueryable();
            var draft = _context.PegPlan.Find(draftId);

            if (shaft != null)
            {
                sley = sley.Where(x => x.Shafts == shaft);
            }
            if (length != null)
            {
                sley = sley.Where(x => x.Lenght == length);
            }
            if (end != null)
            {
                sley = sley.Where(x => x.Ends == end);
            }

            if(draftId != null)
            {
                sley = sley.Where(x => x.Shafts == draft.Shafts);
            }

            ViewBag.draft = draftId;
            ViewBag.shaft = shaft;
            ViewBag.length = length;
            ViewBag.end = end;
            return View(await sley.ToListAsync());
        }

        // GET: Sleys/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sley = await _context.Draft
                .Include(s => s.Autorized)
                .Include(s => s.User)
                .FirstOrDefaultAsync(m => m.DraftId == id);
            if (sley == null)
            {
                return NotFound();
            }

            return View(sley);
        }

        // GET: Sleys/Create
        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Culture");
            ViewData["AuthorizedBy"] = new SelectList(_context.Users, "UserId", "Culture");
            return View();
        }

        // POST: Sleys/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create( Draft sley)
        {

            var PlanCode = _context.Draft.Where(x => x.PlanCode == sley.PlanCode).SingleOrDefault();
            var SleyPlan = _context.Draft.Where(x => x.Code == sley.Code).SingleOrDefault();

            if (PlanCode != null)
            {
                ModelState.AddModelError($"PlanCode", "error");
                ViewBag.MsgError = "Este dibujo ya existe .";

            }

            if (SleyPlan != null)
            {

                ModelState.AddModelError($"Code", "Este codigo ya existe ");

            }

            if (sley.PlanCode == null)
            {
                ModelState.AddModelError($"PlanCode", "error");
                ViewBag.MsgError = "Debes seleccionar al menos una casilla para crear el dibujo.";

            }
            //if (sley.SleyCode == null)
            //{
            //    ModelState.AddModelError($"SleyCode", "error");
            //    ViewBag.MsgError2 = "Debes seleccionar al menos sley.";

            //}

            List<int> Efila = new List<int>();
            List<int> ECol = new List<int>();
            List<int> ECol2 = new List<int>();

            string coordMalas = "";
            string coordMalas2 = "";

            int p = 0;

            if (sley.Repeats != null)
            {
                string[] repeticiones = sley.Repeats.Split("|");
                if (repeticiones.Length > 1)
                {
                    var r = repeticiones[0];

                    string[] re = r.Split(",");

                    int ini = Convert.ToInt32(re[0]);
                    int fin = Convert.ToInt32(re[1]);

                    foreach (var item3 in repeticiones)
                    {
                        string[] re2 = item3.Split(",");
                        int a = Convert.ToInt32(re2[0]);
                        int b = Convert.ToInt32(re2[1]);

                        if (a != ini || b != fin)
                        {
                            if (a <= ini || a <= fin)
                            {
                                ModelState.AddModelError($"PlanCode", "error");
                                ViewBag.MsgError = ($"Existe solapado en las repeticiones .");
                            }
                        }

                    }
                }

            }

            if (sley.PlanCode != null)
            {
                string[] coordenadas = sley.PlanCode.Split("|");


                for (int i = 1; i < sley.Shafts + 1; i++)
                {
                    for (int j = 1; j < sley.Lenght + 1; j++)
                    {
                        bool tiene = TieneDatos(i, j, coordenadas);
                        if (tiene)
                        {
                            break;
                        }
                        if (j == sley.Lenght)
                        {
                            Efila.Add(i);
                            for (int k = 1; k < sley.Lenght + 1; k++)
                            {
                                coordMalas = coordMalas + $"{i};{k}|";
                            }
                        }
                    }
                }

                for (int j = 1; j < sley.Lenght + 1; j++)
                {
                    for (int i = 1; i < sley.Shafts + 1; i++)
                    {
                        bool tiene = TieneDatos(i, j, coordenadas);
                        if (tiene)
                        {
                            break;
                        }
                        if (i == sley.Shafts)
                        {
                            ECol.Add(j);

                            for (int k = 1; k < sley.Shafts + 1; k++)
                            {
                                coordMalas = coordMalas + $"{k};{j}|";
                            }
                        }
                    }
                }
                if (coordMalas.Length > 1)
                {
                    coordMalas = coordMalas.Remove(coordMalas.Length - 1);
                }

                for (int j = 1; j < sley.Lenght + 1; j++)
                {
                    for (int i = 1; i < sley.Shafts + 1; i++)
                    {
                        bool tiene = TieneDatos(i, j, coordenadas);
                        if (tiene)
                        {
                            p++;
                        }
                        if (p > 1)
                        {
                            ECol2.Add(j);
                            for (int k = 1; k < sley.Shafts + 1; k++)
                            {
                                coordMalas2 = coordMalas2 + $"{k};{j}|";
                            }
                            break;
                        }

                    }
                    p = 0;
                }
                if (coordMalas2.Length > 1)
                {
                    coordMalas2 = coordMalas2.Remove(coordMalas2.Length - 1);
                }
            }

            if (Efila.Count > 0 || ECol.Count > 0)
            {
                if(Efila.Count > 0 && ECol.Count > 0)
                {
                    ModelState.AddModelError($"DraftPlan", "error");
                    ViewBag.MsgError = ($"Existe un error en la fila y columna coloreada .");
                }
                else if (Efila.Count > 0)
                {
                    ModelState.AddModelError($"DraftPlan", "error");
                    ViewBag.MsgError = ($"Existe un error en la fila coloreada .");
                }
                else if(ECol.Count > 0)
                {
                    ModelState.AddModelError($"DraftPlan", "error");
                    ViewBag.MsgError = ($"Existe un error en la columna coloreada .");
                }

            }


            if(ECol2.Count > 0)
            {
                ModelState.AddModelError($"DraftPlan", "error");
                ViewBag.MsgError3 = ($"No pueden existir 2 puntos la misma columna.");
            }
            if (ModelState.IsValid)
            {
                if (sley.Authorized == true)
                {
                    sley.AuthorizedBy = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                }

                sley.UserId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier).Value);

                int suma = 0;
                if (sley.Repeats != null)
                {
                    string[] repeticiones = sley.Repeats.Split("|");
                    
                    foreach (var item3 in repeticiones)
                    {
                        string[] rep = item3.Split(",");
                        int inicio = Convert.ToInt32(rep[0]);
                        int fin = Convert.ToInt32(rep[1]);
                        int cantidad = Convert.ToInt32(rep[2]);
                        int cant = cantidad - 1;
                        var x = (fin - inicio + 1) * cant;
                        suma = suma + x; 
                    }

                }
                sley.Ends = sley.Lenght + suma;


                _context.Add(sley);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.hola = sley;
            ViewBag.Efila = Efila;
            ViewBag.ECol = ECol;
            ViewBag.coordMalas = coordMalas;
            ViewBag.coordMalas2 = coordMalas2;
            ViewBag.repeat = true;
            return View(sley);
        }
        bool TieneDatos(int i, int j, string[] coordenadas)
        {

            foreach (var item in coordenadas)
            {
                if (i.ToString() == item.Split(";")[0] && j.ToString() == item.Split(";")[1])
                {
                    return true;
                }
            }
            return false;
        }
        // GET: Sleys/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sley = await _context.Draft.FindAsync(id);
            if (sley == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Culture", sley.UserId);
            ViewData["AuthorizedBy"] = new SelectList(_context.Users, "UserId", "Culture", sley.AuthorizedBy);
            return View(sley);
        }

        // POST: Sleys/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit( Draft sley)
        {
            var PlanCode = _context.Draft.Where(x => x.PlanCode == sley.PlanCode && x.DraftId != sley.DraftId).SingleOrDefault();
            var SleyPlan = _context.Draft.Where(x => x.Code == sley.Code && x.DraftId != sley.DraftId).SingleOrDefault();

            if (PlanCode != null)
            {
                ModelState.AddModelError($"PlanCode", "error");
                ViewBag.MsgError = "Este dibujo ya existe .";

            }

            if (SleyPlan != null)
            {
                ModelState.AddModelError($"Code", "Este codigo ya existe ");

            }

            if (sley.PlanCode == null)
            {
                ModelState.AddModelError($"PlanCode", "error");
                ViewBag.MsgError = "Debes seleccionar al menos una casilla para crear el dibujo.";

            }
            //if (sley.SleyCode == null)
            //{
            //    ModelState.AddModelError($"SleyCode", "error");
            //    ViewBag.MsgError2 = "Debes seleccionar al menos sley.";

            //}

            List<int> Efila = new List<int>();
            List<int> ECol = new List<int>();
            List<int> ECol2 = new List<int>();

            string coordMalas = "";
            string coordMalas2 = "";

            int p = 0;
            if (sley.Repeats != null)
            {
                string[] repeticiones = sley.Repeats.Split("|");
                if (repeticiones.Length > 1)
                {
                    var r = repeticiones[0];

                    string[] re = r.Split(",");

                    int ini = Convert.ToInt32(re[0]);
                    int fin = Convert.ToInt32(re[1]);

                    foreach (var item3 in repeticiones)
                    {
                        string[] re2 = item3.Split(",");
                        int a = Convert.ToInt32(re2[0]);
                        int b = Convert.ToInt32(re2[1]);

                        if (a != ini || b != fin)
                        {
                            if (a <= ini || a <= fin)
                            {
                                ModelState.AddModelError($"PlanCode", "error");
                                ViewBag.MsgError = ($"Existe solapado en las repeticiones .");
                            }
                        }

                    }
                }

            }

            if (sley.PlanCode != null)
            {
                string[] coordenadas = sley.PlanCode.Split("|");


                for (int i = 1; i < sley.Shafts + 1; i++)
                {
                    for (int j = 1; j < sley.Lenght + 1; j++)
                    {
                        bool tiene = TieneDatos(i, j, coordenadas);
                        if (tiene)
                        {
                            break;
                        }
                        if (j == sley.Lenght)
                        {
                            Efila.Add(i);
                            for (int k = 1; k < sley.Lenght + 1; k++)
                            {
                                coordMalas = coordMalas + $"{i};{k}|";
                            }
                        }
                    }
                }

                for (int j = 1; j < sley.Lenght + 1; j++)
                {
                    for (int i = 1; i < sley.Shafts + 1; i++)
                    {
                        bool tiene = TieneDatos(i, j, coordenadas);
                        if (tiene)
                        {
                            break;
                        }
                        if (i == sley.Shafts)
                        {
                            ECol.Add(j);

                            for (int k = 1; k < sley.Shafts + 1; k++)
                            {
                                coordMalas = coordMalas + $"{k};{j}|";
                            }
                        }
                    }
                }
                if (coordMalas.Length > 1)
                {
                    coordMalas = coordMalas.Remove(coordMalas.Length - 1);
                }


                for (int j = 1; j < sley.Lenght + 1; j++)
                {
                    for (int i = 1; i < sley.Shafts + 1; i++)
                    {
                        bool tiene = TieneDatos(i, j, coordenadas);
                        if (tiene)
                        {
                            p++;
                        }
                        if (p > 1)
                        {
                            ECol2.Add(j);
                            for (int k = 1; k < sley.Shafts + 1; k++)
                            {
                                coordMalas2 = coordMalas2 + $"{k};{j}|";
                            }
                            break;
                        }

                    }
                    p = 0;
                }
                if (coordMalas2.Length > 1)
                {
                    coordMalas2 = coordMalas2.Remove(coordMalas2.Length - 1);
                }
            }

            if (Efila.Count > 0 || ECol.Count > 0)
            {
                if (Efila.Count > 0 && ECol.Count > 0)
                {
                    ModelState.AddModelError($"DraftPlan", "error");
                    ViewBag.MsgError = ($"Existe un error en la fila y columna coloreada .");
                }
                else if (Efila.Count > 0)
                {
                    ModelState.AddModelError($"DraftPlan", "error");
                    ViewBag.MsgError = ($"Existe un error en la fila coloreada .");
                }
                else if (ECol.Count > 0)
                {
                    ModelState.AddModelError($"DraftPlan", "error");
                    ViewBag.MsgError = ($"Existe un error en la columna coloreada .");
                }

            }
            if (ECol2.Count > 0)
            {
                ModelState.AddModelError($"DraftPlan", "error");
                ViewBag.MsgError = ($"No puede haber 2 puntos en una columna.");
            }
            if (ModelState.IsValid)
            {
                try
                {
                    if (sley.Authorized == true)
                    {
                        sley.AuthorizedBy = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                    }

                    sley.UserId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier).Value);

                    int suma = 0;
                    if (sley.Repeats != null)
                    {
                        string[] repeticiones = sley.Repeats.Split("|");

                        foreach (var item3 in repeticiones)
                        {
                            string[] rep = item3.Split(",");
                            int inicio = Convert.ToInt32(rep[0]);
                            int fin = Convert.ToInt32(rep[1]);
                            int cantidad = Convert.ToInt32(rep[2]);
                            int cant = cantidad - 1;
                            var x = (fin - inicio + 1) * cant;
                            suma = suma + x;
                        }

                    }
                    sley.Ends = sley.Lenght + suma;
                    sley.UserId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                    _context.Update(sley);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SleyExists(sley.DraftId))
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
            ViewBag.hola = sley;
            ViewBag.Efila = Efila;
            ViewBag.ECol = ECol;
            ViewBag.coordMalas = coordMalas;
            ViewBag.coordMalas2 = coordMalas2;
            return View(sley);
        }

        // GET: Sleys/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sley = await _context.Draft
                .Include(s => s.Autorized)
                .Include(s => s.User)
                .FirstOrDefaultAsync(m => m.DraftId == id);
            if (sley == null)
            {
                return NotFound();
            }

            return View(sley);
        }

        // POST: Sleys/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Draft sley)
        {
            var sleys = await _context.Draft.FindAsync(sley.DraftId);
            sley.UserId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            _context.Draft.Remove(sleys);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SleyExists(int id)
        {
            return _context.Draft.Any(e => e.DraftId == id);
        }

        public IActionResult GetColsDraws(int? Shafts, int? Lenght, string data, string sley, string repeats)
        {
            return ViewComponent(typeof(DesignSleyViewComponent), new { cols = Lenght, rows = Shafts,  data, sley, repeats });
        }

        public IActionResult GetColsDraft(int? Shafts, int? Lenght, string data, string sley, string repeats)
        {
            return ViewComponent(typeof(DesignDraftPreviewViewComponent), new { cols = Lenght, rows = Shafts, data, sley, repeats });
        }


        public async Task<IActionResult> ImagePrint2(int id, int draftId,string desde)
        {

            var draft = await _context.PegPlan.FindAsync(id);
            if(desde == null)
            {
                PegPlanDraft draftSley = new PegPlanDraft();
                draftSley.PegPlanId = id;
                draftSley.DraftId = draftId;

                _context.Entry(draftSley).State = EntityState.Added;
                await _context.SaveChangesAsync();
            }
            draft.DraftId = draftId;

         
            draft.wwwrootpath = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}";
            //return PartialView("ReportPrint", _incidente);
            //string fecha_actualizacion = "";
            //string actualizacion_user = "";

            var datosHeaderFooter = new Dictionary<string, string>
            {
                { "codigo_informe", $"{draft.Code}" } };


            var options = new ConvertOptions
            {
                PageMargins = new Wkhtmltopdf.NetCore.Options.Margins()
                {
                    Left = 25,
                    Right = 25,
                    Top = 10,
                    Bottom = 10
                },
                //HeaderHtml = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}",
                //HeaderSpacing = 0,
                //FooterHtml = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}",
                //FooterSpacing = 0,
                Replacements = datosHeaderFooter,
                PageSize = Wkhtmltopdf.NetCore.Options.Size.A3,
                PageOrientation = Wkhtmltopdf.NetCore.Options.Orientation.Landscape,
                FooterHtml = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}/html/DraftFooter.html",
            };

            _generarPDF.SetConvertOptions(options);
            var pdf = await _generarPDF.GetByteArray("Views/PegPlan/ImagePrint2.cshtml", draft);
            var pdfStream = new MemoryStream();
            pdfStream.Write(pdf, 0, pdf.Length);
            pdfStream.Position = 0;
            return new FileStreamResult(pdfStream, "application/pdf");
        }

        public async Task<IActionResult> ImagePrint(int id)
        {

            var sley = await _context.Draft.FindAsync(id);

            sley.wwwrootpath = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}";
            //return PartialView("ReportPrint", _incidente);
            //string fecha_actualizacion = "";
            //string actualizacion_user = "";

            var datosHeaderFooter = new Dictionary<string, string>
            {
                { "codigo_informe", $"{sley.Code}" } 
            };


            var options = new ConvertOptions
            {
                PageMargins = new Wkhtmltopdf.NetCore.Options.Margins()
                {
                    Left = 25,
                    Right = 25,
                    Top = 10,
                    Bottom = 10
                },
                Replacements = datosHeaderFooter,
                PageSize = Wkhtmltopdf.NetCore.Options.Size.A3,
                PageOrientation = Wkhtmltopdf.NetCore.Options.Orientation.Landscape,
                FooterHtml = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}/html/DraftFooter.html",
            };
            _generarPDF.SetConvertOptions(options);
            var pdf = await _generarPDF.GetByteArray("Views/Draft/ImagePrint.cshtml", sley);
            var pdfStream = new MemoryStream();
            pdfStream.Write(pdf, 0, pdf.Length);
            pdfStream.Position = 0;
            return new FileStreamResult(pdfStream, "application/pdf");
        }

        static HSSFWorkbook excelWorkbook;
        public async Task<IActionResult> ImagePrintExcel(int id)
        {
            LLenarHoja(id);
            LLenarHoja2(id);
            string fileName = "Draft";
            using (var exportData = new MemoryStream())
            {
                excelWorkbook.Write(exportData);

                DateTime startDate = DateTime.Now;

                string saveAsFileName = string.Format(fileName + " {0:d}.xls", startDate.ToShortDateString()).ToUpper();

                byte[] bytes = exportData.ToArray();
                return File(bytes, "application/vnd.ms-excel", saveAsFileName);
            }
        }

        private void LLenarHoja(int id)
        {
            ICellStyle estiloColumna = NpoiFunctions.EstiloColumna(excelWorkbook);
            ICellStyle estiloAzul = NpoiFunctions.EstiloAzul(excelWorkbook);
            ICellStyle estiloRojo = NpoiFunctions.EstiloRojo(excelWorkbook);
            ICellStyle estiloVerde = NpoiFunctions.EstiloVerde(excelWorkbook);
            ICellStyle estiloAmarillo = NpoiFunctions.EstiloAmarillo(excelWorkbook);

            ISheet sheet = excelWorkbook.CreateSheet("Sley");
            Draft sley = _context.Draft.FirstOrDefault(x => x.DraftId == id);
            DesignDrawSley design = DesignDrawSleyData(sley.Lenght, sley.Shafts, sley.PlanCode, sley.SleyCode, sley.Repeats);
            IRow row1 = sheet.CreateRow(1);
            row1.Height = 3 * 140;
            for (int i = 0; i < design.Cols + 1; i++)
            {
                if (design.Cols > 0)
                {
                    NpoiFunctions.NewCell(row1, estiloColumna, i, i);
                    NpoiFunctions.SetColumn(sheet, i, 4);
                }
            }
            int r = design.Rows;
            int fila = 2;
            for (int i = 1; i < design.Rows + 1; i++)
            {
                IRow row = sheet.CreateRow(fila);
                row.Height = 3 * 140;
                NpoiFunctions.NewCell(row, estiloColumna, 0, r);
                r -= 1;
                fila += 1;
                for (int j = 1; j < design.Cols + 1; j++)
                {
                    if (design.PointDataSley.Where(p => p.row == i && p.col == j).FirstOrDefault() != null)
                    {
                        NpoiFunctions.NewCell(row, estiloAzul, j, "");
                    }
                    else if (design.PointDataSley2.Where(p => p.row == i && p.col == j).FirstOrDefault() != null)
                    {

                    }
                    else if (design.PointDataSley4.Where(p => p.row == i && p.col == j).FirstOrDefault() != null)
                    {

                    }
                    else
                    {
                        NpoiFunctions.NewCell(row, estiloColumna, j, "");
                    }
                }
            }

            foreach (var item in design.Repeats)
            {
                IRow row = sheet.CreateRow(fila);
                row.Height = 3 * 140;
                NpoiFunctions.NewCell(row, estiloColumna, 0, "R");
                fila += 1;
                for (int j = 1; j < design.Cols + 1; j++)
                {
                    if (item.inicio <= j && item.fin >= j)
                    {
                        NpoiFunctions.NewCell(row, estiloRojo, j, item.cantidad);
                    }
                    else
                    {
                        NpoiFunctions.NewCell(row, estiloColumna, j, "");
                    }
                }
            }

            if (design.Rows > 0)
            {
                fila += 1;
                int filaNumber = 2;
                for (int i = 1; i < 2 + 1; i++)
                {
                    IRow row = sheet.CreateRow(fila);
                    row.Height = 3 * 140;
                    NpoiFunctions.NewCell(row, estiloColumna, 0, filaNumber);
                    fila += 1;
                    filaNumber -= 1;

                    for (int j = 1; j < design.Cols + 1; j++)
                    {
                        ICellStyle estilo = NpoiFunctions.EstiloRojo(excelWorkbook);
                        if (design.PointDataSley3.Where(p => p.row == i && p.col == j).FirstOrDefault() != null)
                        {
                            switch (design.PointDataSley3.Where(p => p.row == i && p.col == j).FirstOrDefault().color)
                            {
                                case 1:
                                    estilo = estiloRojo;
                                    break;
                                case 2:
                                    estilo = estiloVerde;
                                    break;
                                case 3:
                                    estilo = estiloAzul;
                                    break;
                                case 4:
                                    estilo = estiloAmarillo;
                                    break;
                            }
                            NpoiFunctions.NewCell(row, estilo, j, "");
                        }
                        else
                        {
                            NpoiFunctions.NewCell(row, estiloColumna, j, "");
                        }
                        
                    }
                }
            }
        }

        private void LLenarHoja2(int id)
        {
            ICellStyle estiloColumna = NpoiFunctions.EstiloColumna(excelWorkbook);
            ICellStyle estiloAzul = NpoiFunctions.EstiloAzul(excelWorkbook);
            ICellStyle estiloRojo = NpoiFunctions.EstiloRojo(excelWorkbook);

            ISheet sheet = excelWorkbook.CreateSheet("Sley Full");
            Draft sley = _context.Draft.FirstOrDefault(x => x.DraftId == id);
            DesignDrawSley design = DesignDrawSleyData(sley.Lenght, sley.Shafts, sley.PlanCode, sley.SleyCode, sley.Repeats);
            IRow row1 = sheet.CreateRow(1);
            row1.Height = 3 * 140;
            for (int i = 0; i < matriz.ColumnaSleys.Count() + 1; i++)
            {
                if (matriz.ColumnaSleys.Count() > 0)
                {
                    if (i != 0)
                    {
                        NpoiFunctions.NewCell(row1, estiloColumna, i, i);
                        NpoiFunctions.SetColumn(sheet, i, 4);
                    }
                    else
                    {
                        NpoiFunctions.NewCell(row1, estiloColumna, i, 0);
                        NpoiFunctions.SetColumn(sheet, i, 4);
                    }
                }
            }
            int r = design.Rows;
            int fila = 2;
            for (int i = 1; i < matriz.Rows + 1; i++)
            {
                IRow row = sheet.CreateRow(fila);
                row.Height = 3 * 140;
                NpoiFunctions.NewCell(row, estiloColumna, 0, r);
                r -= 1;
                fila += 1;
                for (int j = 0; j < matriz.ColumnaSleys.Count; j++)
                {
                    foreach (var item in matriz.ColumnaSleys[j].filaSleys)
                    {
                        if (item.fila == i)
                        {
                            if (item.Dato == "SieKipo")
                            {
                                NpoiFunctions.NewCell(row, estiloAzul, j+1, "");
                            }
                            else
                            {
                                NpoiFunctions.NewCell(row, estiloColumna, j+1, "");
                            }
                        }
                    }
                }
            }
        }

        MatrizSley matriz;
        public DesignDrawSley DesignDrawSleyData(int cols, int rows, string data, string sley, string repeats)
        {
            string coordMalas2 ="";
            string puntos = "";
            DesignDrawSley draw = new DesignDrawSley
            {
                Cols = cols,
                Rows = rows,


            };
            MatrizSley matrizSley = new MatrizSley
            {
                Cols = cols,
                Rows = rows,
            };

            if (data != null)
            {
                string[] _data = data.Split("|");
                if (data.Length > 1)
                {
                    for (int i = 0; i < _data.Length; i++)
                    {
                        string[] datos = _data[i].Split(";");
                        PointDataSley p = new PointDataSley();
                        p.row = int.Parse(datos[0]);
                        p.col = int.Parse(datos[1]);
                        draw.PointDataSley.Add(p);
                    }
                }
            }

            if (sley != null)
            {
                string[] _sley = sley.Split("|");
                if (sley.Length > 1)
                {
                    for (int i = 0; i < _sley.Length; i++)
                    {
                        string[] datos = _sley[i].Split(";");
                        PointDataSley p = new PointDataSley();
                        p.row = int.Parse(datos[0]);
                        p.col = int.Parse(datos[1]);
                        if (datos.Count() > 2)
                        {
                            int color = 0;
                            if (datos[2] == "rgb(255, 0, 0)")
                            {
                                color = 1;
                            }
                            if (datos[2] == "rgb(0, 255, 0)")
                            {
                                color = 2;
                            }
                            if (datos[2] == "rgb(0, 0, 255)")
                            {
                                color = 3;
                            }
                            if (datos[2] == "rgb(255, 233, 0)")
                            {
                                color = 4;
                            }
                            p.color = color;
                        }
                        else
                        {
                            p.color = 0;
                        }
                        draw.PointDataSley3.Add(p);
                    }
                }
            }

            if (puntos != null)
            {
                string[] _puntos = puntos.Split("|");
                if (puntos.Length > 1)
                {
                    for (int i = 0; i < _puntos.Length; i++)
                    {
                        string[] datos = _puntos[i].Split(";");
                        PointDataSley p = new PointDataSley();
                        p.row = int.Parse(datos[0]);
                        p.col = int.Parse(datos[1]);
                        draw.PointDataSley2.Add(p);
                    }
                }

                ViewBag.puntos = puntos;
            }


            if (coordMalas2 != null)
            {
                string[] _coordMalas2 = coordMalas2.Split("|");
                if (coordMalas2.Length > 1)
                {
                    for (int i = 0; i < _coordMalas2.Length; i++)
                    {
                        string[] datos = _coordMalas2[i].Split(";");
                        PointDataSley p = new PointDataSley();
                        p.row = int.Parse(datos[0]);
                        p.col = int.Parse(datos[1]);
                        draw.PointDataSley4.Add(p);
                    }
                }

                ViewBag.coordMalas = coordMalas2;
            }

            if (repeats != null)
            {

                string[] _repeats = repeats.Split("|");
                if (repeats.Length > 1)
                {
                    for (int i = 0; i < _repeats.Length; i++)
                    {
                        string[] datos = _repeats[i].Split(",");
                        Repeats r = new Repeats();
                        int ini = int.Parse(datos[0]);
                        r.inicio = int.Parse(datos[0]);
                        int fini = int.Parse(datos[1]);
                        r.fin = int.Parse(datos[1]);
                        r.cantidad = int.Parse(datos[2]);
                        draw.Repeats.Add(r);
                    }
                }

            }


            #region Matriz Sley


            List<ColumnaSley> columnaSleys = new List<ColumnaSley>();
            int j = 1;
            while (j <= cols)
            {
                if (IsRepeatSley(draw.Repeats, j))
                {
                    int cursor = j;
                    int inicio = j;
                    int fin = RepeatFinSley(draw.Repeats, j);
                    int cantidad = RepeatCantSley(draw.Repeats, j);
                    int repeticion = 0;

                    while (repeticion < cantidad)
                    {
                        while (cursor <= fin)
                        {
                            ColumnaSley columnaSley = new ColumnaSley();
                            columnaSley.col = cursor;
                            columnaSley.filaSleys = GetColsSley(draw, cursor);
                            columnaSleys.Add(columnaSley);
                            matrizSley.ColumnaSleys = columnaSleys;
                            cursor++;
                        }
                        repeticion++;
                        cursor = j;
                    }
                    j = j + ((fin - inicio) + 1);
                }
                else
                {
                    ColumnaSley columnaSley = new ColumnaSley();
                    columnaSley.col = j;
                    columnaSley.filaSleys = GetColsSley(draw, j);
                    columnaSleys.Add(columnaSley);
                    matrizSley.ColumnaSleys = columnaSleys;
                    j++;
                }
            }
            matriz = matrizSley;
            #endregion


            return draw;
        }

        public static List<FilaSley> GetColsSley(DesignDrawSley drawSley, int columna)
        {
            int go = 0;
            List<FilaSley> filaSleys = new List<FilaSley>();
            foreach (var item in drawSley.PointDataSley)
            {
                if (item.col == columna && go == 0)
                {
                    for (int j = 1; j < drawSley.Rows + 1; j++)
                    {
                        FilaSley filaSley = new FilaSley();
                        filaSley.fila = j;
                        filaSley.Dato = GetMiLetraSley(drawSley.PointDataSley, j, item.col) == 1 ? "SieKipo" : "No tiene";
                        filaSleys.Add(filaSley);
                    }
                    go++;
                }
            }
            return filaSleys;
        }

        public static int GetMiLetraSley(List<PointDataSley> pointDatas, int fila, int columna)
        {
            foreach (var item in pointDatas)
            {
                if (item.col == columna && item.row == fila)
                {
                    return 1;
                }
            }
            return 0;
        }

        public static bool IsRepeatSley(List<Repeats> repeats, int columna)
        {
            foreach (var item in repeats)
            {
                if (item.inicio == columna)
                {
                    return true;
                }
            }
            return false;
        }
        public static int RepeatFinSley(List<Repeats> repeats, int columna)
        {
            foreach (var item in repeats)
            {
                if (item.inicio == columna)
                {
                    return item.fin;
                }
            }
            return 0;
        }
        public static int RepeatCantSley(List<Repeats> repeats, int columna)
        {
            foreach (var item in repeats)
            {
                if (item.inicio == columna)
                {
                    return item.cantidad;
                }
            }
            return 0;
        }
    }
}
