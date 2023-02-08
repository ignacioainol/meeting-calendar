using System;
using System.Collections.Generic;

#nullable disable

namespace INSPECTION.EF.Entidades
{
    public partial class Log
    {
        public int CodeLog { get; set; }
        public DateTime Datetime { get; set; }
        public string Username { get; set; }
        public string Description { get; set; }
    }
}
