using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace TCS.WebUI
{
    [Table("PASSWORD")]
    public partial class Password
    {
        [Key]
        public int PasswordId { get; set; }
        [Required]
        [StringLength(100)]
        public string PasswordUsuario { get; set; }
        public int UsuarioId { get; set; }
        public int? AddUserId { get; set; }
        public DateTime AddDate { get; set; }
        public int? ChgUserId { get; set; }
        public DateTime? ChgDate { get; set; }
        public bool Activo { get; set; }

        [ForeignKey(nameof(AddUserId))]
        [InverseProperty("PasswordAddUsers")]
        public virtual Usuario AddUser { get; set; }
        [ForeignKey(nameof(ChgUserId))]
        [InverseProperty("PasswordChgUsers")]
        public virtual Usuario ChgUser { get; set; }
        [ForeignKey(nameof(UsuarioId))]
        [InverseProperty("PasswordUsuarios")]
        public virtual Usuario Usuario { get; set; }
    }
}
