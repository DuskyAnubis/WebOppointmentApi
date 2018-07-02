using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebOppointmentApi.Dtos
{
    public class PaymentApiQuery
    {
        public string Head { get; set; }
        public string Body { get; set; }
    }

    public class PaymentApiResult
    {
        public string Head { get; set; }
        public string Body { get; set; }
    }

    public class PaymentApiHeader
    {
        public string Token { get; set; }
        public string Version { get; set; }
        public string Fromtype { get; set; }
        public string Sessionid { get; set; }
        public string Time { get; set; }
    }

    public class PaymentApiBody
    {
        public int Code { get; set; }
        public string Msg { get; set; }
        public string Result { get; set; }
    }

    #region 平台提供接口

    #endregion

    #region HIS提供接口

    #endregion
}
