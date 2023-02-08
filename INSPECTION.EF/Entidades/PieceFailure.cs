using System;
using System.Collections.Generic;

#nullable disable

namespace INSPECTION.EF.Entidades
{
    public partial class PieceFailure
    {
        public int CodePieceFailure { get; set; }
        public string CodeTmsPiece { get; set; }
        public byte CodeFailure { get; set; }
        public DateTime DateFailure { get; set; }
        public string ColourBonus { get; set; }
        public double InitMeter { get; set; }
        public double BonusQuantity { get; set; }
        public string Mapping { get; set; }

        public virtual Failure CodeFailureNavigation { get; set; }
        public virtual InspectedPiece CodeTmsPieceNavigation { get; set; }
    }
}
