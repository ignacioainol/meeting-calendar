using System;
using System.Collections.Generic;

namespace TCS.EF.Entidades
{
    public partial class CurrentShift
    {
        public string Shift { get; set; }
        public DateTime? Date { get; set; }
        public int? Hour { get; set; }
    }
}
