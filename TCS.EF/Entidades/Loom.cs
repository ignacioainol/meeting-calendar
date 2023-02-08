using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace TCS.EF.Entidades
{
    public partial class Loom
    {
        public Loom()
        {
            CurrentProdHistory = new HashSet<CurrentProdHistory>();
            CurrentShiftSpeed = new HashSet<CurrentShiftSpeed>();
            CurrentShiftStop = new HashSet<CurrentShiftStop>();
            LoomPos = new HashSet<LoomPos>();
            ProductionHistory = new HashSet<ProductionHistory>();
        }

        public int LoomId { get; set; }
        public string LoomNumber { get; set; }
        public int? LoomTypeId { get; set; }
        public int? LoomGroupId { get; set; }
        public int? StandardSpeed { get; set; }
        public int? StandardEfficiency { get; set; }
        public bool? Busy { get; set; }
        public bool? Status { get; set; }
        public string LoomIp { get; set; }
        public bool? Arduino { get; set; }
        public string Serial { get; set; }
        public DateTime? LastTimeOnline { get; set; }
        public bool? IsCalculated { get; set; }

        public virtual LoomGroup LoomGroup { get; set; }
        public virtual LoomType LoomType { get; set; }
        public virtual CurrentProd CurrentProd { get; set; }
        public virtual CurrentStop CurrentStop { get; set; }
        public virtual LoomState LoomState { get; set; }
        public virtual ICollection<CurrentProdHistory> CurrentProdHistory { get; set; }
        public virtual ICollection<CurrentShiftSpeed> CurrentShiftSpeed { get; set; }
        public virtual ICollection<CurrentShiftStop> CurrentShiftStop { get; set; }
        public virtual ICollection<LoomPos> LoomPos { get; set; }
        public virtual ICollection<ProductionHistory> ProductionHistory { get; set; }

        [NotMapped]
        public int? SumStop { get; set; }
    }
}
