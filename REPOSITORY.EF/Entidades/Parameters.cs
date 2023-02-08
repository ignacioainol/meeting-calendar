using System;
using System.Collections.Generic;
using System.Text;

namespace REPOSITORY.EF.Entidades
{
    public partial class Parameters
    {
        public int ID_Paramater { get; set; }
        public int ID_Query { get; set; }
        public string Parameter_Value { get; set; }
        public string Paramater_Name { get; set; }
        public string Parameter_Type { get; set; }
        public Query Query { get; set; }
    }
}
