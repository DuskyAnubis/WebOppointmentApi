using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebOppointmentApi.Common
{
    public static class ScanCodeTools
    {
        public static string GetOrderString(string hospitalCode, string typeCode, string billNum)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("B");
            sb.Append(hospitalCode);
            sb.Append(DateTime.Now.ToString("yyMMddhhmmss"));
            sb.Append(typeCode);
            sb.Append(billNum);

            return sb.ToString();
        }
    }
}
