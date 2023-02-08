using System;
using System.Collections.Generic;
using System.Text;

namespace TCS.EF.Entidades
{
    public partial class UserPermission
    {
        public int UserId { get; set; }
        public int PermissionId { get; set; }
        public int ModuleId { get; set; }

        public virtual Module Module { get; set; }
        public virtual Permission Permission { get; set; }
        public virtual User User { get; set; }
    }
}
