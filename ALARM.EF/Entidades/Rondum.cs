using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace TCS.WebUI
{
    [Table("RONDA")]
    public partial class Rondum
    {
        public Rondum()
        {
            Coordenada = new HashSet<Coordenadum>();
        }

        [Key]
        public int RondaId { get; set; }
        public int EmpresaId { get; set; }
        [Required]
        [StringLength(100)]
        public string Nombre { get; set; }
        [StringLength(500)]
        public string Descripcion { get; set; }
        public int? AddUserId { get; set; }
        public DateTime AddDate { get; set; }
        public int? ChgUserId { get; set; }
        public DateTime? ChgDate { get; set; }
        public bool Activo { get; set; }

        [ForeignKey(nameof(AddUserId))]
        [InverseProperty(nameof(Usuario.RondumAddUsers))]
        public virtual Usuario AddUser { get; set; }
        [ForeignKey(nameof(ChgUserId))]
        [InverseProperty(nameof(Usuario.RondumChgUsers))]
        public virtual Usuario ChgUser { get; set; }
        [ForeignKey(nameof(EmpresaId))]
        [InverseProperty("Ronda")]
        public virtual Empresa Empresa { get; set; }
        [InverseProperty(nameof(Coordenadum.Ronda))]
        public virtual ICollection<Coordenadum> Coordenada { get; set; }
    }
}
