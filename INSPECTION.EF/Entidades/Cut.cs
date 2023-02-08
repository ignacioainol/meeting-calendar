using System;
using System.Collections.Generic;

#nullable disable

namespace INSPECTION.EF.Entidades
{
    public partial class Cut
    {
        public int CodeCut { get; set; }
        public string CodeTmsPiece { get; set; }
        public string Quality { get; set; }
        public double InitMeter { get; set; }
        public double FinalMeter { get; set; }
        public int Width { get; set; }
        public string Cause { get; set; }

        public virtual InspectedPiece CodeTmsPieceNavigation { get; set; }
    }
}
