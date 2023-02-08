using System;
using System.Collections.Generic;

namespace TCS.EF.Entidades
{
    public partial class ResumeTable
    {
        public string LoomId { get; set; }
        public string LoomNumber { get; set; }
        public string BeamId { get; set; }
        public int? Position { get; set; }
        public string LinkId { get; set; }
        public string LinkDescription { get; set; }
        public string LinkHours { get; set; }
        public DateTime? Date { get; set; }
        public int? NumeroPiezas { get; set; }
        public double? StimatedTime { get; set; }
        public int? StandardSpeed { get; set; }
        public int? StandardEfficiency { get; set; }
        public int? ActualSpeed { get; set; }
        public double? ActualEff { get; set; }
        public string JobNumber { get; set; }
        public string TicketName { get; set; }
        public string Tercera { get; set; }
        public string Cuarta { get; set; }
        public string Customer { get; set; }
        public string ProgStatus { get; set; }
        public string WeekNumber { get; set; }
        public int? StoppedTime { get; set; }
        public string PrimeraLinea { get; set; }
        public string SegundaLinea { get; set; }
    }
}
