using System;
using System.Collections.Generic;

namespace TCS.EF.Entidades
{
    public partial class PbStatus
    {
        public int StatusId { get; set; }
        public bool? Maintenance { get; set; }
        public bool? Updating { get; set; }
    }
}
