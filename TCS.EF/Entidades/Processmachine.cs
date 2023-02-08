using System;
using System.Collections.Generic;

namespace TCS.EF.Entidades
{
    public partial class Processmachine
    {
        public Processmachine()
        {
            Tracking = new HashSet<Tracking>();
        }

        public int ProcessId { get; set; }
        public int MachineId { get; set; }

        public virtual Machine Machine { get; set; }
        public virtual Process Process { get; set; }
        public virtual ICollection<Tracking> Tracking { get; set; }
    }
}
