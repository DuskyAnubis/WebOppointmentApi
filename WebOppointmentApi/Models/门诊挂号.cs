using System;
using System.Collections.Generic;

namespace WebOppointmentApi.Models
{
    public partial class 门诊挂号
    {
        public int 门诊号 { get; set; }
        public string 姓名 { get; set; }
        public short? 年龄 { get; set; }
        public string 性别 { get; set; }
        public string 婚姻 { get; set; }
        public string 籍贯 { get; set; }
        public string 职业 { get; set; }
        public string 通信地址 { get; set; }
        public string 电话 { get; set; }
        public DateTime? 日期 { get; set; }
        public string 科室 { get; set; }
        public string 挂号类别 { get; set; }
        public string 操作员 { get; set; }
        public string 医师 { get; set; }
        public decimal? 挂号费 { get; set; }
        public decimal? 工本费 { get; set; }
        public decimal? 金额 { get; set; }
        public int? 作废标志 { get; set; }
        public string 卡号 { get; set; }
        public int 初诊 { get; set; }
        public int 复诊 { get; set; }
        public int 急诊 { get; set; }
        public bool? 交班 { get; set; }
        public DateTime? 交班日期 { get; set; }
        public decimal? 退票号 { get; set; }
        public DateTime? 日结帐日期 { get; set; }
        public bool? 就诊标志 { get; set; }
        public string 民族 { get; set; }
        public int 接诊医师id { get; set; }
        public DateTime? 出生日期 { get; set; }
        public string 过敏史 { get; set; }
        public string 年龄单位 { get; set; }
        public string 身份证 { get; set; }
        public decimal? 总预存款 { get; set; }
        public decimal? 总费用 { get; set; }
        public decimal? 预存款支付 { get; set; }
        public decimal? 现金支付 { get; set; }
        public string Password { get; set; }
        public string 来源 { get; set; }
        public decimal? 预存款余额 { get; set; }
        public int 状态 { get; set; }
        public decimal 退款 { get; set; }
        public int Dw_Id { get; set; }
        public int Czy_Id { get; set; }
    }
}
