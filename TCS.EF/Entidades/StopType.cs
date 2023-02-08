using System;
using System.Collections.Generic;

namespace TCS.EF.Entidades
{
    public partial class StopType
    {
        public StopType()
        {
            CurrentShiftStop = new HashSet<CurrentShiftStop>();
            CurrentStop = new HashSet<CurrentStop>();
            StopHistory = new HashSet<StopHistory>();
        }

        public int StopTypeId { get; set; }
        private string description;
        public string Description 
        {
            get
            {
                return this.description.Replace("wx"," ");
            }
            set
            {
                this.description = value;
            }
        }

        public virtual ICollection<CurrentShiftStop> CurrentShiftStop { get; set; }
        public virtual ICollection<CurrentStop> CurrentStop { get; set; }
        public virtual ICollection<StopHistory> StopHistory { get; set; }
    }
}
