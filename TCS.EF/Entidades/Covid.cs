using System;
using System.Collections.Generic;
using System.Text;

namespace TCS.EF.Entidades
{
    public partial class Covid
    {
        public int CovidId { get; set; }
        public int UserId { get; set; }
        public bool Confirmacion { get; set; }
        public DateTime FechaConfirmacion { get; set; }
        public virtual User User { get; set; }
    }
}
