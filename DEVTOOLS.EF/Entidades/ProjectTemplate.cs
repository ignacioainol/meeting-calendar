using DEVTOOLS.EF.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEVTOOLS.EF.Entidades
{
    public class ProjectTemplate
    {
        public int ProjectTemplateId { get; set; }
        public int ProjectId { get; set; }
        public string? ProjectTemplateName { get; set; }
        public string? ProjectTemplateObservations { get; set; }
        public SourceCodeType ProjectTemplateSourceCodeType { get; set; }
        public string? ProjectTemplateContent { get; set; }
        public Project? Project { get; set; }
    }
}
