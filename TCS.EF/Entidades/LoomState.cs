using System;
using System.Collections.Generic;

namespace TCS.EF.Entidades
{
    public partial class LoomState
    {
        public int? Duration { get; set; }
        public int? StateId { get; set; }
        public int LoomId { get; set; }
        public DateTime? DurationDate { get; set; }

        public virtual Loom Loom { get; set; }
        public virtual State State { get; set; }
    }
}
