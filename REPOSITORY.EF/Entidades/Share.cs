using System;
using System.Collections.Generic;
using System.Text;

namespace REPOSITORY.EF.Entidades
{
    public partial class Share
    {
        public int ID_Share { get; set; }
        public int ID_Query { get; set; }
        public string User_Share { get; set; }
        public bool Read_Share { get; set; }
        public bool Write_Share { get; set; }

        public Query Query { get; set; }
    }
}
