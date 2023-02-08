using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TCS.EF;
using TCS.EF.Entidades;
using Wkhtmltopdf.NetCore;

namespace TCS.WebUI.Controllers
{
    public class PegPlanDraftController : Controller
    {
        private readonly TCSContext _context;
        readonly IGeneratePdf _generarPDF;
        public PegPlanDraftController(TCSContext context, IGeneratePdf generatePdf)
        {
            _context = context;
            _generarPDF = generatePdf;
        }

        // GET: DraftSleys
        public async Task<IActionResult> Index()
        {
            var tCSContext = _context.PegPlanDraft.Include(d => d.PegPlan).Include(d => d.Draft);
            return View(await tCSContext.ToListAsync());
        }

        // GET: DraftSleys/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var draftSley = await _context.PegPlanDraft
                .Include(d => d.PegPlan)
                .Include(d => d.Draft)
                .FirstOrDefaultAsync(m => m.PegPlanId == id);
            if (draftSley == null)
            {
                return NotFound();
            }

            return View(draftSley);
        }

        // GET: DraftSleys/Create
        public IActionResult Create()
        {
            ViewData["PegPlanId"] = new SelectList(_context.PegPlan, "PegPlanId", "Code");
            ViewData["DraftId"] = new SelectList(_context.Draft, "DraftId", "Code");
            return View();
        }

        // POST: DraftSleys/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PegPlanId,DraftId")] PegPlanDraft draftSley)
        {
            if (ModelState.IsValid)
            {
                _context.Add(draftSley);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PegPlanId"] = new SelectList(_context.PegPlan, "PegPlanId", "Code", draftSley.PegPlanId);
            ViewData["DraftId"] = new SelectList(_context.Draft, "DraftId", "Code", draftSley.DraftId);
            return View(draftSley);
        }

        // GET: DraftSleys/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var draftSley = await _context.PegPlanDraft.FindAsync(id);
            if (draftSley == null)
            {
                return NotFound();
            }
            ViewData["PegPlanId"] = new SelectList(_context.PegPlan, "PegPlanId", "Code", draftSley.PegPlanId);
            ViewData["DraftId"] = new SelectList(_context.Draft, "DraftId", "Code", draftSley.DraftId);
            return View(draftSley);
        }

        // POST: DraftSleys/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PegPlanId,DraftId")] PegPlanDraft draftSley)
        {
            if (id != draftSley.PegPlanId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(draftSley);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DraftSleyExists(draftSley.PegPlanId))
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
            ViewData["PegPlanId"] = new SelectList(_context.PegPlan, "PegPlanId", "Code", draftSley.PegPlanId);
            ViewData["DraftId"] = new SelectList(_context.Draft, "DraftId", "Code", draftSley.DraftId);
            return View(draftSley);
        }

        // GET: DraftSleys/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var draftSley = await _context.PegPlanDraft
                .Include(d => d.PegPlan)
                .Include(d => d.Draft)
                .FirstOrDefaultAsync(m => m.PegPlanId == id);
            if (draftSley == null)
            {
                return NotFound();
            }

            return View(draftSley);
        }

        // POST: DraftSleys/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var draftSley = await _context.PegPlanDraft.FindAsync(id);
            _context.PegPlanDraft.Remove(draftSley);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DraftSleyExists(int id)
        {
            return _context.PegPlanDraft.Any(e => e.PegPlanId == id);
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
    }
}
