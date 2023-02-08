using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TCS.EF.Entidades
{
    public partial class Draft
    {
        public int DraftId { get; set; }
        [Required(ErrorMessage = "El codigo es requerido.")]
        public string Code { get; set; }
        [Required(ErrorMessage = "Este dato es requerido.")]
        public int Shafts { get; set; }
        [Required(ErrorMessage = "Este dato es requerido.")]
        public int Lenght { get; set; }
        //[Required(ErrorMessage = "Este dato es requerido.")]
        public int? Ends { get; set; }
        public string PlanCode { get; set; }
        public string SleyCode { get; set; }
        public string Description { get; set; }
        public bool Authorized { get; set; }
        public int? UserId { get; set; }
        public string Repeats { get; set; }
        public int? AuthorizedBy { get; set; }

        public virtual User User { get; set; }  
        public virtual User Autorized { get; set; }
        public virtual ICollection<PegPlanDraft> DraftSleys { get; set; }
        public virtual ICollection<Design> Designs { get; set; }

        //public virtual ICollection<DraftsAudit> DraftsAudit { get; set; }

        [NotMapped]
        public string wwwrootpath { get; set; }
    }
}
