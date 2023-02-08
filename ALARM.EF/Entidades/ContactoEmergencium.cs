using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace TCS.WebUI
{
    [Table("CONTACTO_EMERGENCIA")]
    public partial class ContactoEmergencium
    {
        [Key]
        public int ContactoEmergenciaId { get; set; }
        public int PortadorId { get; set; }
        [Required]
        [StringLength(100)]
        public string Nombres { get; set; }
        [Required]
        [StringLength(100)]
        public string Apellidos { get; set; }
        [StringLength(500)]
        public string Descripcion { get; set; }
        public int PrioridadContacto { get; set; }
        public int Telefono1 { get; set; }
        public int? Telefono2 { get; set; }
        public int? AddUserId { get; set; }
        public DateTime AddDate { get; set; }
        public int? ChgUserId { get; set; }
        public DateTime? ChgDate { get; set; }
        public bool Activo { get; set; }

        [ForeignKey(nameof(AddUserId))]
        [InverseProperty(nameof(Usuario.ContactoEmergenciumAddUsers))]
        public virtual Usuario AddUser { get; set; }
        [ForeignKey(nameof(ChgUserId))]
        [InverseProperty(nameof(Usuario.ContactoEmergenciumChgUsers))]
        public virtual Usuario ChgUser { get; set; }
        [ForeignKey(nameof(PortadorId))]
        [InverseProperty("ContactoEmergencia")]
        public virtual Portador Portador { get; set; }
    }
}
