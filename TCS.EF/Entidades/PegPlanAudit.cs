using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TCS.EF.Entidades
{
    public partial class PegPlanAudit
    {
        public int PegPlanAuditId { get; set; }
        public int PegPlanId { get; set; }
        public int? UserId { get; set; }
        public string Code { get; set; }
        public int Shafts { get; set; }
        public int Ends { get; set; }
        public string DraftPlan { get; set; }
        public string Description { get; set; }
        public bool Authorized { get; set; }
        public string Repeats { get; set; }
        public string Process { get; set; }
        public int? ByUser { get; set; }
        public DateTime OnDay { get; set; }
        public int? AuthorizedBy { get; set; }
        public virtual User User { get; set; }
        public virtual PegPlan Draft { get; set; }

    }
}
