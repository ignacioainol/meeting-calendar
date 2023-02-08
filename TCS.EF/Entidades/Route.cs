using System;
using System.Collections.Generic;

namespace TCS.EF.Entidades
{
    public partial class Route
    {
        public Route()
        {
            Routeitem = new HashSet<Routeitem>();
        }

        public string RouteId { get; set; }
        public string RouteDescription { get; set; }

        public virtual ICollection<Routeitem> Routeitem { get; set; }
    }
}
