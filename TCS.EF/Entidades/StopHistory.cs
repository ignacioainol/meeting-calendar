using System;
using System.Collections.Generic;

namespace TCS.EF.Entidades
{
    public partial class StopHistory
    {
        public int LoomId { get; set; }
        public DateTime Date { get; set; }
        public string Shift { get; set; }
        public int StopTypeId { get; set; }
        public int? Stops { get; set; }
        public int? StoppedTime { get; set; }

        public virtual ProductionHistory ProductionHistory { get; set; }
        public virtual StopType StopType { get; set; }
    }
}
