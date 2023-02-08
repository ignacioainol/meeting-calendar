using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SOFTLAND.EF.Entidades
{
    [Table("sw_ccostoper", Schema = "softland")]
    public class Ccostoper
    {
        [Key]
        public string ficha { get; set; }
        public string codiCC { get; set; }
        public DateTime vigDesde { get; set; }
        public DateTime vigHasta { get; set; }
        public virtual Personal personal { get; set; }
        public virtual Cwtccos cwtcco { get; set; }
    }
}
