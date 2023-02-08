using System;
using System.Collections.Generic;

namespace TCS.EF.Entidades
{
    public partial class CalendarP
    {
        public int? CalendarId { get; set; }
        public DateTime CalendarDate { get; set; }
        public int? IsHoliday { get; set; }
    }
}
