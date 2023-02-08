using System;
using System.Collections.Generic;

namespace TCS.EF.Entidades
{
    public partial class WorkerGroup
    {
        public WorkerGroup()
        {
            WeaverAssignation = new HashSet<WeaverAssignation>();
        }

        public int WorkerGroupId { get; set; }
        public string WorkerGroupName { get; set; }

        public virtual ICollection<WeaverAssignation> WeaverAssignation { get; set; }
        public virtual ICollection<Worker> Worker { get; set; }
    }
}
