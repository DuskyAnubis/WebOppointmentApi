using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebOppointmentApi.Dtos
{
    #region 平台提供接口

    #region 住院患者信息同步
    public class SynchronizingInpatientParam
    {
        public string InpatientNum { get; set; }
    }

    public class SynchronizingInpatientInput
    {
        public string Pid { get; set; }
        public string Adnum { get; set; }
        public string Bednum { get; set; }
        public string Name { get; set; }
        public string Date { get; set; }
        public int State { get; set; }
        public string Idcard { get; set; }
        public string Phone { get; set; }
        public string Hospid { get; set; }
        public string Hospname { get; set; }
        public string Deptid { get; set; }
        public string Deptname { get; set; }
        public string Doctid { get; set; }
        public string Docname { get; set; }
        public int Sex { get; set; }
        public int Age { get; set; }
        public int Mtype { get; set; }
    }
    #endregion

    #region 邀请绑定住院服务
    public class InviteBindInpatientParam
    {
        public string InpatientNum { get; set; }
    }

    public class InviteBindInpatientInput
    {
        public string Pid { get; set; }
        public string Hospid { get; set; }
    }
    #endregion

    #region 解除绑定住院服务
    public class RelieveBindInpatientParam
    {
        public string InpatientNum { get; set; }
    }

    public class RelieveBindInpatientInput
    {
        public string Pid { get; set; }
        public string Hospid { get; set; }
        public string Userid { get; set; }
    }
    #endregion

    #region 检验报告同步

    #endregion

    #region 费用清单同步
    public class SynchronizingCostListParam
    {
        public string InpatientNum { get; set; }
        public string Date { get; set; }
    }

    public class SynchronizingCostListInput
    {
        public string Pid { get; set; }
        public string Hospid { get; set; }
        public List<SynchronizingCostItem> Item { get; set; }
    }

    public class SynchronizingCostItem
    {
        public string Time { get; set; }
        public string Total { get; set; }
        public SynchronizingCostItemTitle Title { get; set; }
        public List<SynchronizingCostItemContent> Content { get; set; }
    }

    public class SynchronizingCostItemTitle
    {
        public string Name { get; set; }
        public string Spec { get; set; }
        public string Count { get; set; }
        public string Sum { get; set; }
        public string Ext1 { get; set; }
        public string Ext2 { get; set; }
    }

    public class SynchronizingCostItemContent
    {
        public int Id { get; set; }
        public string Time { get; set; }
        public string Name { get; set; }
        public string Spec { get; set; }
        public string Count { get; set; }
        public string Sum { get; set; }
        public string Ext1 { get; set; }
        public string Ext2 { get; set; }
    }
    #endregion

    #endregion

    #region HIS提供接口

    #region 同步绑定关系
    public class SynchronizingBindParam
    {
        public int Opcode { get; set; }
        public string Hospid { get; set; }
        public string Pid { get; set; }
        public string Phone { get; set; }
        public string Userid { get; set; }
    }

    public class SynchronizingBindOutput
    {
        public int Code { get; set; }
        public string Msg { get; set; }
        public string Result { get; set; }
    }
    #endregion

    #region 获取检验报告明细

    #endregion

    #region 获取费用清单
    public class UpdateCostListParam
    {
        public string Hospid { get; set; }
        public string Pid { get; set; }
        public string Time { get; set; }
    }

    public class UpdateCostListOutput
    {
        public int Code { get; set; }
        public string Msg { get; set; }
        public string Hospid { get; set; }
        public string Pid { get; set; }
        public List<UpdateCostItem> Item { get; set; }
    }

    public class UpdateCostItem
    {
        public string Time { get; set; }
        public string Total { get; set; }
        public UpdateCostItemTitle Title { get; set; }
        public List<UpdateCostItemContent> Content { get; set; }
    }

    public class UpdateCostItemTitle
    {
        public string Name { get; set; }
        public string Spec { get; set; }
        public string Count { get; set; }
        public string Sum { get; set; }
        public string Ext1 { get; set; }
        public string Ext2 { get; set; }
    }

    public class UpdateCostItemContent
    {
        public int Id { get; set; }
        public string Time { get; set; }
        public string Name { get; set; }
        public string Spec { get; set; }
        public string Count { get; set; }
        public string Sum { get; set; }
        public string Ext1 { get; set; }
        public string Ext2 { get; set; }
    }
    #endregion

    #region 获取费用汇总
    public class CostTotalParam
    {
        public string Hospid { get; set; }
        public string Pid { get; set; }
    }

    public class CostTotalOutput
    {
        public int Code { get; set; }
        public string Msg { get; set; }
        public string Time { get; set; }
        public string Cid { get; set; }
        public string Tprice { get; set; }
        public string Cash { get; set; }
        public string Dbcash { get; set; }
        public string Copay { get; set; }
        public string Ownpay { get; set; }
        public string Hctprice { get; set; }
        public string State { get; set; }
        public CostTotalItem Item { get; set; }
    }

    public class CostTotalItem
    {
        public CostTotalTitle Title { get; set; }
        public List<CostTotalContent> Content { get; set; }
    }

    public class CostTotalTitle
    {
        public string Name { get; set; }
        public string Sum { get; set; }
        public string Ext1 { get; set; }
    }

    public class CostTotalContent
    {
        public string Name { get; set; }
        public string Sum { get; set; }
        public string Ext1 { get; set; }
    }
    #endregion

    #region 获取费用明细
    public class CostDetailParam
    {
        public string Hospid { get; set; }
        public string Pid { get; set; }
    }

    public class CostDetailOutput
    {
        public int Code { get; set; }
        public string Msg { get; set; }
        public string Time { get; set; }
        public string Total { get; set; }
        public CostDetailTitle Title { get; set; }
        public List<CostDetailContent> Content { get; set; }
    }

    public class CostDetailTitle
    {
        public string Name { get; set; }
        public string Spec { get; set; }
        public string Count { get; set; }
        public string Sum { get; set; }
        public string Ext1 { get; set; }
        public string Ext2 { get; set; }
    }

    public class CostDetailContent
    {
        public int Id { get; set; }
        public string Time { get; set; }
        public string Name { get; set; }
        public string Spec { get; set; }
        public string Count { get; set; }
        public string Sum { get; set; }
        public string Ext1 { get; set; }
        public string Ext2 { get; set; }
    }
    #endregion

    #region 住院患者信息查询
    public class SearchInpatientParam
    {
        public string Hospid { get; set; }
        public string Inpcode { get; set; }
        public string Pid { get; set; }
    }

    public class SearchInpatientOutput
    {
        [JsonProperty("code")]
        public int Code { get; set; }
        [JsonProperty("msg")]
        public string Msg { get; set; }
        [JsonProperty("pid")]
        public string Pid { get; set; }
        [JsonProperty("hospid")]
        public string Hospid { get; set; }
        [JsonProperty("hospname")]
        public string Hospname { get; set; }
        [JsonProperty("deptid")]
        public string Deptid { get; set; }
        [JsonProperty("deptname")]
        public string Deptname { get; set; }
        [JsonProperty("doctid")]
        public string Doctid { get; set; }
        [JsonProperty("docname")]
        public string Docname { get; set; }
        [JsonProperty("gender")]
        public int Gender { get; set; }
        [JsonProperty("age")]
        public int Age { get; set; }
        public string PatientName { get; set; }
        public string InPcode { get; set; }
        [JsonProperty("bednum")]
        public string Bednum { get; set; }
        [JsonProperty("identityCard")]
        public string IdentityCard { get; set; }
        [JsonProperty("cpatientcode")]
        public string Cpatientcode { get; set; }
        [JsonProperty("state")]
        public string State { get; set; }
        [JsonProperty("tel")]
        public string Tel { get; set; }
        [JsonProperty("times")]
        public string Times { get; set; }
        [JsonProperty("admissionDate")]
        public string AdmissionDate { get; set; }
        public string PayMoney { get; set; }
        [JsonProperty("totalfare")]
        public string Totalfare { get; set; }
        [JsonProperty("leftmoney")]
        public string Leftmoney { get; set; }
    }
    #endregion

    #region 住院押金支付
    public class PayDepositParam
    {
        public string Inpcode { get; set; }
        public string Cpatientcode { get; set; }
        public string PayMoney { get; set; }
        public string Userid { get; set; }
        public string Payway { get; set; }
        public string Cinterpayfromtype { get; set; }
        public string Orderno { get; set; }
    }

    public class PayDepositOutput
    {
        public int Code { get; set; }
        public string Msg { get; set; }
        public string Cflowcode { get; set; }
    }
    #endregion

    #region 住院押金退费查询
    public class SearchDepositFadeParam
    {
        public string Cflowcode { get; set; }
    }

    public class SearchDepositFadeOutput
    {
        public int Code { get; set; }
        public string Msg { get; set; }
        public string Autoid { get; set; }
        public string CflowCode { get; set; }
        public string NMoney { get; set; }
        public string Payway { get; set; }
        public string InPcode { get; set; }
        public string No_TreatList { get; set; }
        public string PatName { get; set; }
        public string TradeDate { get; set; }
    }
    #endregion

    #region 住院押金退费置标识
    public class FlaghDepositFadeParam
    {
        public string CFlowCode { get; set; }
    }

    public class FlaghDepositFadeOutput
    {
        public int Code { get; set; }
        public string Msg { get; set; }
    }
    #endregion

    #region 检验报告更新

    #endregion

    #endregion
}
