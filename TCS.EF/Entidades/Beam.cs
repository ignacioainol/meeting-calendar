using System;
using System.Collections.Generic;

namespace TCS.EF.Entidades
{
    public partial class Beam
    {
        public Beam()
        {
            BeamPos = new HashSet<BeamPos>();
            LoomPos = new HashSet<LoomPos>();
        }

        public int BeamId { get; set; }
        public bool? Woven { get; set; }

        public virtual ICollection<BeamPos> BeamPos { get; set; }
        public virtual ICollection<LoomPos> LoomPos { get; set; }
    }
}
