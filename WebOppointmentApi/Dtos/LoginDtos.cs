using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebOppointmentApi.Dtos
{
    public class LoginInputDto
    {
        public string UserCode { get; set; }
        public string PassWord { get; set; }
    }
}
