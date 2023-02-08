using System;
using System.Collections.Generic;

#nullable disable

namespace INSPECTION.EF.Entidades
{
    public partial class TmsPiece
    {
        public string CodeTmsPiece { get; set; }
        public string Quality { get; set; }
        public string Design { get; set; }
        public string Shade { get; set; }
        public string CodeCustomer { get; set; }
        public string NameCustomer { get; set; }
        public decimal Sp { get; set; }
        public double EcruMeters { get; set; }
        public double EcruWeigth { get; set; }
        public double EcruWidth { get; set; }
        public string LoomNumber { get; set; }
        public DateTime LoomDate { get; set; }
        public string CodeMender { get; set; }
        public string NameMender { get; set; }
        public string CategoryName { get; set; }
        public DateTime? DateScann { get; set; }

        public virtual InspectedPiece InspectedPiece { get; set; }
    }
}
