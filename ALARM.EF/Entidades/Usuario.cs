using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace TCS.WebUI
{
    [Table("USUARIO")]
    public partial class Usuario
    {
        public Usuario()
        {
            AuditoriumAddUsers = new HashSet<Auditorium>();
            AuditoriumChgUsers = new HashSet<Auditorium>();
            AuditoriumUsuarios = new HashSet<Auditorium>();
            ContactoEmergenciumAddUsers = new HashSet<ContactoEmergencium>();
            ContactoEmergenciumChgUsers = new HashSet<ContactoEmergencium>();
            CoordenadumAddUsers = new HashSet<Coordenadum>();
            CoordenadumChgUsers = new HashSet<Coordenadum>();
            DispositivoAddUsers = new HashSet<Dispositivo>();
            DispositivoChgUsers = new HashSet<Dispositivo>();
            DispositivoCoordenadumAddUsers = new HashSet<DispositivoCoordenadum>();
            DispositivoCoordenadumChgUsers = new HashSet<DispositivoCoordenadum>();
            EmpresaAddUsers = new HashSet<Empresa>();
            EmpresaChgUsers = new HashSet<Empresa>();
            EventoAddUsers = new HashSet<Evento>();
            EventoChgUsers = new HashSet<Evento>();
            EventoUsuarios = new HashSet<Evento>();
            InverseAddUser = new HashSet<Usuario>();
            InverseChgUser = new HashSet<Usuario>();
            PasswordAddUsers = new HashSet<Password>();
            PasswordChgUsers = new HashSet<Password>();
            PasswordUsuarios = new HashSet<Password>();
            PortadorAddUsers = new HashSet<Portador>();
            PortadorChgUsers = new HashSet<Portador>();
            RondumAddUsers = new HashSet<Rondum>();
            RondumChgUsers = new HashSet<Rondum>();
            VehiculoAddUsers = new HashSet<Vehiculo>();
            VehiculoChgUsers = new HashSet<Vehiculo>();
        }

        [Key]
        public int UsuarioId { get; set; }
        [Required]
        [StringLength(100)]
        public string Correo { get; set; }
        public int EmpresaId { get; set; }
        public DateTime FechaLogin { get; set; }
        public DateTime? FechaLoginAnterior { get; set; }
        [StringLength(500)]
        public string Observacion { get; set; }
        [Required]
        [StringLength(100)]
        public string Nombres { get; set; }
        [Required]
        [StringLength(100)]
        public string Apellidos { get; set; }
        public int TipoUsuario { get; set; }
        public int? AddUserId { get; set; }
        public DateTime AddDate { get; set; }
        public int? ChgUserId { get; set; }
        public DateTime? ChgDate { get; set; }
        public bool Activo { get; set; }
        public int Telefono { get; set; }

        [ForeignKey(nameof(AddUserId))]
        [InverseProperty(nameof(Usuario.InverseAddUser))]
        public virtual Usuario AddUser { get; set; }
        [ForeignKey(nameof(ChgUserId))]
        [InverseProperty(nameof(Usuario.InverseChgUser))]
        public virtual Usuario ChgUser { get; set; }
        [ForeignKey(nameof(EmpresaId))]
        [InverseProperty("Usuarios")]
        public virtual Empresa Empresa { get; set; }
        [InverseProperty(nameof(Auditorium.AddUser))]
        public virtual ICollection<Auditorium> AuditoriumAddUsers { get; set; }
        [InverseProperty(nameof(Auditorium.ChgUser))]
        public virtual ICollection<Auditorium> AuditoriumChgUsers { get; set; }
        [InverseProperty(nameof(Auditorium.Usuario))]
        public virtual ICollection<Auditorium> AuditoriumUsuarios { get; set; }
        [InverseProperty(nameof(ContactoEmergencium.AddUser))]
        public virtual ICollection<ContactoEmergencium> ContactoEmergenciumAddUsers { get; set; }
        [InverseProperty(nameof(ContactoEmergencium.ChgUser))]
        public virtual ICollection<ContactoEmergencium> ContactoEmergenciumChgUsers { get; set; }
        [InverseProperty(nameof(Coordenadum.AddUser))]
        public virtual ICollection<Coordenadum> CoordenadumAddUsers { get; set; }
        [InverseProperty(nameof(Coordenadum.ChgUser))]
        public virtual ICollection<Coordenadum> CoordenadumChgUsers { get; set; }
        [InverseProperty(nameof(Dispositivo.AddUser))]
        public virtual ICollection<Dispositivo> DispositivoAddUsers { get; set; }
        [InverseProperty(nameof(Dispositivo.ChgUser))]
        public virtual ICollection<Dispositivo> DispositivoChgUsers { get; set; }
        [InverseProperty(nameof(DispositivoCoordenadum.AddUser))]
        public virtual ICollection<DispositivoCoordenadum> DispositivoCoordenadumAddUsers { get; set; }
        [InverseProperty(nameof(DispositivoCoordenadum.ChgUser))]
        public virtual ICollection<DispositivoCoordenadum> DispositivoCoordenadumChgUsers { get; set; }
        [InverseProperty("AddUser")]
        public virtual ICollection<Empresa> EmpresaAddUsers { get; set; }
        [InverseProperty("ChgUser")]
        public virtual ICollection<Empresa> EmpresaChgUsers { get; set; }
        [InverseProperty(nameof(Evento.AddUser))]
        public virtual ICollection<Evento> EventoAddUsers { get; set; }
        [InverseProperty(nameof(Evento.ChgUser))]
        public virtual ICollection<Evento> EventoChgUsers { get; set; }
        [InverseProperty(nameof(Evento.Usuario))]
        public virtual ICollection<Evento> EventoUsuarios { get; set; }
        [InverseProperty(nameof(Usuario.AddUser))]
        public virtual ICollection<Usuario> InverseAddUser { get; set; }
        [InverseProperty(nameof(Usuario.ChgUser))]
        public virtual ICollection<Usuario> InverseChgUser { get; set; }
        [InverseProperty(nameof(Password.AddUser))]
        public virtual ICollection<Password> PasswordAddUsers { get; set; }
        [InverseProperty(nameof(Password.ChgUser))]
        public virtual ICollection<Password> PasswordChgUsers { get; set; }
        [InverseProperty(nameof(Password.Usuario))]
        public virtual ICollection<Password> PasswordUsuarios { get; set; }
        [InverseProperty(nameof(Portador.AddUser))]
        public virtual ICollection<Portador> PortadorAddUsers { get; set; }
        [InverseProperty(nameof(Portador.ChgUser))]
        public virtual ICollection<Portador> PortadorChgUsers { get; set; }
        [InverseProperty(nameof(Rondum.AddUser))]
        public virtual ICollection<Rondum> RondumAddUsers { get; set; }
        [InverseProperty(nameof(Rondum.ChgUser))]
        public virtual ICollection<Rondum> RondumChgUsers { get; set; }
        [InverseProperty(nameof(Vehiculo.AddUser))]
        public virtual ICollection<Vehiculo> VehiculoAddUsers { get; set; }
        [InverseProperty(nameof(Vehiculo.ChgUser))]
        public virtual ICollection<Vehiculo> VehiculoChgUsers { get; set; }
    }
}
