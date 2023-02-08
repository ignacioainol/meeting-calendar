using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace TCS.WebUI
{
    [Table("DISPOSITIVO")]
    public partial class Dispositivo
    {
        public Dispositivo()
        {
            Auditoria = new HashSet<Auditorium>();
            DispositivoCoordenada = new HashSet<DispositivoCoordenadum>();
            Eventos = new HashSet<Evento>();
        }

        [Key]
        public int DispositivoId { get; set; }
        public int EmpresaId { get; set; }
        public int? PortadorId { get; set; }
        [StringLength(50)]
        public string Status { get; set; }
        public int NumeroTelefono { get; set; }
        public int TipoDispositivo { get; set; }
        public int? AddUserId { get; set; }
        public DateTime AddDate { get; set; }
        public int? ChgUserId { get; set; }
        public DateTime? ChgDate { get; set; }
        public bool Activo { get; set; }

        [ForeignKey(nameof(AddUserId))]
        [InverseProperty(nameof(Usuario.DispositivoAddUsers))]
        public virtual Usuario AddUser { get; set; }
        [ForeignKey(nameof(ChgUserId))]
        [InverseProperty(nameof(Usuario.DispositivoChgUsers))]
        public virtual Usuario ChgUser { get; set; }
        [ForeignKey(nameof(EmpresaId))]
        [InverseProperty("Dispositivos")]
        public virtual Empresa Empresa { get; set; }
        [ForeignKey(nameof(PortadorId))]
        [InverseProperty("Dispositivos")]
        public virtual Portador Portador { get; set; }
        [InverseProperty(nameof(Auditorium.Dispositivo))]
        public virtual ICollection<Auditorium> Auditoria { get; set; }
        [InverseProperty(nameof(DispositivoCoordenadum.Dispositivo))]
        public virtual ICollection<DispositivoCoordenadum> DispositivoCoordenada { get; set; }
        [InverseProperty(nameof(Evento.Dispositivo))]
        public virtual ICollection<Evento> Eventos { get; set; }
    }
}
