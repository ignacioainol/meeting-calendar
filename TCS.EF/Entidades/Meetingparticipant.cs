using System;
using System.Collections.Generic;

namespace TCS.EF.Entidades
{
    public partial class Meetingparticipant
    {
        public int MeetingId { get; set; }
        public int UserId { get; set; }
        public bool Attended { get; set; }
        public string Justification { get; set; }

        public virtual Meeting Meeting { get; set; }
        public virtual User User { get; set; }
    }
}
