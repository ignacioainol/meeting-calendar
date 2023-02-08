using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace INVENTORY.EF.Entidades
{
    public partial class TmsPiece
    {
        public string PieceNo { get; set; }
        public int? InventoryId { get; set; }
        public int? DepartmentId { get; set; }
        public int? StatusId { get; set; }
        public string WarpShade { get; set; }
        public string WeftShade { get; set; }
        public string Quality { get; set; }
        public string Design { get; set; }
        public string Notes { get; set; }
        public string Location { get; set; }
        public string RackNo { get; set; }
        public string LoomNo { get; set; }
        public DateTime? ScanDate { get; set; }

        public virtual Department Department { get; set; }
        public virtual Inventory Inventory { get; set; }
        public virtual InventoryStatus Status { get; set; }

        [NotMapped]
        public string Shade
        {
            get
            {
                return WarpShade + " " + WeftShade;
            }
            set { Shade = value; }
        }
    }
}
