using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebOppointmentApi.Models
{
    public class Zy记帐流水帐
    {
        public int Id { get; set; }
        public int 病人编号 { get; set; }
        public DateTime 日期 { get; set; }
        public string 代码 { get; set; }
        public string 名称 { get; set; }
        public string 规格 { get; set; }
        public string 单位 { get; set; }
        public decimal 单价 { get; set; }
        public decimal 数量 { get; set; }
        public decimal 库存量 { get; set; }
        public short 中药付数 { get; set; }
        public string 材质分类 { get; set; }
        public string 物理分类 { get; set; }
        public decimal 实交金额 { get; set; }
        public decimal 应交金额 { get; set; }
        public string 收费科室 { get; set; }
        public string 操作员 { get; set; }
        public string 医师编码 { get; set; }
        public bool 特殊药品 { get; set; }
        public bool 门诊药房标志 { get; set; }
        public short 科室id { get; set; }
        public string 司药人 { get; set; }
        public DateTime? 发药日期 { get; set; }
        public int 发药标志 { get; set; }
        public string 备注 { get; set; }
        public bool? 实施标志 { get; set; }
        public string 用法 { get; set; }
        public string 用量 { get; set; }
        public string 执行频率 { get; set; }
        public int? 分组 { get; set; }
        public string 组别 { get; set; }
        public string 医嘱类别 { get; set; }
        public string 医保分类 { get; set; }
        public decimal 医保比例 { get; set; }
        public string 医保码 { get; set; }
        public decimal? YfId { get; set; }
        public string 批号 { get; set; }
        public string 接口码1 { get; set; }
        public int FId { get; set; }
        public decimal 合疗比例 { get; set; }
        public DateTime? 长天上传时间 { get; set; }
        public string 所在科室 { get; set; }
        public string 接口码2 { get; set; }
        public string 合疗分类 { get; set; }
        public DateTime? 上传日期 { get; set; }
        public decimal 政府采购价 { get; set; }
        public string 套餐名称 { get; set; }
        public int? LskId { get; set; }
        public int CzyId { get; set; }
        public int DwId { get; set; }
    }
}
