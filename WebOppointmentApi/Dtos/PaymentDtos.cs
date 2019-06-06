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
        public List<PendingPaymentDetails> Detail { get; set; }
    }
    #endregion

    #region 面对面扫码付款
    public class ScanCodePayParam
    {
        public string BillNum { get; set; }
        public string ScanCode { get; set; }
    }

    public class ScanCodePrepaymentParam
    {
        public string SerialCode { get; set; }
        public string ScanCode { get; set; }
    }

    public class ScanCodePayDetail
    {
        public string Orderid { get; set; }
        public string Price { get; set; }
        public string Cateid { get; set; }
        public string Name { get; set; }
        public string Gender { get; set; }
        public string State { get; set; }
        public string Dname { get; set; }
        public string Docname { get; set; }
        public string Time { get; set; }
        public string Title { get; set; }
    }

    public class ScanCodePayInput
    {
        public string Scantext { get; set; }
        public string Userid { get; set; }
        public string Hospid { get; set; }
        public string Totaloid { get; set; }
        public string Totalprice { get; set; }
        public string Cateid { get; set; }
        public string Totaltitle { get; set; }
        public string State { get; set; }
        public List<ScanCodePayDetail> Orderids { get; set; }
    }

    public class ScanCodeResultDetail
    {
        public string Oid { get; set; }
        public string Ouno { get; set; }
    }

    public class ScanCodeResult
    {
        public string Ouno { get; set; }
        public List<ScanCodeResultDetail> Ounos { get; set; }
    }

    public class ScanCodePayBody
    {
        public int Code { get; set; }
        public string Msg { get; set; }
        public ScanCodeResult Result { get; set; }
    }
    #endregion

    #region 面对面扫码付款状态查询
    public class ScanCodeQueryParam
    {
        public string BillNum { get; set; }
    }

    public class ScanCodePrepaymentQueryParam
    {
        public string SerialCode { get; set; }
    }

    public class ScanCodeQueryInput
    {
        public string Userid { get; set; }
        public string Hospid { get; set; }
        public string Orderid { get; set; }
    }
    #endregion

    #region 面对面扫码付款支付完成
    public class ScanCodeCompleteParam
    {
        public string BillNum { get; set; }
        public string CFlowCode { get; set; }
    }

    public class ScanCodePrepaymentCompleteParam
    {
        public string SerialCode { get; set; }
    }

    public class ScanCodeCompleteDetail
    {
        public string Ouno { get; set; }
        public string Price { get; set; }
        public string Cflowcode { get; set; }
    }

    public class ScanCodeCompleteInput
    {
        public string Touno { get; set; }
        public string Userid { get; set; }
        public string Code { get; set; }
        public string Totalprice { get; set; }
        public string Tcflowcode { get; set; }
        public List<ScanCodeCompleteDetail> Ounos { get; set; }
    }
    #endregion

    #region 面对面扫码付款取消订单
    public class ScanCodeCancelParam
    {
        public string BillNum { get; set; }
    }

    public class ScanCodePrepaymentCancelParam
    {
        public string SerialCode { get; set; }
    }

    public class ScanCodeCancelInput
    {
        public string Userid { get; set; }
        public string Hospid { get; set; }
        public string Orderid { get; set; }
    }
    #endregion

    #endregion

    #region HIS提供接口

    #region 查询待缴费项
    public class SearchPendingPaymentParam
    {
        public string Indextype { get; set; }
        public string Cpatientcode { get; set; }
        public string Orderid { get; set; }
        public string Cateid { get; set; }
        public string Opcode { get; set; }
    }

    public class PendingPayment
    {
        public string Name { get; set; }
        public string Gender { get; set; }
        public int State { get; set; }
        public string Dname { get; set; }
        public string Docname { get; set; }
        public string Time { get; set; }
        public string Cateid { get; set; }
        public string Title { get; set; }
        public string Price { get; set; }
        public string Orderid { get; set; }
        public List<PendingPaymentDetails> Detail { get; set; }
    }

    public class SearchPendingPaymentOutput
    {
        public int Code { get; set; }
        public string Msg { get; set; }
        public List<PendingPayment> Items { get; set; }
    }
    #endregion

    #region 订单支付
    public class PayOrderParam
    {
        public string Indextype { get; set; }
        public string Cpatientcode { get; set; }
        public string Orderid { get; set; }
        public string Cateid { get; set; }
        public string Userid { get; set; }
        public string Ls_cpscode { get; set; }
        public string Cinterpayfromtype { get; set; }
    }

    public class PayOrderOutput
    {
        public int Code { get; set; }
        public string Msg { get; set; }
        public string Cflowcode { get; set; }
        public string Cpatientcode { get; set; }
        public string Cidentitycard { get; set; }
        public string Cpatientname { get; set; }
        public string Csex { get; set; }
        public string Ddiagnosetime { get; set; }
        public string Deptname { get; set; }
        public string Doctorname { get; set; }
        public string Cdiagnosetypename { get; set; }
        public string Ndiagnosenum { get; set; }
        public string Chousesectionname { get; set; }
        public string Chousename { get; set; }
        public string Clocation { get; set; }
        public string Nmoney { get; set; }
        public string Diagnoseid { get; set; }
        public string Windowmsg { get; set; }
        public string Windowname { get; set; }
    }
    #endregion

    #region 退费
    public class RefundOrderParam
    {
        public string Cflowcode { get; set; }
        public string Userid { get; set; }
    }

    public class RefundOrderOutput
    {
        public int Code { get; set; }
        public string Msg { get; set; }
    }
    #endregion

    #region 人工窗口退费查询
    public class SearchWindowRefundParam
    {
        public string CFlowCode { get; set; }
    }

    public class SearchWindowRefund
    {
        public string CFlowCode { get; set; }
        public string CRateType { get; set; }
        public string NMoney { get; set; }
        public string CPatientCode { get; set; }
        public string CPatientName { get; set; }
        public string Ordercode { get; set; }
        public string TradeDate { get; set; }
    }

    public class SearchWindowRefundOutput
    {
        public int Code { get; set; }
        public string Msg { get; set; }
        public List<SearchWindowRefund> Result { get; set; }
    }
    #endregion

    #region 人工窗口退费置标志
    public class FlagWindowRefundParam
    {
        public string CFlowCode { get; set; }
    }

    public class FlagWindowRefundOutput
    {
        public int Code { get; set; }
        public string Msg { get; set; }
    }
    #endregion

    #region 查询交易流水
    public class SearchTradeFlowParam
    {
        public string Cinterpayfromtype { get; set; }
        public string Trade_beg { get; set; }
        public string Trade_end { get; set; }
    }

    public class SearchTradeFlow
    {
        public string Cflowcode { get; set; }
        public string OrderCode { get; set; }
        public string Cateid { get; set; }
        public string Tradetype { get; set; }
        public string Nmoney { get; set; }
        public string Tradedate { get; set; }
        public string Ls_cpscode { get; set; }
        public string Cpatientname { get; set; }
        public string Cpatientcode { get; set; }
        public string Cidentitycard { get; set; }
        public string Tongchoumoney { get; set; }
        public string Accountmoney { get; set; }
        public string Factmoney { get; set; }
        public string Bdayend { get; set; }
        public string Dayendtime { get; set; }
    }

    public class SearchTradeFlowOutput
    {
        public int Code { get; set; }
        public string Msg { get; set; }
        public List<SearchTradeFlow> Results { get; set; }
    }
    #endregion

    #endregion
}
