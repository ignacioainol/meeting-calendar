using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TCS.EF;
using TCS.WebUI.Models;
using TCS.EF.Entidades;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Resources;
using System.Reflection;
using NPOI.HSSF.UserModel;
using System.Security.Claims;
using System.IO;
using TCS.WebUI.Helpers;
using NPOI.SS.UserModel;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;
using TCS.WebUI.Extensions;
using TCS.WebUI.Services;

namespace TCS.WebUI.Controllers
{
    [Authorize(Roles = "Weaving-Admin, Weaving-Private, Weaving-Public")]
    public class WeavingController : Controller
    {
        private readonly TCSContext _context;
        private readonly TmsService _tmsService;
        private static List<ShiftPattern> _shiftPattern;
        private static List<Worker> _worker;
        private static List<WorkerPosition> _workerPosition;
        private static List<WeaverAssignation> _weaverAssignation;
        private static List<WorkerGroup> _workerGroup;
        private static List<LoomGroup> _loomGroup;
        private static List<Loom> _loom;
        static HSSFWorkbook excelWorkbook;
        static ResourceManager rm = new ResourceManager("TCS.WebUI.Resources.SharedResource", Assembly.GetExecutingAssembly());
        public WeavingController(TCSContext context, TmsService tmsService)
        {
            _context = context;
            _tmsService = tmsService;
            excelWorkbook = new HSSFWorkbook();
            _shiftPattern = _context.ShiftPattern.ToList();
            _worker = _context.Worker.ToList();
            _workerPosition = _context.WorkerPosition.ToList();
            _weaverAssignation = _context.WeaverAssignation.ToList();
            _workerGroup = _context.WorkerGroup.ToList();
            _loomGroup = _context.LoomGroup.ToList();
            _loom = _context.Loom.ToList();
        }

        public IActionResult OutOfLoom()
        {
            return View();
        }

        public async Task<IActionResult> Index()
        {
            var HolidayDays = _context.DayCalendar.Select(g => g.Date).ToList();

            //PROD CURRENT
            var GetCurrentShift = await _context.CurrentProd
                                    .Include(x => x.Loom)
                                    .ThenInclude(x => x.LoomType)
                                    .Where(x => x.Loom.Status == true && x.Loom.IsCalculated == true)
                                    .ToListAsync();
            ViewBag.GetRunningEfficiencyCurrentShiftAvg = GetCurrentShift.Average(x => x.ActualEff);
            ViewBag.GetStandardEfficiencyCurrentShiftAvg = GetCurrentShift.Average(x => x.StdEff);
            ViewBag.GetStandardEfficiencyCurrentShiftTop5Best = GetCurrentShift.OrderByDescending(x => x.StdEff).Take(5);
            ViewBag.GetStandardEfficiencyCurrentShiftTop5Worst = GetCurrentShift.OrderBy(x => x.StdEff).Take(5);

            //PROD LAST SHIFT

            DateTime LastShiftDate = _context.ShiftHistory.Max(z => z.Date);
            string LastShiftLetter = _context.ShiftHistory.Where(x => x.Date == _context.ShiftHistory.Max(z => z.Date)).Max(z => z.Shift);
            var GetLastShift = await _context.ProductionHistory
                                .Include(x => x.Loom)
                                .ThenInclude(x => x.LoomType)
                                .Where(x=>
                                x.Date == LastShiftDate
                                && x.Shift == _context.ShiftHistory.Where(x => x.Date == _context.ShiftHistory.Max(z => z.Date)).Max(z => z.Shift)
                                && x.Loom.Status == true
                                && x.Loom.IsCalculated == true)
                                .ToListAsync();
            ViewBag.GetRunningEfficiencyLastShiftAvg = GetLastShift.Average(x => x.ActualEff);
            ViewBag.GetStandardEfficiencyLastShiftAvg = GetLastShift.Average(x => x.StdEff);
            ViewBag.GetStandardEfficiencyLastShiftTop5Best = GetLastShift.OrderByDescending(x => x.StdEff).Take(5);
            ViewBag.GetStandardEfficiencyLastShiftTop5Worst = GetLastShift.OrderBy(x => x.StdEff).Take(5);
            ViewBag.LastShiftDate = LastShiftDate;
            ViewBag.LastShiftLetter = LastShiftLetter;

            //PROD LAST 24 HORAS
            var GetLast24 = await _context.CurrentProdHistory
                                                        .Include(x=>x.Loom)
                                                        .ThenInclude(x => x.LoomType)
                                                        .Where(x =>
                                                    x.Loom.Status == true
                                                    && x.Loom.IsCalculated == true)
                                                    .ToListAsync();
            ViewBag.GetRunningEfficiencyLast24Avg = GetLast24.Average(x => x.ActualEff);
            ViewBag.GetStandardEfficiencyLast24Avg = GetLast24.Average(x => x.StdEff);
            ViewBag.GetStandardEfficiencyLast24Top5Best = GetLast24.GroupBy(x => x.LoomId)
                                                        .Select(g => new TopProd
                                                        {
                                                            LoomNumber = g.FirstOrDefault().Loom.LoomNumber,
                                                            LoomType = g.FirstOrDefault().Loom.LoomType.Description,
                                                            StdEff = g.Average(x => x.StdEff)
                                                        }).OrderByDescending(x => x.StdEff).Take(5);
            ViewBag.GetStandardEfficiencyLast24Top5Worst = GetLast24.GroupBy(x => x.LoomId)
                                                            .Select(g => new TopProd
                                                            {
                                                                LoomNumber = g.FirstOrDefault().Loom.LoomNumber,
                                                                LoomType = g.FirstOrDefault().Loom.LoomType.Description,
                                                                StdEff = g.Average(x => x.StdEff)
                                                            }).OrderBy(x => x.StdEff).Take(5);

            //PROD CURRENT MONTH
            var GetCurrentMonth =  _context.ProductionHistory
                                    .Include(x => x.Loom)
                                    .ThenInclude(x => x.LoomType)
                                    .Where(x =>
                                    x.Loom.Status == true
                                    && x.Loom.IsCalculated == true
                                    && x.Date.Month == DateTime.Now.Month
                                    && x.Date.Year == DateTime.Now.Year
                                    && !HolidayDays.Contains(x.Date))
                                    .ToList()
                                    .Where(x=>
                                    x.Date.DayOfWeek != DayOfWeek.Saturday
                                    && x.Date.DayOfWeek != DayOfWeek.Sunday)
                                    .ToList();
            ViewBag.GetRunningEfficiencyCurrentMonthAvg = GetCurrentMonth.Average(x => x.ActualEff);
            ViewBag.GetStandardEfficiencyCurrentMonthAvg = GetCurrentMonth.Average(x => x.StdEff);
            ViewBag.GetStandardEfficiencyCurrentMonthTop5Best = GetCurrentMonth.GroupBy(x => x.LoomId)
                                                                .Select(g => new TopProd
                                                                {
                                                                    LoomNumber = g.FirstOrDefault().Loom.LoomNumber,
                                                                    LoomType = g.FirstOrDefault().Loom.LoomType.Description,
                                                                    StdEff = g.Average(x => x.StdEff)
                                                                }).OrderByDescending(x => x.StdEff).Take(5);
            ViewBag.GetStandardEfficiencyCurrentMonthTop5Worst = GetCurrentMonth.GroupBy(x => x.LoomId)
                                                                .Select(g => new TopProd
                                                                {
                                                                    LoomNumber = g.FirstOrDefault().Loom.LoomNumber,
                                                                    LoomType = g.FirstOrDefault().Loom.LoomType.Description,
                                                                    StdEff = g.Average(x => x.StdEff)
                                                                }).OrderBy(x => x.StdEff).Take(5);

            //STOPS CURRENT
            var querycurrentshiftstop = await _context.CurrentShiftStop
                           .Include(x => x.StopType)
                           .Include(x => x.Loom).ToListAsync();

            var querycurrentstop = await _context.CurrentStop
                                .Include(x => x.StopType)
                                .Include(x => x.Loom)
                                .Select(x =>
                                new CurrentShiftStop
                                {
                                    Loom = x.Loom,
                                    LoomId = x.LoomId,
                                    StopHour = (DateTime)x.Date,
                                    StartHour = DateTime.Now,
                                    StoppedTime = Convert.ToInt32(DateTime.Now.Subtract((DateTime)x.Date).TotalSeconds),
                                    StopType = x.StopType,
                                    StopTypeId = x.StopTypeId
                                }).ToListAsync();

            var queryfull = querycurrentshiftstop.Concat(querycurrentstop);

            var GetCurrentStop = queryfull
                                .Where(x => x.Loom.Status == true && x.Loom.IsCalculated == true)
                                .GroupBy(x=>x.LoomId)
                                .Select(x=> new CurrentShiftStop
                                {
                                    LoomId = x.FirstOrDefault().LoomId,
                                    StoppedTime = Convert.ToInt32(x.Average(x=>x.StoppedTime)),
                                    Loom = x.FirstOrDefault().Loom
                                })
                                .ToList();
            ViewBag.GetCurrentStopAvg = GetCurrentStop.Average(x => x.StoppedTime);
            ViewBag.GetCurrentStopTop5Best = GetCurrentStop.OrderBy(x => x.StoppedTime).Take(5);
            ViewBag.GetCurrentStopTop5Worst = GetCurrentStop.OrderByDescending(x => x.StoppedTime).Take(5);

            //STOPS LAST SHIFT
            var StopLastShift = await _context.StopHistory
                                .Include(x => x.StopType)
                                .Include(x => x.ProductionHistory)
                                    .ThenInclude(x => x.Loom)
                                .Where(x => x.StoppedTime > 0
                                && x.Date == DateTime.Now.Date
                                &&  x.Shift == _context.ShiftHistory.Where(x => x.Date == _context.ShiftHistory.Max(z => z.Date)).Max(z => z.Shift))
                                .ToListAsync();

            var StopLastShiftGroup = StopLastShift.GroupBy(x => x.LoomId)
                                    .Select(x => new StopHistory
                                    {
                                        LoomId = x.FirstOrDefault().LoomId,
                                        StoppedTime = Convert.ToInt32(x.Average(x => x.StoppedTime))
                                    }).ToList();

            ViewBag.GetLastShiftStopAvg = StopLastShiftGroup.Average(x => x.StoppedTime);
            ViewBag.GetLastShiftStopTop5Best = StopLastShiftGroup.OrderBy(x => x.StoppedTime).Take(5);
            ViewBag.GetLastShiftStopTop5Worst = StopLastShiftGroup.OrderByDescending(x => x.StoppedTime).Take(5);

            //STOPS LAST 24
            var shiftdate = _context.ShiftHistory.OrderByDescending(x => x.Date).ThenByDescending(x => x.Shift).Take(3);
            var shift = shiftdate.Select(x => x.Shift).ToList();
            var date = shiftdate.Select(x => x.Date.Date).ToList();
            var StopLast24 = await _context.StopHistory
                            .Include(x => x.StopType)
                            .Include(x => x.ProductionHistory)
                                .ThenInclude(x => x.Loom)
                            .Where(x => x.StoppedTime > 0
                                && shift.Contains(x.Shift)
                                && date.Contains( x.Date.Date)
                                )
                            .ToListAsync();

            var StopLast24Group = StopLast24.GroupBy(x => x.LoomId)
                                .Select(x => new StopHistory
                                {
                                    LoomId = x.FirstOrDefault().LoomId,
                                    StoppedTime = Convert.ToInt32(x.Average(x => x.StoppedTime))
                                }).ToList();

            ViewBag.GetLast24StopAvg = StopLast24Group.Average(x => x.StoppedTime);
            ViewBag.GetLast24StopTop5Best = StopLast24Group.OrderBy(x => x.StoppedTime).Take(5);
            ViewBag.GetLast24StopTop5Worst = StopLast24Group.OrderByDescending(x => x.StoppedTime).Take(5);

            //STOPS MONTH
            var StopMonth =  _context.StopHistory
            .Include(x => x.StopType)
            .Include(x => x.ProductionHistory)
                .ThenInclude(x => x.Loom)
            .Where(x => x.StoppedTime > 0
            && x.Date.Month == DateTime.Now.Date.Month
            && x.Date.Year == DateTime.Now.Date.Year)
            .ToList()
            .Where(x =>
            x.Date.DayOfWeek != DayOfWeek.Saturday
            && x.Date.DayOfWeek != DayOfWeek.Sunday)
            .ToList();

            var StopMonthGroup = StopMonth.GroupBy(x => x.LoomId)
            .Select(x => new StopHistory
            {
                LoomId = x.FirstOrDefault().LoomId,
                StoppedTime = Convert.ToInt32(x.Average(x => x.StoppedTime))
            }).ToList();

            ViewBag.GetMonthStopAvg = StopMonthGroup.Average(x => x.StoppedTime);
            ViewBag.GetMonthStopTop5Best = StopMonthGroup.OrderBy(x => x.StoppedTime).Take(5);
            ViewBag.GetMonthStopTop5Worst = StopMonthGroup.OrderByDescending(x => x.StoppedTime).Take(5);

            ViewBag.Loom = await _context.Loom
                .Include(x=>x.LoomType)
                .ToListAsync();
            return View();
        }

        [Authorize(Roles = "Weaving-Admin, Weaving-Public,All")]
        public async Task<IActionResult> LoomList()
        {
            CurrentShift currentShift = GetShiftFull(DateTime.Now);

            ViewBag.CurrentShift = currentShift;

            ViewBag.ShiftSupervisor = (GetShiftSupervisorAssignedToShiftAndDate((DateTime)currentShift.Date, currentShift.Shift) == null)? "Sin Asignar" : GetShiftSupervisorAssignedToShiftAndDate((DateTime)currentShift.Date, currentShift.Shift).WorkerName;

            var tCSContext = _context.CurrentProd
                            .Include(x => x.Loom)
                            .ThenInclude(x => x.LoomType)
                            .OrderBy(x => x.Loom.LoomNumber);

            foreach (var item in tCSContext)
            {
                item.WeaverName = GetWeaverAssignatedByLoomAndShiftAndDate(item.LoomId, item.Shift, (DateTime)item.Date) !=null ? GetWeaverAssignatedByLoomAndShiftAndDate(item.LoomId, item.Shift, (DateTime)item.Date).WorkerName : "";
            }

            return View(await tCSContext.ToListAsync());
        }

        [Authorize(Roles = "Weaving-Admin, Weaving-Public,All")]
        public async Task<IActionResult> DailyProduction()
        {
            var queryproductionA = await _context.ProductionHistory
                                  .Include(x => x.Loom).ThenInclude(x => x.LoomType)
                                  .Include(x => x.Weaver)
                                  .Where(x => x.Date == DateTime.Now.Date && x.Shift == "A")
                                  .OrderBy(x => x.Loom.LoomNumber).ToListAsync();

            var queryproductionB = await _context.ProductionHistory
                                  .Include(x => x.Loom).ThenInclude(x => x.LoomType)
                                  .Include(x => x.Weaver)
                                  .Where(x => x.Date == DateTime.Now.Date && x.Shift == "B")
                                  .OrderBy(x => x.Loom.LoomNumber).ToListAsync();

            var queryproductionC = await _context.ProductionHistory
                                  .Include(x => x.Loom).ThenInclude(x => x.LoomType)
                                  .Include(x => x.Weaver)
                                  .Where(x => x.Date == DateTime.Now.Date && x.Shift == "C")
                                  .OrderBy(x => x.Loom.LoomNumber).ToListAsync();

            //return View(await queryproduction.ToListAsync());
            ViewBag.ProdQueryA = queryproductionA;
            ViewBag.ProdQueryB = queryproductionB;
            ViewBag.ProdQueryC = queryproductionC;
            ViewBag.selectedDate = DateTime.Now.Date;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> DailyProduction( DateTime selectedDate)
        {
            var queryproductionA = await _context.ProductionHistory
                                  .Include(x => x.Loom).ThenInclude(x => x.LoomType)
                                  .Include(x => x.Weaver)
                                  .Where(x => x.Date == selectedDate && x.Shift == "A")
                                  .OrderBy(x => x.Loom.LoomNumber).ToListAsync();

            var queryproductionB = await _context.ProductionHistory
                                  .Include(x => x.Loom).ThenInclude(x => x.LoomType)
                                  .Include(x => x.Weaver)
                                  .Where(x => x.Date == selectedDate && x.Shift == "B")
                                  .OrderBy(x => x.Loom.LoomNumber).ToListAsync();

            var queryproductionC = await _context.ProductionHistory
                                  .Include(x => x.Loom).ThenInclude(x => x.LoomType)
                                  .Include(x => x.Weaver)
                                  .Where(x => x.Date == selectedDate && x.Shift == "C")
                                  .OrderBy(x => x.Loom.LoomNumber).ToListAsync();

            //return View(await queryproduction.ToListAsync());
            ViewBag.ProdQueryA = queryproductionA;
            ViewBag.ProdQueryB = queryproductionB;
            ViewBag.ProdQueryC = queryproductionC;
            ViewBag.selectedDate = selectedDate;
            return View();
        }

        public async Task<JsonResult> DailyProductionJson(DateTime selectedDate)
        {
            var queryproductionA = await _context.ProductionHistory
                                  .Include(x => x.Loom).ThenInclude(x => x.LoomType)
                                  .Include(x => x.Weaver)
                                  .Where(x => x.Date == selectedDate && x.Shift == "A")
                                  .OrderBy(x => x.Loom.LoomNumber).AsNoTracking().ToListAsync();

            var queryproductionB = await _context.ProductionHistory
                                  .Include(x => x.Loom).ThenInclude(x => x.LoomType)
                                  .Include(x => x.Weaver)
                                  .Where(x => x.Date == selectedDate && x.Shift == "B")
                                  .OrderBy(x => x.Loom.LoomNumber).AsNoTracking().ToListAsync();

            var queryproductionC = await _context.ProductionHistory
                                  .Include(x => x.Loom).ThenInclude(x => x.LoomType)
                                  .Include(x => x.Weaver)
                                  .Where(x => x.Date == selectedDate && x.Shift == "C")
                                  .OrderBy(x => x.Loom.LoomNumber).AsNoTracking().ToListAsync();

            queryproductionA.AddRange(queryproductionB);
            queryproductionA.AddRange(queryproductionC);
            return Json(queryproductionA);
        }
        public async Task<IActionResult> Last24Hours()
        {
            var GetLast24 = await _context.CurrentProdHistory
                                            .Include(x => x.Loom)
                                            .ThenInclude(x => x.LoomType)
                                            .Where(x =>
                                        x.Loom.Status == true
                                        && x.Loom.IsCalculated == true).ToListAsync();
            var GetLast24PostAsync=  GetLast24.GroupBy(x => x.Loom)
                                                        .Select(g => new CurrentProdHistory
                                                        {
                                                            Loom = g.Key,
                                                            ActualEff = g.Average(x => x.ActualEff),
                                                            StdEff = g.Average(x => x.StdEff),
                                                            Ppc = g.Average(x => x.Ppc),
                                                            Lenght = g.Sum(x => x.Lenght),
                                                            Picks = (int?)g.Average(x => x.Picks),
                                                            ActualSpeed = (int?)g.Average(x => x.ActualSpeed),
                                                            WarpStops = g.Sum(x => x.WarpStops),
                                                            WarpStopTime = g.Sum(x => x.WarpStopTime),
                                                            WeftStops = g.Sum(x => x.WeftStops),
                                                            WeftStopTime = g.Sum(x => x.WeftStopTime),
                                                            OtherStops = g.Sum(x => x.OtherStops),
                                                            OtherStopTime = g.Sum(x => x.OtherStopTime)
                                                        })
                                        .ToList();

            return View(GetLast24);
        }

        public async Task<IActionResult> ShiftProduction()
        {
            var shiftProd = await _context.ProductionHistory
                            .Include(x=>x.Loom).ThenInclude(x => x.LoomType)
                            .Where(x => x.Date == DateTime.Now.Date && x.Shift == _context.ShiftHistory.Where(x => x.Date == _context.ShiftHistory.Max(z => z.Date)).Max(z => z.Shift))
                            .ToListAsync();

            ViewBag.selectedDate = DateTime.Now.Date;
            var ShiftList = _context.Shift
            .Select(x => new { ShiftId = x.ShiftId, ShiftLetter = x.ShiftLetter, ShiftText = rm.GetString(x.ShiftText) });
            ViewData["ShiftLetter"] = new SelectList(ShiftList, "ShiftLetter", "ShiftText");
            return View(shiftProd);
        }

        [HttpPost]
        public async Task<IActionResult> ShiftProduction(DateTime date, string shift)
        {
            var shiftProd = await _context.ProductionHistory
                            .Include(x => x.Loom).ThenInclude(x => x.LoomType)
                            .Where(x => x.Date == date && x.Shift == shift)
                            .ToListAsync();

            ViewBag.selectedDate = date;
            var ShiftList = _context.Shift
            .Select(x => new { ShiftId = x.ShiftId, ShiftLetter = x.ShiftLetter, ShiftText = rm.GetString(x.ShiftText) });
            ViewData["ShiftLetter"] = new SelectList(ShiftList, "ShiftLetter", "ShiftText", shift);
            return View(shiftProd);
        }

        public async Task<JsonResult> ShiftProductionJson(DateTime date, string shift)
        {
            var shiftProd = await _context.ProductionHistory
                            .Include(x => x.Loom).ThenInclude(x => x.LoomType)
                            .Where(x => x.Date == date && x.Shift == shift)
                            .AsNoTracking()
                            .ToListAsync();

            ViewBag.selectedDate = date;
            var ShiftList = _context.Shift
            .Select(x => new { ShiftId = x.ShiftId, ShiftLetter = x.ShiftLetter, ShiftText = rm.GetString(x.ShiftText) });
            ViewData["ShiftLetter"] = new SelectList(ShiftList, "ShiftLetter", "ShiftText", shift);
            return Json(shiftProd);
        }

        public async Task<IActionResult> GeneralProduction(DateTime since, DateTime until, string shift)
        {
            var ShiftList = await _context.Shift
            .Select(x => new { ShiftId = x.ShiftId, ShiftLetter = x.ShiftLetter, ShiftText = rm.GetString(x.ShiftText) }).ToListAsync();
            ShiftList.Add(new { ShiftId = 0, ShiftLetter = "T", ShiftText = rm.GetString("all") });

            ViewData["ShiftLetter"] = new SelectList(ShiftList, "ShiftLetter", "ShiftText","T");

            if (since == DateTime.MinValue) { since = DateTime.Today; }

            if (until == DateTime.MinValue) { until = DateTime.Today; }

            if (shift == null)
            {
                shift = "T";
            }

            ViewBag.Since = since.ToString("yyyy-MM-dd");
            ViewBag.Until = until.ToString("yyyy-MM-dd");
            ViewBag.Shift = shift;

            return View();
        }

        public async Task<JsonResult> GeneralProductionJson(DateTime since, DateTime until, string shift)
        {
            var generalProduction = await _context.ProductionHistory
                .Include(x => x.Loom)
                .ThenInclude(x => x.LoomType)
                .Where(x => x.Date >= since.Date && x.Date <= until.Date
                && x.Loom.IsCalculated == true && x.Loom.Status == true)
                .OrderBy(x => x.Loom.LoomNumber).AsNoTracking().ToListAsync();

            if(shift != "T")
            {
                generalProduction = generalProduction.Where(x => x.Shift == shift).ToList();
            }

            var generalProductionGroup = generalProduction.GroupBy(x => x.LoomId)
                                .Select(g => new ProductionHistory
                                {
                                    LoomId = g.Key,
                                    Loom = g.FirstOrDefault(x=>x.LoomId == g.Key).Loom,
                                    ActualEff = g.Average(x => x.ActualEff),
                                    StdEff = g.Average(x => x.StdEff),
                                    Ppc = g.Average(x => x.Ppc),
                                    Lenght = g.Sum(x => x.Lenght),
                                    Picks = (int?)g.Average(x => x.Picks),
                                    Speed = (int?)g.Average(x => x.Speed),
                                    WarpStops = g.Sum(x => x.WarpStops),
                                    WarpStopTime = g.Sum(x => x.WarpStopTime),
                                    WeftStops = g.Sum(x => x.WeftStops),
                                    WeftStopTime = g.Sum(x => x.WeftStopTime),
                                    OtherStops = g.Sum(x => x.OtherStops),
                                    OtherStopTime = g.Sum(x => x.OtherStopTime)
                                }).ToList();

            return Json(generalProductionGroup);
        }

        public async Task<IActionResult> LoomMonitor(int loomId)
        {
            CurrentProd loom = await _context.CurrentProd
                        .Include(x=>x.Loom)
                        .ThenInclude(x => x.LoomType)
                        .FirstOrDefaultAsync(x=>x.LoomId == loomId);

            loom.WeaverName = GetWeaverAssignatedByLoomAndShiftAndDate(loom.LoomId, loom.Shift, (DateTime)loom.Date) != null ? GetWeaverAssignatedByLoomAndShiftAndDate(loom.LoomId, loom.Shift, (DateTime)loom.Date).WorkerName : "";

            List<ProductionHistory> prodhist = await _context.ProductionHistory.Where(x => x.LoomId == loomId).OrderByDescending(x => x.Date).Take(3).ToListAsync();

            var GetLast24 = await _context.CurrentProdHistory
                                            .Where(x =>x.LoomId == loomId)
                                            .ToListAsync();

            var GetLast24Avg = GetLast24.GroupBy(x => x.Hora)
                                        .Select(g => new ProductionAvg
                                        {
                                            Hora = g.Key,
                                            Date = (DateTime)g.FirstOrDefault().Date,
                                            ActualEff = Convert.ToInt32(g.Average(x => x.ActualEff)),
                                            StdEff = Convert.ToInt32(g.Average(x => x.StdEff))
                                        }).OrderBy(x=>x.Date);

            var GetLast24ActualEffJson = GetLast24Avg.Select(x=> new { Hora = x.Hora.ToString(), ActualEff = x.ActualEff });
            ViewBag.GetLast24ActualEffJson = JsonConvert.SerializeObject(GetLast24ActualEffJson);

            var GetLast24StdEffJson = GetLast24Avg.Select(x => new { Hora = x.Hora.ToString(), StdEff = x.StdEff });
            ViewBag.GetLast24StdEffJson = JsonConvert.SerializeObject(GetLast24StdEffJson);

            var GetLast30 = await _context.ProductionHistory
                                .Where(x => x.LoomId == loomId)
                                .OrderByDescending(x=>x.Date).Take(90)
                                .ToListAsync();

            var GetLast30Avg = GetLast30.GroupBy(x => x.Date)
                            .Select(g => new ProductionAvg
                            {
                                Date = (DateTime)g.FirstOrDefault().Date,
                                ActualEff = Convert.ToInt32(g.Average(x => x.ActualEff)),
                                StdEff = Convert.ToInt32(g.Average(x => x.StdEff))
                            }).OrderBy(x => x.Date);

            var GetLast30ActualEffJson = GetLast30Avg.Select(x => new { Date = x.Date.ToString("dd-MM-yy"), ActualEff = x.ActualEff }).OrderBy(x=>x.Date);
            ViewBag.GetLast30ActualEffJson = JsonConvert.SerializeObject(GetLast30ActualEffJson);

            var GetLas30StdEffJson = GetLast30Avg.Select(x => new { Date = x.Date.ToString("dd-MM-yy"), StdEff = x.StdEff }).OrderBy(x => x.Date);
            ViewBag.GetLast30StdEffJson = JsonConvert.SerializeObject(GetLas30StdEffJson);

            ViewBag.ProdHist = prodhist;

            return View(loom);
        }

        public async Task<IActionResult> WeekAssignation()
        {
            var ShiftList = await _context.ShiftPattern
                .Select(x => new { x.PatternId, x.Pattern, PatternText = $"Noche: Grupo { x.Pattern[0]} - Mañana: Grupo { x.Pattern[1]} - Tarde: Grupo { x.Pattern[2]}" }).ToListAsync();
            ViewBag.ShiftPattern = new SelectList(ShiftList, "PatternId", "PatternText", GetShiftPatternByDate(DateTime.Now).PatternId);

            int groupA = GetWorkerGroupIdByDateAndShift(DateTime.Now, "A");
            int groupB = GetWorkerGroupIdByDateAndShift(DateTime.Now, "B");
            int groupC = GetWorkerGroupIdByDateAndShift(DateTime.Now, "C");

            var weaverAssigA = await _context.WeaverAssignation
                             .Include(x => x.WorkerGroup)
                             .Include(x => x.Worker)
                             .Include(x => x.LoomGroup)
                             .Where(x => x.WorkerGroupId == groupA)
                            .ToArrayAsync();
            Worker supervisorA = await _context.Worker
                              .FirstOrDefaultAsync(x => x.WorkerGroupId == groupA && x.PositionId == 2);

            var weaverAssigB = await _context.WeaverAssignation
                 .Include(x => x.WorkerGroup)
                 .Include(x => x.Worker)
                 .Include(x => x.LoomGroup)
                 .Where(x => x.WorkerGroupId == groupB)
                 .ToArrayAsync();
            Worker supervisorB = await _context.Worker
                  .FirstOrDefaultAsync(x => x.WorkerGroupId == groupB && x.PositionId == 2);

            var weaverAssigC = await _context.WeaverAssignation
                 .Include(x => x.WorkerGroup)
                 .Include(x => x.Worker)
                 .Include(x => x.LoomGroup)
                 .Where(x => x.WorkerGroupId == groupC)
                 .ToArrayAsync();
            Worker supervisorC = await _context.Worker
                  .FirstOrDefaultAsync(x => x.WorkerGroupId == groupC && x.PositionId == 2);

            List<UnassignedWorker> weaverNoassig = _context.Worker
                                                   .Include(x => x.WeaverAssignation)
                                                   .Include(x => x.WorkerPosition)
                                                  .Where(x => x.WorkerPosition.PositionId == 1 && x.Status && !x.WeaverAssignation.Any(z=>z.WorkerId == x.WorkerId))
                                                  .Select(x=> new UnassignedWorker
                                                  {
                                                      WorkerName = x.WorkerName,
                                                      Cargo = x.WorkerPosition.Spdescr
                                                  }).ToList();

            List<UnassignedWorker> supNoassig = _context.Worker
                                                   .Include(x => x.WeaverAssignation)
                                                   .Include(x => x.WorkerPosition)
                                                  .Where(x => x.WorkerPosition.PositionId == 2 && x.Status && x.WorkerGroupId == null)
                                                  .Select(x => new UnassignedWorker
                                                  {
                                                      WorkerName = x.WorkerName,
                                                      Cargo = x.WorkerPosition.Spdescr
                                                  }).ToList();

            List<UnassignedWorker> unssigned = new();
            unssigned.AddRange(weaverNoassig);
            unssigned.AddRange(supNoassig);

            ViewData["WeaverUnassigned"] = new SelectList(weaverNoassig, "WorkerId", "WorkerName");

            ViewBag.WeaverAssigA = weaverAssigA;
            ViewBag.WeaverAssigB = weaverAssigB;
            ViewBag.WeaverAssigC = weaverAssigC;
            ViewBag.SupervisorA = supervisorA;
            ViewBag.SupervisorB = supervisorB;
            ViewBag.SupervisorC = supervisorC;
            ViewBag.Unassigned = unssigned.OrderBy(x => x.Cargo);

            ViewBag.FirstDayOfWeek = DateTime.Now.StartOfWeek(DayOfWeek.Monday).ToShortDateString();
            return View();
        }

        public async Task<IActionResult> GetTejedores()
        {
            List<UnassignedWorker> weaverNoassig = await _context.Worker
                                       .Include(x => x.WeaverAssignation)
                                       .Include(x => x.WorkerPosition)
                                      .Where(x => x.WorkerPosition.PositionId == 1 && x.Status && !x.WeaverAssignation.Any(z => z.WorkerId == x.WorkerId))
                                      .Select(x => new UnassignedWorker
                                      {
                                          WorkerId = x.WorkerId,
                                          WorkerName = x.WorkerName,
                                          Cargo = x.WorkerPosition.Spdescr
                                      }).ToListAsync();
            weaverNoassig.Add(new UnassignedWorker {WorkerId = null, WorkerName = "SIN ASIGNAR"});

            return Json(weaverNoassig);
        }

        public async Task<IActionResult> GetSurpervisores()
        {
            List<UnassignedWorker> supNoassig = await _context.Worker
                                                   .Include(x => x.WeaverAssignation)
                                                   .Include(x => x.WorkerPosition)
                                                  .Where(x => x.WorkerPosition.PositionId == 2 && x.Status && x.WorkerGroupId == null)
                                                  .Select(x => new UnassignedWorker
                                                  {
                                                      WorkerId = x.WorkerId,
                                                      WorkerName = x.WorkerName,
                                                      Cargo = x.WorkerPosition.Spdescr
                                                  }).ToListAsync();

            supNoassig.Add(new UnassignedWorker { WorkerId = null, WorkerName = "SIN ASIGNAR" });

            return Json(supNoassig);
        }

        public async Task<IActionResult> GetTejedorAsignado(int loomGroupId, int workerGroupId)
        {
            var weaverAssig = await _context.WeaverAssignation
                            .Include(x=>x.Worker)
                            .FirstOrDefaultAsync(x=>x.LoomGroupId == loomGroupId && x.WorkerGroupId == workerGroupId);

            string weaver = (weaverAssig.Worker != null)? weaverAssig.Worker.WorkerName :rm.GetString("unassigned");

            return Json(weaver);
        }

        public async Task<IActionResult> GetSupervisorAsignado(int workerGroupId)
        {
            var weaverAssig = await _context.Worker
                             .FirstOrDefaultAsync(x => x.WorkerGroupId == workerGroupId && x.PositionId == 2);

            string weaver = (weaverAssig != null) ? weaverAssig.WorkerName : rm.GetString("unassigned");

            return Json(weaver);
        }

        public async Task GuardarTejedorAsignado(int loomGroupId, int workerGroupId, int selectWeaver)
        {
            WeaverAssignation weaverAssignation = _context.WeaverAssignation.FirstOrDefault(x => x.LoomGroupId == loomGroupId && x.WorkerGroupId == workerGroupId);
            weaverAssignation.WorkerId = (selectWeaver != 0)? selectWeaver : null;
            _context.Update(weaverAssignation);

            await _context.SaveChangesAsync();
        }

        public async Task GuardarSupervisorAsignado(int workerGroupId, int selectWeaver)
        {
            if (selectWeaver !=0)
            {
                Worker supAssignation = _context.Worker.FirstOrDefault(x => x.WorkerId == selectWeaver);
                supAssignation.WorkerGroupId = workerGroupId;

                _context.Update(supAssignation);
            }
            else
            {
                Worker supAssignation = _context.Worker.FirstOrDefault(x => x.WorkerGroupId == workerGroupId);
                supAssignation.WorkerGroupId = null;

                _context.Update(supAssignation);
            }

            await _context.SaveChangesAsync();
        }

        [Authorize(Roles = "Weaving-Admin,All")]
        public async Task<IActionResult> ShiftStop()
        {
            var querycurrentshiftstop = await _context.CurrentShiftStop
                           .Include(x => x.StopType)
                           .Include(x => x.Loom).ToListAsync();

            var querycurrentstop = await _context.CurrentStop
                            .Include(x => x.StopType)
                            .Include(x => x.Loom)
                            .Select(x =>
                            new CurrentShiftStop
                            {
                                Loom = x.Loom,
                                LoomId = x.LoomId,
                                StopHour = (DateTime)x.Date,
                                StartHour = DateTime.Now,
                                StoppedTime = Convert.ToInt32(DateTime.Now.Subtract((DateTime)x.Date).TotalSeconds),
                                StopType = x.StopType,
                                StopTypeId = x.StopTypeId
                            }).ToListAsync();

            var queryfull = querycurrentshiftstop.Concat(querycurrentstop);
            return View(queryfull);
        }

        public async Task<IActionResult> StopProductionHistory()
        {
            DateTime since = DateTime.Today;
            DateTime until = DateTime.Today;
            const string shift = "T";

            var ShiftList = await _context.Shift
            .Select(x => new { ShiftId = x.ShiftId, ShiftLetter = x.ShiftLetter, ShiftText = rm.GetString(x.ShiftText) }).ToListAsync();
            ShiftList.Add(new { ShiftId = 0, ShiftLetter = "T", ShiftText = rm.GetString("all") });

            var StopType = await _context.StopType.ToListAsync();
            StopType.Add(new StopType{ StopTypeId = 0, Description = rm.GetString("all") });

            ViewData["ShiftLetter"] = new SelectList(ShiftList, "ShiftLetter", "ShiftText", "T");
            ViewData["StopType"] = new SelectList(StopType, "StopTypeId", "Description", 0);

            ViewBag.Since = since.ToString("yyyy-MM-dd");
            ViewBag.Until = until.ToString("yyyy-MM-dd");
            ViewBag.Shift = shift;

            return View();
        }

        public async Task<JsonResult> StopProductionHistoryJson(DateTime since, DateTime until, string shift, int stopType)
        {
            var stopHistory = await _context.StopHistory
                        .Include(x => x.StopType)
                        .Include(x => x.ProductionHistory)
                            .ThenInclude(x => x.Loom)
                        .Where(x => x.StoppedTime > 0
                        && x.Date >= since.Date && x.Date <= until.Date)
                        .AsNoTracking()
                        .ToListAsync();

            if (shift != "T")
            {
                stopHistory = stopHistory.Where(x => x.Shift == shift).ToList();
            }

            if (stopType != 0)
            {
                stopHistory = stopHistory.Where(x => x.StopType.StopTypeId == stopType).ToList();
            }

            var stopHistoryGroup = stopHistory.GroupBy(x => new { x.LoomId, x.StopTypeId })
                                .Select(g => new StopHistory
                                {
                                    LoomId = g.Key.LoomId,
                                    ProductionHistory = g.FirstOrDefault(x => x.LoomId == g.Key.LoomId).ProductionHistory,
                                    Shift = g.FirstOrDefault(x => x.LoomId == g.Key.LoomId).Shift,
                                    Stops= g.Sum(x=>x.Stops),
                                    StoppedTime = g.Sum(x=>x.StoppedTime),
                                    StopType = g.FirstOrDefault(x =>x.StopTypeId == g.Key.StopTypeId).StopType
                                }).ToList();
            return Json(stopHistoryGroup);
        }

        [Authorize(Roles = "Weaving-Admin,All")]
        public async Task<IActionResult> StopProductionHistoryDetail()
        {
            var querycurrentshiftstop = await _context.CurrentShiftStop
                           .Include(x => x.StopType)
                           .Include(x => x.Loom).ToListAsync();

            var querycurrentstop = await _context.CurrentStop
                            .Include(x => x.StopType)
                            .Include(x => x.Loom)
                            .Select(x =>
                            new CurrentShiftStop
                            {
                                Loom = x.Loom,
                                LoomId = x.LoomId,
                                StopHour = (DateTime)x.Date,
                                StartHour = DateTime.Now,
                                StoppedTime = Convert.ToInt32(DateTime.Now.Subtract((DateTime)x.Date).TotalSeconds),
                                StopType = x.StopType,
                                StopTypeId = x.StopTypeId
                            }).ToListAsync();

            List<Loom> listStopFull = new();
            IEnumerable<CurrentShiftStop> currentStopList = querycurrentshiftstop.Concat(querycurrentstop).OrderBy(x=>x.LoomId).ThenBy(x => x.StopHour);

            foreach (Loom item2 in _context.Loom.Include(x=>x.LoomType).Where(x=>x.Status == true))
            {
                 if (currentStopList.Any(x => x.LoomId == item2.LoomId))
                 {
                        List<CurrentShiftStop> listStop = new();
                        listStop = currentStopList.Where(x => x.LoomId == item2.LoomId).ToList();
                        List<CurrentShiftStop> NewCurrentStopList = new();

                        Loom stopData = new();

                        stopData.LoomId = item2.LoomId;
                        stopData.LoomNumber = item2.LoomNumber;
                        TimeSpan t = (DateTime.Now - GetShiftFull(DateTime.Now).Date ?? default);
                        int sum = Convert.ToInt32(t.TotalSeconds) / 50;
                        listStop = listStop.Where(x => x.StoppedTime > sum).ToList();

                        if (listStop.Any())
                        {
                            for (int i = 0; i < listStop.Count; i++)
                            {
                                if (listStop[i].StopHour > GetShiftFull(DateTime.Now).Date && i == 0)
                                {
                                    CurrentShiftStop cur = new();
                                    cur.StopHour = GetShiftFull(DateTime.Now).Date;
                                    cur.StartHour = listStop[i].StopHour;
                                    cur.StopTypeId = 0;
                                    cur.LoomId = item2.LoomId;
                                    TimeSpan ts = cur.StartHour - cur.StopHour ?? default;
                                    cur.StoppedTime = Convert.ToInt32(ts.TotalSeconds);

                                    if (cur.StoppedTime > sum)
                                    {
                                        NewCurrentStopList.Add(cur);
                                    }
                                }

                                NewCurrentStopList.Add(listStop[i]);

                                if (i + 1 < listStop.Count)
                                {
                                    CurrentShiftStop cur = new();
                                    cur.StopHour = listStop[i].StartHour;
                                    cur.StartHour = listStop[i + 1].StopHour;
                                    cur.StopTypeId = 0;
                                    cur.LoomId = listStop[i].LoomId;
                                    TimeSpan ts = listStop[i + 1].StopHour - listStop[i].StartHour ?? default;
                                    cur.StoppedTime = Convert.ToInt32(ts.TotalSeconds);
                                    if (cur.StoppedTime > sum)
                                    {
                                        NewCurrentStopList.Add(cur);
                                    }
                                }

                                if (listStop[i].StartHour < DateTime.Now && i == listStop.Count - 1)
                                {
                                    CurrentShiftStop cur = new();
                                    cur.StopHour = listStop[i].StartHour;
                                    cur.StartHour = DateTime.Now;
                                    cur.StopTypeId = 0;
                                    cur.LoomId = item2.LoomId;
                                    TimeSpan ts = cur.StartHour - cur.StopHour ?? default;
                                    cur.StoppedTime = Convert.ToInt32(ts.TotalSeconds);

                                    if (cur.StoppedTime > sum)
                                    {
                                        NewCurrentStopList.Add(cur);
                                    }
                                }
                            }
                        }
                        else
                        {
                            CurrentShiftStop cur = new();
                            cur.StopHour = GetShiftFull(DateTime.Now).Date;
                            cur.StartHour = DateTime.Now;
                            cur.StopTypeId = 0;
                            cur.LoomId = item2.LoomId;
                            TimeSpan ts = cur.StartHour - cur.StopHour ?? default;
                            cur.StoppedTime = Convert.ToInt32(ts.TotalSeconds);

                            NewCurrentStopList.Add(cur);
                        }

                        foreach (CurrentShiftStop item in NewCurrentStopList)
                        {
                            item.Percent = ((item.StoppedTime) * 100 / NewCurrentStopList.Sum(x => x.StoppedTime));
                        }

                        //    NewCurrentStopList = NewCurrentStopList.Where(x=>x.Percent >0).ToList();

                        int totalpercent = NewCurrentStopList.Sum(x => x.Percent) ?? 0;
                        while (totalpercent < 100)
                        {
                            foreach (CurrentShiftStop item in NewCurrentStopList)
                            {
                                item.Percent += 1;
                                totalpercent++;
                                if (totalpercent == 100)
                                {
                                    break;
                                }
                            }
                        }

                        NewCurrentStopList = NewCurrentStopList.OrderBy(x => x.StopHour).ToList();

                        List<CurrentShiftStop> FinalCurrentStopList = new();
                        for (int i = 0; i < NewCurrentStopList.Count; i++)
                        {
                            if (i + 1 == NewCurrentStopList.Count)
                            {
                                FinalCurrentStopList.Add(NewCurrentStopList[i]);
                            }
                            else if (NewCurrentStopList[i].StopTypeId == NewCurrentStopList[i + 1].StopTypeId)
                            {
                                CurrentShiftStop cur = new();
                                cur.StopHour = NewCurrentStopList[i].StopHour;
                                cur.StartHour = NewCurrentStopList[i + 1].StartHour;
                                cur.StopTypeId = NewCurrentStopList[i].StopTypeId;
                                cur.LoomId = item2.LoomId;
                                TimeSpan ts = cur.StartHour - cur.StopHour ?? default;
                                cur.StoppedTime = Convert.ToInt32(ts.TotalSeconds);
                                cur.Percent = NewCurrentStopList[i].Percent + NewCurrentStopList[i + 1].Percent;
                                FinalCurrentStopList.Add(cur);
                                i++;
                            }
                            else
                            {
                                FinalCurrentStopList.Add(NewCurrentStopList[i]);
                            }
                        }

                        stopData.CurrentShiftStop = FinalCurrentStopList;
                        stopData.SumStop = FinalCurrentStopList.Sum(x => x.StoppedTime);
                        listStopFull.Add(stopData);
                    }
                }
            ViewBag.stoptypelist = _context.StopType.ToList();
            ViewBag.currentprodlist = _context.CurrentProd.Include(x=>x.Loom).ToList();
            ViewBag.curreshift = GetShiftFull(DateTime.Now);
            return View(listStopFull);
        }

        [Authorize(Roles = "Weaving-Admin,All")]
        public async Task<IActionResult> PenultimatePiece()
        {
            List<Ticket> ticketCompletos = new();
            ticketCompletos = _context.Ticket.Where(x => x.Woven == false).ToList();
            List<Piece> piecesComp = new();
            piecesComp = _context.Piece.Where(x => x.ProgStatus != "110" && x.ProgStatus != "100" && x.ProgStatus != "999").ToList();
            foreach (var tick in ticketCompletos)
            {
                tick.Piece = piecesComp.Where(x => x.TicketId == tick.TicketId).ToList();
                tick.FirstUnwovenPiece = tick.Piece.Where(p => int.Parse(p.ProgStatus) <= 040).Min(p => p.PieceNumber);
                tick.LastUnwovenPiece = tick.Piece.Where(p => int.Parse(p.ProgStatus) <= 040).Max(p => p.PieceNumber);
                tick.FirstPieceNumber = tick.Piece.Min(p => p.PieceNumber);
                tick.LastPieceNumber = tick.Piece.Max(p => p.PieceNumber);
            }
            ticketCompletos = ticketCompletos.Where(x => x.FirstUnwovenPiece != null).ToList();

            var Telares = (from lp in _context.LoomPos.Where(x => x.Position == 0).ToList()
                           join l in _context.Loom.ToList()
                           on lp.LoomId equals l.LoomId
                           join bp in _context.BeamPos.ToList()
                           on lp.BeamId equals bp.BeamId
                           join jp in _context.JobPos.ToList()
                           on bp.JobNumber equals jp.JobNumber
                           join t in ticketCompletos
                           on jp.TicketId equals t.TicketId
                           select new
                           {
                               LoomID = lp.LoomId,
                               LoomNumber = l.LoomNumber,
                               JobNumber = jp.JobNumber,
                               TicketName = t.UnwovenPiecesToShortString,
                               Articulo = t.Design,
                               Color = t.WarpShade + " " + t.WeftShade,
                               Position = lp.Position,
                               LastUnwovenPiece = Convert.ToInt32(t.LastUnwovenPiece.Substring(t.LastUnwovenPiece.Length - 2)) - Convert.ToInt32(t.FirstUnwovenPiece.Substring(t.LastUnwovenPiece.Length - 2))
                           }).ToList().Where(x => x.LastUnwovenPiece <= 1).OrderBy(x => x.LoomNumber);

            var AmarreCambio = await _context.LoomPos
                                .Include(x => x.Loom)
                                .Include(x => x.Link)
                                .Select(x =>
                                new
                                {
                                    LoomId = x.LoomId,
                                    LoomNumber = x.Loom.LoomNumber,
                                    Descripcion = x.Link.Description,
                                    Position = x.Position
                                })
                                .Where(x => x.Position == 1)
                                .OrderBy(x => x.LoomNumber)
                                .ToListAsync();

            var TelarPorTerminar = (from l1 in Telares
                                    join l2 in AmarreCambio
                                    on l1.LoomID equals l2.LoomId
                                    select new PenultimatePieceModel
                                    {
                                        LoomNumber = l1.LoomNumber,
                                        JobNumber = l1.JobNumber,
                                        TicketName = l1.TicketName,
                                        Articulo = l1.Articulo,
                                        Color = l1.Color,
                                        Descripcion = l2.Descripcion
                                    }).ToList();

            return View(TelarPorTerminar);
        }

        public static CurrentShift GetShiftFull(DateTime Now)
        {
            CurrentShift result = new();
            string thisTime = GetStringTime(Now);
            TimeSpan ts;
            try
            {
                if (Convert.ToDateTime(thisTime) >= Convert.ToDateTime("00:00:00") && Convert.ToDateTime(thisTime) <= Convert.ToDateTime("07:59:59"))
                {
                    result.Shift = "A";
                    ts = new TimeSpan(00, 00, 00);
                    result.Date = Now.Date + ts;
                }
                else if (Convert.ToDateTime(thisTime) >= Convert.ToDateTime("08:00:00") && Convert.ToDateTime(thisTime) <= Convert.ToDateTime("15:59:59"))
                {
                    result.Shift = "B";
                    ts = new TimeSpan(08, 00, 00);
                    result.Date = Now.Date + ts;
                }
                else if (Convert.ToDateTime(thisTime) >= Convert.ToDateTime("16:00:00") && Convert.ToDateTime(thisTime) <= Convert.ToDateTime("23:59:59"))
                {
                    result.Shift = "C";
                    ts = new TimeSpan(16, 00, 00);
                    result.Date = Now.Date + ts;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            result.Hour = Now.Hour;

            return result;
        }

        public static string GetStringTime(DateTime date)
        {
            return string.Format("{0}:{1}:{2}", date.Hour, date.Minute, date.Second);
        }

        public Worker GetShiftSupervisorAssignedToShiftAndDate(DateTime date, string shift)
        {
            int groupID = GetWorkerGroupIdByDateAndShift(date, shift);

            return GetWorkersByPositionAndGroupID(groupID, 2).SingleOrDefault();
        }

        public List<Worker> GetWorkersByPositionAndGroupID(int groupID, int positionID)
        {
            var workers = from w in _context.Worker
                          join wp in _context.WorkerPosition
                          on w.PositionId equals wp.PositionId
                          where w.WorkerGroupId == groupID
                          && w.PositionId == positionID
                          select new Worker
                          {
                              WorkerId = w.WorkerId,
                              WorkerName = w.WorkerName,
                              WorkerGroupId = w.WorkerGroupId,
                              PositionId = w.PositionId,
                              Ficha = w.Ficha,
                              ShiftHistory = w.ShiftHistory,
                              ProductionHistory = w.ProductionHistory,
                              Status = w.Status,
                              WeaverAssignation = w.WeaverAssignation
                          };

            return workers.ToList();
        }

        public static Worker GetWeaverAssignatedByLoomAndShiftAndDate(int loomID, string shift, DateTime date)
        {
            int workerGroupID = GetWorkerGroupIdByDateAndShift(date, shift);

            return GetWeaverAssignatedByLoomAndWorkerGroup(loomID, workerGroupID);
        }

        public static Worker GetWeaverAssignatedByLoomAndWorkerGroup(int loomID, int workerGroupID)
        {
            var workers = from w in _worker
                          join wp in _workerPosition
                          on w.PositionId equals wp.PositionId
                          join wa in _weaverAssignation
                          on w.WorkerId equals wa.WorkerId
                          join wg in _workerGroup
                          on wa.WorkerGroupId equals wg.WorkerGroupId
                          join lg in _loomGroup
                          on wa.LoomGroupId equals lg.LoomGroupId
                          join l in _loom
                          on lg.LoomGroupId equals l.LoomGroupId
                          where l.LoomId == loomID
                          && wg.WorkerGroupId == workerGroupID
                          select new Worker
                          {
                              WorkerId = w.WorkerId,
                              WorkerName = w.WorkerName,
                              WorkerGroupId = w.WorkerGroupId,
                              PositionId = w.PositionId,
                              Ficha = w.Ficha,
                              ShiftHistory = w.ShiftHistory,
                              ProductionHistory = w.ProductionHistory,
                              Status = w.Status,
                              WeaverAssignation = w.WeaverAssignation
                          };

            return workers.ToList().FirstOrDefault();
        }

        public static int GetWorkerGroupIdByDateAndShift(DateTime date, string shift)
        {
            string pattern = GetShiftPatternByDate(date).Pattern;

            int groupID = 0;

            switch (shift)
            {
                case "A":
                    switch (pattern)
                    {
                        case "ABC":
                            groupID = 1;
                            break;
                        case "CAB":
                            groupID = 3;
                            break;
                        case "BCA":
                            groupID = 2;
                            break;
                    }
                    break;
                case "B":
                    switch (pattern)
                    {
                        case "ABC":
                            groupID = 2;
                            break;
                        case "CAB":
                            groupID = 1;
                            break;
                        case "BCA":
                            groupID = 3;
                            break;
                    }
                    break;
                case "C":
                    switch (pattern)
                    {
                        case "ABC":
                            groupID = 3;
                            break;
                        case "CAB":
                            groupID = 2;
                            break;
                        case "BCA":
                            groupID = 1;
                            break;
                    }
                    break;
            }

            return groupID;
        }

        public static ShiftPattern GetShiftPatternByDate(DateTime date)
        {
            double week = GetIso8601WeekOfYear(date);

            double div = week / 3;

            //div = Math.Round(div, 2);

            double rest = div - Math.Truncate(div);

            rest = Math.Round(rest, 2);

            ShiftPattern weekPattern = new();

            if (rest == 0)
            {
                weekPattern = _shiftPattern.Find(x=>x.Index == 1);
            }
            else if (rest == 0.33)
            {
                weekPattern = _shiftPattern.Find(x => x.Index == 2);
            }
            else if (rest == 0.67)
            {
                weekPattern = _shiftPattern.Find(x => x.Index == 3);
            }

            return weekPattern;
        }

        public async Task ChangeShiftPattern(string patternID)
        {
            ShiftPattern newPattern = await _context.ShiftPattern.FirstOrDefaultAsync(x => x.PatternId == patternID);
            ShiftPattern current = GetShiftPatternByDate(DateTime.Now);

            if (current.PatternId != newPattern.PatternId)
            {
                ExperimentalPattern(newPattern.Pattern, (int)current.Index);
            }
        }
        public void ExperimentalPattern(string pattern, int index)
        {
            int iterations = 0;
            while (iterations < 3)
            {
                ShiftPattern updatePatern = new();
                updatePatern = _context.ShiftPattern.FirstOrDefault(x => x.Pattern == pattern);
                updatePatern.Index = index;
                _context.ShiftPattern.Update(updatePatern);
                _context.SaveChanges();

                if (index == 3)
                {
                    index = 1;
                }
                else
                {
                    index++;
                }

                string nextPattern = pattern.Substring(1) + pattern.Substring(0, 1);

                pattern = nextPattern;

                iterations++;
            }
        }
        public static int GetIso8601WeekOfYear(DateTime time)
        {
            // Seriously cheat.  If its Monday, Tuesday or Wednesday, then it'll 
            // be the same week# as whatever Thursday, Friday or Saturday are,
            // and we always get those right
            DayOfWeek day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(time);
            if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
            {
                time = time.AddDays(3);
            }
            // Return the week of our adjusted day
            return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(time, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        }
        public async Task<IActionResult> LoomListToExcel()
        {
            DetalleReporte(DateTime.Now, null ,rm.GetString("currentshift").ToUpper(), null);

            string supervisorName = string.Empty;
            var curProd = _context.CurrentProd.FirstOrDefault();
            if (_context.ShiftHistory.FirstOrDefault(x => x.Shift == curProd.Shift && x.Date == curProd.Date) != null)
            {
                supervisorName = _context.ShiftHistory.FirstOrDefault(x => x.Shift == curProd.Shift && x.Date == curProd.Date).Supervisor.WorkerName;
            }
            else
            {
                supervisorName = "No Ingresado";
            }

            string[] columnsNames =
{
                rm.GetString("loom"),
                rm.GetString("loomtype"),
                rm.GetString("shift"),
                rm.GetString("shiftsupervisor"),
                rm.GetString("running"),
                rm.GetString("standardefficiency"),
                rm.GetString("quality"),
                rm.GetString("designA"),
                "PPC".ToUpper(),
                rm.GetString("meters").ToUpper(),
                "Picks".ToUpper(),
                rm.GetString("speed").ToUpper(),
                rm.GetString("warpstops").ToUpper(),
                rm.GetString("weftstops").ToUpper(),
                rm.GetString("otherstops").ToUpper(),
                rm.GetString("weaver").ToUpper(),
            };

            var currentProd =  await _context.CurrentProd
                                .Include(x => x.Loom)
                                    .ThenInclude(x => x.LoomType)
                                .Select(x => new
                                {
                                    loom = x.Loom.LoomNumber,
                                    loomtype = x.Loom.LoomType.Description,
                                    shift =   x.Shift,
                                    shiftsupervisor = supervisorName,
                                    running = string.Format("{0}%", (int)x.ActualEff),
                                    standardefficiency = string.Format("{0}%", (int)x.StdEff),
                                    quality = ObtenerCalidad(x.Article).Result,
                                    designA = x.Article,
                                    ppc = (int)x.Ppc,
                                    meters = x.Lenght / 100,
                                    picks = x.Picks,
                                    speed = x.ActualSpeed,
                                    warpstops = $"{rm.GetString("quantity").ToUpper()} : {(int)x.WarpStops}  {rm.GetString("time").ToUpper()} : {TimeSpan.FromSeconds((double)x.WarpStopTime).ToString("hh':'mm':'ss")}  {rm.GetString("minutes").ToUpper()} : {(int)(x.WarpStopTime / 60)}",
                                    weftstops = $"{rm.GetString("quantity").ToUpper()} : {(int)x.WeftStops}  {rm.GetString("time").ToUpper()} : {TimeSpan.FromSeconds((double)x.WeftStopTime).ToString("hh':'mm':'ss")}  {rm.GetString("minutes").ToUpper()} : {(int)(x.WeftStopTime / 60)}",
                                    otherstops = $"{rm.GetString("quantity").ToUpper()} : {(int)x.OtherStops}  {rm.GetString("time").ToUpper()} : {TimeSpan.FromSeconds((double)x.OtherStopTime).ToString("hh':'mm':'ss")}  {rm.GetString("minutes").ToUpper()} : {(int)(x.OtherStopTime / 60)}",
                                    weaver = GetWeaverAssignatedByLoomAndShiftAndDate(x.LoomId, x.Shift, (DateTime)x.Date).WorkerName
                                })
                                .ToListAsync();

            string sheetTitle = string.Empty;

            switch (curProd.Shift)
            {
                case "A":
                    sheetTitle = rm.GetString("nightshift");
                    break;
                case "B":
                    sheetTitle = rm.GetString("morningshift");
                    break;
                case "C":
                    sheetTitle = rm.GetString("afternoonshift");
                    break;
            }

            string title = string.Format("{0} {1}\n{2}: {3}", sheetTitle.ToUpper(), curProd.Date.Value.ToString("dd/MM/yyyy")
                , rm.GetString("shiftsupervisor").ToUpper(), supervisorName);

            NpoiFunctions.CreateE(excelWorkbook, title, sheetTitle, currentProd, columnsNames);

            const string fileName = "Reporte Produccion Actual";
            using var exportData = new MemoryStream();
            excelWorkbook.Write(exportData);

            DateTime startDate = DateTime.Now;

            string saveAsFileName = string.Format("{0} {1:d}.xls", fileName, startDate.ToShortDateString()).ToUpper();

            byte[] bytes = exportData.ToArray();
            return File(bytes, "application/vnd.ms-excel", saveAsFileName);
        }
        public async Task<IActionResult> DailyProductionToExcel(DateTime selectedDate)
        {
            string[] columnsNames =
            {
                rm.GetString("loom"),
                rm.GetString("loomtype"),
                rm.GetString("shift"),
                rm.GetString("shiftsupervisor"),
                rm.GetString("running"),
                rm.GetString("standardefficiency"),
                rm.GetString("quality"),
                rm.GetString("designA"),
                "PPC".ToUpper(),
                rm.GetString("meters").ToUpper(),
                "Picks".ToUpper(),
                rm.GetString("speed").ToUpper(),
                rm.GetString("warpstops").ToUpper(),
                rm.GetString("weftstops").ToUpper(),
                rm.GetString("otherstops").ToUpper(),
                rm.GetString("weaver").ToUpper(),
            };

            string supervisorNameA = string.Empty;
            var shiftHistoryA = _context.ShiftHistory.FirstOrDefault(x => x.Shift == "A" && x.Date == selectedDate);
            if (shiftHistoryA != null && shiftHistoryA.Supervisor != null)
            {
                supervisorNameA = shiftHistoryA.Supervisor.WorkerName;
            }
            else
            {
                supervisorNameA = "No Ingresado";
            }

            var queryproductionA =await  _context.ProductionHistory
                                  .Include(x => x.Loom).ThenInclude(x => x.LoomType)
                                  .Include(x => x.Weaver)
                                  .Where(x => x.Date == selectedDate && x.Shift == "A")
                                  .Select(x => new
                                  {
                                      loom = x.Loom.LoomNumber,
                                      loomtype = x.Loom.LoomType.Description,
                                      shift = x.Shift,
                                      shiftsupervisor = supervisorNameA,
                                      running = string.Format("{0}%", (int)x.ActualEff),
                                      standardefficiency = string.Format("{0}%", (int)x.StdEff),
                                      quality = ObtenerCalidad(x.Article).Result,
                                      designA = x.Article,
                                      ppc = (int)x.Ppc,
                                      meters = x.Lenght / 100,
                                      picks = x.Picks,
                                      speed = x.Speed,
                                      warpstops = $"{rm.GetString("quantity").ToUpper()} : {(int)x.WarpStops}  {rm.GetString("time").ToUpper()} : {TimeSpan.FromSeconds((double)x.WarpStopTime).ToString("hh':'mm':'ss")}  {rm.GetString("minutes").ToUpper()} : {(int)(x.WarpStopTime / 60)}",
                                      weftstops = $"{rm.GetString("quantity").ToUpper()} : {(int)x.WeftStops}  {rm.GetString("time").ToUpper()} : {TimeSpan.FromSeconds((double)x.WeftStopTime).ToString("hh':'mm':'ss")}  {rm.GetString("minutes").ToUpper()} : {(int)(x.WeftStopTime / 60)}",
                                      otherstops = $"{rm.GetString("quantity").ToUpper()} : {(int)x.OtherStops}  {rm.GetString("time").ToUpper()} : {TimeSpan.FromSeconds((double)x.OtherStopTime).ToString("hh':'mm':'ss")}  {rm.GetString("minutes").ToUpper()} : {(int)(x.OtherStopTime / 60)}",
                                      weaver = GetWeaverAssignatedByLoomAndShiftAndDate(x.LoomId, x.Shift, (DateTime)x.Date).WorkerName
                                  })
                                  .OrderBy(x => x.loom).ToListAsync();

            string supervisorNameB = string.Empty;
            var shiftHistoryB = _context.ShiftHistory.FirstOrDefault(x => x.Shift == "B" && x.Date == selectedDate);
            if (shiftHistoryB != null && shiftHistoryB.Supervisor != null)
            {
                supervisorNameB = shiftHistoryB.Supervisor.WorkerName;
            }
            else
            {
                supervisorNameB = "No Ingresado";
            }

            var queryproductionB = await _context.ProductionHistory
                                  .Include(x => x.Loom).ThenInclude(x => x.LoomType)
                                  .Include(x => x.Weaver)
                                  .Where(x => x.Date == selectedDate && x.Shift == "B")
                                  .Select(x => new
                                  {
                                      loom = x.Loom.LoomNumber,
                                      loomtype = x.Loom.LoomType.Description,
                                      shift = x.Shift,
                                      shiftsupervisor = supervisorNameB,
                                      running = string.Format("{0}%", (int)x.ActualEff),
                                      standardefficiency = string.Format("{0}%", (int)x.StdEff),
                                      quality = ObtenerCalidad(x.Article).Result,
                                      designA = x.Article,
                                      ppc = (int)x.Ppc,
                                      meters = x.Lenght / 100,
                                      picks = x.Picks,
                                      speed = x.Speed,
                                      warpstops = $"{rm.GetString("quantity").ToUpper()} : {(int)x.WarpStops}  {rm.GetString("time").ToUpper()} : {TimeSpan.FromSeconds((double)x.WarpStopTime):hh':'mm':'ss}  {rm.GetString("minutes").ToUpper()} : {(int)(x.WarpStopTime / 60)}",
                                      weftstops = $"{rm.GetString("quantity").ToUpper()} : {(int)x.WeftStops}  {rm.GetString("time").ToUpper()} : {TimeSpan.FromSeconds((double)x.WeftStopTime):hh':'mm':'ss}  {rm.GetString("minutes").ToUpper()} : {(int)(x.WeftStopTime / 60)}",
                                      otherstops = $"{rm.GetString("quantity").ToUpper()} : {(int)x.OtherStops}  {rm.GetString("time").ToUpper()} : {TimeSpan.FromSeconds((double)x.OtherStopTime):hh':'mm':'ss}  {rm.GetString("minutes").ToUpper()} : {(int)(x.OtherStopTime / 60)}",
                                      weaver = GetWeaverAssignatedByLoomAndShiftAndDate(x.LoomId, x.Shift, (DateTime)x.Date).WorkerName
                                  })
                                  .OrderBy(x => x.loom).ToListAsync();

            string supervisorNameC = string.Empty;
            var shiftHistoryC = _context.ShiftHistory.FirstOrDefault(x => x.Shift == "C" && x.Date == selectedDate);
            if (shiftHistoryC != null && shiftHistoryC.Supervisor != null)
            {
                supervisorNameC = shiftHistoryC.Supervisor.WorkerName;
            }
            else
            {
                supervisorNameC = "No Ingresado";
            }

            var queryproductionC = await _context.ProductionHistory
                                  .Include(x => x.Loom).ThenInclude(x => x.LoomType)
                                  .Include(x => x.Weaver)
                                  .Where(x => x.Date == selectedDate && x.Shift == "C")
                                  .Select(x => new
                                  {
                                      loom = x.Loom.LoomNumber,
                                      loomtype = x.Loom.LoomType.Description,
                                      shift = x.Shift,
                                      shiftsupervisor = supervisorNameC,
                                      running = string.Format("{0}%", (int)x.ActualEff),
                                      standardefficiency = string.Format("{0}%", (int)x.StdEff),
                                      quality = ObtenerCalidad(x.Article).Result,
                                      designA = x.Article,
                                      ppc = (int)x.Ppc,
                                      meters = x.Lenght / 100,
                                      picks = x.Picks,
                                      speed = x.Speed,
                                      warpstops = $"{rm.GetString("quantity").ToUpper()} : {(int)x.WarpStops}  {rm.GetString("time").ToUpper()} : {TimeSpan.FromSeconds((double)x.WarpStopTime).ToString("hh':'mm':'ss")}  {rm.GetString("minutes").ToUpper()} : {(int)(x.WarpStopTime / 60)}",
                                      weftstops = $"{rm.GetString("quantity").ToUpper()} : {(int)x.WeftStops}  {rm.GetString("time").ToUpper()} : {TimeSpan.FromSeconds((double)x.WeftStopTime).ToString("hh':'mm':'ss")}  {rm.GetString("minutes").ToUpper()} : {(int)(x.WeftStopTime / 60)}",
                                      otherstops = $"{rm.GetString("quantity").ToUpper()} : {(int)x.OtherStops}  {rm.GetString("time").ToUpper()} : {TimeSpan.FromSeconds((double)x.OtherStopTime).ToString("hh':'mm':'ss")}  {rm.GetString("minutes").ToUpper()} : {(int)(x.OtherStopTime / 60)}",
                                      weaver = GetWeaverAssignatedByLoomAndShiftAndDate(x.LoomId, x.Shift, (DateTime)x.Date).WorkerName
                                  })
                                  .OrderBy(x => x.loom).ToListAsync();
            //LLenarHoja(id);
            DetalleReporte(selectedDate, null ,rm.GetString("dailyproduction").ToUpper(), null);
            ProduccionDiaria(selectedDate);
            if (queryproductionA.Count > 0) 
            {
                string title = string.Format("{0} {1}\n{2}: {3}", rm.GetString("nightshift").ToUpper(), selectedDate.Date.ToString("dd/MM/yyyy")
                , rm.GetString("shiftsupervisor").ToUpper(), supervisorNameA);

                NpoiFunctions.CreateE(excelWorkbook, title, rm.GetString("nightshift"), queryproductionA, columnsNames);
            }
            if (queryproductionB.Count > 0)
            {
                string title = string.Format("{0} {1}\n{2}: {3}", rm.GetString("morningshift").ToUpper(), selectedDate.Date.ToString("dd/MM/yyyy")
                , rm.GetString("shiftsupervisor").ToUpper(), supervisorNameB);

                NpoiFunctions.CreateE(excelWorkbook, title, rm.GetString("morningshift"), queryproductionB, columnsNames);
            }
            if (queryproductionC.Count > 0)
            {
                string title = string.Format("{0} {1}\n{2}: {3}", rm.GetString("afternoonshift").ToUpper(), selectedDate.Date.ToString("dd/MM/yyyy")
                , rm.GetString("shiftsupervisor").ToUpper(), supervisorNameC);

                NpoiFunctions.CreateE(excelWorkbook, title, rm.GetString("afternoonshift"), queryproductionC, columnsNames);
            }
            const string fileName = "Reporte Produccion Diaria";
            using (var exportData = new MemoryStream())
            {
                excelWorkbook.Write(exportData);

                string saveAsFileName = string.Format("{0} {1:d}.xls", fileName, selectedDate.ToShortDateString()).ToUpper();

                byte[] bytes = exportData.ToArray();
                return File(bytes, "application/vnd.ms-excel", saveAsFileName);
            }
        }
        public async Task<IActionResult> ShiftProductionToExcel(DateTime selectedDate, string shift)
        {
            string supervisorName = string.Empty;
            if (_context.ShiftHistory.FirstOrDefault(x => x.Shift == shift && x.Date == selectedDate) != null)
            {
                supervisorName = _context.ShiftHistory.FirstOrDefault(x => x.Shift == shift && x.Date == selectedDate).Supervisor.WorkerName;
            }
            else
            {
                supervisorName = "No Ingresado";
            }

            string[] columnsNames =
            {
                rm.GetString("loom"),
                rm.GetString("loomtype"),
                rm.GetString("shift"),
                rm.GetString("shiftsupervisor"),
                rm.GetString("running"),
                rm.GetString("standardefficiency"),
                rm.GetString("quality"),
                rm.GetString("designA"),
                "PPC".ToUpper(),
                rm.GetString("meters").ToUpper(),
                "Picks".ToUpper(),
                rm.GetString("speed").ToUpper(),
                rm.GetString("warpstops").ToUpper(),
                rm.GetString("weftstops").ToUpper(),
                rm.GetString("otherstops").ToUpper(),
                rm.GetString("weaver").ToUpper(),
            };

            var queryproduction = await _context.ProductionHistory
                                  .Include(x => x.Loom).ThenInclude(x => x.LoomType)
                                  .Include(x => x.Weaver)
                                  .Where(x => x.Date == selectedDate && x.Shift == shift)
                                    .Select(x => new
                                    {
                                        loom = x.Loom.LoomNumber,
                                        loomtype = x.Loom.LoomType.Description,
                                        shift = x.Shift,
                                        shiftsupervisor = supervisorName,
                                        running = string.Format("{0}%", (int)x.ActualEff),
                                        standardefficiency = string.Format("{0}%", (int)x.StdEff),
                                        quality = ObtenerCalidad(x.Article).Result,
                                        designA = x.Article,
                                        ppc = (int)x.Ppc,
                                        meters = x.Lenght / 100,
                                        picks = x.Picks,
                                        speed = x.Speed,
                                        warpstops = $"{rm.GetString("quantity").ToUpper()} : {(int)x.WarpStops}  {rm.GetString("time").ToUpper()} : {TimeSpan.FromSeconds((double)x.WarpStopTime).ToString("hh':'mm':'ss")}  {rm.GetString("minutes").ToUpper()} : {(int)(x.WarpStopTime / 60)}",
                                        weftstops = $"{rm.GetString("quantity").ToUpper()} : {(int)x.WeftStops}  {rm.GetString("time").ToUpper()} : {TimeSpan.FromSeconds((double)x.WeftStopTime).ToString("hh':'mm':'ss")}  {rm.GetString("minutes").ToUpper()} : {(int)(x.WeftStopTime / 60)}",
                                        otherstops = $"{rm.GetString("quantity").ToUpper()} : {(int)x.OtherStops}  {rm.GetString("time").ToUpper()} : {TimeSpan.FromSeconds((double)x.OtherStopTime).ToString("hh':'mm':'ss")}  {rm.GetString("minutes").ToUpper()} : {(int)(x.OtherStopTime / 60)}",
                                        weaver = GetWeaverAssignatedByLoomAndShiftAndDate(x.LoomId, x.Shift, (DateTime)x.Date).WorkerName
                                    })
                                  .OrderBy(x => x.loom).ToListAsync();
            string fileName = string.Empty;
            switch (shift)
            {
                case "A":
                    fileName = rm.GetString("nightshift");
                    break;
                case "B":
                    fileName = rm.GetString("morningshift");
                    break;
                case "C":
                    fileName = rm.GetString("afternoonshift");
                    break;
            }

            DetalleReporte(selectedDate, null ,rm.GetString("shift").ToUpper(), shift);

            string title = string.Format("{0} {1}\n{2}: {3}", fileName.ToUpper(), selectedDate.Date.ToString("dd/MM/yyyy")
            , rm.GetString("shiftsupervisor").ToUpper(), supervisorName);

            if (queryproduction.Count > 0) { NpoiFunctions.CreateE(excelWorkbook, title, fileName, queryproduction, columnsNames); }

            using (var exportData = new MemoryStream())
            {
                excelWorkbook.Write(exportData);

                string saveAsFileName = string.Format("Reporte {0} {1:d}.xls", fileName, selectedDate.ToShortDateString()).ToUpper();

                byte[] bytes = exportData.ToArray();
                return File(bytes, "application/vnd.ms-excel", saveAsFileName);
            }
        }
        public async Task<IActionResult> GeneralProductionToExcel(DateTime since, DateTime until, string shift)
        {
            var queryproduction = await _context.ProductionHistory
                                  .Include(x => x.Loom).ThenInclude(x => x.LoomType)
                                  .Include(x => x.Weaver)
                                   .Where(x => x.Date >= since.Date && x.Date <= until.Date
                                   && x.Loom.IsCalculated == true && x.Loom.Status == true)
                                  .OrderBy(x => x.Loom.LoomNumber).ToListAsync();

            if (shift != "T")
            {
                queryproduction = queryproduction.Where(x => x.Shift == shift).ToList();
            }
            var generalProductionGroup = queryproduction.GroupBy(x => x.LoomId)
                    .Select(g => new ProductionHistory
                    {
                        LoomId = g.Key,
                        Loom = g.FirstOrDefault(x => x.LoomId == g.Key).Loom,
                        ActualEff = g.Average(x => x.ActualEff),
                        StdEff = g.Average(x => x.StdEff),
                        Ppc = g.Average(x => x.Ppc),
                        Lenght = g.Sum(x => x.Lenght),
                        Picks = (int?)g.Average(x => x.Picks),
                        Speed = (int?)g.Average(x => x.Speed),
                        WarpStops = g.Sum(x => x.WarpStops),
                        WarpStopTime = g.Sum(x => x.WarpStopTime),
                        WeftStops = g.Sum(x => x.WeftStops),
                        WeftStopTime = g.Sum(x => x.WeftStopTime),
                        OtherStops = g.Sum(x => x.OtherStops),
                        OtherStopTime = g.Sum(x => x.OtherStopTime)
                    }).ToList();
            string fileName = rm.GetString("all");
            switch (shift)
            {
                case "A":
                    fileName = rm.GetString("nightshift");
                    break;
                case "B":
                    fileName = rm.GetString("morningshift");
                    break;
                case "C":
                    fileName = rm.GetString("afternoonshift");
                    break;
            }
            DetalleReporte(since, until.ToShortDateString(), rm.GetString("generalproduccion").ToUpper(), shift);

            var List = generalProductionGroup
                .Select(x => 
                new {
                      loom = x.Loom.LoomNumber,
                      loomtype = x.Loom.LoomType.Description,
                      running = string.Format("{0}%", (int)x.ActualEff),
                      standardefficiency = string.Format("{0}%", (int)x.StdEff),
                      ppc = (int)x.Ppc,
                      meters = x.Lenght/100,
                      picks = x.Picks,
                      speed = x.Speed,
                      warpstops = $"{rm.GetString("quantity").ToUpper()} : {(int)x.WarpStops}  {rm.GetString("time").ToUpper()} : {TimeSpan.FromSeconds((double)x.WarpStopTime):hh':'mm':'ss}  {rm.GetString("minutes").ToUpper()} : {(int)(x.WarpStopTime / 60)}",
                      weftstops = $"{rm.GetString("quantity").ToUpper()} : {(int)x.WeftStops}  {rm.GetString("time").ToUpper()} : {TimeSpan.FromSeconds((double)x.WeftStopTime):hh':'mm':'ss}  {rm.GetString("minutes").ToUpper()} : {(int)(x.WeftStopTime / 60)}",
                      otherstops = $"{rm.GetString("quantity").ToUpper()} : {(int)x.OtherStops}  {rm.GetString("time").ToUpper()} : {TimeSpan.FromSeconds((double)x.OtherStopTime):hh':'mm':'ss}  {rm.GetString("minutes").ToUpper()} : {(int)(x.OtherStopTime / 60)}",
                }).ToList();

            string[] columnsNames =
            {
                rm.GetString("loom"),
                rm.GetString("loomtype"),
                rm.GetString("running"),
                rm.GetString("standardefficiency"),
                "PPC".ToUpper(),
                rm.GetString("meters").ToUpper(),
                "Picks".ToUpper(),
                rm.GetString("speed").ToUpper(),
                rm.GetString("warpstops").ToUpper(),
                rm.GetString("weftstops").ToUpper(),
                rm.GetString("otherstops").ToUpper(),
            };

            if (queryproduction.Count > 0)
            {
                NpoiFunctions.CreateE(excelWorkbook, rm.GetString("generalproduccion"), fileName, List, columnsNames);
            }

            using (var exportData = new MemoryStream())
            {
                excelWorkbook.Write(exportData);

                string saveAsFileName = string.Format("Reporte {0} {1:d} {2:d}.xls", fileName, since.ToShortDateString(), until.ToShortDateString()).ToUpper();

                byte[] bytes = exportData.ToArray();
                return File(bytes, "application/vnd.ms-excel", saveAsFileName);
            }
        }

        public async Task<IActionResult> StopProductionHistoryToExcel(DateTime since, DateTime until, string shift, int stopType)
        {
            var stopHistory = await _context.StopHistory
                        .Include(x => x.StopType)
                        .Include(x => x.ProductionHistory)
                            .ThenInclude(x => x.Loom)
                        .Where(x => x.StoppedTime > 0
                        && x.Date >= since.Date && x.Date <= until.Date)
                        .AsNoTracking()
                        .ToListAsync();

            if (shift != "T")
            {
                stopHistory = stopHistory.Where(x => x.Shift == shift).ToList();
            }

            var stopHistoryGroup = stopHistory.GroupBy(x => new { x.LoomId, x.StopTypeId })
                                .Select(g => new StopHistory
                                {
                                    LoomId = g.Key.LoomId,
                                    ProductionHistory = g.FirstOrDefault(x => x.LoomId == g.Key.LoomId).ProductionHistory,
                                    Shift = g.FirstOrDefault(x => x.LoomId == g.Key.LoomId).Shift,
                                    Stops = g.Sum(x => x.Stops),
                                    StoppedTime = g.Sum(x => x.StoppedTime),
                                    StopType = g.FirstOrDefault(x => x.StopTypeId == g.Key.StopTypeId).StopType

                                }).ToList();
            string fileName = rm.GetString("all");
            switch (shift)
            {
                case "A":
                    fileName = rm.GetString("nightshift");
                    break;
                case "B":
                    fileName = rm.GetString("morningshift");
                    break;
                case "C":
                    fileName = rm.GetString("afternoonshift");
                    break;
            }
            DetalleReporte(since, until.ToShortDateString(), rm.GetString("stoppagehistory").ToUpper(), shift);

            var List = stopHistoryGroup
                .ConvertAll(x =>
                new {
                        loom = x.ProductionHistory.Loom.LoomNumber,
                        shift = x.Shift,
                        stops = x.Stops,
                        minutes = Math.Round((decimal)(x.StoppedTime / 60),1),
                        stoptype = x.StopType.Description
                });

            string[] columnsNames =
            {
                rm.GetString("loom"),
                rm.GetString("shift"),
                rm.GetString("stops"),
                rm.GetString("minutes"),
                rm.GetString("stoptype").ToUpper(),
            };

            if (stopHistory.Count > 0)
            {
                NpoiFunctions.CreateE(excelWorkbook, rm.GetString("stoppagehistory"), fileName, List, columnsNames);
            }

            using (var exportData = new MemoryStream())
            {
                excelWorkbook.Write(exportData);

                string saveAsFileName = string.Format("Reporte {0} {1:d} {2:d}.xls", fileName, since.ToShortDateString(), until.ToShortDateString()).ToUpper();

                byte[] bytes = exportData.ToArray();
                return File(bytes, "application/vnd.ms-excel", saveAsFileName);
            }
        }

        private void DetalleReporte(DateTime selectedDate, string secondDate, string tipoReporte, string shift)
        {
            ICellStyle estiloFila = NpoiFunctions.EstiloFila(excelWorkbook);
            ICellStyle estiloHeader = NpoiFunctions.EstiloHeader(excelWorkbook);

            List<ReportDetailBO> details = new()
            {
                new ReportDetailBO()
                {
                    Description = rm.GetString("date"),
                    Content = selectedDate.ToShortDateString()
                }
            };

            if (secondDate != null)
            {
                details.Add(new ReportDetailBO()
                {
                    Description = rm.GetString("shift"),
                    Content = secondDate
                });
            }

            if (shift != null)
            {
                details.Add(new ReportDetailBO()
                {
                    Description = rm.GetString("shift"),
                    Content = (shift == "T")?"Todos":shift
                });
            }

            ISheet sheet = excelWorkbook.CreateSheet("Detalles Reporte");
            int num = 0;
            #region Headers
            IRow row = sheet.CreateRow(num);
            ICell cell = row.CreateCell(0);
            cell.SetCellValue(string.Format("{0}", "SISTEMA DE REPORTES DE TCS").ToUpper());
            cell.CellStyle = estiloHeader;
            cell.Row.Height = 800;
            NpoiFunctions.MergeCells(sheet, excelWorkbook, num, num, 0, 1);
            num++;

            row = sheet.CreateRow(num);
            NpoiFunctions.NewCell(row, estiloHeader, 0, "Nombre de Reporte".ToUpper());
            NpoiFunctions.NewCell(row, estiloFila, 1,tipoReporte);
            num++;

            row = sheet.CreateRow(num);
            NpoiFunctions.NewCell(row, estiloHeader, 0, rm.GetString("date").ToUpper());
            NpoiFunctions.NewCell(row, estiloFila, 1, DateTime.Now.ToShortDateString());
            num++;

            row = sheet.CreateRow(num);
            NpoiFunctions.NewCell(row, estiloHeader, 0, rm.GetString("hour").ToUpper());
            NpoiFunctions.NewCell(row, estiloFila, 1, DateTime.Now.ToShortTimeString());
            num++;

            row = sheet.CreateRow(num);
            NpoiFunctions.NewCell(row, estiloHeader, 0, "Ejecutado por".ToUpper());
            NpoiFunctions.NewCell(row, estiloFila, 1, User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value.ToUpper());
            num += 2;

            if (details != null && details.Count > 0)
            {
                row = sheet.CreateRow(num);
                cell = row.CreateCell(0);
                cell.SetCellValue(string.Format("{0}", "Opciones de Filtrado".ToUpper()));
                cell.CellStyle = estiloHeader;
                NpoiFunctions.MergeCells(sheet, excelWorkbook, num, num, 0, 1);
                num++;

                foreach (var item in details)
                {
                    row = sheet.CreateRow(num);
                    NpoiFunctions.NewCell(row, estiloHeader, 0, item.Description.ToUpper());
                    NpoiFunctions.NewCell(row, estiloFila, 1, item.Content.ToUpper());
                    num++;
                }
            }

            #endregion

            NpoiFunctions.SetColumn(sheet, 0, 30);
            NpoiFunctions.SetColumn(sheet, 1, 50);
        }

        private void ProduccionDiaria(DateTime selectedDate)
        {
            string sheetTitle = rm.GetString("dailyproduction");

            ISheet sheet = excelWorkbook.CreateSheet(sheetTitle);

            #region Cell Styles
            #region Basic CellStyle
            //IFont font = excelWorkbook.CreateFont();

            ICellStyle cellStyle = excelWorkbook.CreateCellStyle();
            cellStyle.Alignment = HorizontalAlignment.Center;
            cellStyle.VerticalAlignment = VerticalAlignment.Center;
            //cellStyle.SetFont(font);

            cellStyle.BorderTop = BorderStyle.Thin;
            cellStyle.BorderLeft = BorderStyle.Thin;
            cellStyle.BorderRight = BorderStyle.Thin;
            cellStyle.BorderBottom = BorderStyle.Thin;
            #endregion

            #region Titles CellStyle
            IFont titlesFont = excelWorkbook.CreateFont();
            titlesFont.Boldweight = (short)FontBoldWeight.Bold;

            ICellStyle titlesCS = excelWorkbook.CreateCellStyle();
            titlesCS.Alignment = HorizontalAlignment.Center;
            titlesCS.VerticalAlignment = VerticalAlignment.Center;
            titlesCS.SetFont(titlesFont);
            titlesCS.WrapText = true;

            titlesCS.BorderTop = BorderStyle.Thin;
            titlesCS.BorderLeft = BorderStyle.Thin;
            titlesCS.BorderRight = BorderStyle.Thin;
            titlesCS.BorderBottom = BorderStyle.Thin;

            titlesCS.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Grey25Percent.Index;
            titlesCS.FillPattern = FillPattern.SolidForeground;
            #endregion

            #region Decimal CellStyle
            ICellStyle decimalsCS = excelWorkbook.CreateCellStyle();
            decimalsCS.Alignment = HorizontalAlignment.Center;
            decimalsCS.VerticalAlignment = VerticalAlignment.Center;
            decimalsCS.SetFont(excelWorkbook.CreateFont());

            decimalsCS.BorderTop = BorderStyle.Thin;
            decimalsCS.BorderLeft = BorderStyle.Thin;
            decimalsCS.BorderRight = BorderStyle.Thin;
            decimalsCS.BorderBottom = BorderStyle.Thin;
            decimalsCS.DataFormat = HSSFDataFormat.GetBuiltinFormat("0.00");
            #endregion
            #endregion

            int num = 0;
            List<ProductionHistory> shiftA = _context.ProductionHistory.Where(x=>x.Date == selectedDate && x.Shift == "A").ToList();
            List<ProductionHistory> shiftB = _context.ProductionHistory.Where(x => x.Date == selectedDate && x.Shift == "B").ToList();
            List<ProductionHistory> shiftC = _context.ProductionHistory.Where(x => x.Date == selectedDate && x.Shift == "C").ToList();

            ShiftHistory infoShiftA = _context.ShiftHistory.Include(x=>x.Supervisor).FirstOrDefault(x=>x.Date == selectedDate && x.Shift == "A");
            ShiftHistory infoShiftB = _context.ShiftHistory.Include(x => x.Supervisor).FirstOrDefault(x => x.Date == selectedDate && x.Shift == "B");
            ShiftHistory infoShiftC = _context.ShiftHistory.Include(x => x.Supervisor).FirstOrDefault(x => x.Date == selectedDate && x.Shift == "C");

            //Group Production by Weaver (Loom Group)
            List<IGrouping<int?, ProductionHistory>> shiftAGroupedByWeaver = shiftA.GroupBy(x => x.LoomGroupId).ToList();
            List<IGrouping<int?, ProductionHistory>> shiftBGroupedByWeaver = shiftB.GroupBy(x => x.LoomGroupId).ToList();
            List<IGrouping<int?, ProductionHistory>> shiftCGroupedByWeaver = shiftC.GroupBy(x => x.LoomGroupId).ToList();

            #region Header
            IRow row = sheet.CreateRow(num);
            ICell cell = row.CreateCell(0);
            cell.SetCellValue(string.Format("{0} {1}", rm.GetString("dailyproduction").ToUpper(), selectedDate.ToShortDateString()).ToUpper());
            cell.CellStyle = titlesCS;
            cell.Row.Height = 800;
            NPOI.SS.Util.CellRangeAddress cra = new(num, num, 0, 14);
            sheet.AddMergedRegion(cra);
            NPOI.SS.Util.RegionUtil.SetBorderTop(1, cra, sheet, excelWorkbook);
            NPOI.SS.Util.RegionUtil.SetBorderRight(1, cra, sheet, excelWorkbook);
            NPOI.SS.Util.RegionUtil.SetBorderLeft(1, cra, sheet, excelWorkbook);
            NPOI.SS.Util.RegionUtil.SetBorderBottom(1, cra, sheet, excelWorkbook);
            num++;

            row = sheet.CreateRow(num);
            cell = row.CreateCell(0);
            cell.SetCellValue(rm.GetString("nightshift").ToUpper());
            cell.CellStyle = titlesCS;
            cell.Row.Height = 400;
            cra = new NPOI.SS.Util.CellRangeAddress(num, num, 0, 4);
            sheet.AddMergedRegion(cra);
            NPOI.SS.Util.RegionUtil.SetBorderTop(1, cra, sheet, excelWorkbook);
            NPOI.SS.Util.RegionUtil.SetBorderRight(1, cra, sheet, excelWorkbook);
            NPOI.SS.Util.RegionUtil.SetBorderLeft(1, cra, sheet, excelWorkbook);
            NPOI.SS.Util.RegionUtil.SetBorderBottom(1, cra, sheet, excelWorkbook);

            cell = row.CreateCell(5);
            cell.SetCellValue(rm.GetString("morningshift").ToUpper());
            cell.CellStyle = titlesCS;
            cell.Row.Height = 400;
            cra = new NPOI.SS.Util.CellRangeAddress(num, num, 5, 9);
            sheet.AddMergedRegion(cra);
            NPOI.SS.Util.RegionUtil.SetBorderTop(1, cra, sheet, excelWorkbook);
            NPOI.SS.Util.RegionUtil.SetBorderRight(1, cra, sheet, excelWorkbook);
            NPOI.SS.Util.RegionUtil.SetBorderLeft(1, cra, sheet, excelWorkbook);
            NPOI.SS.Util.RegionUtil.SetBorderBottom(1, cra, sheet, excelWorkbook);

            cell = row.CreateCell(10);
            cell.SetCellValue(rm.GetString("afternoonshift").ToUpper());
            cell.CellStyle = titlesCS;
            cell.Row.Height = 400;
            cra = new NPOI.SS.Util.CellRangeAddress(num, num, 10, 14);
            sheet.AddMergedRegion(cra);
            NPOI.SS.Util.RegionUtil.SetBorderTop(1, cra, sheet, excelWorkbook);
            NPOI.SS.Util.RegionUtil.SetBorderRight(1, cra, sheet, excelWorkbook);
            NPOI.SS.Util.RegionUtil.SetBorderLeft(1, cra, sheet, excelWorkbook);
            NPOI.SS.Util.RegionUtil.SetBorderBottom(1, cra, sheet, excelWorkbook);
            num++;

            #region Fila Jefes de Turno
            row = sheet.CreateRow(num);
            #region Jefe A            
            cell = row.CreateCell(0);
            cell.SetCellValue(rm.GetString("shiftsupervisor").ToUpper());
            cell.CellStyle = titlesCS;
            cra = new NPOI.SS.Util.CellRangeAddress(num, num, 0, 1);
            sheet.AddMergedRegion(cra);
            NPOI.SS.Util.RegionUtil.SetBorderTop(1, cra, sheet, excelWorkbook);
            NPOI.SS.Util.RegionUtil.SetBorderRight(1, cra, sheet, excelWorkbook);
            NPOI.SS.Util.RegionUtil.SetBorderLeft(1, cra, sheet, excelWorkbook);
            NPOI.SS.Util.RegionUtil.SetBorderBottom(1, cra, sheet, excelWorkbook);

            cell = row.CreateCell(2);
            if (infoShiftA != null)
                cell.SetCellValue(infoShiftA.Supervisor.WorkerName);
            cell.CellStyle = cellStyle;
            cra = new NPOI.SS.Util.CellRangeAddress(num, num, 2, 4);
            sheet.AddMergedRegion(cra);
            NPOI.SS.Util.RegionUtil.SetBorderTop(1, cra, sheet, excelWorkbook);
            NPOI.SS.Util.RegionUtil.SetBorderRight(1, cra, sheet, excelWorkbook);
            NPOI.SS.Util.RegionUtil.SetBorderLeft(1, cra, sheet, excelWorkbook);
            NPOI.SS.Util.RegionUtil.SetBorderBottom(1, cra, sheet, excelWorkbook);
            #endregion

            #region Jefe B
            cell = row.CreateCell(5);
            cell.SetCellValue(rm.GetString("shiftsupervisor").ToUpper());
            cell.CellStyle = titlesCS;
            cra = new NPOI.SS.Util.CellRangeAddress(num, num, 5, 6);
            sheet.AddMergedRegion(cra);
            NPOI.SS.Util.RegionUtil.SetBorderTop(1, cra, sheet, excelWorkbook);
            NPOI.SS.Util.RegionUtil.SetBorderRight(1, cra, sheet, excelWorkbook);
            NPOI.SS.Util.RegionUtil.SetBorderLeft(1, cra, sheet, excelWorkbook);
            NPOI.SS.Util.RegionUtil.SetBorderBottom(1, cra, sheet, excelWorkbook);

            cell = row.CreateCell(7);
            if (infoShiftB != null)
                cell.SetCellValue((infoShiftB.Supervisor!=null) ? infoShiftB.Supervisor.WorkerName : "No Ingresado");
            cell.CellStyle = cellStyle;
            cra = new NPOI.SS.Util.CellRangeAddress(num, num, 7, 9);
            sheet.AddMergedRegion(cra);
            NPOI.SS.Util.RegionUtil.SetBorderTop(1, cra, sheet, excelWorkbook);
            NPOI.SS.Util.RegionUtil.SetBorderRight(1, cra, sheet, excelWorkbook);
            NPOI.SS.Util.RegionUtil.SetBorderLeft(1, cra, sheet, excelWorkbook);
            NPOI.SS.Util.RegionUtil.SetBorderBottom(1, cra, sheet, excelWorkbook);
            #endregion

            #region Jefe C
            cell = row.CreateCell(10);
            cell.SetCellValue(rm.GetString("shiftsupervisor").ToUpper());
            cell.CellStyle = titlesCS;
            cra = new NPOI.SS.Util.CellRangeAddress(num, num, 10, 11);
            sheet.AddMergedRegion(cra);
            NPOI.SS.Util.RegionUtil.SetBorderTop(1, cra, sheet, excelWorkbook);
            NPOI.SS.Util.RegionUtil.SetBorderRight(1, cra, sheet, excelWorkbook);
            NPOI.SS.Util.RegionUtil.SetBorderLeft(1, cra, sheet, excelWorkbook);
            NPOI.SS.Util.RegionUtil.SetBorderBottom(1, cra, sheet, excelWorkbook);

            cell = row.CreateCell(12);
            if (infoShiftC != null)
                cell.SetCellValue((infoShiftC.Supervisor != null)? infoShiftC.Supervisor.WorkerName : string.Empty);
            cell.CellStyle = cellStyle;
            cra = new NPOI.SS.Util.CellRangeAddress(num, num, 12, 14);
            sheet.AddMergedRegion(cra);
            NPOI.SS.Util.RegionUtil.SetBorderTop(1, cra, sheet, excelWorkbook);
            NPOI.SS.Util.RegionUtil.SetBorderRight(1, cra, sheet, excelWorkbook);
            NPOI.SS.Util.RegionUtil.SetBorderLeft(1, cra, sheet, excelWorkbook);
            NPOI.SS.Util.RegionUtil.SetBorderBottom(1, cra, sheet, excelWorkbook);
            #endregion
            num++;
            #endregion
            #endregion

            #region Create Rows

            List<Loom> datalogLooms = _context.ProductionHistory
                                        .Where(x => x.Date == selectedDate)
                                        .Include(x => x.Loom)
                                        .GroupBy(x => new { x.LoomId, x.Loom.LoomNumber, x.LoomGroupId })
                                        .Select(x => new Loom
                                        {
                                            LoomId = x.Key.LoomId,
                                            LoomNumber = x.Key.LoomNumber,
                                            LoomGroupId = x.Key.LoomGroupId
                                        }).ToList(); 
            //Obtener listado de telares que tienen produccion en ese dia (no necesariamente en todos los turnos) para darle forma al reporte
            List<IGrouping<int?, Loom>> loomsGroupedByLoomGroup = datalogLooms.GroupBy(x => x.LoomGroupId).ToList();

            int[] rowTotalLoomGroup = new int[loomsGroupedByLoomGroup.Count];

            int loomGroupID = 1;
            foreach (var item in loomsGroupedByLoomGroup)
            {
                IGrouping<int?, ProductionHistory> itemShiftA = null;
                IGrouping<int?, ProductionHistory> itemShiftB = null;
                IGrouping<int?, ProductionHistory> itemShiftC = null;

                itemShiftA = shiftAGroupedByWeaver.SingleOrDefault(x => x.Key == loomGroupID);

                itemShiftB = shiftBGroupedByWeaver.SingleOrDefault(x => x.Key == loomGroupID);

                itemShiftC = shiftCGroupedByWeaver.SingleOrDefault(x => x.Key == loomGroupID);

                #region Tejedores
                #region Tejedor Turno A
                row = sheet.CreateRow(num);
                cell = row.CreateCell(0);
                cell.SetCellValue(rm.GetString("weaver").ToUpper());
                cell.CellStyle = titlesCS;
                cell.CellStyle.WrapText = true;
                cra = new NPOI.SS.Util.CellRangeAddress(num, num, 0, 1);
                sheet.AddMergedRegion(cra);
                NPOI.SS.Util.RegionUtil.SetBorderTop(1, cra, sheet, excelWorkbook);
                NPOI.SS.Util.RegionUtil.SetBorderRight(1, cra, sheet, excelWorkbook);
                NPOI.SS.Util.RegionUtil.SetBorderLeft(1, cra, sheet, excelWorkbook);
                NPOI.SS.Util.RegionUtil.SetBorderBottom(1, cra, sheet, excelWorkbook);

                cell = row.CreateCell(2);
                if (itemShiftA != null)
                    cell.SetCellValue(itemShiftA.First().Weaver.WorkerName);
                cell.CellStyle = cellStyle;
                cell.CellStyle.WrapText = true;
                cra = new NPOI.SS.Util.CellRangeAddress(num, num, 2, 4);
                sheet.AddMergedRegion(cra);
                NPOI.SS.Util.RegionUtil.SetBorderTop(1, cra, sheet, excelWorkbook);
                NPOI.SS.Util.RegionUtil.SetBorderRight(1, cra, sheet, excelWorkbook);
                NPOI.SS.Util.RegionUtil.SetBorderLeft(1, cra, sheet, excelWorkbook);
                NPOI.SS.Util.RegionUtil.SetBorderBottom(1, cra, sheet, excelWorkbook);
                #endregion

                #region Tejedor Turno B
                cell = row.CreateCell(5);
                cell.SetCellValue(rm.GetString("weaver").ToUpper());
                cell.CellStyle = titlesCS;
                cell.CellStyle.WrapText = true;
                cra = new NPOI.SS.Util.CellRangeAddress(num, num, 5, 6);
                sheet.AddMergedRegion(cra);
                NPOI.SS.Util.RegionUtil.SetBorderTop(1, cra, sheet, excelWorkbook);
                NPOI.SS.Util.RegionUtil.SetBorderRight(1, cra, sheet, excelWorkbook);
                NPOI.SS.Util.RegionUtil.SetBorderLeft(1, cra, sheet, excelWorkbook);
                NPOI.SS.Util.RegionUtil.SetBorderBottom(1, cra, sheet, excelWorkbook);

                cell = row.CreateCell(7);
                if (itemShiftB != null)
                    cell.SetCellValue((itemShiftB.First().Weaver != null) ? itemShiftB.First().Weaver.WorkerName: rm.GetString("unassigned"));
                cell.CellStyle = cellStyle;
                cell.CellStyle.WrapText = true;
                cra = new NPOI.SS.Util.CellRangeAddress(num, num, 7, 9);
                sheet.AddMergedRegion(cra);
                NPOI.SS.Util.RegionUtil.SetBorderTop(1, cra, sheet, excelWorkbook);
                NPOI.SS.Util.RegionUtil.SetBorderRight(1, cra, sheet, excelWorkbook);
                NPOI.SS.Util.RegionUtil.SetBorderLeft(1, cra, sheet, excelWorkbook);
                NPOI.SS.Util.RegionUtil.SetBorderBottom(1, cra, sheet, excelWorkbook);
                #endregion

                #region Tejedor Turno C
                cell = row.CreateCell(10);
                cell.SetCellValue(rm.GetString("weaver").ToUpper());
                cell.CellStyle = titlesCS;
                cell.CellStyle.WrapText = true;
                cra = new NPOI.SS.Util.CellRangeAddress(num, num, 10, 11);
                sheet.AddMergedRegion(cra);
                NPOI.SS.Util.RegionUtil.SetBorderTop(1, cra, sheet, excelWorkbook);
                NPOI.SS.Util.RegionUtil.SetBorderRight(1, cra, sheet, excelWorkbook);
                NPOI.SS.Util.RegionUtil.SetBorderLeft(1, cra, sheet, excelWorkbook);
                NPOI.SS.Util.RegionUtil.SetBorderBottom(1, cra, sheet, excelWorkbook);

                cell = row.CreateCell(12);
                if (itemShiftC != null)
                    cell.SetCellValue((itemShiftC.FirstOrDefault().Weaver != null) ? itemShiftC.FirstOrDefault().Weaver.WorkerName : string.Empty);
                cell.CellStyle = cellStyle;
                cell.CellStyle.WrapText = true;
                cra = new NPOI.SS.Util.CellRangeAddress(num, num, 12, 14);
                sheet.AddMergedRegion(cra);
                NPOI.SS.Util.RegionUtil.SetBorderTop(1, cra, sheet, excelWorkbook);
                NPOI.SS.Util.RegionUtil.SetBorderRight(1, cra, sheet, excelWorkbook);
                NPOI.SS.Util.RegionUtil.SetBorderLeft(1, cra, sheet, excelWorkbook);
                NPOI.SS.Util.RegionUtil.SetBorderBottom(1, cra, sheet, excelWorkbook);
                num++;
                #endregion
                #endregion

                row = sheet.CreateRow(num);
                NpoiFunctions.NewCell(row, titlesCS, 0, rm.GetString("loom").ToUpper());
                NpoiFunctions.NewCell(row, titlesCS, 1, "KPICKS");
                NpoiFunctions.NewCell(row, titlesCS, 2, "MTS");
                NpoiFunctions.NewCell(row, titlesCS, 3, "%");
                NpoiFunctions.NewCell(row, titlesCS, 4, "OBS.");
                NpoiFunctions.NewCell(row, titlesCS, 5, rm.GetString("loom").ToUpper());
                NpoiFunctions.NewCell(row, titlesCS, 6, "KPICKS");
                NpoiFunctions.NewCell(row, titlesCS, 7, "MTS");
                NpoiFunctions.NewCell(row, titlesCS, 8, "%");
                NpoiFunctions.NewCell(row, titlesCS, 9, "OBS.");
                NpoiFunctions.NewCell(row, titlesCS, 10, rm.GetString("loom").ToUpper());
                NpoiFunctions.NewCell(row, titlesCS, 11, "KPICKS");
                NpoiFunctions.NewCell(row, titlesCS, 12, "MTS");
                NpoiFunctions.NewCell(row, titlesCS, 13, "%");
                NpoiFunctions.NewCell(row, titlesCS, 14, "OBS.");
                num++;

                int startRow = num + 1;

                foreach (var loom in item) //looms
                {
                    IRow newRow = sheet.CreateRow(num);
                    NpoiFunctions.NewCell(newRow, titlesCS, 0, loom.LoomNumber);//Loom Number
                    NpoiFunctions.NewCell(newRow, cellStyle, 1, 0);//KPICKS
                    NpoiFunctions.NewCell(newRow, cellStyle, 2, 0);//MTS
                    NpoiFunctions.NewCell(newRow, cellStyle, 3, 0);//EFF
                    NpoiFunctions.NewCell(newRow, cellStyle, 4, string.Empty);//OBS
                    NpoiFunctions.NewCell(newRow, titlesCS, 5, loom.LoomNumber);
                    NpoiFunctions.NewCell(newRow, cellStyle, 6, 0);
                    NpoiFunctions.NewCell(newRow, cellStyle, 7, 0);
                    NpoiFunctions.NewCell(newRow, cellStyle, 8, 0);
                    NpoiFunctions.NewCell(newRow, cellStyle, 9, string.Empty);
                    NpoiFunctions.NewCell(newRow, titlesCS, 10, loom.LoomNumber);
                    NpoiFunctions.NewCell(newRow, cellStyle, 11, 0);
                    NpoiFunctions.NewCell(newRow, cellStyle, 12, 0);
                    NpoiFunctions.NewCell(newRow, cellStyle, 13, 0);
                    NpoiFunctions.NewCell(newRow, cellStyle, 14, string.Empty);

                    if (itemShiftA != null)
                    {
                        ProductionHistory prodShiftA = itemShiftA.SingleOrDefault(x => x.LoomId == loom.LoomId);
                        if (prodShiftA != null)
                        {
                            newRow.GetCell(1).SetCellValue(Convert.ToDouble(prodShiftA.Picks / 1000));
                            newRow.GetCell(2).SetCellValue(Convert.ToDouble(prodShiftA.Lenght / 100));
                            newRow.GetCell(3).SetCellValue(Convert.ToDouble(prodShiftA.StdEff));
                        }
                    }

                    if (itemShiftB != null)
                    {
                        ProductionHistory prodShiftB = itemShiftB.SingleOrDefault(x => x.LoomId == loom.LoomId);
                        if (prodShiftB != null)
                        {
                            newRow.GetCell(6).SetCellValue(Convert.ToDouble(prodShiftB.Picks / 1000));
                            newRow.GetCell(7).SetCellValue(Convert.ToDouble(prodShiftB.Lenght / 100));
                            newRow.GetCell(8).SetCellValue(Convert.ToDouble(prodShiftB.StdEff));
                        }
                    }

                    if (itemShiftC != null)
                    {
                        ProductionHistory prodShiftC = itemShiftC.Where(x => x.LoomId == loom.LoomId).SingleOrDefault();
                        if (prodShiftC != null)
                        {
                            newRow.GetCell(6).SetCellValue(Convert.ToDouble(prodShiftC.Picks / 1000));
                            newRow.GetCell(7).SetCellValue(Convert.ToDouble(prodShiftC.Lenght / 100));
                            newRow.GetCell(8).SetCellValue(Convert.ToDouble(prodShiftC.StdEff));
                        }
                    }

                    num++;
                }

                int endRow = num;

                #region Totals Row   
                rowTotalLoomGroup[loomGroupID - 1] = num + 1;

                row = sheet.CreateRow(num);

                NpoiFunctions.NewCell(row, titlesCS, 0, "TOTAL");

                cell = row.CreateCell(1);
                cell.SetCellFormula(string.Format("SUM(B{0}:B{1})", startRow, endRow));
                cell.CellStyle = cellStyle;

                cell = row.CreateCell(2);
                cell.SetCellFormula(string.Format("SUM(C{0}:C{1})", startRow, endRow));
                cell.CellStyle = cellStyle;

                cell = row.CreateCell(3);
                cell.SetCellFormula(string.Format("AVERAGE(D{0}:D{1})", startRow, endRow));
                cell.CellStyle = decimalsCS;

                NpoiFunctions.NewCell(row, cellStyle, 4, string.Empty);
                NpoiFunctions.NewCell(row, titlesCS, 5, "TOTAL");

                cell = row.CreateCell(6);
                cell.SetCellFormula(string.Format("SUM(G{0}:G{1})", startRow, endRow));
                cell.CellStyle = cellStyle;

                cell = row.CreateCell(7);
                cell.SetCellFormula(string.Format("SUM(H{0}:H{1})", startRow, endRow));
                cell.CellStyle = cellStyle;

                cell = row.CreateCell(8);
                cell.SetCellFormula(string.Format("AVERAGE(I{0}:I{1})", startRow, endRow));
                cell.CellStyle = decimalsCS;

                NpoiFunctions.NewCell(row, cellStyle, 9, string.Empty);
                NpoiFunctions.NewCell(row, titlesCS, 10, "TOTAL");

                cell = row.CreateCell(11);
                cell.SetCellFormula(string.Format("SUM(L{0}:L{1})", startRow, endRow));
                cell.CellStyle = cellStyle;

                cell = row.CreateCell(12);
                cell.SetCellFormula(string.Format("SUM(M{0}:M{1})", startRow, endRow));
                cell.CellStyle = cellStyle;

                cell = row.CreateCell(13);
                cell.SetCellFormula(string.Format("AVERAGE(N{0}:N{1})", startRow, endRow));
                cell.CellStyle = decimalsCS;

                NpoiFunctions.NewCell(row, cellStyle, 14, string.Empty);

                num++;
                #endregion

                loomGroupID++;
            }
                #endregion

                #region Footer
                ICellStyle footerTitlesCS = excelWorkbook.CreateCellStyle();
                footerTitlesCS.Alignment = HorizontalAlignment.Left;
                footerTitlesCS.VerticalAlignment = VerticalAlignment.Center;
                footerTitlesCS.SetFont(titlesFont);
                footerTitlesCS.WrapText = true;

                footerTitlesCS.BorderTop = BorderStyle.Thin;
                footerTitlesCS.BorderLeft = BorderStyle.Thin;
                footerTitlesCS.BorderRight = BorderStyle.Thin;
                footerTitlesCS.BorderBottom = BorderStyle.Thin;

                footerTitlesCS.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Grey25Percent.Index;
                footerTitlesCS.FillPattern = FillPattern.SolidForeground;

                ICellStyle dayTitlesCS = excelWorkbook.CreateCellStyle();
                dayTitlesCS.Alignment = HorizontalAlignment.Center;
                dayTitlesCS.VerticalAlignment = VerticalAlignment.Center;
                dayTitlesCS.SetFont(titlesFont);
                dayTitlesCS.WrapText = true;

                dayTitlesCS.BorderTop = BorderStyle.Thin;
                dayTitlesCS.BorderLeft = BorderStyle.Thin;
                dayTitlesCS.BorderRight = BorderStyle.Thin;
                dayTitlesCS.BorderBottom = BorderStyle.Thin;

                dayTitlesCS.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Grey25Percent.Index;
                dayTitlesCS.FillPattern = FillPattern.SolidForeground;

                num++;

                //TOTAL KPICKS TURNO NOCHE
                row = sheet.CreateRow(num);
                cell = row.CreateCell(0);
                cell.SetCellValue(string.Format("KPICKS {0}", rm.GetString("night").ToUpper()));
                cell.CellStyle = footerTitlesCS;
                cell.CellStyle.WrapText = true;
                cra = new NPOI.SS.Util.CellRangeAddress(num, num, 0, 2);
                sheet.AddMergedRegion(cra);
                NPOI.SS.Util.RegionUtil.SetBorderTop(1, cra, sheet, excelWorkbook);
                NPOI.SS.Util.RegionUtil.SetBorderRight(1, cra, sheet, excelWorkbook);
                NPOI.SS.Util.RegionUtil.SetBorderLeft(1, cra, sheet, excelWorkbook);
                NPOI.SS.Util.RegionUtil.SetBorderBottom(1, cra, sheet, excelWorkbook);

                cell = row.CreateCell(3);
                cell.SetCellFormula(string.Format("B{0}+B{1}+B{2}+B{3}+B{4}+B{5}", rowTotalLoomGroup[0], rowTotalLoomGroup[1], rowTotalLoomGroup[2], rowTotalLoomGroup[3], rowTotalLoomGroup[4], rowTotalLoomGroup[5]));
                cell.CellStyle = cellStyle;
                //FIN TOTAL KPICKS TURNO NOCHE

                //TOTAL KPICKS DIARIOS
                int rowNumberTotalDay = num;
                cell = row.CreateCell(5);
                cell.SetCellValue(string.Format("KIPCKS\n{0}", rm.GetString("day").ToUpper()));
                cell.CellStyle = dayTitlesCS;

                cell = row.CreateCell(7);
                cell.SetCellFormula(string.Format("D{0}+D{1}+D{2}", num + 1, num + 5, num + 9));
                cell.CellStyle = cellStyle;
                //FIN TOTAL KPICKS DIARIOS

                num++;

                //TOTAL MTS TURNO NOCHE
                row = sheet.CreateRow(num);
                cell = row.CreateCell(0);
                cell.SetCellValue(string.Format("MTS {0}", rm.GetString("night").ToUpper()));
                cell.CellStyle = footerTitlesCS;
                cell.CellStyle.WrapText = true;
                cra = new NPOI.SS.Util.CellRangeAddress(num, num, 0, 2);
                sheet.AddMergedRegion(cra);
                NPOI.SS.Util.RegionUtil.SetBorderTop(1, cra, sheet, excelWorkbook);
                NPOI.SS.Util.RegionUtil.SetBorderRight(1, cra, sheet, excelWorkbook);
                NPOI.SS.Util.RegionUtil.SetBorderLeft(1, cra, sheet, excelWorkbook);
                NPOI.SS.Util.RegionUtil.SetBorderBottom(1, cra, sheet, excelWorkbook);

                cell = row.CreateCell(3);
                cell.SetCellFormula(string.Format("C{0}+C{1}+C{2}+C{3}+C{4}+C{5}", rowTotalLoomGroup[0], rowTotalLoomGroup[1], rowTotalLoomGroup[2], rowTotalLoomGroup[3], rowTotalLoomGroup[4], rowTotalLoomGroup[5]));
                cell.CellStyle = cellStyle;
                //FIN TOTAL MTS TURNO NOCHE

                num++;

                //TOTAL EFF TURNO NOCHE
                row = sheet.CreateRow(num);
                cell = row.CreateCell(0);
                cell.SetCellValue(string.Format("{0} {1} (%)", rm.GetString("efficiency").ToUpper(), rm.GetString("night").ToUpper()));
                cell.CellStyle = footerTitlesCS;
                cell.CellStyle.WrapText = true;
                cra = new NPOI.SS.Util.CellRangeAddress(num, num, 0, 2);
                sheet.AddMergedRegion(cra);
                NPOI.SS.Util.RegionUtil.SetBorderTop(1, cra, sheet, excelWorkbook);
                NPOI.SS.Util.RegionUtil.SetBorderRight(1, cra, sheet, excelWorkbook);
                NPOI.SS.Util.RegionUtil.SetBorderLeft(1, cra, sheet, excelWorkbook);
                NPOI.SS.Util.RegionUtil.SetBorderBottom(1, cra, sheet, excelWorkbook);

                cell = row.CreateCell(3);
                //cell.SetCellFormula(string.Format("AVERAGE(C{0},C{1},C{2},C{3})", rowTotalLoomGroup[0], rowTotalLoomGroup[1], rowTotalLoomGroup[2], rowTotalLoomGroup[3])); //poner promedio correcto (suma de todos los telares en vez de los grupos)
                cell.SetCellFormula(string.Format("(SUM(D{0}:D{1})+SUM(D{2}:D{3})+SUM(D{4}:D{5})+SUM(D{6}:D{7})+SUM(D{8}:D{9}))/{10}",
                    rowTotalLoomGroup[0] - loomsGroupedByLoomGroup.ElementAt(0).Count(),
                    rowTotalLoomGroup[0] - 1,
                    rowTotalLoomGroup[1] - loomsGroupedByLoomGroup.ElementAt(1).Count(),
                    rowTotalLoomGroup[1] - 1,
                    rowTotalLoomGroup[2] - loomsGroupedByLoomGroup.ElementAt(2).Count(),
                    rowTotalLoomGroup[2] - 1,
                    rowTotalLoomGroup[3] - loomsGroupedByLoomGroup.ElementAt(3).Count(),
                    rowTotalLoomGroup[3] - 1,
                    rowTotalLoomGroup[4] - loomsGroupedByLoomGroup.ElementAt(4).Count(),
                    rowTotalLoomGroup[4] - 1,
                    datalogLooms.Count() - 6)); //TRUCAZO: se puede hacer mucho mejor pero me da paja
                cell.CellStyle = decimalsCS;
                //FIN TOTAL EFF TURNO NOCHE

                num++;

                //TOTAL METROS DIARIOS
                row = sheet.CreateRow(num);
                cell = row.CreateCell(5);
                cell.SetCellValue(string.Format("{0}\n{1}", rm.GetString("meters").ToUpper(), rm.GetString("day").ToUpper()));
                cell.CellStyle = dayTitlesCS;

                cell = row.CreateCell(7);
                cell.SetCellFormula(string.Format("D{0}+D{1}+D{2}", num - 1, num + 3, num + 7));
                cell.CellStyle = cellStyle;
                //FIN TOTAL METROS DIARIOS

                num++;

                //TOTAL KPICKS TURNO MAÑANA
                row = sheet.CreateRow(num);
                cell = row.CreateCell(0);
                cell.SetCellValue(string.Format("KPICKS {0}", rm.GetString("morning").ToUpper()));
                cell.CellStyle = footerTitlesCS;
                cell.CellStyle.WrapText = true;
                cra = new NPOI.SS.Util.CellRangeAddress(num, num, 0, 2);
                sheet.AddMergedRegion(cra);
                NPOI.SS.Util.RegionUtil.SetBorderTop(1, cra, sheet, excelWorkbook);
                NPOI.SS.Util.RegionUtil.SetBorderRight(1, cra, sheet, excelWorkbook);
                NPOI.SS.Util.RegionUtil.SetBorderLeft(1, cra, sheet, excelWorkbook);
                NPOI.SS.Util.RegionUtil.SetBorderBottom(1, cra, sheet, excelWorkbook);

                cell = row.CreateCell(3);
                cell.SetCellFormula(string.Format("G{0}+G{1}+G{2}+G{3}+G{4}+G{5}", rowTotalLoomGroup[0], rowTotalLoomGroup[1], rowTotalLoomGroup[2], rowTotalLoomGroup[3], rowTotalLoomGroup[4], rowTotalLoomGroup[5]));
                cell.CellStyle = cellStyle;
                //FIN TOTAL KPICKS TURNO MAÑANA

                num++;

                //TOTAL MTS TURNO MAÑANA
                row = sheet.CreateRow(num);
                cell = row.CreateCell(0);
                cell.SetCellValue(string.Format("MTS {0}", rm.GetString("morning").ToUpper()));
                cell.CellStyle = footerTitlesCS;
                cell.CellStyle.WrapText = true;
                cra = new NPOI.SS.Util.CellRangeAddress(num, num, 0, 2);
                sheet.AddMergedRegion(cra);
                NPOI.SS.Util.RegionUtil.SetBorderTop(1, cra, sheet, excelWorkbook);
                NPOI.SS.Util.RegionUtil.SetBorderRight(1, cra, sheet, excelWorkbook);
                NPOI.SS.Util.RegionUtil.SetBorderLeft(1, cra, sheet, excelWorkbook);
                NPOI.SS.Util.RegionUtil.SetBorderBottom(1, cra, sheet, excelWorkbook);

                cell = row.CreateCell(3);
                cell.SetCellFormula(string.Format("H{0}+H{1}+H{2}+H{3}+H{4}+H{5}", rowTotalLoomGroup[0], rowTotalLoomGroup[1], rowTotalLoomGroup[2], rowTotalLoomGroup[3], rowTotalLoomGroup[4], rowTotalLoomGroup[5]));
                cell.CellStyle = cellStyle;
                //FIN TOTAL MTS TURNO MAÑANA

                num++;

                //TOTAL EFF TURNO MAÑANA
                row = sheet.CreateRow(num);
                cell = row.CreateCell(0);
                cell.SetCellValue(string.Format("{0} {1} (%)", rm.GetString("efficiency").ToUpper(), rm.GetString("morning").ToUpper()));
                cell.CellStyle = footerTitlesCS;
                cell.CellStyle.WrapText = true;
                cra = new NPOI.SS.Util.CellRangeAddress(num, num, 0, 2);
                sheet.AddMergedRegion(cra);
                NPOI.SS.Util.RegionUtil.SetBorderTop(1, cra, sheet, excelWorkbook);
                NPOI.SS.Util.RegionUtil.SetBorderRight(1, cra, sheet, excelWorkbook);
                NPOI.SS.Util.RegionUtil.SetBorderLeft(1, cra, sheet, excelWorkbook);
                NPOI.SS.Util.RegionUtil.SetBorderBottom(1, cra, sheet, excelWorkbook);

                cell = row.CreateCell(3);
                //cell.SetCellFormula(string.Format("AVERAGE(G{0},G{1},G{2},G{3})", rowTotalLoomGroup[0], rowTotalLoomGroup[1], rowTotalLoomGroup[2], rowTotalLoomGroup[3]));
                cell.SetCellFormula(string.Format("(SUM(I{0}:I{1})+SUM(I{2}:I{3})+SUM(I{4}:I{5})+SUM(I{6}:I{7})+SUM(I{8}:I{9}))/{10}",
                    rowTotalLoomGroup[0] - loomsGroupedByLoomGroup.ElementAt(0).Count(),
                    rowTotalLoomGroup[0] - 1,
                    rowTotalLoomGroup[1] - loomsGroupedByLoomGroup.ElementAt(1).Count(),
                    rowTotalLoomGroup[1] - 1,
                    rowTotalLoomGroup[2] - loomsGroupedByLoomGroup.ElementAt(2).Count(),
                    rowTotalLoomGroup[2] - 1,
                    rowTotalLoomGroup[3] - loomsGroupedByLoomGroup.ElementAt(3).Count(),
                    rowTotalLoomGroup[3] - 1,
                    rowTotalLoomGroup[4] - loomsGroupedByLoomGroup.ElementAt(4).Count(),
                    rowTotalLoomGroup[4] - 1,
                    datalogLooms.Count() - 6));
                cell.CellStyle = decimalsCS;
                //FIN TOTAL EFF TURNO MAÑANA

                //AVG EFICIENCIA DIARIOS
                //row = sheet.CreateRow(num);
                cell = row.CreateCell(5);
                cell.SetCellValue(string.Format("{0}\n{1} (%)", rm.GetString("efficiency").ToUpper(), rm.GetString("day").ToUpper()));
                cell.CellStyle = dayTitlesCS;

                cell = row.CreateCell(7);
                cell.SetCellFormula(string.Format("(D{0}+D{1}+D{2})/3", num - 3, num + 1, num + 5));
                cell.CellStyle = decimalsCS;
                //FIN AVG EFICIENCIA DIARIOS

                num += 2;

                //TOTAL KPICKS TARDE
                row = sheet.CreateRow(num);
                cell = row.CreateCell(0);
                cell.SetCellValue(string.Format("KPICKS {0}", rm.GetString("afternoon").ToUpper()));
                cell.CellStyle = footerTitlesCS;
                cell.CellStyle.WrapText = true;
                cra = new NPOI.SS.Util.CellRangeAddress(num, num, 0, 2);
                sheet.AddMergedRegion(cra);
                NPOI.SS.Util.RegionUtil.SetBorderTop(1, cra, sheet, excelWorkbook);
                NPOI.SS.Util.RegionUtil.SetBorderRight(1, cra, sheet, excelWorkbook);
                NPOI.SS.Util.RegionUtil.SetBorderLeft(1, cra, sheet, excelWorkbook);
                NPOI.SS.Util.RegionUtil.SetBorderBottom(1, cra, sheet, excelWorkbook);

                cell = row.CreateCell(3);
                cell.SetCellFormula(string.Format("L{0}+L{1}+L{2}+L{3}+L{4}+L{5}", rowTotalLoomGroup[0], rowTotalLoomGroup[1], rowTotalLoomGroup[2], rowTotalLoomGroup[3], rowTotalLoomGroup[4], rowTotalLoomGroup[5]));
                cell.CellStyle = cellStyle;
                //FIN TOTAL KPICKS TARDE

                num++;

                //TOTAL MTS TARDE
                row = sheet.CreateRow(num);
                cell = row.CreateCell(0);
                cell.SetCellValue(string.Format("MTS {0}", rm.GetString("afternoon").ToUpper()));
                cell.CellStyle = footerTitlesCS;
                cell.CellStyle.WrapText = true;
                cra = new NPOI.SS.Util.CellRangeAddress(num, num, 0, 2);
                sheet.AddMergedRegion(cra);
                NPOI.SS.Util.RegionUtil.SetBorderTop(1, cra, sheet, excelWorkbook);
                NPOI.SS.Util.RegionUtil.SetBorderRight(1, cra, sheet, excelWorkbook);
                NPOI.SS.Util.RegionUtil.SetBorderLeft(1, cra, sheet, excelWorkbook);
                NPOI.SS.Util.RegionUtil.SetBorderBottom(1, cra, sheet, excelWorkbook);

                cell = row.CreateCell(3);
                cell.SetCellFormula(string.Format("M{0}+M{1}+M{2}+M{3}+M{4}+M{5}", rowTotalLoomGroup[0], rowTotalLoomGroup[1], rowTotalLoomGroup[2], rowTotalLoomGroup[3], rowTotalLoomGroup[4], rowTotalLoomGroup[5]));
                cell.CellStyle = cellStyle;
                //FIN TOTAL MTS TARDE

                num++;

                //TOTAL EFF TARDE
                row = sheet.CreateRow(num);
                cell = row.CreateCell(0);
                cell.SetCellValue(string.Format("{0} {1} (%)", rm.GetString("efficiency").ToUpper(), rm.GetString("afternoon").ToUpper()));
                cell.CellStyle = footerTitlesCS;
                cell.CellStyle.WrapText = true;
                cra = new NPOI.SS.Util.CellRangeAddress(num, num, 0, 2);
                sheet.AddMergedRegion(cra);
                NPOI.SS.Util.RegionUtil.SetBorderTop(1, cra, sheet, excelWorkbook);
                NPOI.SS.Util.RegionUtil.SetBorderRight(1, cra, sheet, excelWorkbook);
                NPOI.SS.Util.RegionUtil.SetBorderLeft(1, cra, sheet, excelWorkbook);
                NPOI.SS.Util.RegionUtil.SetBorderBottom(1, cra, sheet, excelWorkbook);

                cell = row.CreateCell(3);
                //cell.SetCellFormula(string.Format("AVERAGE(K{0},K{1},K{2},K{3})", rowTotalLoomGroup[0], rowTotalLoomGroup[1], rowTotalLoomGroup[2], rowTotalLoomGroup[3]));
                cell.SetCellFormula(string.Format("(SUM(N{0}:N{1})+SUM(N{2}:N{3})+SUM(N{4}:N{5})+SUM(N{6}:N{7})+SUM(N{8}:N{9}))/{10}",
                    rowTotalLoomGroup[0] - loomsGroupedByLoomGroup[0].Count(),
                    rowTotalLoomGroup[0] - 1,
                    rowTotalLoomGroup[1] - loomsGroupedByLoomGroup[1].Count(),
                    rowTotalLoomGroup[1] - 1,
                    rowTotalLoomGroup[2] - loomsGroupedByLoomGroup[2].Count(),
                    rowTotalLoomGroup[2] - 1,
                    rowTotalLoomGroup[3] - loomsGroupedByLoomGroup[3].Count(),
                    rowTotalLoomGroup[3] - 1,
                    rowTotalLoomGroup[4] - loomsGroupedByLoomGroup[4].Count(),
                    rowTotalLoomGroup[4] - 1,
                    datalogLooms.Count() - 6));
                cell.CellStyle = decimalsCS;
                //FIN TOTAL EFF TARDE

                #region Day Totals Merged Regions
                cra = new NPOI.SS.Util.CellRangeAddress(rowNumberTotalDay, rowNumberTotalDay + 1, 5, 6);
                sheet.AddMergedRegion(cra);
                NPOI.SS.Util.RegionUtil.SetBorderTop(1, cra, sheet, excelWorkbook);
                NPOI.SS.Util.RegionUtil.SetBorderRight(1, cra, sheet, excelWorkbook);
                NPOI.SS.Util.RegionUtil.SetBorderLeft(1, cra, sheet, excelWorkbook);
                NPOI.SS.Util.RegionUtil.SetBorderBottom(1, cra, sheet, excelWorkbook);

                cra = new NPOI.SS.Util.CellRangeAddress(rowNumberTotalDay, rowNumberTotalDay + 1, 7, 7);
                sheet.AddMergedRegion(cra);
                NPOI.SS.Util.RegionUtil.SetBorderTop(1, cra, sheet, excelWorkbook);
                NPOI.SS.Util.RegionUtil.SetBorderRight(1, cra, sheet, excelWorkbook);
                NPOI.SS.Util.RegionUtil.SetBorderLeft(1, cra, sheet, excelWorkbook);
                NPOI.SS.Util.RegionUtil.SetBorderBottom(1, cra, sheet, excelWorkbook);

                rowNumberTotalDay += 3;

                cra = new NPOI.SS.Util.CellRangeAddress(rowNumberTotalDay, rowNumberTotalDay + 1, 5, 6);
                sheet.AddMergedRegion(cra);
                NPOI.SS.Util.RegionUtil.SetBorderTop(1, cra, sheet, excelWorkbook);
                NPOI.SS.Util.RegionUtil.SetBorderRight(1, cra, sheet, excelWorkbook);
                NPOI.SS.Util.RegionUtil.SetBorderLeft(1, cra, sheet, excelWorkbook);
                NPOI.SS.Util.RegionUtil.SetBorderBottom(1, cra, sheet, excelWorkbook);

                cra = new NPOI.SS.Util.CellRangeAddress(rowNumberTotalDay, rowNumberTotalDay + 1, 7, 7);
                sheet.AddMergedRegion(cra);
                NPOI.SS.Util.RegionUtil.SetBorderTop(1, cra, sheet, excelWorkbook);
                NPOI.SS.Util.RegionUtil.SetBorderRight(1, cra, sheet, excelWorkbook);
                NPOI.SS.Util.RegionUtil.SetBorderLeft(1, cra, sheet, excelWorkbook);
                NPOI.SS.Util.RegionUtil.SetBorderBottom(1, cra, sheet, excelWorkbook);

                rowNumberTotalDay += 3;

                cra = new NPOI.SS.Util.CellRangeAddress(rowNumberTotalDay, rowNumberTotalDay + 1, 5, 6);
                sheet.AddMergedRegion(cra);
                NPOI.SS.Util.RegionUtil.SetBorderTop(1, cra, sheet, excelWorkbook);
                NPOI.SS.Util.RegionUtil.SetBorderRight(1, cra, sheet, excelWorkbook);
                NPOI.SS.Util.RegionUtil.SetBorderLeft(1, cra, sheet, excelWorkbook);
                NPOI.SS.Util.RegionUtil.SetBorderBottom(1, cra, sheet, excelWorkbook);

                cra = new NPOI.SS.Util.CellRangeAddress(rowNumberTotalDay, rowNumberTotalDay + 1, 7, 7);
                sheet.AddMergedRegion(cra);
                NPOI.SS.Util.RegionUtil.SetBorderTop(1, cra, sheet, excelWorkbook);
                NPOI.SS.Util.RegionUtil.SetBorderRight(1, cra, sheet, excelWorkbook);
                NPOI.SS.Util.RegionUtil.SetBorderLeft(1, cra, sheet, excelWorkbook);
                NPOI.SS.Util.RegionUtil.SetBorderBottom(1, cra, sheet, excelWorkbook);
                #endregion
                #endregion

                NpoiFunctions.SetColumn(sheet, 0, 8);//Loom Number
                NpoiFunctions.SetColumn(sheet, 1, 10);//KPICKS
                NpoiFunctions.SetColumn(sheet, 2, 8);//MTS
                NpoiFunctions.SetColumn(sheet, 3, 8);//MTS
                NpoiFunctions.SetColumn(sheet, 4, 12);//OBS
                NpoiFunctions.SetColumn(sheet, 5, 8);//Loom Number
                NpoiFunctions.SetColumn(sheet, 6, 10);//KPICKS
                NpoiFunctions.SetColumn(sheet, 7, 8);//MTS
                NpoiFunctions.SetColumn(sheet, 8, 8);//MTS
                NpoiFunctions.SetColumn(sheet, 9, 12);//OBS
                NpoiFunctions.SetColumn(sheet, 10, 8);//Loom Number
                NpoiFunctions.SetColumn(sheet, 11, 10);//KPICKS
                NpoiFunctions.SetColumn(sheet, 12, 8);//MTS
                NpoiFunctions.SetColumn(sheet, 13, 8);//MTS
                NpoiFunctions.SetColumn(sheet, 14, 12);//OBS
            }

        private static async Task<string> ObtenerCalidad(string article)
        {
            var requestStock = new HttpRequestMessage(HttpMethod.Get, string.Format("http://services.crossvillefabric.com/service/tms.asmx/GetQualityByDesign?design={0}",article));
            var httpClientStock = new HttpClient();
            HttpResponseMessage WebRespStock = await httpClientStock.SendAsync(requestStock).ConfigureAwait(false);

            var jsongetStock = await WebRespStock.Content.ReadAsStringAsync();
            string result = jsongetStock.Trim();
            return result;
        }
    }

    public class TopProd
    {
        public string LoomNumber { get; set; }
        public string LoomType { get; set; }
        public decimal? StdEff { get; set; }
    }

    public class ProductionAvg
    {
        public int Hora { get; set; }
        public DateTime Date { get; set; }
        public int? StdEff { get; set; }
        public int? ActualEff { get; set; }
    }

    public class ReportDetailBO
    {
        public string Description { get; set; }

        public string Content { get; set; }
    }

    public class UnassignedWorker
    {
        public int? WorkerId { get; set; }
        public string WorkerName { get; set; }

        public string Cargo { get; set; }
    }
}