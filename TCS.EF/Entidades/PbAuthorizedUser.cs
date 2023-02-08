using System;
using System.Collections.Generic;

namespace TCS.EF.Entidades
{
    public partial class PbAuthorizedUser
    {
        public PbAuthorizedUser()
        {
            PbViewerFilter = new HashSet<PbViewerFilter>();
        }

        public string UserId { get; set; }

        public virtual ICollection<PbViewerFilter> PbViewerFilter { get; set; }
    }
}
