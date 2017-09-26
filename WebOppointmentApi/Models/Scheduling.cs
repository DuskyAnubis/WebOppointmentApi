using System;

namespace WebOppointmentApi.Models
{
    public class Scheduling
    {
        public int Id { get; set; }
        public string SchedulingTypeCode { get; set; }
        public string SchedulingTypeName { get; set; }
        public string EndTreatCode { get; set; }
        public string EndTreatName { get; set; }
        public int UserId { get; set; }
        public DateTime? SurgeryDate { get; set; }
        public DateTime? SchedulingDate { get; set; }
        public DateTime? EndTreatDate { get; set; }
        public DateTime? RecoveryTreatDate { get; set; }
        public string EndTreatReason { get; set; }
        public int MaxCount { get; set; }
        public int TotalCount { get; set; }
        public string PeriodTypeCode { get; set; }
        public string PeriodTypeName { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public int Price { get; set; }
        public int TreatPrice { get; set; }
        public int PlusPrice { get; set; }
        public string Address { get; set; }
        public string Status { get; set; }

        public User User { get; set; }
    }
}
