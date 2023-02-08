using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace TCS.WebUI
{
    [Table("EMPRESA")]
    public partial class Empresa
    {
        public Empresa()
        {
            Dispositivos = new HashSet<Dispositivo>();
            Portadors = new HashSet<Portador>();
            Ronda = new HashSet<Rondum>();
            Usuarios = new HashSet<Usuario>();
        }

        [Key]
        public int EmpresaId { get; set; }
        [Required]
        [StringLength(50)]
        public string Nombre { get; set; }
        [Required]
        [StringLength(500)]
        public string RazonSocial { get; set; }
        [StringLength(30)]
        public string Direccion { get; set; }
        [StringLength(50)]
        public string Rut { get; set; }
        [StringLength(500)]
        public string Observacion { get; set; }
        public int? AddUserId { get; set; }
        public DateTime AddDate { get; set; }
        public int? ChgUserId { get; set; }
        public DateTime? ChgDate { get; set; }
        public bool Activo { get; set; }

        [ForeignKey(nameof(AddUserId))]
        [InverseProperty(nameof(Usuario.EmpresaAddUsers))]
        public virtual Usuario AddUser { get; set; }
        [ForeignKey(nameof(ChgUserId))]
        [InverseProperty(nameof(Usuario.EmpresaChgUsers))]
        public virtual Usuario ChgUser { get; set; }
        [InverseProperty(nameof(Dispositivo.Empresa))]
        public virtual ICollection<Dispositivo> Dispositivos { get; set; }
        [InverseProperty(nameof(Portador.Empresa))]
        public virtual ICollection<Portador> Portadors { get; set; }
        [InverseProperty(nameof(Rondum.Empresa))]
        public virtual ICollection<Rondum> Ronda { get; set; }
        [InverseProperty(nameof(Usuario.Empresa))]
        public virtual ICollection<Usuario> Usuarios { get; set; }
    }
}
