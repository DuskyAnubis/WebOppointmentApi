using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebOppointmentApi.Common
{
    public class OppointmentApiOptions
    {
        public string HospitalId { get; set; }
        public string HospitalName { get; set; }
        public string BaseUri1 { get; set; }
        public string BaseUri2 { get; set; }
        public string FromType { get; set; }
        public string Version { get; set; }
        public string SecretKey { get; set; }
    }
}
