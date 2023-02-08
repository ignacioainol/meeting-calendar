using System;
using System.Collections.Generic;

namespace INVENTORY.EF.Entidades
{
    public partial class UserInventory
    {
        public UserInventory()
        {
            Department = new HashSet<Department>();
        }

        public string UserId { get; set; }
        public string Name { get; set; }
        public int? Permission { get; set; }

        public virtual ICollection<Department> Department { get; set; }
    }
}
