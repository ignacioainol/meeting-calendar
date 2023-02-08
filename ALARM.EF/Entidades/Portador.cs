using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace TCS.WebUI
{
    [Table("PORTADOR")]
    public partial class Portador
    {
        public Portador()
        {
            ContactoEmergencia = new HashSet<ContactoEmergencium>();
            Dispositivos = new HashSet<Dispositivo>();
        }

        [Key]
        public int PortadorId { get; set; }
        public int EmpresaId { get; set; }
        public int? VehiculoId { get; set; }
        [Required]
        [StringLength(130)]
        public string Direccion { get; set; }
        [Required]
        [StringLength(500)]
        public string Nombres { get; set; }
        [Required]
        [StringLength(100)]
        public string Apellidos { get; set; }
        public int Telefono { get; set; }
        [Required]
        [StringLength(150)]
        public string Correo { get; set; }
        public int TipoPortador { get; set; }
        [StringLength(500)]
        public string Observacion { get; set; }
        [StringLength(150)]
        public string Ubicacion { get; set; }
        public int? AddUserId { get; set; }
        public DateTime AddDate { get; set; }
        public int? ChgUserId { get; set; }
        public DateTime? ChgDate { get; set; }
        public bool Activo { get; set; }
        public string Rut { get; set; }

        [ForeignKey(nameof(AddUserId))]
        [InverseProperty(nameof(Usuario.PortadorAddUsers))]
        public virtual Usuario AddUser { get; set; }
        [ForeignKey(nameof(ChgUserId))]
        [InverseProperty(nameof(Usuario.PortadorChgUsers))]
        public virtual Usuario ChgUser { get; set; }
        [ForeignKey(nameof(EmpresaId))]
        [InverseProperty("Portadors")]
        public virtual Empresa Empresa { get; set; }
        [ForeignKey(nameof(VehiculoId))]
        [InverseProperty("Portadors")]
        public virtual Vehiculo Vehiculo { get; set; }
        [InverseProperty(nameof(ContactoEmergencium.Portador))]
        public virtual ICollection<ContactoEmergencium> ContactoEmergencia { get; set; }
        [InverseProperty(nameof(Dispositivo.Portador))]
        public virtual ICollection<Dispositivo> Dispositivos { get; set; }
    }
}
