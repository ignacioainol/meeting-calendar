using System;
using System.Collections.Generic;

namespace TCS.EF.Entidades
{
    public partial class ShiftPattern
    {
        public string PatternId { get; set; }
        public string Pattern { get; set; }
        public int? Index { get; set; }
    }
}
