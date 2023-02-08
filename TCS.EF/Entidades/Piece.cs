using System;
using System.Collections.Generic;

namespace TCS.EF.Entidades
{
    public partial class Piece
    {
        public string PieceNumber { get; set; }
        public string TicketId { get; set; }
        public string ProgStatus { get; set; }
        public string Quality { get; set; }
        public string BatchNumber { get; set; }
        public string RouteId { get; set; }

        public virtual Ticket Ticket { get; set; }
    }
}
