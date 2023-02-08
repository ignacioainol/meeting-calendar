using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace TCS.EF.Entidades
{
    public partial class CurrentProd
    {
        public DateTime? Date { get; set; }
        public string Shift { get; set; }
        public string Status { get; set; }
        public decimal? ActualEff { get; set; }
        public decimal? StdEff { get; set; }
        public int? ActualSpeed { get; set; }
        public string Article { get; set; }
        public decimal? Ppc { get; set; }
        public int? Picks { get; set; }
        public int? Lenght { get; set; }
        public int? WarpStops { get; set; }
        public int? WeftStops { get; set; }
        public int? OtherStops { get; set; }
        public int? Stops { get; set; }
        public int? WarpStopTime { get; set; }
        public int? WeftStopTime { get; set; }
        public int? OtherStopTime { get; set; }
        public int? WeaverId { get; set; }
        public int? CurrentStopType { get; set; }
        public int? CurrentStopTime { get; set; }
        public int LoomId { get; set; }
        public DateTime? YarnStoreDate { get; set; }
        public DateTime? ToWarpDate { get; set; }
        public DateTime? WarpingDate { get; set; }
        public DateTime? WeavingDate { get; set; }

        public virtual Loom Loom { get; set; }

        [NotMapped]
        public string WeaverName { get; set; }
    }
}
