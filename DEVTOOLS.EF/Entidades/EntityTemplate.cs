using DEVTOOLS.EF.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEVTOOLS.EF.Entidades
{
    public class EntityTemplate
    {
        public int EntityTemplateId { get; set; }
        public int EntityId { get; set; }
        public string? EntityTemplateName { get; set; }
        public string? EntityTemplateObservations { get; set; }
        public SourceCodeType EntityTemplateSourceCodeType { get; set; }
        public string? EntityTemplateContent { get; set; }
        public virtual Entity? Entity { get; set; }
    }
}
