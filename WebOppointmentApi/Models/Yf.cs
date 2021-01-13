using System;
using System.Collections.Generic;

namespace WebOppointmentApi.Models
{
    public partial class Yf
    {
        public int Id { get; set; }
        public string 代码 { get; set; }
        public string 拼音码 { get; set; }
        public string 货号 { get; set; }
        public string 名称 { get; set; }
        public string 规格 { get; set; }
        public string 单位 { get; set; }
        public decimal 单价 { get; set; }
        public float 库存量 { get; set; }
        public decimal 进价 { get; set; }
        public string 包装单位 { get; set; }
        public int 散装数 { get; set; }
        public string 产地 { get; set; }
        public short 库存下限 { get; set; }
        public string 材质分类 { get; set; }
        public string 类别 { get; set; }
        public string 物理分类 { get; set; }
        public string 批号 { get; set; }
        public DateTime? 失效期 { get; set; }
        public float 药品极限 { get; set; }
        public decimal 批发价 { get; set; }
        public double 药房库存 { get; set; }
        public string 存放位置 { get; set; }
        public string 收费科室 { get; set; }
        public bool? 特殊药品 { get; set; }
        public int 科室id { get; set; }
        public string 医保分类 { get; set; }
        public float 医保比例 { get; set; }
        public string 医保码 { get; set; }
        public string 药理作用 { get; set; }
        public bool? 锁定 { get; set; }
        public string 接口码1 { get; set; }
        public int FId { get; set; }
        public decimal 合疗比例 { get; set; }
        public string 统计内容 { get; set; }
        public string 接口码2 { get; set; }
        public string 合疗分类 { get; set; }
        public decimal 政府采购价 { get; set; }
        public string 五笔码 { get; set; }
        public string 药品目录 { get; set; }
        public string 用法 { get; set; }
        public string 一次用量 { get; set; }
        public string 执行频率 { get; set; }
        public string 统计内容1 { get; set; }
        public int 抗生素级别 { get; set; }
        public int YkId { get; set; }
        public int YyJb { get; set; }
        public int DwId { get; set; }
        public bool Jzfp { get; set; }
        public bool? 计价 { get; set; }
    }
}
