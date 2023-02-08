using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TCS.WebUI.Models
{
    public class BodyOrderModel
    {
        public string Status { get; set; }
        public string Piece_No { get; set; }
        public string Yarn_Status { get; set; }
        public string Quality { get; set; }
        public string Design { get; set; }
        public string Warp_Shade { get; set; }
        public string Weft_Shade { get; set; }
        public string Customer_No { get; set; }
        public string Length { get; set; }
        public string Job_No { get; set; }
        public string Ticket_Issued { get; set; }
        public string To_Warp { get; set; }
        public string Warp_Date { get; set; }
        public string InLoom_Date { get; set; }
        public string Loom_No { get; set; }
        public string OOLoom_Date { get; set; }
        public string Mending_Date { get; set; }
        public string Date_Issued { get; set; }
        public string Mended_Date { get; set; }
        public string ToFinish_Date { get; set; }
        public string Date_Scoured { get; set; }
        public string InWarehouse_Date { get; set; }
        public string Despatch_Date { get; set; }
        public string Invoice_No { get; set; }
        public string Finish_Length { get; set; }
        public string Invoice_Length { get; set; }
    }
}
