using System;
using System.ComponentModel.DataAnnotations;

namespace WebOppointmentApi.Dtos
{
    public class RegisteredOutput
    {
        public int Id { get; set; }
        public int SchedulingId { get; set; }
        public int OrganazitionId { get; set; }
        public string OrganazitionCode { get; set; }
        public string OrganazitionName { get; set; }
        public int UserId { get; set; }
        public string UserCode { get; set; }
        public string UserName { get; set; }
        public int Price { get; set; }
        public int TreatPrice { get; set; }
        public int PlusPrice { get; set; }
        public string RegisteredTypeCode { get; set; }
        public string RegisteredTypeName { get; set; }
        public string OrderId { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public DateTime? DoctorDate { get; set; }
        public string DoctorTime { get; set; }
        public DateTime? TransactionDate { get; set; }
        public DateTime? RegisteredDate { get; set; }
        public string IDCard { get; set; }
        public string GenderCode { get; set; }
        public string GenderName { get; set; }
        public string Address { get; set; }
        public string Birth { get; set; }
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
    }

    public class RegisteredQueryInput : IPageAndSortInputDto
    {
        public RegisteredQueryInput()
        {
            base.SortBy = "DoctorDate Asc";
        }
        public int OrganazitionId { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; }
        public string OrderId { get; set; }
        public string RegisteredStateCode { get; set; }
        public DateTime? DoctorDate { get; set; }
    }

    public class RegisteredCreateInput
    {
        [Required(ErrorMessage = "请选择预约班次")]
        public int SchedulingId { get; set; }
        [Required(ErrorMessage = "请选择预约日期")]
        public DateTime? DoctorDate { get; set; }
        [Required(ErrorMessage = "请选择预约时间")]
        public string DoctorTime { get; set; }
        public DateTime? RegisteredDate { get; set; }
        public string RegisteredTypeCode { get; set; }
        public string RegisteredTypeName { get; set; }
        [Required(ErrorMessage = "请输入患者姓名")]
        public string Name { get; set; }
        [Required(ErrorMessage = "请输入患者手机号")]
        public string Phone { get; set; }
        [Required(ErrorMessage = "请输入患者身份证号")]
        public string IDCard { get; set; }
        [Required(ErrorMessage = "请输入患者性别")]
        public string GenderCode { get; set; }
        public string GenderName { get; set; }
        [Required(ErrorMessage = "请输入患者地址")]
        public string Address { get; set; }
        [Required(ErrorMessage = "请输入患者生日")]
        public string Birth { get; set; }
        [Required(ErrorMessage = "请输入患者来源")]
        public string FromType { get; set; }
        [Required(ErrorMessage = "请输入患者医保类型")]
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
    }

    public class RegisteredUpdateInput
    {
        [Required(ErrorMessage = "Id不能为空")]
        public int Id { get; set; }
        [Required(ErrorMessage = "请选择预约班次")]
        public int SchedulingId { get; set; }
        public string OrderId { get; set; }
        [Required(ErrorMessage = "请选择预约日期")]
        public DateTime? DoctorDate { get; set; }
        [Required(ErrorMessage = "请选择预约时间")]
        public string DoctorTime { get; set; }
        public DateTime? TransactionDate { get; set; }
        public DateTime? RegisteredDate { get; set; }
        public string RegisteredTypeCode { get; set; }
        public string RegisteredTypeName { get; set; }
        [Required(ErrorMessage = "请输入患者姓名")]
        public string Name { get; set; }
        [Required(ErrorMessage = "请输入患者手机号")]
        public string Phone { get; set; }
        [Required(ErrorMessage = "请输入患者身份证号")]
        public string IDCard { get; set; }
        [Required(ErrorMessage = "请输入患者性别")]
        public string GenderCode { get; set; }
        public string GenderName { get; set; }
        [Required(ErrorMessage = "请输入患者地址")]
        public string Address { get; set; }
        [Required(ErrorMessage = "请输入患者生日")]
        public string Birth { get; set; }
        [Required(ErrorMessage = "请输入患者来源")]
        public string FromType { get; set; }
        [Required(ErrorMessage = "请输入患者医保类型")]
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
    }

}
