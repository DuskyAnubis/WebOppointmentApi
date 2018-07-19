using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebOppointmentApi.Models
{
    public class 工作人员
    {
        public int Id { get; set; }
        public string 代码 { get; set; }
        public string 姓名 { get; set; }
        public string 密码 { get; set; }
        public short? 权限 { get; set; }
        public string 科室 { get; set; }
        public string 性别 { get; set; }
        public int 流水号 { get; set; }
        public bool 药库运行权限 { get; set; }
        public bool 门诊药房运行权限 { get; set; }
        public bool 划价运行权限 { get; set; }
        public bool 住院运行权限 { get; set; }
        public bool 病区运行权限 { get; set; }
        public bool 住院药房运行权限 { get; set; }
        public bool 新建权限 { get; set; }
        public bool 审核权限 { get; set; }
        public bool 系统设置权限 { get; set; }
        public bool 人员维护权限 { get; set; }
        public bool 药库维护权限 { get; set; }
        public bool 货源维护权限 { get; set; }
        public bool? 入库录入 { get; set; }
        public bool? 出库录入 { get; set; }
        public bool? 调价录入 { get; set; }
        public bool? 报损录入 { get; set; }
        public bool? 入库审核 { get; set; }
        public bool? 出库审核 { get; set; }
        public bool? 调价审核 { get; set; }
        public bool? 报损审核 { get; set; }
        public bool? 采购计划权限 { get; set; }
        public bool 社区运行权限 { get; set; }
        public bool 卫生材料运行权限 { get; set; }
        public bool 登陆标志 { get; set; }
        public string Ip { get; set; }
        public string 机器名 { get; set; }
        public DateTime 日期 { get; set; }
        public bool 院长查询运行权限 { get; set; }
        public int KsId { get; set; }
        public bool 挂号运行权限 { get; set; }
        public bool? 入网许可 { get; set; }
        public int DwId { get; set; }
    }
}
