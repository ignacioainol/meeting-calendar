using System;
using System.Collections.Generic;

namespace TCS.EF.Entidades
{
    public partial class JobPos
    {
        public string JobNumber { get; set; }
        public int Position { get; set; }
        public string TicketId { get; set; }

        public virtual Job JobNumberNavigation { get; set; }
        public virtual Ticket Ticket { get; set; }
    }
}
