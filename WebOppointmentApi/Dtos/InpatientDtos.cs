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

    #endregion
}
