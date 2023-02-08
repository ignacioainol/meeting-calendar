using System;
using System.Collections.Generic;

namespace TCS.EF.Entidades
{
    public partial class Process
    {
        public Process()
        {
            Processmachine = new HashSet<Processmachine>();
            Routeitem = new HashSet<Routeitem>();
        }

        public int ProcessId { get; set; }
        public string ProcessDescription { get; set; }

        public virtual ICollection<Processmachine> Processmachine { get; set; }
        public virtual ICollection<Routeitem> Routeitem { get; set; }
    }
}
