using System;
using System.Collections.Generic;

namespace TCS.EF.Entidades
{
    public partial class State
    {
        public State()
        {
            LoomState = new HashSet<LoomState>();
        }

        public int StateId { get; set; }
        public string Description { get; set; }

        public virtual ICollection<LoomState> LoomState { get; set; }
    }
}
