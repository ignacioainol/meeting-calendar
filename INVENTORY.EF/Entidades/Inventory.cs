using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace INVENTORY.EF.Entidades
{
    public partial class Inventory
    {
        public Inventory()
        {
            PhysicalInventory = new HashSet<PhysicalInventory>();
            PhysicalInventoryHist = new HashSet<PhysicalInventoryHist>();
            TmsPiece = new HashSet<TmsPiece>();
            TmsPieceHist = new HashSet<TmsPieceHist>();
        }

        public int InventoryId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string CreatorOwner { get; set; }
        public int? Month { get; set; }
        public int? Year { get; set; }
        public string CloseOwner { get; set; }
        [NotMapped]
        public string Duration { get; set; }
        [NotMapped]
        public string CreatorOwnerName { get; set; }
        [NotMapped]
        public string CloseOwnerName { get; set; }
        [NotMapped]
        public decimal Finished { get; set; }

        public virtual ICollection<PhysicalInventory> PhysicalInventory { get; set; }
        public virtual ICollection<PhysicalInventoryHist> PhysicalInventoryHist { get; set; }
        public virtual ICollection<TmsPiece> TmsPiece { get; set; }
        public virtual ICollection<TmsPieceHist> TmsPieceHist { get; set; }
    }
}
