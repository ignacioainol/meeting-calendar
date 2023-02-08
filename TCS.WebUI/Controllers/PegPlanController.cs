using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
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
    [Authorize(Roles = "Design-Admin, Design-Private, Design-Public")]
    public class PegPlanController : Controller
    {
        private readonly TCSContext _context;
        readonly IGeneratePdf _generarPDF;
        static HSSFWorkbook excelWorkbook;
        public PegPlanController(TCSContext context, IGeneratePdf generatePdf)
        {
            _context = context;
            _generarPDF = generatePdf;
            excelWorkbook = new HSSFWorkbook();
        }

        // GET: Drafts
        public async Task<IActionResult> Index(int? shaft, int? end)
        {
            var pegPlan = _context.PegPlan.Include(d => d.User).AsQueryable();

            if (shaft != null)
            {
                pegPlan = pegPlan.Where(x => x.Shafts == shaft);
            }
            if (end != null)
            {
                pegPlan = pegPlan.Where(x => x.Ends == end);
            }
            ViewBag.sleys = _context.Draft.ToList();
            ViewBag.shaft = shaft;
            ViewBag.end = end;
            return View(await pegPlan.ToListAsync());
        }

        // GET: Drafts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pegPlan = await _context.PegPlan
                .Include(d => d.User)
                .FirstOrDefaultAsync(m => m.PegPlanId == id);
            if (pegPlan == null)
            {
                return NotFound();
            }

            return View(pegPlan);
        }

        // GET: Drafts/Create
        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserName");
            return View();
        }

        // POST: Drafts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PegPlan pegPlan)
        {

            var DraftPlan = _context.PegPlan.Where(x => x.DraftPlan == pegPlan.DraftPlan).SingleOrDefault();
            var DraftPlan2 = _context.PegPlan.Where(x => x.Code == pegPlan.Code).SingleOrDefault();

            if (DraftPlan != null)
            {
                ModelState.AddModelError($"DraftPlan", "error");
                ViewBag.MsgError = "Este dibujo ya existe .";

            }

            if (DraftPlan2 != null)
            {
                ModelState.AddModelError($"Code", "Este codigo ya existe ");

            }

            if (pegPlan.DraftPlan == null)
            {
                ModelState.AddModelError($"DraftPlan", "error");
                ViewBag.MsgError = "Debes seleccionar al menos una casilla para crear el dibujo.";

            }

            List<int> Efila = new List<int>();
            List<int> ECol = new List<int>();

            string coordMalas = "";


            if (pegPlan.Repeats != null)
            {
                string[] repeticiones = pegPlan.Repeats.Split("|");
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
                                ModelState.AddModelError($"DraftPlan", "error");
                                ViewBag.MsgError = ($"Existe solapado en las repeticiones .");
                            }
                        }

                    }
                }

            } 

            if (pegPlan.DraftPlan != null)
            {
                string[] coordenadas = pegPlan.DraftPlan.Split("|");


                for (int i = 1; i < pegPlan.Ends + 1; i++)
                {
                    for (int j = 1; j < pegPlan.Shafts + 1; j++)
                    {
                        bool tiene = TieneDatos(i, j, coordenadas);
                        if (tiene)
                        {
                            break;
                        }
                        if (j == pegPlan.Shafts)
                        {
                            Efila.Add(i);
                            for (int k = 1; k < pegPlan.Shafts + 1; k++)
                            {
                                coordMalas = coordMalas + $"{i};{k}|";
                            }
                        }
                    }
                }

                for (int j = 1; j < pegPlan.Shafts + 1; j++)
                {
                    for (int i = 1; i < pegPlan.Ends + 1; i++)
                    {
                        bool tiene = TieneDatos(i, j, coordenadas);
                        if (tiene)
                        {
                            break;
                        }
                        if (i == pegPlan.Ends)
                        {
                            ECol.Add(j);

                            for (int k = 1; k < pegPlan.Ends + 1; k++)
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


            if (ModelState.IsValid)
            {
                if(pegPlan.Authorized == true)
                {
                    pegPlan.AuthorizedBy = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                }

                pegPlan.UserId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                _context.Add(pegPlan);
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {

                    throw;
                }

                return RedirectToAction(nameof(Index));
            }


            ViewBag.hola = pegPlan;
            ViewBag.Efila = Efila;
            ViewBag.ECol = ECol;
            ViewBag.coordMalas = coordMalas;
            ViewBag.repeat = true;
            
            return View(pegPlan);
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

        // GET: Drafts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pegPlan = await _context.PegPlan.FindAsync(id);
            if (pegPlan == null)
            {
                return NotFound();
            }

            var auditory = await _context.PegPlanAudit.Include(x => x.User).Include(x => x.Draft).Where(x => x.PegPlanId == pegPlan.PegPlanId).OrderByDescending(x => x.OnDay).ToListAsync();

            ViewBag.auditory = auditory;

            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Culture", pegPlan.UserId);
            return View(pegPlan);
        }

        // POST: Drafts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(PegPlan pegPlan)
        {

            var DraftPlan = _context.PegPlan.Include(x => x.Autorized).Where(x => x.DraftPlan == pegPlan.DraftPlan && x.PegPlanId != pegPlan.PegPlanId).SingleOrDefault();
            var DraftPlan2 = _context.PegPlan.Where(x => x.Code == pegPlan.Code && x.PegPlanId != pegPlan.PegPlanId).SingleOrDefault();
            if (DraftPlan != null)
            {
                ModelState.AddModelError($"DraftPlan", "error");
                ViewBag.MsgError = "Este dibujo ya existe .";

            }

            if (DraftPlan2 != null)
            {
                ModelState.AddModelError($"Code", "Este codigo ya existe ");

            }

            if (pegPlan.DraftPlan == null)
            {
                ModelState.AddModelError($"DraftPlan", "error");
                ViewBag.MsgError = "Debes seleccionar al menos una casilla para crear el dibujo.";

            }

            List<int> Efila = new List<int>();
            List<int> ECol = new List<int>();

            string coordMalas = "";

            if (pegPlan.DraftPlan != null)
            {
                string[] coordenadas = pegPlan.DraftPlan.Split("|");


                for (int i = 1; i < pegPlan.Ends + 1; i++)
                {
                    for (int j = 1; j < pegPlan.Shafts + 1; j++)
                    {
                        bool tiene = TieneDatos(i, j, coordenadas);
                        if (tiene)
                        {
                            break;
                        }
                        if (j == pegPlan.Shafts)
                        {
                            Efila.Add(i);
                            for (int k = 1; k < pegPlan.Shafts + 1; k++)
                            {
                                coordMalas = coordMalas + $"{i};{k}|";
                            }
                        }
                    }
                }

                for (int j = 1; j < pegPlan.Shafts + 1; j++)
                {
                    for (int i = 1; i < pegPlan.Ends + 1; i++)
                    {
                        bool tiene = TieneDatos(i, j, coordenadas);
                        if (tiene)
                        {
                            break;
                        }
                        if (i == pegPlan.Ends)
                        {
                            ECol.Add(j);

                            for (int k = 1; k < pegPlan.Ends + 1; k++)
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


            if (pegPlan.Repeats != null)
            {
                string[] repeticiones = pegPlan.Repeats.Split("|");
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
                                ModelState.AddModelError($"DraftPlan", "error");
                                ViewBag.MsgError = ($"Existe solapado en las repeticiones .");
                            }
                        }

                    }
                }

            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (pegPlan.Authorized == false)
                    {
                        pegPlan.AuthorizedBy = null;

                    }
                    else
                    {
                        pegPlan.AuthorizedBy = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier).Value);

                    }

                    pegPlan.UserId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                    _context.Update(pegPlan);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PegPlanExists(pegPlan.PegPlanId))
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
            var auditory = await _context.PegPlanAudit.Include(x => x.User).Include(x => x.Draft).Where(x => x.PegPlanId == pegPlan.PegPlanId).OrderByDescending(x => x.OnDay).ToListAsync();
            ViewBag.auditory = auditory;

            ViewBag.hola = pegPlan;
            ViewBag.Efila = Efila;
            ViewBag.ECol = ECol;
            ViewBag.coordMalas = coordMalas;
            return View(pegPlan);
        }

        // GET: Drafts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pegPlan = await _context.PegPlan
                .Include(d => d.User)
                .FirstOrDefaultAsync(m => m.PegPlanId == id);
            if (pegPlan == null)
            {
                return NotFound();
            }

            return View(pegPlan);
        }

        // POST: Drafts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(PegPlan pegPlan)
        {
            var pegPlans = await _context.PegPlan.FindAsync(pegPlan.PegPlanId);
            pegPlans.UserId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            _context.PegPlan.Remove(pegPlans);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PegPlanExists(int id)
        {
            return _context.PegPlan.Any(e => e.PegPlanId == id);
        }

        public IActionResult GetColsDraws(int? Shafts, int? Ends, string data, string repeats)
        {
            return ViewComponent(typeof(DesignDrawViewComponent), new { cols = Shafts, rows = Ends, data, repeats });
        }
        public IActionResult GetModalPegPlan(int? Shafts, int? Ends, string data, string repeats)
        {
            return ViewComponent(typeof(PegPlanViewComponent), new { cols = Shafts, rows = Ends, data, repeats });
        }

        public async Task<IActionResult> ImagePrint(int id)
        {

            var pegPlan = await _context.PegPlan.FindAsync(id);

            pegPlan.wwwrootpath = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}";
            //return PartialView("ReportPrint", _incidente);
            //string fecha_actualizacion = "";
            //string actualizacion_user = "";

            var datosHeaderFooter = new Dictionary<string, string>
            {
                { "codigo_informe", $"{pegPlan.Code}" } };


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
            var pdf = await _generarPDF.GetByteArray("Views/PegPlan/ImagePrint.cshtml", pegPlan);
            var pdfStream = new MemoryStream();
            pdfStream.Write(pdf, 0, pdf.Length);
            pdfStream.Position = 0;
            return new FileStreamResult(pdfStream, "application/pdf");
        }
        public async Task<IActionResult> ImagePrint2(int id)
        {

            var pegPlan = await _context.PegPlan.FindAsync(id);

            pegPlan.wwwrootpath = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}";
            //return PartialView("ReportPrint", _incidente);
            //string fecha_actualizacion = "";
            //string actualizacion_user = "";
            ViewBag.sley = "hola";
            var datosHeaderFooter = new Dictionary<string, string>
            {
                { "codigo_informe", $"{pegPlan.Code}" } };


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
            var pdf = await _generarPDF.GetByteArray("Views/PegPlan/ImagePrint2.cshtml", pegPlan);
            var pdfStream = new MemoryStream();
            pdfStream.Write(pdf, 0, pdf.Length);
            pdfStream.Position = 0;
            return new FileStreamResult(pdfStream, "application/pdf");
        }

        public async Task<IActionResult> ImagePrintExcel(int id)
        {
            LLenarHoja(id);
            LLenarHoja2(id);
            string fileName = "PegPlan";
            using (var exportData = new MemoryStream())
            {
                excelWorkbook.Write(exportData);

                DateTime startDate = DateTime.Now;

                string saveAsFileName = string.Format(fileName + " {0:d}.xls", startDate.ToShortDateString()).ToUpper();

                byte[] bytes = exportData.ToArray();
                return File(bytes, "application/vnd.ms-excel", saveAsFileName);
            }
        }

        public async Task<IActionResult> ImagePrintExcelDibujo(string id, int pegPlanId, int draftId)
        {
            LLenarHoja2(pegPlanId);
            LLenarSley(draftId);
            LlenarMatrizFinal(pegPlanId, draftId);
            string fileName = "Full";
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

            ISheet sheet = excelWorkbook.CreateSheet("PegPlan");
            PegPlan pegPlan = _context.PegPlan.FirstOrDefault(x => x.PegPlanId == id);
            DesignDraw design = DesignDraw(pegPlan.Shafts, pegPlan.Ends, pegPlan.DraftPlan, pegPlan.Repeats);
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
            int col = design.Cols +1;
            foreach (var item in design.Repeats)
            {

                if (design.Cols > 0)
                {
                    NpoiFunctions.NewCell(row1, estiloColumna, col, "R");
                    NpoiFunctions.SetColumn(sheet, col, 4);
                }

                col += 1;

            }
            int r = design.Rows;
            int col2 = design.Cols + 1;
            for (int i = 1; i < design.Rows + 1; i++)
            {
                IRow row = sheet.CreateRow(i + 1);
                row.Height = 3 * 140;
                NpoiFunctions.NewCell(row, estiloColumna, 0, r);
                r -= 1;
                for (int j = 1; j < design.Cols + 1; j++)
                {
                    if (design.Points.Where(p => p.row == i && p.col == j).FirstOrDefault() != null)
                    {
                        NpoiFunctions.NewCell(row, estiloAzul, j, "");
                    }
                    else if (design.Points2.Where(p => p.row == i && p.col == j).FirstOrDefault() != null)
                    {

                    }
                    else
                    {
                        NpoiFunctions.NewCell(row, estiloColumna, j, "");
                    }
                }
                foreach (var item in design.Repeats)
                {
                    if (item.inicio <= i && item.fin >= i)
                    {
                        NpoiFunctions.NewCell(row, estiloRojo, col2, item.cantidad);
                    }
                    else
                    {
                        NpoiFunctions.NewCell(row, estiloColumna, col2, "");
                    }
                    col2++;
                }
                col2 = design.Cols + 1;
            }
        }
        
        private void LLenarHoja2(int id)
        {
            ICellStyle estiloColumna = NpoiFunctions.EstiloColumna(excelWorkbook);
            ICellStyle estiloAzul = NpoiFunctions.EstiloAzul(excelWorkbook);
            ICellStyle estiloRojo = NpoiFunctions.EstiloRojo(excelWorkbook);

            ISheet sheet = excelWorkbook.CreateSheet("PegPlan Full");
            PegPlan pegPlan = _context.PegPlan.FirstOrDefault(x => x.PegPlanId == id);
            DesignDraw design = DesignDraw(pegPlan.Shafts, pegPlan.Ends, pegPlan.DraftPlan, pegPlan.Repeats);
            IRow row1 = sheet.CreateRow(1);
            row1.Height = 3 * 140;
            for (int i = 0; i < matriz.Cols + 1; i++)
            {
                if (matriz.Cols > 0)
                {
                    NpoiFunctions.NewCell(row1, estiloColumna, i, i);
                    NpoiFunctions.SetColumn(sheet, i, 4);
                }
            }
            int r = matriz.Filas.Count();
            int col2 = matriz.Cols + 1;
            int c = 1;
            int h = 1;
               
                foreach (var item in matriz.Filas)
                {

                    IRow row = sheet.CreateRow(h + 1);
                    row.Height = 3 * 140;
                    NpoiFunctions.NewCell(row, estiloColumna, 0, r);
                    r -= 1;

                    foreach (var item2 in item.Columnas)
                    {
                        
                        if (item2.Dato == "SieKipo")
                        {
                            NpoiFunctions.NewCell(row, estiloAzul, c , "");
                        }
                        else
                        {
                            NpoiFunctions.NewCell(row, estiloColumna, c , "");
                        }
                        c++;
                    }
                    c = 1;
                    h++;
                }
        }

        private void LLenarSley(int id)
        {
            ICellStyle estiloColumna = NpoiFunctions.EstiloColumna(excelWorkbook);
            ICellStyle estiloAzul = NpoiFunctions.EstiloAzul(excelWorkbook);
            ICellStyle estiloRojo = NpoiFunctions.EstiloRojo(excelWorkbook);

            ISheet sheet = excelWorkbook.CreateSheet("Draft");
            //var draft = _context.Drafts.First(x => x.DraftPlan == id);
            Draft sley = _context.Draft.FirstOrDefault(x => x.DraftId == id);
            DesignDrawSley design = DesignDrawSleyData(sley.Lenght, sley.Shafts, sley.PlanCode, sley.SleyCode, sley.Repeats);
            IRow row1 = sheet.CreateRow(1);
            row1.Height = 3 * 140;
            for (int i = 0; i < matriz2.ColumnaSleys.Count() + 1; i++)
            {
                if (matriz2.ColumnaSleys.Count() > 0)
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
            for (int i = 1; i < matriz2.Rows + 1; i++)
            {
                IRow row = sheet.CreateRow(fila);
                row.Height = 3 * 140;
                NpoiFunctions.NewCell(row, estiloColumna, 0, r);
                r -= 1;
                fila += 1;
                for (int j = 0; j < matriz2.ColumnaSleys.Count; j++)
                {
                    foreach (var item in matriz2.ColumnaSleys[j].filaSleys)
                    {
                        if (item.fila == i)
                        {
                            if (item.Dato == "SieKipo")
                            {
                                NpoiFunctions.NewCell(row, estiloAzul, j + 1, "");
                            }
                            else
                            {
                                NpoiFunctions.NewCell(row, estiloColumna, j + 1, "");
                            }
                        }
                    }
                }
            }
        }

        private void LlenarMatrizFinal(int id, int draftId)
        {
            ICellStyle estiloColumna = NpoiFunctions.EstiloColumna(excelWorkbook);
            ICellStyle estiloAzul = NpoiFunctions.EstiloAzul(excelWorkbook);
            ICellStyle estiloRojo = NpoiFunctions.EstiloRojo(excelWorkbook);


            ISheet sheet = excelWorkbook.CreateSheet("Dibujo final");
            PegPlan pegPlan = _context.PegPlan.FirstOrDefault(x => x.PegPlanId == id);
            DesignDraw design = DesignDrawFinal(pegPlan.Shafts, pegPlan.Ends, pegPlan.DraftPlan, pegPlan.Repeats, draftId);
            IRow row1 = sheet.CreateRow(1);
            row1.Height = 3 * 140;
            for (int i = 0; i < matrizFinal.ColumnaSleys2.Count() + 1; i++)
            {
                if (matriz.Cols > 0)
                {
                    NpoiFunctions.NewCell(row1, estiloColumna, i, i);
                    NpoiFunctions.SetColumn(sheet, i, 4);
                }
            }
            int r = matriz.Filas.Count();
            int col2 = matriz.Cols + 1;
            int c = 1;
            int h = 1;

            for(int i = 0; i < matriz.Filas.Count; i++)
            {

                IRow row = sheet.CreateRow(h + 1);
                row.Height = 3 * 140;
                NpoiFunctions.NewCell(row, estiloColumna, 0, r);
                r -= 1;

                foreach (var item in matrizFinal.ColumnaSleys2)
                {
                    foreach (var item2 in item.filaSleys2)
                    {
                        if (item2.fila == i + 1)
                        {
                            if (item2.Dato == "SieKipo")
                            {
                                NpoiFunctions.NewCell(row, estiloAzul, c, "");
                            }
                            else
                            {
                                NpoiFunctions.NewCell(row, estiloColumna, c, "");
                            }
                            c++;
                        }
                    }
                }
                c = 1;
                h++;
            }
        }
        
        Matriz matriz;
        public DesignDraw DesignDraw(int cols, int rows, string data, string repeats)
        {
            DesignDraw draw = new DesignDraw
            {
                Cols = cols,
                Rows = rows
            };
            Matriz matriz1 = new Matriz
            {
                Cols = cols,
                Rows = rows
            };

            if (data != null)
            {
                string[] _data = data.Split("|");
                if (data.Length > 1)
                {
                    for (int i = 0; i < _data.Length; i++)
                    {
                        string[] datos = _data[i].Split(";");
                        PointData p = new PointData();
                        p.row = int.Parse(datos[0]);
                        p.col = int.Parse(datos[1]);
                        draw.Points.Add(p);
                    }
                }
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
                        r.fin = (rows - ini) + 1;
                        int fini = int.Parse(datos[1]);
                        r.inicio = (rows - fini) + 1;
                        r.cantidad = int.Parse(datos[2]);
                        draw.Repeats.Add(r);
                    }
                }

            }



            #region Matriz 

            List<Fila> filas = new List<Fila>();
            int h = 1;
            while (h <= rows)
            {
                if (IsRepeat(draw.Repeats, h))
                {
                    int cursor = h;
                    int inicio = h;
                    int fin = RepeatFin(draw.Repeats, h);
                    int cantidad = RepeatCant(draw.Repeats, h);
                    int repeticion = 0;

                    while (repeticion < cantidad)
                    {
                        while (cursor <= fin)
                        {

                            Fila fila = new Fila();
                            fila.row = cursor;
                            fila.Columnas = GetCols(draw, cursor);
                            filas.Add(fila);
                            matriz1.Filas = filas;
                            cursor++;
                        }
                        repeticion++;
                        cursor = h;
                    }
                    h = h + ((fin - inicio) + 1);
                }
                else
                {
                    Fila fila = new Fila();
                    fila.row = h;
                    fila.Columnas = GetCols(draw, h);
                    filas.Add(fila);
                    matriz1.Filas = filas;
                    h++;
                }
            }
            matriz = matriz1;
            #endregion
            return draw;
        }

        MatrizSley matriz2;
        public DesignDrawSley DesignDrawSleyData(int cols, int rows, string data, string sley, string repeats)
        {
            string coordMalas2 = "";
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
            matriz2 = matrizSley;
            #endregion


            return draw;
        }
        MatrizSley2 matrizFinal;
        public DesignDraw DesignDrawFinal(int cols, int rows, string data, string repeats ,int draftId)
        {
            var pegPlan = _context.PegPlan.First(x => x.DraftPlan == data);
            var sleys = _context.Draft.First(x => x.DraftId == draftId);

            int suma = 0;
            if (repeats != null)
            {
                string[] repeticiones = repeats.Split("|");

                foreach (var item3 in repeticiones)
                {
                    string[] rep = item3.Split(",");
                    int inicio = Convert.ToInt32(rep[0]);
                    int fin = Convert.ToInt32(rep[1]);
                    int cantidad = Convert.ToInt32(rep[2]);
                    int cant = cantidad - 1;
                    var G = (fin - inicio + 1) * cant;
                    suma = suma + G;
                }

            }
            DesignDrawSley drawSley = new DesignDrawSley
            {
                Cols = sleys.Lenght,
                Rows = sleys.Shafts,

            };

            DesignDraw draw = new DesignDraw
            {
                Cols = cols,
                Rows = rows
            };

            Matriz matriz = new Matriz
            {
                Cols = cols,
                Rows = rows
            };
            MatrizSley matrizSley = new MatrizSley
            {
                Cols = sleys.Lenght,
                Rows = sleys.Shafts,
            };
            MatrizSley2 matrizSley2 = new MatrizSley2
            {
                Cols = sleys.Lenght,
                Rows = sleys.Shafts,
            };


            #region Matriz 
            if (data != null)
            {
                string[] _data = data.Split("|");
                if (data.Length > 1)
                {
                    for (int l = 0; l < _data.Length; l++)
                    {
                        string[] datos = _data[l].Split(";");
                        PointData p = new PointData();
                        p.row = int.Parse(datos[0]);
                        p.col = int.Parse(datos[1]);
                        draw.Points.Add(p);
                    }
                }
            }
            
            if (repeats != null)
            {

                string[] _repeats = repeats.Split("|");
                if (repeats.Length > 1)
                {
                    for (int a = 0; a < _repeats.Length; a++)
                    {
                        string[] datos = _repeats[a].Split(",");
                        Repeats r = new Repeats();
                        int ini = int.Parse(datos[0]);
                        r.fin = (rows - ini) + 1;
                        int fini = int.Parse(datos[1]);
                        r.inicio = (rows - fini) + 1;
                        r.cantidad = int.Parse(datos[2]);
                        draw.Repeats.Add(r);
                    }
                }

            }
            List<Fila> filas = new List<Fila>();
            int i = 1;
            while (i <= rows)
            {
                if (IsRepeat(draw.Repeats, i))
                {
                    int cursor = i;
                    int inicio = i;
                    int fin = RepeatFin(draw.Repeats, i);
                    int cantidad = RepeatCant(draw.Repeats, i);
                    int repeticion = 0;

                    while (repeticion < cantidad)
                    {
                        while (cursor <= fin)
                        {

                            Fila fila = new Fila();
                            fila.row = cursor;
                            fila.Columnas = GetCols(draw, cursor);
                            filas.Add(fila);
                            matriz.Filas = filas;
                            cursor++;
                        }
                        repeticion++;
                        cursor = i;
                    }
                    i = i + ((fin - inicio) + 1);
                }
                else
                {
                    Fila fila = new Fila();
                    fila.row = i;
                    fila.Columnas = GetCols(draw, i);
                    filas.Add(fila);
                    matriz.Filas = filas;
                    i++;
                }
            }
            ViewBag.matriz = matriz;
            #endregion




            #region Matriz Sley
            if (sleys.PlanCode != null)
            {
                string[] _data = sleys.PlanCode.Split("|");
                if (sleys.PlanCode.Length > 1)
                {
                    for (int l = 0; l < _data.Length; l++)
                    {
                        string[] datos = _data[l].Split(";");
                        PointDataSley p = new PointDataSley();
                        p.row = int.Parse(datos[0]);
                        p.col = int.Parse(datos[1]);
                        drawSley.PointDataSley.Add(p);
                    }
                }
            }
            if (sleys.Repeats != null)
            {

                string[] _repeats = sleys.Repeats.Split("|");
                if (sleys.Repeats.Length > 1)
                {
                    for (int a = 0; a < _repeats.Length; a++)
                    {
                        string[] datos = _repeats[a].Split(",");
                        Repeats r = new Repeats();
                        int ini = int.Parse(datos[0]);
                        r.inicio = int.Parse(datos[0]);
                        int fini = int.Parse(datos[1]);
                        r.fin = int.Parse(datos[1]);
                        r.cantidad = int.Parse(datos[2]);
                        drawSley.Repeats.Add(r);
                    }
                }

            }
            List<ColumnaSley> columnaSleys = new List<ColumnaSley>();
            int j = 1;
            while (j <= sleys.Lenght)
            {
                if (IsRepeatSley(drawSley.Repeats, j))
                {
                    int cursor = j;
                    int inicio = j;
                    int fin = RepeatFinSley(drawSley.Repeats, j);
                    int cantidad = RepeatCantSley(drawSley.Repeats, j);
                    int repeticion = 0;

                    while (repeticion < cantidad)
                    {
                        while (cursor <= fin)
                        {
                            ColumnaSley columnaSley = new ColumnaSley();
                            columnaSley.col = cursor;
                            columnaSley.filaSleys = GetColsSley(drawSley, cursor);
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
                    columnaSley.filaSleys = GetColsSley(drawSley, j);
                    columnaSleys.Add(columnaSley);
                    matrizSley.ColumnaSleys = columnaSleys;
                    j++;
                }
            }
            ViewBag.matrizSley = matrizSley;
            #endregion
            #region Ultima matriz
            List<ColumnaSley2> columnaSleys2 = new List<ColumnaSley2>();
            int z = 1;
            int zx = 1;
            foreach (var item in matrizSley.ColumnaSleys)
            {
                foreach (var item2 in item.filaSleys)
                {
                    ColumnaSley2 columnaSley2 = new ColumnaSley2();
                    if (item2.Dato == "SieKipo")
                    {

                        List<FilaSley2> filaSleys2 = new List<FilaSley2>();
                        foreach (var item3 in matriz.Filas)
                        {
                            columnaSley2.col = z;
                            foreach (var item4 in item3.Columnas)
                            {
                                if (item4.col == (sleys.Shafts - item2.fila) + 1)
                                {
                                    FilaSley2 filaSley2 = new FilaSley2();
                                    filaSley2.fila = zx;
                                    filaSley2.Dato = item4.Dato;
                                    filaSleys2.Add(filaSley2);
                                    zx++;
                                }

                            }

                            columnaSley2.filaSleys2 = filaSleys2;
                        }
                        zx = 1;
                        columnaSleys2.Add(columnaSley2);
                        z++;
                        matrizSley2.ColumnaSleys2 = columnaSleys2;
                    }
                }
            }
            matrizFinal = matrizSley2;
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

        public static bool IsRepeat(List<Repeats> repeats, int fila)
        {
            foreach (var item in repeats)
            {
                if (item.inicio == fila)
                {
                    return true;
                }
            }
            return false;
        }
        public static int RepeatFin(List<Repeats> repeats, int fila)
        {
            foreach (var item in repeats)
            {
                if (item.inicio == fila)
                {
                    return item.fin;
                }
            }
            return 0;
        }
        public static int RepeatCant(List<Repeats> repeats, int fila)
        {
            foreach (var item in repeats)
            {
                if (item.inicio == fila)
                {
                    return item.cantidad;
                }
            }
            return 0;
        }
        public static int GetMiLetra(List<PointData> pointDatas, int fila, int columna)
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
        public static List<Columna> GetCols(DesignDraw draw, int fila)
        {
            int go = 0;
            List<Columna> columnas = new List<Columna>();
            foreach (var item in draw.Points)
            {
                if (item.row == fila && go == 0)
                {
                    for (int j = 1; j < draw.Cols + 1; j++)
                    {
                        Columna columna = new Columna();
                        columna.col = j;
                        columna.Dato = GetMiLetra(draw.Points, item.row, j) == 1 ? "SieKipo" : "No tiene";
                        columnas.Add(columna);
                    }
                    go++;
                }
            }
            return columnas;
        }
    }
}
