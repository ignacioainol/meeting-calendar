using DEVTOOLS.EF.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEVTOOLS.EF.Entidades
{
    public class ColumnTemplate
    {
        public int ColumnTemplateId { get; set; }
        public int ColumnId { get; set; }
        public SourceCodeType ColumnTemplateSourceCodeType { get; set; }
        public string? ColumnTemplateName { get; set; }
        public string? ColumnTemplateObservations { get; set; }
        public string? ColumnTemplateContent { get; set; }
        public Column? Column { get; set; }
    }
}
