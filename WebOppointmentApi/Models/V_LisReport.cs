using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebOppointmentApi.Models
{
    public class V_LisReport
    {
        public string PatientName { get; set; }
        public string Sex { get; set; }
        public string PatientCode { get; set; }
        public string ReqNo { get; set; }
        public string ReportNo { get; set; }
        public string InstrName { get; set; }
        public string Sampletype { get; set; }
        public string ReportStatus { get; set; }
        public DateTime? Cysj { get; set; }
        public string Cyr { get; set; }
        public string Sjr { get; set; }
        public DateTime? Jssj { get; set; }
        public string Jsr { get; set; }
        public string ReportName { get; set; }
        public string Jyr { get; set; }
        public DateTime? Jysj { get; set; }
        public string Reporter { get; set; }
        public DateTime? ReportDate { get; set; }
        public string Shr { get; set; }
        public DateTime Shsj { get; set; }
    }
}
