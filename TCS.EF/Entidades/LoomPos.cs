using System;
using System.Collections.Generic;

namespace TCS.EF.Entidades
{
    public partial class LoomPos
    {
        public int LoomId { get; set; }
        public int Position { get; set; }
        public int? BeamId { get; set; }
        public int? LinkId { get; set; }

        public virtual Beam Beam { get; set; }
        public virtual LinkType Link { get; set; }
        public virtual Loom Loom { get; set; }
    }
}
