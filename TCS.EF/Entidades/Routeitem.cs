using System;
using System.Collections.Generic;

namespace TCS.EF.Entidades
{
    public partial class Routeitem
    {
        public int ProcessId { get; set; }
        public string RouteId { get; set; }
        public int NumberOrder { get; set; }

        public virtual Process Process { get; set; }
        public virtual Route Route { get; set; }
    }
}
