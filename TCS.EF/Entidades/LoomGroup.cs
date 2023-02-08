using System;
using System.Collections.Generic;

namespace TCS.EF.Entidades
{
    public partial class LoomGroup
    {
        public LoomGroup()
        {
            Loom = new HashSet<Loom>();
            WeaverAssignation = new HashSet<WeaverAssignation>();
        }

        public int LoomGroupId { get; set; }
        public string Name { get; set; }

        public virtual ICollection<Loom> Loom { get; set; }
        public virtual ICollection<WeaverAssignation> WeaverAssignation { get; set; }
    }
}
