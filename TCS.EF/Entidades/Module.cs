using System;
using System.Collections.Generic;
using System.Text;

namespace TCS.EF.Entidades
{
    public partial class Module
    {
        public Module()
        {
            UserPermissions = new HashSet<UserPermission>();
        }

        public int ModuleId { get; set; }
        public string Description { get; set; }

        public virtual ICollection<UserPermission> UserPermissions { get; set; }
    }
}
