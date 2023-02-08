using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCS.EF.Entidades
{
    public partial class PerformanceEvaluation
    {
        public int PerformanceEvaluationId { get; set; }

        public int ScoreCYE { get; set; }

        public int ScoreNFP { get; set; }

        public int ScorePRO { get; set; }

        public int TotalPerformanceTask { get; set; }

        public int ScoreRA { get; set; }

        public int ScoreRLM { get; set; }

        public int ScoreAUS { get; set; }

        public int ScoreATR { get; set; }

        public int ScoreAMO { get; set; }

        public int ScoreAL { get; set; }

        public int TotalAttendance { get; set; }

        public int ScoreRES { get; set; }

        public int ScoreSA { get; set; }

        public int ScoreAHE { get; set; }

        public int ScoreAHS { get; set; }

        public int ScoreAHC { get; set; }

        public int TotalAttitudes { get; set; }

        public int ScoreIC { get; set; }

        public int ScoreCMT { get; set; }

        public int ScoreCDA { get; set; }

        public int TotalSkills { get; set; }

        public string Observations { get; set; }

        public int TotalFinal { get; set; }

        public DateTime EvaluationDate { get; set; }

        public string Rut { get; set; }

        public string Name { get; set; }

        public string LastName { get; set; }

        public bool Complete { get; set; }

        public string CodiCC { get; set; }

        public string AddUsr { get; set; }

        public DateTime AddDte { get; set; }

        public string ChgUsr { get; set; }

        public DateTime ChgDte { get; set; }
    }
}
