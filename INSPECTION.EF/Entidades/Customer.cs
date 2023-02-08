using System;
using System.Collections.Generic;

#nullable disable

namespace INSPECTION.EF.Entidades
{
    public partial class Customer
    {
        public string CustomerName { get; set; }
        public bool CustomerShowmsg { get; set; }
        public string CustomerMsg { get; set; }
        public string CustomerFilter { get; set; }
    }
}
