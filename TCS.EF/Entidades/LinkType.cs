using System;
using System.Collections.Generic;

namespace TCS.EF.Entidades
{
    public partial class LinkType
    {
        public LinkType()
        {
            LoomPos = new HashSet<LoomPos>();
        }

        public int LinkId { get; set; }
        public string Description { get; set; }
        public int? Duration { get; set; }

        public virtual ICollection<LoomPos> LoomPos { get; set; }
    }
}
