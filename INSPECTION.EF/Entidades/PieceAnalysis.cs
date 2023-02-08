using System;
using System.Collections.Generic;

#nullable disable

namespace INSPECTION.EF.Entidades
{
    public partial class PieceAnalysis
    {
        public string CodeTmsPiece { get; set; }
        public string Design { get; set; }
        public string Quality { get; set; }
        public string Shade { get; set; }
        public decimal Pilling { get; set; }
        public decimal DryRub { get; set; }
        public decimal WetRub { get; set; }
        public decimal WarpDimensional { get; set; }
        public decimal WeftDimensional { get; set; }
        public decimal WarpResistance { get; set; }
        public decimal WeftResistance { get; set; }
        public decimal MartindaleAbrasion { get; set; }
        public short MartindaleAbrasionBroken { get; set; }
        public decimal DeltaE { get; set; }
        public DateTime DateCreated { get; set; }
        public decimal WarpDensity { get; set; }
        public decimal WeftDensity { get; set; }
        public decimal Width { get; set; }
        public decimal Pm2 { get; set; }
    }
}
