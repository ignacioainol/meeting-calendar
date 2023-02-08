using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace TCS.WebUI
{
    [Table("AUDITORIA")]
    public partial class Auditorium
    {
        [Key]
        public int AuditoriaId { get; set; }
        public int UsuarioId { get; set; }
        public int DispositivoId { get; set; }
        [Required]
        [StringLength(500)]
        public string Operacion { get; set; }
        public int? AddUserId { get; set; }
        public DateTime AddDate { get; set; }
        public int? ChgUserId { get; set; }
        public DateTime? ChgDate { get; set; }
        public bool Activo { get; set; }

        [ForeignKey(nameof(AddUserId))]
        [InverseProperty("AuditoriumAddUsers")]
        public virtual Usuario AddUser { get; set; }
        [ForeignKey(nameof(ChgUserId))]
        [InverseProperty("AuditoriumChgUsers")]
        public virtual Usuario ChgUser { get; set; }
        [ForeignKey(nameof(DispositivoId))]
        [InverseProperty("Auditoria")]
        public virtual Dispositivo Dispositivo { get; set; }
        [ForeignKey(nameof(UsuarioId))]
        [InverseProperty("AuditoriumUsuarios")]
        public virtual Usuario Usuario { get; set; }
    }
}
