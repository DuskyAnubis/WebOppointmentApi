using System;
using System.Collections.Generic;

namespace WebOppointmentApi.Models
{
    public partial class 门诊就诊信息
    {
        public int 就诊序号 { get; set; }
        public DateTime 日期 { get; set; }
        public int? 挂号门诊号 { get; set; }
        public string 身份证 { get; set; }
        public string 社保卡 { get; set; }
        public string 卡号 { get; set; }
        public string 姓名 { get; set; }
        public string 性别 { get; set; }
        public short? 年龄 { get; set; }
        public string 年龄单位 { get; set; }
        public string 婚姻 { get; set; }
        public string 民族 { get; set; }
        public string 籍贯 { get; set; }
        public string 职业 { get; set; }
        public string 通信地址 { get; set; }
        public string 电话 { get; set; }
        public string 科室 { get; set; }
        public string 挂号类别 { get; set; }
        public string 操作员 { get; set; }
        public string 医师 { get; set; }
        public int 初诊 { get; set; }
        public int 复诊 { get; set; }
        public int 急诊 { get; set; }
        public int 接诊医师id { get; set; }
        public DateTime? 出生日期 { get; set; }
        public string 过敏史 { get; set; }
        public int DwId { get; set; }
        public int CzyId { get; set; }
    }
}
