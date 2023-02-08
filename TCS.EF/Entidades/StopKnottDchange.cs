using System;
using System.Collections.Generic;

namespace TCS.EF.Entidades
{
    public partial class StopKnottDchange
    {
        public int LoomId { get; set; }
        public int? LinkId { get; set; }
        public DateTime? DateStart { get; set; }
    }
}
