using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TCS.EF.Entidades
{
    public partial class Meeting
    {
        public Meeting()
        {
            Meetingparticipants = new HashSet<Meetingparticipant>();
        }

        public int MeetingId { get; set; }
        [Required(ErrorMessage = "Debe ingresar fecha de inicio.")]
        public DateTime StartDate { get; set; }
        [Required(ErrorMessage = "Debe ingresar fecha de término.")]
        public DateTime EndDate { get; set; }
        [Required(ErrorMessage = "Debe ingresar hora de inicio.")]
        public TimeSpan StartTime { get; set; }
        [Required(ErrorMessage = "Debe ingresar hora de término.")]
        public TimeSpan EndTime { get; set; }
        [Required(ErrorMessage = "Debe ingresar un título.")]
        public string Title { get; set; }
        public string Description { get; set; }
        public int UserId { get; set; }
        public bool Ended { get; set; }

        //[Required(ErrorMessage = "Debe ingresar una ubicación.")]
        public string Location { get; set; }

        public virtual User User { get; set; }
        public virtual ICollection<Meetingparticipant> Meetingparticipants { get; set; }
    }
    public class MeetingDTO
    {
        public string Title { get; set; }

        public string Description { get; set; }
    }
}
