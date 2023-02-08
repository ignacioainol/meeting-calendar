using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace TCS.WebUI
{
    [Table("DISPOSITIVO_COORDENADA")]
    public partial class DispositivoCoordenadum
    {
        [Key]
        public int DispositivoCoordenadaId { get; set; }
        public int DispositivoId { get; set; }
        public int CoordenadaId { get; set; }
        public DateTime Momento { get; set; }
        public int? AddUserId { get; set; }
        public DateTime AddDate { get; set; }
        public int? ChgUserId { get; set; }
        public DateTime? ChgDate { get; set; }
        public bool Activo { get; set; }

        [ForeignKey(nameof(AddUserId))]
        [InverseProperty(nameof(Usuario.DispositivoCoordenadumAddUsers))]
        public virtual Usuario AddUser { get; set; }
        [ForeignKey(nameof(ChgUserId))]
        [InverseProperty(nameof(Usuario.DispositivoCoordenadumChgUsers))]
        public virtual Usuario ChgUser { get; set; }
        [ForeignKey(nameof(CoordenadaId))]
        [InverseProperty(nameof(Coordenadum.DispositivoCoordenada))]
        public virtual Coordenadum Coordenada { get; set; }
        [ForeignKey(nameof(DispositivoId))]
        [InverseProperty("DispositivoCoordenada")]
        public virtual Dispositivo Dispositivo { get; set; }
    }
}
