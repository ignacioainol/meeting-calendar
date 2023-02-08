using System;
using System.Collections.Generic;

namespace TCS.EF.Entidades
{
    public partial class CurrentShiftSpeed
    {
        public int LoomId { get; set; }
        public DateTime DateAndTime { get; set; }
        public int Speed { get; set; }

        public virtual Loom Loom { get; set; }
    }
}
