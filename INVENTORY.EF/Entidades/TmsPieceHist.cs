using System;
using System.Collections.Generic;

namespace INVENTORY.EF.Entidades
{
    public partial class TmsPieceHist
    {
        public string PieceNo { get; set; }
        public int InventoryId { get; set; }
        public int DepartmentId { get; set; }
        public string WarpShade { get; set; }
        public string WeftShade { get; set; }
        public string Quality { get; set; }
        public string Design { get; set; }
        public int? StatusId { get; set; }
        public string Notes { get; set; }
        public string Location { get; set; }
        public string RackNo { get; set; }
        public string LoomNo { get; set; }
        public DateTime? ScanDate { get; set; }

        public virtual Department Department { get; set; }
        public virtual Inventory Inventory { get; set; }
    }
}
