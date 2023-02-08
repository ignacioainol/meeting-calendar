using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using TCS.WebUI.Models;

namespace TCS.WebUI.Controllers
{
    [Authorize(Roles = "HollandSherry-Admin, HollandSherry-Private, HollandSherry-Public")]
    public class HollandSherryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Order(string order)
        {
            //if (order == null)
            //{
            //  return View();
            //}

            //HttpRequestMessage requestOrder = new HttpRequestMessage(HttpMethod.Get, "https://services.crossvillefabric.com/servicetest/iWarehouse.asmx/GetSalesOrder?OrderNo="+order);
            //var httpClientOrder = new HttpClient();
            //HttpResponseMessage WebRespPiece = await httpClientOrder.SendAsync(requestOrder).ConfigureAwait(false);

            //var jsongetOrder = await WebRespPiece.Content.ReadAsStringAsync();
            //if(jsongetOrder.StartsWith("System"))
            //{
            //    return View();
            //}
            //HeaderOrderModel OrderTmsProg = JsonConvert.DeserializeObject<HeaderOrderModel>(jsongetOrder);
            //return View(OrderTmsProg);
            return View();
        }
    }
}
