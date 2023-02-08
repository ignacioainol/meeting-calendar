using DEVTOOLS.EF.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEVTOOLS.EF.Entidades
{
    public class DatabaseSource
    {
        public int DatabaseSourceId { get; set; }
        public int ProjectId { get; set; }
        public string? DatabaseSourceName { get; set; }
        public Project? Project { get; set; }
        public DatabaseType DatabaseSourceType { get; set; }
        public string? DatabaseSourceDNS { get; set; }
    }
}
