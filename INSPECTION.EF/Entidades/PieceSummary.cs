using System;
using System.Collections.Generic;

#nullable disable

namespace INSPECTION.EF.Entidades
{
    public partial class PieceSummary
    {
        public PieceSummary()
        {
            PieceFailsSummaries = new HashSet<PieceFailsSummary>();
        }

        public string CodeTmsPiece { get; set; }
        public byte PieceNumber { get; set; }
        public string Quality { get; set; }
        public byte Yellow { get; set; }
        public byte White { get; set; }
        public double Green { get; set; }
        public double Bonus { get; set; }
        public double Meters { get; set; }
        public double NetMeters { get; set; }
        public string Creason { get; set; }

        public virtual InspectedPiece CodeTmsPieceNavigation { get; set; }
        public virtual ICollection<PieceFailsSummary> PieceFailsSummaries { get; set; }
    }
}
