using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebOppointmentApi.Models
{
    public class Zy预交金
    {
        public int 预交金编号 { get; set; }
        public int 病人编号 { get; set; }
        public DateTime 日期 { get; set; }
        public bool 类别 { get; set; }
        public decimal? 金额 { get; set; }
        public string 操作员 { get; set; }
        public string 备注 { get; set; }
        public bool 交班标志 { get; set; }
        public DateTime? 日结帐日期 { get; set; }
        public DateTime? 月结帐日期 { get; set; }
        public string 操作员科室 { get; set; }
        public int DwId { get; set; }
        public int CzyId { get; set; }
    }
}
