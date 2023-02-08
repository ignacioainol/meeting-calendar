using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Linq;
using System.Threading.Tasks;
using TCS.EF;
using TCS.WebUI.Interface;
using Microsoft.EntityFrameworkCore;
using TCS.EF.Entidades;
using System;

namespace TCS.WebUI.Controllers
{
    public class PerformanceEvaluationController : Controller
    {
        private readonly ISoftlandService _softlandService;
        private readonly TCSContext _context;
        public PerformanceEvaluationController(TCSContext context, ISoftlandService softlandService)
        {
            _softlandService = softlandService;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            string userid = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var usuario = await _context.Users
            .Where(u => u.UserId.ToString() == userid)
            .FirstOrDefaultAsync();
            ViewBag.Usuario = usuario;
            return View();
        }

        [Authorize]
        [HttpGet]
        [Route("api/persona/listado")]

        public async Task<IActionResult> DropdownTrabajadores()
        {
            return Ok(await _softlandService.GETCurrentEmployee());
        }

        [HttpGet]
        [Route("api/persona/{rut}")]
        public async Task<IActionResult> TrabajadorRut(string rut)
        {
            return Ok(await _softlandService.GETEmployeeRut(rut));
        }

        [HttpGet]
        [Route("api/performanceevaluation/buscar/{rut}")]
        public async Task<IActionResult> BuscarUltimaEvaluacion(string rut)
        {
            if(rut == null)
            {
                return NotFound("faslsts rut");
            }
           
            var evaluacion = await _context.PerformanceEvaluation.Where(x => x.Rut == rut).OrderByDescending(c => c.EvaluationDate).FirstOrDefaultAsync();
            return Ok(evaluacion);
        }

        [HttpPost]
        [Route("performanceevaluation/create")]
        public async Task<IActionResult> Create([FromBody] PerformanceEvaluation performanceevaluation)
        {
            performanceevaluation.AddDte = DateTime.Now;
            performanceevaluation.EvaluationDate = DateTime.Now;
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(performanceevaluation);
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {

                    throw;
                }
               
                return RedirectToAction(nameof(Index));
            }

            return View(performanceevaluation);
        }

        [HttpPost]
        [Route("performanceevaluation/actualizar/{id}")]
        public async Task<IActionResult> Edit(int id,[FromBody] PerformanceEvaluation request)
        {

            PerformanceEvaluation evaluation = await _context.PerformanceEvaluation.FindAsync(request.PerformanceEvaluationId);

            evaluation.PerformanceEvaluationId = request.PerformanceEvaluationId;
            evaluation.ScoreCYE = request.ScoreCYE;
            evaluation.ScoreNFP = request.ScoreNFP;
            evaluation.ScorePRO = request.ScorePRO;
            evaluation.TotalPerformanceTask = request.TotalPerformanceTask;
            evaluation.ScoreRA = request.ScoreRA;
            evaluation.ScoreRLM = request.ScoreRLM;
            evaluation.ScoreAUS = request.ScoreAUS;
            evaluation.ScoreATR = request.ScoreATR;
            evaluation.ScoreAMO = request.ScoreAMO;
            evaluation.ScoreAL = request.ScoreAL;
            evaluation.TotalAttendance = request.TotalAttendance;
            evaluation.ScoreRES = request.ScoreRES;
            evaluation.ScoreSA = request.ScoreSA;
            evaluation.ScoreAHE = request.ScoreAHE;
            evaluation.ScoreAHS = request.ScoreAHS;
            evaluation.ScoreAHC = request.ScoreAHC;
            evaluation.TotalAttitudes = request.TotalAttitudes;
            evaluation.ScoreIC = request.ScoreIC;
            evaluation.ScoreCMT = request.ScoreCMT;
            evaluation.ScoreCDA = request.ScoreCDA;
            evaluation.TotalSkills = request.TotalSkills;
            evaluation.Observations = request.Observations;
            evaluation.TotalFinal = request.TotalFinal;
            evaluation.EvaluationDate = request.EvaluationDate;
            evaluation.Rut = request.Rut;
            evaluation.Name = request.Name;
            evaluation.LastName = request.LastName;
            evaluation.Complete = request.Complete;
            evaluation.CodiCC = request.CodiCC;
            evaluation.ChgUsr = request.ChgUsr;
            evaluation.ChgDte = DateTime.Now;

            if (id != evaluation.PerformanceEvaluationId)
            {
                return NotFound();
            }


            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(evaluation);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (!PerformanceEvaluationExists(evaluation.PerformanceEvaluationId))
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
            return View(request);
    }

        private bool PerformanceEvaluationExists(int id)
        {
            return _context.PerformanceEvaluation.Any(e => e.PerformanceEvaluationId == id);
        }

        public async Task<IActionResult> ScoresDeptos()
        {
            return View();
        }

        [HttpGet]
        [Route("performanceevaluation/scorebydepto/{codiCC}")]
        public async Task<IActionResult> BuscarPuntajesPorDepto(string codiCC)
        {
            if (codiCC == null)
            {
                return NotFound("No hay evaluaciones para ningún trabajador del departamento");
            }

            var evaluaciones = await _context.PerformanceEvaluation.Where(x => x.CodiCC == codiCC).ToListAsync();
            return Ok(evaluaciones);
        }
    }
}
