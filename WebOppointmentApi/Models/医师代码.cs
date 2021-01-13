using System;
using System.Collections.Generic;

namespace WebOppointmentApi.Models
{
    public partial class 医师代码
    {
        public int Id { get; set; }
        public string 医师代码1 { get; set; }
        public string 医师姓名 { get; set; }
        public string 所在科室 { get; set; }
        public string 密码 { get; set; }
        public int 职称 { get; set; }
        public int 权限 { get; set; }
        public bool? 运行权限 { get; set; }
        public int 流水号 { get; set; }
        public string 挂号科室 { get; set; }
        public bool 登陆标志 { get; set; }
        public string Ip { get; set; }
        public string 机器名 { get; set; }
        public DateTime 日期 { get; set; }
        public string 医保医师编码 { get; set; }
        public int 划价号 { get; set; }
        public int DwId { get; set; }
        public int KsId { get; set; }
    }
}
