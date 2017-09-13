using System.ComponentModel.DataAnnotations;


namespace WebOppointmentApi.Dtos
{
    public class HosptialOutput
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string AccessTypeCode { get; set; }
        public string AccessTypeName { get; set; }
        public string Address { get; set; }
        public string Info { get; set; }
        public string LogoUrl { get; set; }
        public string PicUrl { get; set; }
        public string Status { get; set; }
    }

    public class HosptialQueryInput : IPageAndSortInputDto
    {
        public string Name { get; set; }
    }

    public class HosptialCreateInput
    {
        [Required(ErrorMessage = "请输入医院编码")]
        public string Code { get; set; }
        [Required(ErrorMessage = "请输入医院名称")]
        public string Name { get; set; }
        [Required(ErrorMessage = "请输入接入方式编码")]
        public string AccessTypeCode { get; set; }
        [Required(ErrorMessage = "请输入接入方式")]
        public string AccessTypeName { get; set; }
        public string Address { get; set; }
        public string Info { get; set; }
        public string LogoUrl { get; set; }
        public string PicUrl { get; set; }
        public string Status { get; set; }
    }

    public class HosptialUpdateInput
    {
        [Required(ErrorMessage = "请输入医院Id")]
        public int Id { get; set; }
        [Required(ErrorMessage = "请输入医院编码")]
        public string Code { get; set; }
        [Required(ErrorMessage = "请输入医院名称")]
        public string Name { get; set; }
        [Required(ErrorMessage = "请输入接入方式编码")]
        public string AccessTypeCode { get; set; }
        [Required(ErrorMessage = "请输入接入方式")]
        public string AccessTypeName { get; set; }
        public string Address { get; set; }
        public string Info { get; set; }
        public string LogoUrl { get; set; }
        public string PicUrl { get; set; }
        public string Status { get; set; }
    }
}
