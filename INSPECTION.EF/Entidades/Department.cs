using System;
using System.Collections.Generic;

#nullable disable

namespace INSPECTION.EF.Entidades
{
    public partial class Department
    {
        public Department()
        {
            Failures = new HashSet<Failure>();
        }

        public byte CodeDepartment { get; set; }
        public string Description { get; set; }

        public virtual ICollection<Failure> Failures { get; set; }
    }
}
