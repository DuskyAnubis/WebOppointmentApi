using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebOppointmentApi.Models
{
    public class Zy病区床位
    {
        public int Id { get; set; }
        public string 病区名 { get; set; }
        public string 病室名 { get; set; }
        public int 床位号 { get; set; }
        public decimal 床位费 { get; set; }
        public decimal 冷暖费 { get; set; }
        public decimal 诊疗费 { get; set; }
        public int 病人编号 { get; set; }
        public bool 常规费用 { get; set; }
        public string 床位医保码 { get; set; }
        public string 冷暖医保码 { get; set; }
        public string 诊疗医保码 { get; set; }
        public string 护理名称 { get; set; }
        public decimal 护理标准 { get; set; }
        public string 护理医保码 { get; set; }
        public bool? 是否记床位 { get; set; }
        public bool? 是否记冷暖 { get; set; }
        public bool? 是否记诊疗 { get; set; }
        public bool? 是否记护理 { get; set; }
        public string 床位医保分类 { get; set; }
        public string 冷暖医保分类 { get; set; }
        public string 诊疗医保分类 { get; set; }
        public string 护理医保分类 { get; set; }
        public string 床位代码 { get; set; }
        public string 冷暖代码 { get; set; }
        public string 诊疗代码 { get; set; }
        public string 床位名称 { get; set; }
        public string 冷暖名称 { get; set; }
        public string 诊疗名称 { get; set; }
        public string 床位接口码1 { get; set; }
        public string 冷暖接口码1 { get; set; }
        public string 诊疗接口码1 { get; set; }
        public string 护理接口码1 { get; set; }
        public string 床位接口码2 { get; set; }
        public string 冷暖接口码2 { get; set; }
        public string 诊疗接口码2 { get; set; }
        public string 护理接口码2 { get; set; }
        public int 床位yfId { get; set; }
        public int 冷暖yfId { get; set; }
        public int 诊疗yfId { get; set; }
        public int 护理yfId { get; set; }
        public string 合疗分类 { get; set; }
        public decimal 合疗比例 { get; set; }
        public decimal 医保比例 { get; set; }
        public string 床位材质分类 { get; set; }
        public string 冷暖材质分类 { get; set; }
        public string 诊疗材质分类 { get; set; }
        public int DwId { get; set; }
        public int KsId { get; set; }
    }
}
