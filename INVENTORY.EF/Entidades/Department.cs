using System;
using System.Collections.Generic;

namespace INVENTORY.EF.Entidades
{
    public partial class Department
    {
        public Department()
        {
            PhysicalInventory = new HashSet<PhysicalInventory>();
            PhysicalInventoryHist = new HashSet<PhysicalInventoryHist>();
            TmsPiece = new HashSet<TmsPiece>();
            TmsPieceHist = new HashSet<TmsPieceHist>();
        }

        public int DepartmentId { get; set; }
        public string UserId { get; set; }
        public string Name { get; set; }

        public virtual UserInventory User { get; set; }
        public virtual ICollection<PhysicalInventory> PhysicalInventory { get; set; }
        public virtual ICollection<PhysicalInventoryHist> PhysicalInventoryHist { get; set; }
        public virtual ICollection<TmsPiece> TmsPiece { get; set; }
        public virtual ICollection<TmsPieceHist> TmsPieceHist { get; set; }
    }
}
