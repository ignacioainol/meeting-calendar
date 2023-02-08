using System;
using System.Collections.Generic;

namespace TCS.EF.Entidades
{
    public partial class BeamPos
    {
        public int BeamId { get; set; }
        public int Position { get; set; }
        public string JobNumber { get; set; }

        public virtual Beam Beam { get; set; }
        public virtual Job JobNumberNavigation { get; set; }
    }
}
