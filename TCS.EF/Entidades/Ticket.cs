using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace TCS.EF.Entidades
{
    public partial class Ticket
    {
        public Ticket()
        {
            JobPos = new HashSet<JobPos>();
            Piece = new HashSet<Piece>();
        }

        public string TicketId { get; set; }
        public bool? MixMatch { get; set; }
        public bool? LightShade { get; set; }
        public string Healds { get; set; }
        public string Pcolour { get; set; }
        public bool? Wanted { get; set; }
        public string LoomNumber { get; set; }
        public bool? Woven { get; set; }
        public int? Ends { get; set; }
        public int? InitialLength { get; set; }
        public int? InitialWeight { get; set; }
        public int? FinalLength { get; set; }
        public int? FinalWeight { get; set; }
        public int? FinalWidth { get; set; }
        public string Quality { get; set; }
        public string Design { get; set; }
        public string WarpShade { get; set; }
        public string WeftShade { get; set; }
        public decimal? Sley { get; set; }
        public int? EndsPerReed { get; set; }
        public int? SleyWidth { get; set; }
        public string WeaveWeek { get; set; }
        public string ProductionOrder { get; set; }
        public decimal? Ppc { get; set; }
        public string Draft { get; set; }
        public string Customer { get; set; }
        public string Selvedge { get; set; }
        public string SelvedgeWording { get; set; }
        public string SelvedgeColourId { get; set; }
        public string SelvedgeColour { get; set; }
        public DateTime? YarnStoreDate { get; set; }
        public DateTime? ToWarpDate { get; set; }
        public DateTime? WarpingDate { get; set; }
        public DateTime? WeavingDate { get; set; }

        public virtual ICollection<JobPos> JobPos { get; set; }
        public virtual ICollection<Piece> Piece { get; set; }
        [NotMapped]
        public string FirstUnwovenPiece { get; set; }
        [NotMapped]
        public string LastUnwovenPiece { get; set; }
        [NotMapped]
        public string FirstPieceNumber { get; set; }
        [NotMapped]
        public string LastPieceNumber { get; set; }
        [NotMapped]
        public string UnwovenPiecesToShortString
        {
            get
            {
                string ticketTitle = "null";
                //if (!string.IsNullOrEmpty(FirstUnwovenPiece))
                //{

                if (FirstUnwovenPiece.Length < 8)
                {
                    string newMin = FirstUnwovenPiece;
                    for (int i = 0; i < (8 - FirstUnwovenPiece.Length); i++)
                    {
                        newMin = string.Format("0{0}", newMin);
                    }
                    FirstUnwovenPiece = newMin;
                }
                ticketTitle = string.Format("{0}-{1}", FirstUnwovenPiece, LastUnwovenPiece.Substring(LastUnwovenPiece.Length - 2));
                //}

                return ticketTitle;
            }
        }
    }
}
