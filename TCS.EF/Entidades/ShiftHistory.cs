using System;
using System.Collections.Generic;

namespace TCS.EF.Entidades
{
    public partial class ShiftHistory
    {
        public string Shift { get; set; }
        public DateTime Date { get; set; }
        public int? SupervisorId { get; set; }

        public virtual Worker Supervisor { get; set; }
    }
}
