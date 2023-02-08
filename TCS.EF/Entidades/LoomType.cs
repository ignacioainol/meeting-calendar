using System;
using System.Collections.Generic;

namespace TCS.EF.Entidades
{
    public partial class LoomType
    {
        public LoomType()
        {
            Loom = new HashSet<Loom>();
        }

        public int LoomTypeId { get; set; }
        public string Description { get; set; }
        public bool? Status { get; set; }

        public virtual ICollection<Loom> Loom { get; set; }
    }
}
