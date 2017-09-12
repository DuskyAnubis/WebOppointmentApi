using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebOppointmentApi.Models
{
    public class Registered
    {
        public int Id { get; set; }
        public string OrganazitionId { get; set; }
        public string UserId { get; set; }
        public int SchedulingId { get; set; }
        public int SourceId { get; set; }
        public string RegisteredTypeCode { get; set; }
        public string RegisteredTypeName { get; set; }
        public string OrderId { get; set; }
        public int Price { get; set; }
        public int TreatPrice { get; set; }
        public int PlusPrice { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public DateTime DoctorDate { get; set; }
        public string DoctorTime { get; set; }
        public DateTime TransactionDate { get; set; }
        public DateTime RegisteredDate { get; set; }
        public string IDCard { get; set; }
        public string GenderCode { get; set; }
        public string GenderName { get; set; }
        public string Address { get; set; }
        public DateTime Birth { get; set; }
        public string FromType { get; set; }
        public string MedicalInsuranceCode { get; set; }
        public string MedicalInsuranceName { get; set; }
        public string CardTypeCode { get; set; }
        public string CardTypeName { get; set; }
        public string CardNo { get; set; }
        public string MedicalTypeCode { get; set; }
        public string MedicalTypeName { get; set; }
        public string RegisteredStateCode { get; set; }
        public string RegisteredStateName { get; set; }
        public string Status { get; set; }

        public Scheduling Scheduling { get; set; }
    }
}
