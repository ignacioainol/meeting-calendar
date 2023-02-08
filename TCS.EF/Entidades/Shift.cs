using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace TCS.EF.Entidades
{
    public partial class Shift
    {
        public int ShiftId { get; set; }
        public string ShiftLetter { get; set; }
        public DateTime? StartHour { get; set; }
        public DateTime? EndHour { get; set; }

        [NotMapped]
        public string ShiftText
        {
            get
            {
                if (ShiftLetter == "A")
                {
                    return "night";
                }
                else if (ShiftLetter == "B")
                {
                    return "morning";
                }
                else if (ShiftLetter == "C")
                {
                    return "afternoon";
                }
                else
                {
                    return null;
                }
            }
            set
            {
                ShiftText = value;
            }
        }
    }
}
