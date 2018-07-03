using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WebOppointmentApi.Common;

namespace WebOppointmentApi.Dtos
{
    #region 平台提供接口

    #region 对账单接口
    public class DownPayBillParam
    {
        [Required(ErrorMessage = "请输入对账日期")]
        public string TradeDate { get; set; }
    }

    public class DownPayBillInput
    {
        public string Hospid { get; set; }
        public string Tradedate { get; set; }
    }
    #endregion

    #region 预约支付成功短信接口
    public class OrderSmsParam
    {
        public string Phone { get; set; }
        public string Oid { get; set; }
        public string SmsContent { get; set; }
    }

    public class OrderSmsInput
    {
        public string Hospid { get; set; }
        public string Phone { get; set; }
        public string Oid { get; set; }
        public string Smscontent { get; set; }
    }
    #endregion

    #region 开单短信通知接口

    #endregion

    #endregion

    #region HIS提供接口

    #endregion
}
