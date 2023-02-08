using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace TCS.EF.Entidades
{
    public partial class CurrentShiftStop
    {
        public int LoomId { get; set; }
        public int ShiftStopId { get; set; }
        public DateTime? StopHour { get; set; }
        public DateTime? StartHour { get; set; }
        public int? StoppedTime { get; set; }
        public int StopTypeId { get; set; }

        public virtual Loom Loom { get; set; }
        public virtual StopType StopType { get; set; }

        [NotMapped]
        public int? Percent { get; set; }
    }
}
