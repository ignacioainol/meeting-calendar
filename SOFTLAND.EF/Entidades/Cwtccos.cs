using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SOFTLAND.EF.Entidades
{
    [Table("cwtccos", Schema = "softland")]
    public class Cwtccos
    {
        public string CodiCC { get; set; }
        public string DescCC { get; set; }
        public int NivelCC { get; set; }
        public string Activo { get; set; }
        public virtual ICollection<Ccostoper> ccostopers { get; set; }
    }
}
