using System;
using System.Collections.Generic;

namespace TCS.EF.Entidades
{
    public partial class WorkerPosition
    {
        public int PositionId { get; set; }
        public string Endescr { get; set; }
        public string Spdescr { get; set; }

        public virtual ICollection<Worker> Worker { get; set; }
    }
}
