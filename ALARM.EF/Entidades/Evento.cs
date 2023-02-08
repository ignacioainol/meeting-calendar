using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace TCS.WebUI
{
    [Table("EVENTO")]
    public partial class Evento
    {
        [Key]
        public int EventoId { get; set; }
        public int? UsuarioId { get; set; }
        public int DispositivoId { get; set; }
        public DateTime MomentoOcurrido { get; set; }
        [Column(TypeName = "decimal(18, 6)")]
        public decimal Longitud { get; set; }
        [Column(TypeName = "decimal(18, 6)")]
        public decimal Latitud { get; set; }
        [StringLength(500)]
        public string Observacion { get; set; }
        public DateTime? AtendidoEl { get; set; }
        [Required]
        [StringLength(50)]
        public string Precision { get; set; }
        [StringLength(50)]
        public string Status { get; set; }
        public int TipoAlarma { get; set; }
        public int? AddUserId { get; set; }
        public DateTime AddDate { get; set; }
        public int? ChgUserId { get; set; }
        public DateTime? ChgDate { get; set; }
        public bool Activo { get; set; }

        [ForeignKey(nameof(AddUserId))]
        [InverseProperty("EventoAddUsers")]
        public virtual Usuario AddUser { get; set; }
        [ForeignKey(nameof(ChgUserId))]
        [InverseProperty("EventoChgUsers")]
        public virtual Usuario ChgUser { get; set; }
        [ForeignKey(nameof(DispositivoId))]
        [InverseProperty("Eventos")]
        public virtual Dispositivo Dispositivo { get; set; }
        [ForeignKey(nameof(UsuarioId))]
        [InverseProperty("EventoUsuarios")]
        public virtual Usuario Usuario { get; set; }
    }
}
