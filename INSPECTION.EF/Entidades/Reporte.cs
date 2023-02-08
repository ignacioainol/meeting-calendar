using System;
using System.Collections.Generic;

#nullable disable

namespace INSPECTION.EF.Entidades
{
    public partial class Reporte
    {
        public DateTime? InspectionDate { get; set; }
        public string CodeTmsPiece { get; set; }
        public byte PieceNumber { get; set; }
        public string CategoryName { get; set; }
        public string Design { get; set; }
        public string Shade { get; set; }
        public string NameCustomer { get; set; }
        public string Quality { get; set; }
        public byte Yellow { get; set; }
        public byte White { get; set; }
        public double Green { get; set; }
        public double Meters { get; set; }
        public double NetMeters { get; set; }
        public double Bonus { get; set; }
        public string Creason { get; set; }
    }
}
