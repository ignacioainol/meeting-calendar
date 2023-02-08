using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NPOI.HSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Threading.Tasks;
using TCS.WebUI.Helpers;

namespace TCS.WebUI.Controllers
{
    [Authorize(Roles = "English American-Admin, English American-Private, English American-Public")]
    public class EnglishAmericanController : Controller
    {
        static HSSFWorkbook excelWorkbook;
        private static ResourceManager rm = new ResourceManager("TCS.WebUI.Resources.SharedResource", Assembly.GetExecutingAssembly());
        public EnglishAmericanController()
        {
            excelWorkbook = new HSSFWorkbook();
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Invoice(string invoice)
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> InvoiceToExcel([FromBody]string jsonToSend)
        {
            var ExcelData = JsonConvert.DeserializeObject<List<DataInvoice>>(jsonToSend);

            string[] columnsNames =
            {
                "Customers No","Quality",
                "Design/Shade","Composition",
                "Piece No","Status",
                "Information","Customers Order",
                "XV Order","Tariff Code",
                "Width Cms","Gms/Mtr",
                "Ozs/Yd","Gms/Sq Mtr",
                "Ozs/Sq Yd","Gross Mtrs",
                "Gross Yds","Nett Mtrs",
                "Nett Yds","Price USD/Mtrs",
                "Price USD/Yds","Date Ordered",
                "Date Invoiced","Invoice",
                "Mode Of Transport","Terms","Delivery Terms",
                "Customer Name","Customer ID",
            };

            string sheetTitle = "Invoice report";

            string title = string.Format("{0}", sheetTitle.ToUpper());

            NpoiFunctions.CreateE(excelWorkbook, title, sheetTitle, ExcelData, columnsNames);

            string fileName = string.Format("Report Invoice {0}",ExcelData.FirstOrDefault().Invoice_No2.ToString());
            using (var exportData = new MemoryStream())
            {
                excelWorkbook.Write(exportData);

                DateTime startDate = DateTime.Now;

                string saveAsFileName = string.Format("{0}.xls", fileName).ToUpper();

                byte[] bytes = exportData.ToArray();
                HttpContext.Session.Set("excel", bytes);
                HttpContext.Session.SetString("filename", fileName);
                return Json("Ok");
            }
        }
        //[HttpGet]
        public IActionResult Download()
        {       
            byte[] filebytes = (byte[])HttpContext.Session.Get("excel");
            string name = HttpContext.Session.GetString("filename");
            byte[] empy = new byte[0];
            HttpContext.Session.Set("excel", empy);
            HttpContext.Session.SetString("filename", "");
            return File(filebytes, "application/vnd.ms-excel", $"{name}.xls");
        }
    }
}

public class DataInvoice
{
    public string Customer_No { get; set; }
    public string Quality { get; set; }
    public string Design { get; set; }
    public string Composition { get; set; }
    public string Piece_No { get; set; }
    public string Status { get; set; }
    public string Invoice_No { get; set; }
    public string Customer_Order_No { get; set; }
    public string Order_No { get; set; }
    public double TariffCode { get; set; }
    public double Finish_Width { get; set; }
    public double GmsMtr { get; set; }
    public double OzsYds { get; set; }
    public double GmsSqMtr { get; set; }
    public double OzsSqYds { get; set; }
    public double Greasy_Length { get; set; }
    public double Greasy_Length_Yards { get; set; }
    public double Finish_Length { get; set; }
    public double Finish_Length_Yards { get; set; }
    public double Price { get; set; }
    public double Price_Yards { get; set; }
    public string Date_Ordered { get; set; }
    public string Date_Invoiced { get; set; }
    public string Invoice_No2 { get; set; }
    public string Mode_Of_Transport { get; set; }
    public string Payment_Terms { get; set; }
    public string Delivery_Terms { get; set; }
    public string Customer_Name { get; set; }
    public string Customer_ID { get; set; }
}