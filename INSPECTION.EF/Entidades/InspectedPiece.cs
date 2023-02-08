using System;
using System.Collections.Generic;

#nullable disable

namespace INSPECTION.EF.Entidades
{
    public partial class InspectedPiece
    {
        public InspectedPiece()
        {
            Cuts = new HashSet<Cut>();
            PieceFailures = new HashSet<PieceFailure>();
            PieceSummaries = new HashSet<PieceSummary>();
        }

        public string CodeTmsPiece { get; set; }
        public DateTime InspectionDate { get; set; }
        public double FinalMeters { get; set; }
        public double FinalWeigth { get; set; }
        public double FinalWidth { get; set; }
        public string PieceQuality { get; set; }
        public bool Reinspected { get; set; }
        public DateTime? ReinspectedDate { get; set; }
        public bool Dispatch { get; set; }
        public bool? Authorized { get; set; }
        public string Authorizedby { get; set; }
        public string Creason { get; set; }
        public string Inspectedby { get; set; }
        public string Reinspectedby { get; set; }
        public double MFails { get; set; }
        public double MGreen { get; set; }
        public int MYellow { get; set; }
        public int MWhite { get; set; }

        public virtual TmsPiece CodeTmsPieceNavigation { get; set; }
        public virtual ICollection<Cut> Cuts { get; set; }
        public virtual ICollection<PieceFailure> PieceFailures { get; set; }
        public virtual ICollection<PieceSummary> PieceSummaries { get; set; }
    }
}
