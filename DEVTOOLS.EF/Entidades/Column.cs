using DEVTOOLS.EF.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEVTOOLS.EF.Entidades
{
    public class Column
    {
        public int ColumnId { get; set; }
        public int ColumnIndex { get; set; }
        public int EntityId { get; set; }
        public bool Active { get; set; } = true;
        public bool ColumnPrimaryKey { get; set; } = false;
        public bool ColumnForeignKey { get; set; } = false;
        public int ColumnParentForeignKey { get; set; } = 0;
        public string? ColumnName { get; set; }
        public string? ColumnDescription { get; set; }
        public ColType ColumnType { get; set; } = ColType.Integer;
        public ICollection<ColumnTemplate>? ColumnTemplates { get; set; }
        public Entity? Entity { get; set; }
    }
}
