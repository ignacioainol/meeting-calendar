using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace TCS.EF.Entidades
{
    public partial class Yarn
    {
        public int YarnId { get; set; }
        public string YarnCount { get; set; }
        public string Shade { get; set; }
        public ICollection<DesignYarn> DesignYarns { get; set; }

        [NotMapped]
        public string YarnShade
        {
            get
            {
                return $"{this.YarnCount} - {this.Shade}";
            }
        }
    }
}
