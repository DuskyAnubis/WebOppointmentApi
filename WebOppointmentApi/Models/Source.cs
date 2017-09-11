using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebOppointmentApi.Models
{
    public class Source
    {
        public int Id { get; set; }
        public int Num { get; set; }
        public int SchedulingId { get; set; }
        public string Time { get; set; }
        public string Period { get; set; }
        public string OppointmentStateCode { get; set; }
        public string OppointmentStateName { get; set; }

        public Scheduling Scheduling { get; set; }
    }
}
