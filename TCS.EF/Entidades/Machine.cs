using System;
using System.Collections.Generic;

namespace TCS.EF.Entidades
{
    public partial class Machine
    {
        public Machine()
        {
            Processmachine = new HashSet<Processmachine>();
        }

        public int MachineId { get; set; }
        public string MachineDescription { get; set; }
        public string MachineIp { get; set; }

        public virtual ICollection<Processmachine> Processmachine { get; set; }
    }
}
