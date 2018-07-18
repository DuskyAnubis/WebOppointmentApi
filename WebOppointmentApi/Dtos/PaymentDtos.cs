using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WebOppointmentApi.Common;

namespace WebOppointmentApi.Dtos
{
    #region 平台提供接口

    #region 对账单
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

    #region 预约支付成功短信
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

    #region 开单短信通知
    public class BillingSmsParam
    {
        public string BillNum { get; set; }
        public string BillType { get; set; }
    }

    public class BillingSmsInput
    {
        public string Hospid { get; set; }
        public string Hospname { get; set; }
        public string Deptname { get; set; }
        public string Doctorname { get; set; }
        public string Time { get; set; }
        public string Pname { get; set; }
        public string Phone { get; set; }
        public string Cateid { get; set; }
        public string Ccheckcode { get; set; }
        public string Leadplace { get; set; }
        public string Userid { get; set; }
    }
    #endregion

    #region 推送待缴费信息
    public class SendPendingPaymentParam
    {
        public string BillNum { get; set; }
        public string BillType { get; set; }
    }

    public class PendingPaymentDetails
    {
        public string CItemName { get; set; }
        public string NPrice { get; set; }
        public string NNum { get; set; }
        public string CUnit { get; set; }
        public string NMoney { get; set; }
        public string CStandard { get; set; }
    }

    public class SendPendingPaymentinput
    {
        public string Hospid { get; set; }
        public string Oid { get; set; }
        public string Name { get; set; }
        public string Gender { get; set; }
        public string Cpatientcode { get; set; }
        public string Clinicno { get; set; }
        public int state { get; set; }
        public string Dname { get; set; }
        public string Docname { get; set; }
        public string Time { get; set; }
        public string Cateid { get; set; }
        public string Title { get; set; }
        public string Price { get; set; }
        public string Type { get; set; }
        public string Orderid { get; set; }
        public List<PendingPaymentDetails> Details { get; set; }
    }
    #endregion

    #endregion

    #region HIS提供接口

    #region 查询待缴费项

    #endregion

    #region 订单支付

    #endregion

    #region 退费

    #endregion

    #region 人工窗口退费查询

    #endregion

    #region 人工窗口退费置标志

    #endregion

    #region 查询交易流水

    #endregion


    #endregion
}
