using System;
using System.Collections.Generic;

namespace TCS.EF.Entidades
{
    public partial class WeaverAssignation
    {
        public int LoomGroupId { get; set; }
        public int WorkerGroupId { get; set; }
        public int? WorkerId { get; set; }

        public virtual LoomGroup LoomGroup { get; set; }
        public virtual Worker Worker { get; set; }
        public virtual WorkerGroup WorkerGroup { get; set; }
    }
}
