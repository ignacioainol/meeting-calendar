using System;
using System.Collections.Generic;
using System.Text;

namespace TCS.EF.Entidades
{
    public partial class PegPlanDraft
    {

        public int PegPlanId { get; set; }
        public int DraftId { get; set; }

        public virtual PegPlan PegPlan { get; set; }
        public virtual Draft Draft { get; set; }
    }
}
