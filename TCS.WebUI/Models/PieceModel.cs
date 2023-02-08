using System;

namespace TCS.WebUI.Models
{
    public class PieceModel
    {
        public string PieceNo { get; set; }
        public string Quality { get; set; }
        public string Design { get; set; }
        public string WarpShade { get; set; }
        public string WeftShade { get; set; }
        public DateTime OOLoomDate { get; set; }
        public decimal GreasyLength { get; set; }
        public decimal GreasyWeigth { get; set; }
        public decimal Width { get; set; }
    }
}
