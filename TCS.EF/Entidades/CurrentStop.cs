using System;
using System.Collections.Generic;

namespace TCS.EF.Entidades
{
    public partial class CurrentStop
    {
        public int LoomId { get; set; }
        public int StopTypeId { get; set; }
        public DateTime? Date { get; set; }

        public virtual Loom Loom { get; set; }
        public virtual StopType StopType { get; set; }
    }
}
