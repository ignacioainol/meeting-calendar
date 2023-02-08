using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEVTOOLS.EF.Enums
{
    public enum RelationShipType
    {
        None,
        OneToOne,
        ZeroToOne,
        OneToMany,
        ZeroToMany,
        ManyToOne,
        ManyToMany
    }
}
