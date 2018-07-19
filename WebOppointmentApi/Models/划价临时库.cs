using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebOppointmentApi.Models
{
    public class 划价临时库
    {
        public int Id { get; set; }
        public DateTime 日期 { get; set; }
        public int 发票流水号 { get; set; }
        public string 代码 { get; set; }
        public string 货号 { get; set; }
        public string 名称 { get; set; }
        public string 规格 { get; set; }
        public string 单位 { get; set; }
        public decimal 单价 { get; set; }
        public decimal 数量 { get; set; }
        public decimal 金额 { get; set; }
        public short 药品付数 { get; set; }
        public string 材质分类 { get; set; }
        public string 收费科室 { get; set; }
        public int 科室id { get; set; }
        public string 物理分类 { get; set; }
        public bool 特殊药品 { get; set; }
        public decimal 库存量 { get; set; }
        public DateTime? 日结帐日期 { get; set; }
        public DateTime? 月结帐日期 { get; set; }
        public short 发药标志 { get; set; }
        public DateTime? 发药日期 { get; set; }
        public string 操作员 { get; set; }
        public string 医保码 { get; set; }
        public decimal 医保比例 { get; set; }
        public decimal 医保金额 { get; set; }
        public string 医师 { get; set; }
        public string 划价号 { get; set; }
        public string 病人姓名 { get; set; }
        public string 用法 { get; set; }
        public string 用量 { get; set; }
        public string 使用频率 { get; set; }
        public string 性别 { get; set; }
        public string 年龄 { get; set; }
        public string 地址 { get; set; }
        public string 批号 { get; set; }
        public decimal YfId { get; set; }
        public string 疾病诊断 { get; set; }
        public string 接口码1 { get; set; }
        public string 接口码2 { get; set; }
        public string 合疗分类 { get; set; }
        public decimal? 政府采购价 { get; set; }
        public string 禁忌 { get; set; }
        public string 卡号 { get; set; }
        public int? 组别 { get; set; }
        public string 分组标识 { get; set; }
        public string 处方类别 { get; set; }
        public string 套餐名称 { get; set; }
        public string 农合卡号 { get; set; }
        public decimal 一付量 { get; set; }
        public int CzyId { get; set; }
        public int DwId { get; set; }
        public int YsId { get; set; }
    }
}
