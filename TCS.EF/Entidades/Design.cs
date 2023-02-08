using System;
using System.Collections.Generic;
using System.Text;

namespace TCS.EF.Entidades
{
    public partial class Design
    {

        public int DesignId { get; set; }
        public int QualityId { get; set; }
        public int? DraftId { get; set; }
        public int? PegPlanId { get; set; }
        public int? Article { get; set; }
        public int? Healds { get; set; }
        public int NoOfWarps { get; set; }
        public int ShadesperWarp { get; set; }
        public int EndsPerRepeat { get; set; }
        public int Extras { get; set; }
        public int Castouts { get; set; }
        public int? NoOfWefts { get; set; }
        public int? ShadesperWeft { get; set; }
        public int? PicksPerRepeat { get; set; }
        public string WarpPlan { get; set; }
        public string WeftPlan { get; set; }
        public string RepeatsWarp { get; set; }
        public string RepeatsWeft { get; set; }
        public PegPlan PegPlan { get; set; }
        public Draft Draft{ get; set; }
        public ICollection<DesignYarn> DesignYarns { get; set; }
    }
}
