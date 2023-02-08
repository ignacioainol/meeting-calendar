using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace TCS.WebUI
{
    [Table("VEHICULO")]
    public partial class Vehiculo
    {
        public Vehiculo()
        {
            Portadors = new HashSet<Portador>();
        }

        [Key]
        public int VehiculoId { get; set; }
        [StringLength(20)]
        public string Marca { get; set; }
        [StringLength(30)]
        public string Color { get; set; }
        [Required]
        [StringLength(20)]
        public string Patente { get; set; }
        public int? AddUserId { get; set; }
        public DateTime AddDate { get; set; }
        public int? ChgUserId { get; set; }
        public DateTime? ChgDate { get; set; }
        public bool Activo { get; set; }

        [ForeignKey(nameof(AddUserId))]
        [InverseProperty(nameof(Usuario.VehiculoAddUsers))]
        public virtual Usuario AddUser { get; set; }
        [ForeignKey(nameof(ChgUserId))]
        [InverseProperty(nameof(Usuario.VehiculoChgUsers))]
        public virtual Usuario ChgUser { get; set; }
        [InverseProperty(nameof(Portador.Vehiculo))]
        public virtual ICollection<Portador> Portadors { get; set; }
    }
}
