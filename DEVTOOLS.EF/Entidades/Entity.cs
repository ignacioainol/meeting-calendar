using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEVTOOLS.EF.Entidades
{
    public class Entity
    {
        public int EntityId { get; set; }
        public int ProjectId { get; set; }
        public Project? Project { get; set; }
        public string? EntityName { get; set; }
        public string? EntityTableName { get; set; }
        public int EntityProjectDatabaseSourceId { get; set; }
        public DatabaseSource? EntityProjectDatabaseSource { get; set; }
        public ICollection<Column>? EntityColumns { get; set; }
        public ICollection<EntityTemplate>? EntityTemplates { get; set; }
        public bool Active { get; set; } = true;
    }
}
