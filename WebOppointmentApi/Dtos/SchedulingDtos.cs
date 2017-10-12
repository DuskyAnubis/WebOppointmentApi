using System;
using System.ComponentModel.DataAnnotations;

namespace WebOppointmentApi.Dtos
{
    public class SchedulingOutput
    {
        public int Id { get; set; }
        public string SchedulingTypeCode { get; set; }
        public string SchedulingTypeName { get; set; }
        public string EndTreatCode { get; set; }
        public string EndTreatName { get; set; }
        public int OrganazitionId { get; set; }
        public string OrganazitionCode { get; set; }
        public string OrganazitionName { get; set; }
        public int UserId { get; set; }
        public string UserCode { get; set; }
        public string UserName { get; set; }
        public string UserRankCode { get; set; }
        public string UserRankName { get; set; }
        public string RegisteredRankCode { get; set; }
        public string RegisteredRankName { get; set; }
        public DateTime? SurgeryDate { get; set; }
        public DateTime? SchedulingDate { get; set; }
        public DateTime? EndTreatDate { get; set; }
        public DateTime? RecoveryTreatDate { get; set; }
        public string EndTreatReason { get; set; }
        public int MaxCount { get; set; }
        public int TotalCount { get; set; }
        public int RemainCount { get; set; } //剩余可预约数
        public string PeriodTypeCode { get; set; }
        public string PeriodTypeName { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public int Price { get; set; }
        public int TreatPrice { get; set; }
        public int PlusPrice { get; set; }
        public string Address { get; set; }
        public string Status { get; set; }
    }

    public class SchedulingQueryInput : IPageAndSortInputDto
    {
        public SchedulingQueryInput()
        {
            base.SortBy = "SurgeryDate Asc";
        }
        public int OrganazitionId { get; set; }
        public int UserId { get; set; }
        public DateTime? SurgeryDate { get; set; }
        public string EndTreatCode { get; set; }
    }

    public class SchedulingCreateInput
    {
        public string SchedulingTypeCode { get; set; }
        public string SchedulingTypeName { get; set; }
        public string EndTreatCode { get; set; }
        public string EndTreatName { get; set; }
        [Required(ErrorMessage = "请选择医生")]
        public int UserId { get; set; }
        [Required(ErrorMessage = "请选择出诊日期")]
        public DateTime? SurgeryDate { get; set; }
        public DateTime? EndTreatDate { get; set; }
        public DateTime? RecoveryTreatDate { get; set; }
        public string EndTreatReason { get; set; }
        [Required(ErrorMessage = "请输入最大预约数")]
        public int MaxCount { get; set; }
        [Required(ErrorMessage = "请输入总放号量")]
        public int TotalCount { get; set; }
        [Required(ErrorMessage = "请选择时间段")]
        public string PeriodTypeCode { get; set; }
        public string PeriodTypeName { get; set; }
        [Required(ErrorMessage = "请选择开始时间")]
        public string StartTime { get; set; }
        [Required(ErrorMessage = "请选择结束时间")]
        public string EndTime { get; set; }
        public int Price { get; set; }
        public int TreatPrice { get; set; }
        public int PlusPrice { get; set; }
        public string Address { get; set; }
        public int IsSms { get; set; }
        public string SmsDate { get; set; }
        public string Status { get; set; }
    }

    public class SchedulingUpdateInput
    {
        [Required(ErrorMessage = "Id不能为空")]
        public int Id { get; set; }
        public string SchedulingTypeCode { get; set; }
        public string SchedulingTypeName { get; set; }
        public string EndTreatCode { get; set; }
        public string EndTreatName { get; set; }
        [Required(ErrorMessage = "请选择医生")]
        public int UserId { get; set; }
        [Required(ErrorMessage = "请选择出诊日期")]
        public DateTime? SurgeryDate { get; set; }
        public DateTime? SchedulingDate { get; set; }
        public DateTime? EndTreatDate { get; set; }
        public DateTime? RecoveryTreatDate { get; set; }
        public string EndTreatReason { get; set; }
        [Required(ErrorMessage = "请输入最大预约数")]
        public int MaxCount { get; set; }
        [Required(ErrorMessage = "请输入总放号量")]
        public int TotalCount { get; set; }
        [Required(ErrorMessage = "请选择时间段")]
        public string PeriodTypeCode { get; set; }
        public string PeriodTypeName { get; set; }
        [Required(ErrorMessage = "请选择开始时间")]
        public string StartTime { get; set; }
        [Required(ErrorMessage = "请选择结束时间")]
        public string EndTime { get; set; }
        public int Price { get; set; }
        public int TreatPrice { get; set; }
        public int PlusPrice { get; set; }
        public string Address { get; set; }
        public int IsSms { get; set; }
        public string SmsDate { get; set; }
        public string Status { get; set; }
    }

    public class SchedulingEndTreatInput
    {
        [Required(ErrorMessage = "Id不能为空")]
        public int Id { get; set; }
        [Required(ErrorMessage = "请输入停诊原因")]
        public string EndTreatReason { get; set; }
        public int IsSms { get; set; }
        public string SmsDate { get; set; }
        public string SmsTime { get; set; }
    }

    public class SchedulingRecoveryTreatInput
    {
        [Required(ErrorMessage = "Id不能为空")]
        public int Id { get; set; }
        public int IsSms { get; set; }
        public string SmsDate { get; set; }
        public string SmsTime { get; set; }
    }
}
