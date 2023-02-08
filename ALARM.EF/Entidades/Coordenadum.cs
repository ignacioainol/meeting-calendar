using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace TCS.WebUI
{
    [Table("COORDENADA")]
    public partial class Coordenadum
    {
        public Coordenadum()
        {
            DispositivoCoordenada = new HashSet<DispositivoCoordenadum>();
        }

        [Key]
        public int CoordenadaId { get; set; }
        public int? RondaId { get; set; }
        [Column(TypeName = "decimal(18, 6)")]
        public decimal Longitud { get; set; }
        [Column(TypeName = "decimal(18, 6)")]
        public decimal Latitud { get; set; }
        public int? AddUserId { get; set; }
        public DateTime AddDate { get; set; }
        public int? ChgUserId { get; set; }
        public DateTime? ChgDate { get; set; }
        public bool Activo { get; set; }

        [ForeignKey(nameof(AddUserId))]
        [InverseProperty(nameof(Usuario.CoordenadumAddUsers))]
        public virtual Usuario AddUser { get; set; }
        [ForeignKey(nameof(ChgUserId))]
        [InverseProperty(nameof(Usuario.CoordenadumChgUsers))]
        public virtual Usuario ChgUser { get; set; }
        [ForeignKey(nameof(RondaId))]
        [InverseProperty(nameof(Rondum.Coordenada))]
        public virtual Rondum Ronda { get; set; }
        [InverseProperty(nameof(DispositivoCoordenadum.Coordenada))]
        public virtual ICollection<DispositivoCoordenadum> DispositivoCoordenada { get; set; }
    }
}
