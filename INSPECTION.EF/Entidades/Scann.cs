using System;
using System.Collections.Generic;

#nullable disable

namespace INSPECTION.EF.Entidades
{
    public partial class Scann
    {
        public string CodeTmsPiece { get; set; }
        public double FinalWeigth { get; set; }
        public double MetersC { get; set; }
        public double FinalMeters { get; set; }
        public double FinalWidth { get; set; }
        public int White { get; set; }
        public int Yellow { get; set; }
        public double Green { get; set; }
        public string PieceQuality { get; set; }
        public string Inspectedby { get; set; }
    }
}
