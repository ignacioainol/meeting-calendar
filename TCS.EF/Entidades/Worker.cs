using System;
using System.Collections.Generic;

namespace TCS.EF.Entidades
{
    public partial class Worker
    {
        public Worker()
        {
            ProductionHistory = new HashSet<ProductionHistory>();
            ShiftHistory = new HashSet<ShiftHistory>();
            WeaverAssignation = new HashSet<WeaverAssignation>();
        }

        public int WorkerId { get; set; }
        public string WorkerName { get; set; }
        public int? WorkerGroupId { get; set; }
        public int PositionId { get; set; }
        public bool Status { get; set; }
        public string Ficha { get; set; }

        public virtual WorkerPosition WorkerPosition { get; set; }
        public virtual WorkerGroup WorkerGroup { get; set; }

        public virtual ICollection<ProductionHistory> ProductionHistory { get; set; }
        public virtual ICollection<ShiftHistory> ShiftHistory { get; set; }
        public virtual ICollection<WeaverAssignation> WeaverAssignation { get; set; }
    }
}
