using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using INVENTORY.EF;
using Microsoft.EntityFrameworkCore;
using INVENTORY.EF.Entidades;
using INVENTORY.EF.Objetos;
using System.Reflection;
using System.Resources;
using System.Security.Claims;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using NPOI.HSSF.UserModel;
using System.IO;
using TCS.WebUI.Helpers;

namespace TCS.WebUI.Controllers
{
    [Authorize(Roles = "Inventory-Admin, Inventory-Private, Inventory-Public")]
    public class InventoryController : Controller
    {
        private readonly INVENTORYRContext _context;
        private static ResourceManager rm = new ResourceManager("TCS.WebUI.Resources.SharedResource", Assembly.GetExecutingAssembly());
        static HSSFWorkbook excelWorkbook;
        public InventoryController(INVENTORYRContext context)
        {
            _context = context;
            excelWorkbook = new HSSFWorkbook();
        }

        public async Task<IActionResult> Index()
        {
            if (InventarioAbierto())
            {
                var inventotyInfo = await _context.Inventory
                    .Include(x=>x.TmsPiece)
                    .OrderByDescending(x=>x.StartDate)
                    .Select(x=> new InventoryData
                    {
                        StartDate = x.StartDate,
                        CreatorOwner = x.CreatorOwner,
                        EncontradaGeneral = x.TmsPiece.Where(x => x.StatusId != 0 && x.ScanDate <= DateTime.Now).Count(),
                        TotalGeneral = x.TmsPiece.Count(),
                        PorcentajeGeneral = Math.Round(Convert.ToDouble(x.TmsPiece.Where(x=>x.StatusId != 0 && x.ScanDate <= DateTime.Now).Count()) * 100 / Convert.ToDouble(x.TmsPiece.Count()),1,MidpointRounding.AwayFromZero),
                        PrimerScannGeneral = (x.TmsPiece.OrderByDescending(s => s.ScanDate).FirstOrDefault().ScanDate == null)? x.StartDate : (x.TmsPiece.OrderBy(s => s.ScanDate).FirstOrDefault(x=>x.ScanDate !=null).ScanDate),
                        TmsPiece = x.TmsPiece.ToList()
                    })
                    .FirstOrDefaultAsync();

                List<List<GraphicData>> ListaPorcentajesGroup = new List<List<GraphicData>>();
                ListaPorcentajesGroup.Add(ListaPorcentajes(0));
                ListaPorcentajesGroup.Add(ListaPorcentajes(7));
                ListaPorcentajesGroup.Add(ListaPorcentajes(6));
                ListaPorcentajesGroup.Add(ListaPorcentajes(1));
                ListaPorcentajesGroup.Add(ListaPorcentajes(2));
                ListaPorcentajesGroup.Add(ListaPorcentajes(3));
                ListaPorcentajesGroup.Add(ListaPorcentajes(5));
                ListaPorcentajesGroup.Add(ListaPorcentajes(4));
                ViewBag.ListaPorcentajes = JsonConvert.SerializeObject(ListaPorcentajesGroup);
                ViewBag.StartDate = inventotyInfo.StartDate;
                ViewBag.Creator = inventotyInfo.CreatorOwner;
                ViewBag.Duration = TimeElapsed(inventotyInfo.StartDate);
                return View(inventotyInfo);
            }
            else
            {
                return RedirectToAction("InventoryClosed", "Inventory");
            }
        }

        public IActionResult InventoryClosed()
        {
            if (InventarioAbierto())
            {
                return RedirectToAction("Index", "Inventory");
            }
            else
            {
                return View();
            }
        }

        public async Task<IActionResult> Inventories()
        {
            List<Inventory> listInv = new List<Inventory>();
            var tCSContext = await _context.Inventory.AsNoTracking().ToListAsync();
            var tms = await _context.TmsPieceHist
                    .Select(x=>new PieceData { InventoryId =  x.InventoryId, StatusId = x.StatusId })
                    .AsNoTracking()
                    .ToListAsync();
            var userinv =await _context.UserInventory.AsNoTracking().ToListAsync();

            foreach (var item in tCSContext)
            {
                Inventory inv = new Inventory();
                inv.InventoryId = item.InventoryId;
                inv.StartDate = item.StartDate;
                inv.EndDate = item.EndDate;
                inv.Duration = TimeElapsed(item.StartDate);
                inv.CreatorOwnerName = GetUserName(item.CreatorOwner, userinv);
                inv.CloseOwnerName = GetUserName(item.CloseOwner,userinv);
                inv.Finished = GetPorcentajeAvancePorInventario(item.InventoryId, tms);
                listInv.Add(inv);
            }

            return View(listInv);
        }

        public async Task<IActionResult> PieceSearcher()
        {
            if (InventarioAbierto())
            {
                var pieces = await _context.TmsPiece
                    .Include(x => x.Department)
                    .AsNoTracking()
                    .ToListAsync();

                return View(pieces);
            }
            else
            {
                return RedirectToAction("InventoryClosed", "Inventory");
            }
        }

        public async Task<IActionResult> GeneralInventory()
        {
            if (InventarioAbierto())
            {
                ViewBag.Pieces = await _context.TmsPiece
                    .Include(x => x.Department).ToListAsync();
                ViewBag.Department = rm.GetString("generalinventory");
                return View("InventoryReports");
            }
            else
            {
                return RedirectToAction("InventoryClosed", "Inventory");
            }
        }

        public async Task<IActionResult> Weaving()
        {
            if (InventarioAbierto())
            {
                ViewBag.Pieces = await _context.TmsPiece
                    .Include(x => x.Department)
                    .Where(x=>x.DepartmentId == 1)
                    .ToListAsync();
                ViewBag.Department = rm.GetString("weaving");
                return View("InventoryReports");
            }
            else
            {
                return RedirectToAction("InventoryClosed", "Inventory");
            }
        }

        public async Task<IActionResult> Mending()
        {
            if (InventarioAbierto())
            {
                ViewBag.Pieces = await _context.TmsPiece
                    .Include(x => x.Department)
                    .Where(x => x.DepartmentId == 2)
                    .ToListAsync();
                ViewBag.Department = rm.GetString("mending");
                return View("InventoryReports");
            }
            else
            {
                return RedirectToAction("InventoryClosed", "Inventory");
            }
        }

        public async Task<IActionResult> Finish()
        {
            if (InventarioAbierto())
            {
                ViewBag.Pieces = await _context.TmsPiece
                    .Where(x => x.DepartmentId == 3)
                    .Include(x => x.Department)
                    .ToListAsync();
                ViewBag.Department = rm.GetString("finish");
                return View("InventoryReports");
            }
            else
            {
                return RedirectToAction("InventoryClosed", "Inventory");
            }
        }

        public async Task<IActionResult> Inspection()
        {
            if (InventarioAbierto())
            {
                ViewBag.Pieces = await _context.TmsPiece
                    .Where(x => x.DepartmentId == 4)
                    .Include(x => x.Department)
                    .ToListAsync();
                ViewBag.Department = rm.GetString("inspection");
                return View("InventoryReports");
            }
            else
            {
                return RedirectToAction("InventoryClosed", "Inventory");
            }
        }

        public async Task<IActionResult> DispatchWarehouse()
        {
            if (InventarioAbierto())
            {
                ViewBag.Pieces = await _context.TmsPiece
                    .Where(x => x.DepartmentId == 5)
                    .Include(x => x.Department)
                    .ToListAsync();
                ViewBag.Department = rm.GetString("dispatchwarehouse");
                return View("InventoryReports");
            }
            else
            {
                return RedirectToAction("InventoryClosed", "Inventory");
            }
        }

        public async Task<IActionResult> Warping()
        {
            if (InventarioAbierto())
            {
                ViewBag.Pieces = await _context.TmsPiece
                    .Include(x => x.Department)
                    .Where(x => x.DepartmentId == 6)
                    .ToListAsync();
                ViewBag.Department = rm.GetString("warping");
                return View("InventoryReports");
            }
            else
            {
                return RedirectToAction("InventoryClosed", "Inventory");
            }
        }

        public async Task<IActionResult> ToWarp()
        {
            if (InventarioAbierto())
            {
                ViewBag.Pieces = await _context.TmsPiece
                    .Include(x => x.Department)
                    .Where(x => x.DepartmentId == 7)
                    .ToListAsync();
                ViewBag.Department = rm.GetString("towarp");
                return View("InventoryReports");
            }
            else
            {
                return RedirectToAction("InventoryClosed", "Inventory");
            }
        }

        public async Task<IActionResult> ReportToExcel(int departmentId)
        {
            string[] columnsNames =
            {
                "Número de Pieza",
                "Departamento",
                "Color",
                "Calidad",
                "Diseño",
                "Ubicación",
                "RackNo",
                "Telar",
                "Notas",
            };

            var query = await _context.TmsPiece
                   .Where(x => (departmentId !=0) ? x.DepartmentId == departmentId:true)
                   .Include(x => x.Department)
                   .ToListAsync();


            var queryNoEncontradas = query.Where(x => x.StatusId == 0 || x.StatusId == 2 || x.StatusId == null)
                                     .Select(x => new
                                     {
                                         NumeroPieza = x.PieceNo,
                                         Departamento = x.Department.Name,
                                         Color = x.Shade,
                                         Calidad = x.Quality,
                                         Diseno = x.Design,
                                         Ubicacion = x.Location,
                                         RackNo = x.RackNo,
                                         Telar = x.LoomNo,
                                         Notas = x.Notes
                                     }).ToList();

            var queryEncontradas = query.Where(x => x.StatusId == 1)
                                    .Select(x=> new 
                                    { 
                                        NumeroPieza = x.PieceNo, 
                                        Departamento = x.Department.Name,
                                        Color = x.Shade,
                                        Calidad = x.Quality,
                                        Diseno = x.Design,
                                        Ubicacion = x.Location,
                                        RackNo = x.RackNo,
                                        Telar = x.LoomNo,
                                        Notas = x.Notes
                                    }).ToList();

            var queryEncontradasOtroPunto = query.Where(x => x.StatusId == 3)
                                            .Select(x => new
                                            {
                                                NumeroPieza = x.PieceNo,
                                                Departamento = x.Department.Name,
                                                Color = x.Shade,
                                                Calidad = x.Quality,
                                                Diseno = x.Design,
                                                Ubicacion = x.Location,
                                                RackNo = x.RackNo,
                                                Telar = x.LoomNo,
                                                Notas = x.Notes
                                            }).ToList();

            var queryFisicamentoNoTms = query.Where(x => x.StatusId == 4)
                                        .Select(x => new
                                        {
                                            NumeroPieza = x.PieceNo,
                                            Departamento = x.Department.Name,
                                            Color = x.Shade,
                                            Calidad = x.Quality,
                                            Diseno = x.Design,
                                            Ubicacion = x.Location,
                                            RackNo = x.RackNo,
                                            Telar = x.LoomNo,
                                            Notas = x.Notes
                                        }).ToList();
            string departmentName = (departmentId !=0)? _context.Department.FirstOrDefault(x => x.DepartmentId == departmentId).Name : "General";

            NpoiFunctions.CreateE(excelWorkbook, "No Encontradas", "No Encontradas", queryNoEncontradas, columnsNames);
            NpoiFunctions.CreateE(excelWorkbook, "Encontradas", "Encontradas", queryEncontradas, columnsNames);
            NpoiFunctions.CreateE(excelWorkbook, "Encontradas en Otro Punto", "Encontradas en Otro Punto", queryEncontradasOtroPunto, columnsNames);
            NpoiFunctions.CreateE(excelWorkbook, "Físicamente pero no en TMS", "Fisicamente pero no en TMS", queryFisicamentoNoTms, columnsNames);

            using (var exportData = new MemoryStream())
            {
                excelWorkbook.Write(exportData);

                string saveAsFileName = string.Format("Reporte Inventario {0} {1}.xls", departmentName, DateTime.Now.ToLongDateString()).ToUpper();

                byte[] bytes = exportData.ToArray();
                return File(bytes, "application/vnd.ms-excel", saveAsFileName);
            }
        }

        public string GetUserName(string creatorOwner, List<UserInventory> userinv)
        {
            List<UserInventory> usersList = userinv.Where(User => User.UserId == creatorOwner).ToList();

            if (usersList.Count > 0) //si el usuario existe en la base de datos
            {
                UserInventory user = usersList.First();
                return user.Name;
            }
            else
            {
                return creatorOwner;
            }
        }

        public static string TimeElapsed(DateTime inventory)
        {
            TimeSpan elapsedTime = DateTime.Now -inventory;
            int days = elapsedTime.Days;
            int hours = elapsedTime.Hours;
            int minutes = elapsedTime.Minutes;

            string time;

            if (days == 0)
            {
                time = hours + " " + rm.GetString("hours") + " " + minutes + " " + rm.GetString("minutes");
            }
            else
            {
                time = days + " " + rm.GetString("days") + " " + hours + " " + rm.GetString("hours") + " " + minutes + " " + rm.GetString("minutes");
            }

            return time;
        }

        public decimal GetPorcentajeAvancePorInventario(int inventoryId, List<PieceData> tms)
        {
            decimal encontrada = tms.Where(x => x.StatusId != 2 && x.InventoryId == inventoryId).Count();
            decimal total;

            total = tms.Where(x=>x.InventoryId == inventoryId).Count();

            decimal porcentaje;

            if (total == 0) // Para que no arroje error de división por cero cuando el total de piezas sea 0.
                porcentaje = 0;
            else
                porcentaje = encontrada * 100 / total;

            return decimal.Round(porcentaje, 2, MidpointRounding.AwayFromZero);
        }

        public ActionResult CloseInventory()
        {
            if (InventarioAbierto())
            {
                List<TmsPiece> TMSPieceList = _context.TmsPiece.ToList();
                List<PhysicalInventory> PhysicalInventoryList = _context.PhysicalInventory.ToList();

                foreach (var item in TMSPieceList)
                {
                    _context.TmsPieceHist.Add(new TmsPieceHist()
                    {   PieceNo = item.PieceNo,
                        InventoryId = item.InventoryId ?? 0,
                        DepartmentId = item.DepartmentId ?? 0,
                        WarpShade = item.WarpShade,
                        WeftShade = item.WeftShade,
                        Quality = item.Quality,
                        Design = item.Design,
                        StatusId = item.StatusId,
                        Notes = item.Notes,
                        Location = item.Location,
                        RackNo = item.RackNo,
                        LoomNo = item.LoomNo,
                        ScanDate = item.ScanDate
                    });
                }
                foreach (var item in PhysicalInventoryList)
                {
                    _context.PhysicalInventoryHist.Add(new PhysicalInventoryHist()
                    {
                       PieceNo = item.PieceNo,
                       InventoryId = item.InventoryId,
                       DepartmentId = item.DepartmentId,
                       ScanDate = item.ScanDate,
                       Location = item.Location,
                       RackNo = item.RackNo,
                       LoomNo = item.LoomNo
                    });
                }

                _context.TmsPiece.RemoveRange(TMSPieceList);
                _context.SaveChanges();
                _context.PhysicalInventory.RemoveRange(PhysicalInventoryList);
                _context.SaveChanges();

                List<Inventory> InventoryList = _context.Inventory.ToList();
                Inventory currentInventory = InventoryList.Where(x => x.InventoryId == InventoryList.Max(xs => xs.InventoryId)).ToList().First();

                currentInventory.EndDate = DateTime.Now;
                currentInventory.CloseOwner = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.WindowsAccountName)?.Value;

                _context.Inventory.Update(currentInventory);
                _context.SaveChanges();

                List<TmsPieceHist> updatetmshistin = _context.TmsPieceHist.Where(x => x.StatusId == 0).ToList();
                List<TmsPieceHist> updatetmshistout = new List<TmsPieceHist>();

                foreach(var item in updatetmshistin)
                {
                    TmsPieceHist hist = new TmsPieceHist();
                    hist = item;
                    hist.StatusId = 2;
                    updatetmshistout.Add(hist);
                }

                _context.TmsPieceHist.UpdateRange(updatetmshistout);
                _context.SaveChanges();
            }

            return RedirectToAction("Index", "Inventory");
        }

        public async Task<IActionResult> CreateNewInventory()
        {
            if (!InventarioAbierto())
            {
                Inventory inventory = new Inventory();
                inventory.StartDate = DateTime.Now;

                inventory.CreatorOwner = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.WindowsAccountName)?.Value;

                inventory.Month = DateTime.Now.Month;
                inventory.Year = DateTime.Now.Year;

                _context.Inventory.Add(inventory);
                _context.SaveChanges();

                List<Inventory> list = _context.Inventory.ToList();
                Inventory currentInventory = list.Where(x => x.InventoryId == list.Max(xs => xs.InventoryId)).ToList().First();

                HttpRequestMessage requestPiece = new HttpRequestMessage(HttpMethod.Get, "https://services.crossvillefabric.com/xvs/api/pieces-by-prog-status");
                var httpClientPiece = new HttpClient();
                HttpResponseMessage WebRespPiece = await httpClientPiece.SendAsync(requestPiece).ConfigureAwait(false);

                var jsongetPiece = await WebRespPiece.Content.ReadAsStringAsync();

                var requestLoom = new HttpRequestMessage(HttpMethod.Get, "https://services.crossvillefabric.com/xvs/api/pieces-from-loom");
                var httpClientLoom = new HttpClient();
                HttpResponseMessage WebRespLoom = await httpClientLoom.SendAsync(requestLoom).ConfigureAwait(false);

                var jsongetLoom = await WebRespLoom.Content.ReadAsStringAsync();

                var requestStock = new HttpRequestMessage(HttpMethod.Get, "https://services.crossvillefabric.com/xvs/api/pieces-from-stock");
                var httpClientStock = new HttpClient();
                HttpResponseMessage WebRespStock = await httpClientStock.SendAsync(requestStock).ConfigureAwait(false);

                var jsongetStock =  await WebRespStock.Content.ReadAsStringAsync();

                List<PieceBO> pieceTmsProg = JsonConvert.DeserializeObject<List<PieceBO>>(jsongetPiece);
                List<PieceBO> pieceTmsLoom = JsonConvert.DeserializeObject<List<PieceBO>>(jsongetLoom);
                List<PieceBO> pieceTmsStock = JsonConvert.DeserializeObject<List<PieceBO>>(jsongetStock);
                List<TmsPiece> pieces = new List<TmsPiece>();

                foreach (PieceBO item in pieceTmsProg.Where(x=>x.Prog_Status == "050"))
                {
                    pieces.Add(new TmsPiece() { PieceNo = item.PieceNo, DepartmentId = 2, InventoryId = currentInventory.InventoryId, WarpShade = item.WarpShade, WeftShade = item.WeftShade, Design = item.Design, Quality = item.Quality });
                }

                foreach (PieceBO item in pieceTmsProg.Where(x => x.Prog_Status == "090"))
                {
                    pieces.Add(new TmsPiece() { PieceNo = item.PieceNo, DepartmentId = 4, InventoryId = currentInventory.InventoryId, WarpShade = item.WarpShade, WeftShade = item.WeftShade, Design = item.Design, Quality = item.Quality });
                }

                foreach (PieceBO item in pieceTmsProg.Where(x => x.Prog_Status == "030"))
                {
                    pieces.Add(new TmsPiece() { PieceNo = item.PieceNo, DepartmentId = 6, InventoryId = currentInventory.InventoryId, WarpShade = item.WarpShade, WeftShade = item.WeftShade, Design = item.Design, Quality = item.Quality });
                }

                foreach (PieceBO item in pieceTmsLoom)
                {
                    pieces.Add(new TmsPiece() { PieceNo = item.PieceNo, DepartmentId = 1, InventoryId = currentInventory.InventoryId, WarpShade = item.WarpShade, WeftShade = item.WeftShade, Design = item.Design, Quality = item.Quality, LoomNo = item.LoomNo });
                }

                foreach (PieceBO item in pieceTmsStock)
                {
                    pieces.Add(new TmsPiece() { PieceNo = item.PieceNo, DepartmentId = 5, InventoryId = currentInventory.InventoryId, WarpShade = item.WarpShade, WeftShade = item.WeftShade, Design = item.Design, Quality = item.Quality, Location = item.Location, RackNo = item.RackNo });
                }

                foreach (PieceBO item in pieceTmsProg.Where(x => x.Prog_Status == "070"))
                {
                    pieces.Add(new TmsPiece() { PieceNo = item.PieceNo, DepartmentId = 3, InventoryId = currentInventory.InventoryId, WarpShade = item.WarpShade, WeftShade = item.WeftShade, Design = item.Design, Quality = item.Quality });
                }

                foreach (PieceBO item in pieceTmsProg.Where(x => x.Prog_Status == "080").ToList())
                {
                    pieces.Add(new TmsPiece() { PieceNo = item.PieceNo, DepartmentId = 3, InventoryId = currentInventory.InventoryId, WarpShade = item.WarpShade, WeftShade = item.WeftShade, Design = item.Design, Quality = item.Quality });
                }

                foreach (PieceBO item in pieceTmsProg.Where(x => x.Prog_Status == "020").ToList())
                {
                    pieces.Add(new TmsPiece() { PieceNo = item.PieceNo, DepartmentId = 7, InventoryId = currentInventory.InventoryId, WarpShade = item.WarpShade, WeftShade = item.WeftShade, Design = item.Design, Quality = item.Quality });
                }

                _context.TmsPiece.AddRange(pieces);

                _context.SaveChanges();
            }

            return RedirectToAction("Index", "Inventory");
        }

        public bool InventarioAbierto()
        {
            List<Inventory> inventario = _context.Inventory.ToList();
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

        public List<GraphicData> ListaPorcentajes(int departamento)
        {
            var piezas = (departamento == 0) ?
                   _context.TmsPiece.OrderBy(x => x.ScanDate)
                 : _context.TmsPiece.Where(x=>x.DepartmentId == departamento).OrderBy(x => x.ScanDate);

            int totalPiezas = piezas.Count();
            DateTime primerScann;
            if (piezas.Where(x => x.ScanDate != null).Count() == 0)
            {
                primerScann = DateTime.Now;
            }
            else
            {
                primerScann = (DateTime)piezas.FirstOrDefault(x => x.ScanDate != null).ScanDate;
            }

            double date =  (DateTime.Now - primerScann).TotalSeconds;

            List<GraphicData> graph = new List<GraphicData>();

            graph.Add(
                new GraphicData()
                { 
                    DatePoint = primerScann.AddSeconds(date * 0), 
                    PercentajeScanned = ((piezas.Where(x=>x.ScanDate < primerScann.AddSeconds(date * 0)).Count()*100) /totalPiezas) 
                }
            );
            graph.Add(
                new GraphicData()
                {
                    DatePoint = primerScann.AddSeconds(date * 0.25),
                    PercentajeScanned = ((piezas.Where(x => x.ScanDate < primerScann.AddSeconds(date * 0.25)).Count() * 100) / totalPiezas)
                }
            );
            graph.Add(
                new GraphicData()
                {
                    DatePoint = primerScann.AddSeconds(date * 0.50),
                    PercentajeScanned = ((piezas.Where(x => x.ScanDate < primerScann.AddSeconds(date * 0.50)).Count() * 100) / totalPiezas)
                }
            );
            graph.Add(
                new GraphicData()
                {
                    DatePoint = primerScann.AddSeconds(date * 0.75),
                    PercentajeScanned = ((piezas.Where(x => x.ScanDate < primerScann.AddSeconds(date * 0.75)).Count() * 100) / totalPiezas)
                }
            );
            graph.Add(
                new GraphicData()
                {
                    DatePoint = primerScann.AddSeconds(date),
                    PercentajeScanned = ((piezas.Where(x => x.ScanDate < primerScann.AddSeconds(date)).Count() * 100) / totalPiezas)
                }
            );

            return graph;
        }
    }

    public class InventoryData
    {
        public DateTime StartDate { get; set; }
        public string CreatorOwner { get; set; }
        public int? EncontradaGeneral { get; set; }
        public int? TotalGeneral { get; set; }
        public double? PorcentajeGeneral { get; set; }
        private int porcentajeGeneralEntero;
        public int PorcentajeGeneralEntero 
        {
            get { return Convert.ToInt16(PorcentajeGeneral); }
            set { porcentajeGeneralEntero = value; }
        }
        public DateTime? PrimerScannGeneral { get; set; }
        public List<TmsPiece> TmsPiece { get; set; }
        public List<InventoryDepartment> DepartmentsData() 
        {
            List<InventoryDepartment> invList = new List<InventoryDepartment>();
            InventoryDepartment inv = new InventoryDepartment();

            for(int i = 1; i < 8; i++)
            {
                inv.Encontrada = TmsPiece.Where(x => x.StatusId != 0 && x.ScanDate <= DateTime.Now && x.DepartmentId ==i).Count();
                inv.Total = TmsPiece.Where(x=>x.DepartmentId == i).Count();
                inv.Porcentaje = Math.Round((double)TmsPiece.Where(x => x.StatusId != 0 && x.ScanDate <= DateTime.Now && x.DepartmentId == i).Count() * 100 / (double)TmsPiece.Where(x => x.DepartmentId == i).Count(),1,MidpointRounding.AwayFromZero);
                inv.PrimerScann = (TmsPiece.Where(x => x.DepartmentId == i).OrderByDescending(s => s.ScanDate).FirstOrDefault().ScanDate == null) ? StartDate : (TmsPiece.Where(x => x.DepartmentId == i).OrderBy(s => s.ScanDate).FirstOrDefault(x => x.ScanDate != null).ScanDate);
                invList.Add(inv);
                inv = new InventoryDepartment();
            }
            return invList;
        }
    }
    public class InventoryDepartment
    {
        public int? Encontrada { get; set; }
        public int? Total { get; set; }
        public double? Porcentaje{ get; set; }
        private int porcentajeEntero;
        public int PorcentajeEntero
        {
            get { return Convert.ToInt16(Porcentaje); }
            set { porcentajeEntero = value; }
        }
        public DateTime? PrimerScann { get; set; }
    }

    public class PieceData
    {
        public int InventoryId { get; set; }
        public int? StatusId { get; set; }
    }

    public class GraphicData
    {
        public DateTime DatePoint { get; set; }
        public double PercentajeScanned { get; set; }
    }
}
