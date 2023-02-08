using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TCS.EF.Entidades
{
    public partial class User
    {
        public User()
        {

            Meetingparticipants = new HashSet<Meetingparticipant>();
            Meetings = new HashSet<Meeting>();
            Drafts = new HashSet<PegPlan>();
            UserPermissions = new HashSet<UserPermission>();
        }

        public int UserId { get; set; }
        [Required(ErrorMessage = "Este dato es requerido.")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Este dato es requerido.")]
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        [Required(ErrorMessage = "Este dato es requerido.")]
        public string LastName { get; set; }
        public string SecondLastName { get; set; }
        public string Email { get; set; }
        public string PhoneLine { get; set; }
        public string Department { get; set; }
        public DateTime LastLogin { get; set; }
        [Required(ErrorMessage = "Este dato es requerido.")]
        public string Language { get; set; }
        [Required(ErrorMessage = "Este dato es requerido.")]
        public string Culture { get; set; }
        public string Photo { get; set; }
        public bool? WeavingSummary { get; set; }
        public string Rut { get; set; }
        public string Address { get; set; }
        public string Ficha { get; set; }

        [NotMapped]
        public string FullName
        {
            get
            {
                return string.Format("{0} {1}", FirstName, LastName);
            }
            set
            {
                FullName = value;
            }
        }

        [NotMapped]
        public string FullNamePlusUserName
        {
            get
            {
                return string.Format("{0} - {1} {2}", UserName, FirstName, LastName);
            }
            set
            {
                FullNamePlusUserName = value;
            }
        }

        [NotMapped]
        public bool IsLocked { get; set; } = false;
        public virtual ICollection<PegPlan> Drafts { get; set; }
        public virtual ICollection<Draft> Sleys { get; set; }
        public virtual ICollection<PegPlanAudit> DraftsAudit { get; set; }
        public virtual ICollection<UserPermission> UserPermissions { get; set; }
        public virtual ICollection<Covid> Covids { get; set; }

        public virtual ICollection<Meetingparticipant> Meetingparticipants { get; set; }
        public virtual ICollection<Meeting> Meetings { get; set; }
    }
}
