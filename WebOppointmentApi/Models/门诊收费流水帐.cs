using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebOppointmentApi.Models
{
    public class 门诊收费流水帐
    {
        public int 收费id { get; set; }
        public DateTime? 日期 { get; set; }
        public string 操作员 { get; set; }
        public string 病人姓名 { get; set; }
        public string 卡号 { get; set; }
        public decimal? 总金额 { get; set; }
        public decimal? 优惠额 { get; set; }
        public decimal? 账户支付 { get; set; }
        public decimal? 统筹支付 { get; set; }
        public decimal? 补助金 { get; set; }
        public decimal? 现金支付 { get; set; }
        public bool? 交班标志 { get; set; }
        public DateTime? 结帐日期 { get; set; }
        public string 门诊号 { get; set; }
        public string 发票号 { get; set; }
        public string PInfo { get; set; }
        public int? 退票 { get; set; }
        public string 费别 { get; set; }
        public decimal 折扣率 { get; set; }
        public string 单据流水号 { get; set; }
        public string 医疗保险号 { get; set; }
        public decimal 基金支付额 { get; set; }
        public decimal 帐户余额 { get; set; }
        public decimal 公补基金支付 { get; set; }
        public string 医保 { get; set; }
        public string 性别 { get; set; }
        public int DwId { get; set; }
        public int CzyId { get; set; }
        public string PayMethod { get; set; }
        public string PayFrom { get; set; }
        public bool IsWindowRefund { get; set; }
        public int WindowRefundFlag { get; set; }
    }
}
