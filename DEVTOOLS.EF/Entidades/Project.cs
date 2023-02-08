using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEVTOOLS.EF.Entidades
{
    public class Project
    {
        public int ProjectId { get; set; }
        public string? ProjectName { get; set; }
        public ICollection<Entity>? ProjectEntities { get; set; }
        public ICollection<ProjectTemplate>? ProjectTemplates { get; set; }
        public ICollection<DatabaseSource>? ProjectDatabaseSources { get; set; }
    }
}
