using INSPECTION.EF;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using INSPECTION.EF.Entidades;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Globalization;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace TCS.WebUI.Controllers
{
    [Authorize(Roles = "Inspection-Admin, Inspection-Private, Inspection-Public")]
    public class InspectionController : Controller
    {
        private readonly INSPECTIONContext _context;
        private readonly User _usuario;

        public InspectionController(INSPECTIONContext context, IHttpContextAccessor httpContextAccessor)
        {
            var UserName = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("es-CL");
            _context = context;
            _usuario = _context.Users
               .Where(u => u.CodeUser== UserName)
               .SingleOrDefault();
        }

        public async Task<IActionResult> InspectPiece()
        {
            return View();
        }

        public async Task<IActionResult> GetPiece(string pieceid)
        {
            var piece =await _context.TmsPieces
                            .Include(x=>x.InspectedPiece)
                            .ThenInclude(x => x.PieceSummaries)
                                .ThenInclude(x => x.PieceFailsSummaries)
                            .Include(x => x.InspectedPiece)
                                .ThenInclude(x => x.Cuts)
                            .Include(x => x.InspectedPiece)
                                .ThenInclude(x => x.PieceFailures)
                            .AsNoTracking()
                            .FirstOrDefaultAsync(x=>x.CodeTmsPiece == pieceid);
            if (piece == null)
            {
                return Json("nodata");
            }
            else
            {
                return Json(piece);
            }
        }

        public async Task<IActionResult> GetFailure(int codefailure)
        {
            var failure = await _context.Failures
                            .AsNoTracking()
                            .FirstOrDefaultAsync(x => x.CodeFailure == codefailure);

            return Json(failure.Description);
        }

        [HttpPost]
        public async Task<IActionResult> SaveChanges([FromBody]string jsonToSend)
        {
            TmsPiece piece = JsonConvert.DeserializeObject<TmsPiece>(jsonToSend);

            _context.PieceFailsSummaries.RemoveRange(_context.PieceFailsSummaries.Where(x=>x.CodeTmsPiece == piece.CodeTmsPiece));
            _context.PieceSummaries.RemoveRange(_context.PieceSummaries.Where(x => x.CodeTmsPiece == piece.CodeTmsPiece));
            //_context.PieceFailures.RemoveRange(_context.PieceFailures.Where(x => x.CodeTmsPiece == piece.CodeTmsPiece));

            await _context.SaveChangesAsync();

            InspectedPiece inspectedPiece = piece.InspectedPiece;

            if (_context.InspectedPieces.Any(x=>x.CodeTmsPiece == piece.CodeTmsPiece))
            {
                inspectedPiece.Reinspectedby = _usuario == null ? "CRS80062" : _usuario.CodeUser;
                inspectedPiece.ReinspectedDate = DateTime.Now;
                inspectedPiece.Reinspected = true;

                _context.Entry(inspectedPiece).State = EntityState.Modified;
                _context.InspectedPieces.Update(inspectedPiece);
                await _context.SaveChangesAsync();
            }
            else
            {
                inspectedPiece.Inspectedby = _usuario == null ? "CRS80062" : _usuario.CodeUser;
                inspectedPiece.InspectionDate = DateTime.Now;
                inspectedPiece.Reinspected = false;

                _context.InspectedPieces.Add(inspectedPiece);
                await _context.SaveChangesAsync();
            }
            //cortes
            List<Cut> cuts = new ();

            if(inspectedPiece.Cuts.Count > 0)
            {
                cuts = inspectedPiece.Cuts.ToList(); ;
            }
            if (cuts.Count > 0)
            {
                foreach (var i in cuts)
                {
                    if (!_context.Cuts.Any(x => x.CodeCut == i.CodeCut))
                    {
                        _context.Cuts.Add(i);
                        await _context.SaveChangesAsync();
                    }
                }
            }

            var cutsDb = _context.Cuts.Where(x => x.CodeTmsPiece == piece.CodeTmsPiece);
            var cutsEliminados = cutsDb.Where(x => !cuts.Select(y => y.CodeCut).Contains(x.CodeCut));

            if (cutsEliminados.Any()) {
                _context.Cuts.RemoveRange(cutsEliminados);
                await _context.SaveChangesAsync();
            }

            //fin cortes

            //fallas
            List<PieceFailure> failures = new List<PieceFailure>();

            if (inspectedPiece.PieceFailures.Any())
            {
                failures = inspectedPiece.PieceFailures.OrderBy(x=>x.InitMeter).ToList();
            }
            if(failures.Any())
            {
                foreach (var i in failures)
                {
                    if (!_context.PieceFailures.Any(x => x.CodePieceFailure == i.CodePieceFailure))
                    {
                        _context.PieceFailures.Add(i);
                        await _context.SaveChangesAsync();
                    }
                }
            }

            var failuresDb = _context.PieceFailures.Where(x => x.CodeTmsPiece == piece.CodeTmsPiece);
            var failuresEliminados = failuresDb.Where(x => !failures.Select(y => y.CodePieceFailure).Contains(x.CodePieceFailure));

            if (failuresEliminados.Any())
            {
                _context.PieceFailures.RemoveRange(failuresEliminados);
                await _context.SaveChangesAsync();
            }

            //fin fallas

            //sumary

            if (cuts.Count > 0)
            {
                byte count = 0;
                for(int i = 0; i < cuts.Count; i++)
                {
                    if (i == 0 && cuts[i].InitMeter > 0)
                    {
                        count++;
                        PieceSummary pieceSum2 = new();
                        pieceSum2.CodeTmsPiece = cuts[i].CodeTmsPiece;
                        pieceSum2.PieceNumber = count;
                        pieceSum2.Quality = inspectedPiece.PieceQuality;
                        pieceSum2.Meters = cuts[i + 1].InitMeter - cuts[i].FinalMeter;
                        pieceSum2.NetMeters = cuts[i + 1].InitMeter - cuts[i].FinalMeter;
                        pieceSum2.Creason = inspectedPiece.Creason;
                        _context.PieceSummaries.Add(pieceSum2);
                        _context.SaveChanges();
                    }

                    count++;
                    PieceSummary pieceSum = new();
                    pieceSum.CodeTmsPiece = cuts[i].CodeTmsPiece;
                    pieceSum.PieceNumber = count;
                    pieceSum.Quality = cuts[i].Quality;
                    pieceSum.Meters = cuts[i].FinalMeter - cuts[i].InitMeter;
                    pieceSum.NetMeters = 0;
                    pieceSum.Creason = cuts[i].Cause;
                    _context.PieceSummaries.Add(pieceSum);
                    _context.SaveChanges();

                    if (inspectedPiece.FinalMeters > cuts[i].FinalMeter && i +1 == cuts.Count)
                    {
                        count++;
                        PieceSummary pieceSum2 = new();
                        pieceSum2.CodeTmsPiece = cuts[i].CodeTmsPiece;
                        pieceSum2.PieceNumber = count;
                        pieceSum2.Quality = inspectedPiece.PieceQuality;
                        pieceSum2.Meters = inspectedPiece.FinalMeters - cuts[i].FinalMeter;
                        pieceSum2.NetMeters = inspectedPiece.FinalMeters - cuts[i].FinalMeter;
                        pieceSum2.Creason = inspectedPiece.Creason;
                        _context.PieceSummaries.Add(pieceSum2);
                        _context.SaveChanges();
                    }
                    else if(inspectedPiece.FinalMeters > cuts[i].FinalMeter && i + 1 < cuts.Count)
                    {
                        count++;
                        PieceSummary pieceSum2 = new();
                        pieceSum2.CodeTmsPiece = cuts[i].CodeTmsPiece;
                        pieceSum2.PieceNumber = count;
                        pieceSum2.Quality = inspectedPiece.PieceQuality;
                        pieceSum2.Meters = cuts[i+1].InitMeter - cuts[i].FinalMeter;
                        pieceSum2.NetMeters = cuts[i + 1].InitMeter - cuts[i].FinalMeter;
                        pieceSum2.Creason = inspectedPiece.Creason;
                        _context.PieceSummaries.Add(pieceSum2);
                        _context.SaveChanges();
                    }
                }
            }
            else
            {
                PieceSummary pieceSum = new();
                pieceSum.CodeTmsPiece = inspectedPiece.CodeTmsPiece;
                pieceSum.PieceNumber = 1;
                pieceSum.Quality = inspectedPiece.PieceQuality;
                pieceSum.Yellow = (byte)failures.Count(x=>x.ColourBonus == "A");
                pieceSum.White = (byte)failures.Count(x => x.ColourBonus == "B");
                pieceSum.Green = failures.Where(x => x.ColourBonus == "V").Sum(x => x.BonusQuantity);
                pieceSum.Meters = inspectedPiece.FinalMeters;
                pieceSum.NetMeters = inspectedPiece.FinalMeters;
                pieceSum.Creason = inspectedPiece.Creason;
                pieceSum.Bonus = (failures.Count(x => x.ColourBonus == "A") * 0.2)
                    + (failures.Count(x => x.ColourBonus == "B") * 0.1)
                    + (failures.Where(x => x.ColourBonus == "V").Sum(x=>x.BonusQuantity));
                //piecesSums.Add(pieceSum);
                _context.PieceSummaries.Add(pieceSum);
                _context.SaveChanges();
            }

            List<PieceSummary> pieceSumary =await _context.PieceSummaries.Where(x => x.CodeTmsPiece == piece.CodeTmsPiece).OrderBy(x=>x.PieceNumber).ToListAsync();

            if (failures.Count > 0)
            {
                foreach (var i in failures)
                {
                    PieceFailsSummary piecesFailSum = new();
                    piecesFailSum.Bonus = i.BonusQuantity;
                    piecesFailSum.CodeFailure = i.CodeFailure;
                    piecesFailSum.CodeTmsPiece = i.CodeTmsPiece;
                    piecesFailSum.ColourBonus = i.ColourBonus;
                    piecesFailSum.Mapping = i.Mapping;
                    //piecesFailSum.PieceSummary = _context.PieceSummaries.FirstOrDefault(x=>x.CodeTmsPiece == "asd" && x.PieceNumber == 12);

                    double metrosT = 0;

                    foreach (var i2 in pieceSumary)
                    {
                        metrosT += i2.Meters;
                        if(i.InitMeter <= metrosT)
                        {
                            piecesFailSum.PieceNumber = i2.PieceNumber;
                            break;
                        }
                    }

                    //piecesFailSum.PieceNumber = pieceSumary.Where(x=>x.Meters );
                    piecesFailSum.Meter = i.InitMeter;

                    _context.PieceFailsSummaries.Add(piecesFailSum);
                    _context.SaveChanges();
                }

                //update sumario
                if(pieceSumary.Count >1)
                {
                    double metrosT = 0;
                    double metrosA = 0;
                    foreach (var i in pieceSumary)
                    {
                        metrosT += i.Meters;
                        i.Yellow = (byte)failures.Count(x => x.ColourBonus == "A" && x.InitMeter <= metrosT && x.InitMeter >= metrosA);
                        i.White = (byte)failures.Count(x => x.ColourBonus == "B" && x.InitMeter <= metrosT && x.InitMeter >= metrosA);
                        i.Green = failures.Where(x => x.ColourBonus == "V" && x.InitMeter <= metrosT && x.InitMeter >= metrosA).Sum(x => x.BonusQuantity);
                        i.Bonus = failures.Where(x => x.ColourBonus == "A" && x.InitMeter <= metrosT && x.InitMeter >= metrosA).Sum(x=>x.BonusQuantity)
                                + failures.Where(x => x.ColourBonus == "B" && x.InitMeter <= metrosT && x.InitMeter >= metrosA).Sum(x => x.BonusQuantity)
                                + failures.Where(x => x.ColourBonus == "V" && x.InitMeter <= metrosT && x.InitMeter >= metrosA).Sum(x => x.BonusQuantity);
                        metrosA += i.Meters;
                        _context.PieceSummaries.Update(i);
                        _context.SaveChanges();
                    }
                }

                //update[INSPECTED_PIECE]
                inspectedPiece.MFails = cuts.Sum(x => x.FinalMeter - x.InitMeter);
                inspectedPiece.MYellow = (int)failures.Count(x => x.ColourBonus == "A");
                inspectedPiece.MWhite = (int)failures.Count(x => x.ColourBonus == "B");
                inspectedPiece.MGreen = failures.Where(x => x.ColourBonus == "V").Sum(x => x.BonusQuantity);
                _context.InspectedPieces.Update(inspectedPiece);
                _context.SaveChanges();
            }

            if (piece == null)
            {
                return Json("nodata");
            }
            else
            {
                return Json(piece);
            }
        }

        public async Task<IActionResult> RetainedPiece()
        {
            return View();
        }
        public async Task<IActionResult> UnshippedPiece()
        {
            return View();
        }

        public async Task<IActionResult> GetUnshippedPiece(string codigo, string cliente)
        {
            var unshippedPieces = await _context.InspectedPieces
                              .Include(x => x.CodeTmsPieceNavigation)
                              .Where(x => !x.Dispatch)
                              .Where(x => (!string.IsNullOrEmpty(codigo)) ? x.CodeTmsPiece == codigo : true)
                              .Where(x => (!string.IsNullOrEmpty(cliente)) ? x.CodeTmsPieceNavigation.NameCustomer.Contains(cliente) : true)
                              .Select(x=> new {x.CodeTmsPiece, x.InspectionDate, x.PieceQuality })
                              .OrderByDescending(x => x.InspectionDate)
                              .ToListAsync();
            return Json(unshippedPieces);
        }

        public async Task<IActionResult> GetRetainedPiece(string codigo, string cliente)
        {
            var unshippedPieces = await _context.InspectedPieces
                              .Include(x => x.CodeTmsPieceNavigation)
                              .Where(x => !x.Dispatch)
                              .Where(x => (!string.IsNullOrEmpty(codigo)) ? x.CodeTmsPiece == codigo : true)
                              .Where(x => (!string.IsNullOrEmpty(cliente)) ? x.CodeTmsPieceNavigation.NameCustomer.Contains(cliente) : true)
                              .Select(x => new { x.CodeTmsPiece, x.InspectionDate, x.PieceQuality })
                              .OrderByDescending(x => x.InspectionDate)
                              .ToListAsync();
            return Json(unshippedPieces);
        }

        public async Task<IActionResult> GetPieceInfo(string pieceNumber)
        {
            var unshippedPiece = await _context.InspectedPieces
                              .Include(x => x.CodeTmsPieceNavigation)
                              .Where(x => x.CodeTmsPiece == pieceNumber)
                              .Select(x => new { x.CodeTmsPiece, x.InspectionDate, x.PieceQuality, x.CodeTmsPieceNavigation.NameCustomer })
                              .FirstOrDefaultAsync();
            return Json(unshippedPiece);
        }

        public async Task<IActionResult> GetPieceInfoDetails(string pieceNumber)
        {
            var piece = await _context.InspectedPieces
                              .Include(x => x.CodeTmsPieceNavigation)
                              .Include(x => x.PieceSummaries)
                              .Where(x => x.CodeTmsPiece == pieceNumber)
                              .Select(x => new { x.CodeTmsPiece, x.Inspectedby,
                                  Retazos = x.PieceSummaries.Count, Bonus = x.PieceSummaries.Sum(x=>x.Bonus), x.Cuts.Count,
                                  yellow = x.PieceSummaries.Sum(x => x.Yellow),
                                  green = x.PieceSummaries.Sum(x => x.Green),
                                  white = x.PieceSummaries.Sum(x => x.White)
                              })
                              .FirstOrDefaultAsync();
            return Json(piece);
        }

        public async Task<IActionResult> Binnacle()
        {
            ViewBag.User = _usuario == null? "CRS80062" : _usuario.CodeUser;
            return View();
        }

        public async Task<IActionResult> GetNoInspectedPiece()
        {
            var noInspectedPieces = await _context.TmsPieces
                              .Include(x => x.InspectedPiece)
                              .Where(x => x.InspectedPiece == null)
                              .Select(x => new { x.CodeTmsPiece, x.Quality, x.Design, x.Shade, x.NameCustomer, x.DateScann })
                              .OrderByDescending(x => x.DateScann)
                              .ToListAsync();
            return Json(noInspectedPieces);
        }

        public async Task<IActionResult> GetInspectedPieces(DateTime? since, DateTime? until, string inspectedby, bool isAllInspector)
        {
            var inspectedPieces = await _context.InspectedPieces
                              .Include(x => x.CodeTmsPieceNavigation)
                              .Where(x => (since != null) ? x.InspectionDate >= since: true)
                              .Where(x => (until != null) ? x.InspectionDate <= Convert.ToDateTime(until).AddHours(24) : true)
                              .Where(x => (!isAllInspector) ? x.Inspectedby == inspectedby : true)
                              .Select(x => new { x.CodeTmsPiece, x.InspectionDate, x.FinalMeters, x.FinalWeigth, x.FinalWidth, x.PieceQuality, x.Dispatch, x.Inspectedby })
                              .OrderByDescending(x => x.InspectionDate)
                              .ToListAsync();

            return Json(inspectedPieces);
        }

        public async Task<IActionResult> NewPiece()
        {
            return View();
        }

        public async Task<IActionResult> Reports()
        {
            List<dynamic> ReportList = new();
            ReportList.Add(new { ReportId = 1, ReportType = "DetailedReport" });
            ReportList.Add(new { ReportId = 2, ReportType = "Summary" });
            ReportList.Add(new { ReportId = 3, ReportType = "InspectorSummary" });
            ReportList.Add(new { ReportId = 4, ReportType = "Report" });
            ReportList.Add(new { ReportId = 5, ReportType = "DailySummary" });
            ReportList.Add(new { ReportId = 6, ReportType = "FailureA" });
            ReportList.Add(new { ReportId = 7, ReportType = "FailureC" });

            ViewBag.ReportsTypes = new SelectList(ReportList, "ReportId", "ReportType", 1);

            return View();
        }
    }
}
