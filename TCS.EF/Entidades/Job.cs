using System;
using System.Collections.Generic;

namespace TCS.EF.Entidades
{
    public partial class Job
    {
        public Job()
        {
            BeamPos = new HashSet<BeamPos>();
            JobPos = new HashSet<JobPos>();
        }

        public string JobNumber { get; set; }
        public bool? Woven { get; set; }

        public virtual ICollection<BeamPos> BeamPos { get; set; }
        public virtual ICollection<JobPos> JobPos { get; set; }
    }
}
