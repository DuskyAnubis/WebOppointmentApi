using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebOppointmentApi.Models
{
    public class InpatientPrepayment
    {
        public int Id { get; set; }
        public int PrepaymentId { get; set; }
        public int PatientId { get; set; }
        public string SerialCode { get; set; }
        public DateTime TradeTime { get; set; }
        public decimal Money { get; set; }
        public string PayMethod { get; set; }
        public string PayFrom { get; set; }
        public string OrderCode { get; set; }
        public string ParentOrderCode { get; set; }
        public string PlatformCode { get; set; }
        public string ParentPlatformCode { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public int Dw_Id { get; set; }
    }
}
