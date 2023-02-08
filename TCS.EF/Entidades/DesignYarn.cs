using System;
using System.Collections.Generic;
using System.Text;

namespace TCS.EF.Entidades
{
    public partial class DesignYarn
    {
        public int DesignYarnId { get; set; }
        public int DesignId { get; set; }
        public int YarnId { get; set; }
        public int Tipo { get; set; }
        public int Linea { get; set; }
        public Yarn Yarn { get; set; }
        public Design Design { get; set; }
    }
}
