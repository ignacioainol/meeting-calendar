using System;
using System.Collections.Generic;

#nullable disable

namespace INSPECTION.EF.Entidades
{
    public partial class Failure
    {
        public Failure()
        {
            PieceFailures = new HashSet<PieceFailure>();
        }

        public byte CodeFailure { get; set; }
        public byte CodeDepartment { get; set; }
        public string Description { get; set; }

        public virtual Department CodeDepartmentNavigation { get; set; }
        public virtual ICollection<PieceFailure> PieceFailures { get; set; }
    }
}
