using System;
using System.Collections.Generic;

namespace INVENTORY.EF.Entidades
{
    public partial class PhysicalInventory
    {
        public string PieceNo { get; set; }
        public int InventoryId { get; set; }
        public int DepartmentId { get; set; }
        public DateTime ScanDate { get; set; }
        public string Location { get; set; }
        public string RackNo { get; set; }
        public string LoomNo { get; set; }

        public virtual Department Department { get; set; }
        public virtual Inventory Inventory { get; set; }
    }
}
