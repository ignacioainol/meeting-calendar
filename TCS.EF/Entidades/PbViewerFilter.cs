using System;
using System.Collections.Generic;

namespace TCS.EF.Entidades
{
    public partial class PbViewerFilter
    {
        public PbViewerFilter()
        {
            PbViewerFilterData = new HashSet<PbViewerFilterData>();
        }

        public string FilterName { get; set; }
        public string UserId { get; set; }
        public string FilterType { get; set; }
        public int? Zoom { get; set; }
        public bool? HideLoom { get; set; }
        public int? Minutes { get; set; }
        public string Description { get; set; }

        public virtual PbAuthorizedUser User { get; set; }
        public virtual ICollection<PbViewerFilterData> PbViewerFilterData { get; set; }
    }
}
