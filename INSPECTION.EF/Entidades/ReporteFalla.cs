using System;
using System.Collections.Generic;

#nullable disable

namespace INSPECTION.EF.Entidades
{
    public partial class ReporteFalla
    {
        public DateTime InspectionDate { get; set; }
        public string CodeTmsPiece { get; set; }
        public byte PieceNumber { get; set; }
        public string CategoryName { get; set; }
        public string Quality { get; set; }
        public double Meter { get; set; }
        public byte CodeFailure { get; set; }
        public string Description { get; set; }
        public string Department { get; set; }
        public string ColourBonus { get; set; }
        public string Mapping { get; set; }
        public double Bonus { get; set; }
    }
}
