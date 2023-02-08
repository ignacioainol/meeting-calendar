using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TCS.WebUI.Models
{
    public class HeaderOrderModel
    {
        public string Order_No { get; set; }
        public string Delivery_Address { get; set; }
        public string Date_Ordered { get; set; }
        public string Delivery { get; set; }
        public string Payment_Terms { get; set; }
        public string Termsdescr { get; set; }
        public string Samples { get; set; }
        public int Mtrs_Ordered { get; set; }
        public string si { get; set; }
        public string ShipmentMethod { get; set; }
       // public string Pieces { get; set; }
        //public string Grouped { get; set; }
    }
}
