using System;
using System.Collections.Generic;

namespace TCS.EF.Entidades
{
    public partial class Tracking
    {
        public string PieceNumber { get; set; }
        public int ProcessId { get; set; }
        public int MachineId { get; set; }
        public DateTime? ScannDate { get; set; }
        public int? ReprocessCount { get; set; }
        public bool? StatusProcess { get; set; }

        public virtual Processmachine Processmachine { get; set; }
    }
}
