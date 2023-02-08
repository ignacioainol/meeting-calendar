using System;
using System.Collections.Generic;

#nullable disable

namespace INSPECTION.EF.Entidades
{
    public partial class PieceFailsSummary
    {
        public long Id { get; set; }
        public string CodeTmsPiece { get; set; }
        public byte PieceNumber { get; set; }
        public double Meter { get; set; }
        public byte CodeFailure { get; set; }
        public string ColourBonus { get; set; }
        public string Mapping { get; set; }
        public double Bonus { get; set; }

        public virtual PieceSummary PieceSummary { get; set; }
    }
}
