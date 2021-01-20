using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebOppointmentApi.Models
{
    public class V_LisReportDetail
    {
        public string PatientName { get; set; }
        public string Sex { get; set; }
        public string PatientCode { get; set; }
        public string ReqNo { get; set; }
        public string ReportNo { get; set; }
        public string ItemNo { get; set; }
        public string ItemName { get; set; }
        public string Result { get; set; }
        public string Unit { get; set; }
        public string Ckz { get; set; }
        public string ResultFlag { get; set; }
        public string TestMethod { get; set; }
        public DateTime? ReportDate { get; set; }
    }
}
