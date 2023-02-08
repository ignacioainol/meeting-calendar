using System;
using System.Collections.Generic;

namespace INVENTORY.EF.Entidades
{
    public partial class InventoryStatus
    {
        public InventoryStatus()
        {
            TmsPiece = new HashSet<TmsPiece>();
        }

        public int StatusId { get; set; }
        public string StatusName { get; set; }

        public virtual ICollection<TmsPiece> TmsPiece { get; set; }
    }
}
