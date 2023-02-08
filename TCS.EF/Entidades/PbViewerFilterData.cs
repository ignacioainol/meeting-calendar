using System;
using System.Collections.Generic;

namespace TCS.EF.Entidades
{
    public partial class PbViewerFilterData
    {
        public string FilterData { get; set; }
        public string FilterName { get; set; }

        public virtual PbViewerFilter FilterNameNavigation { get; set; }
    }
}
