using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using WebOppointmentApi.Data;
using WebOppointmentApi.Dtos;
using WebOppointmentApi.Filters;
using WebOppointmentApi.Models;
using WebOppointmentApi.Common;
using Newtonsoft.Json.Serialization;


namespace WebOppointmentApi.Controllers
{
    [Produces("application/json")]
    [Route("api/v1/Payments")]
    public class PaymentController : Controller
    {
        private readonly ApiContext dbContext;
        private readonly HisContext hisContext;
        private readonly IMapper mapper;
        private readonly OppointmentApiOptions apiOptions;

        public PaymentController(ApiContext dbContext, HisContext hisContext, IMapper mapper, OppointmentApiOptions apiOptions)
        {
            this.dbContext = dbContext;
            this.hisContext = hisContext;
            this.mapper = mapper;
            this.apiOptions = apiOptions;
        }

        //同步信息时，生成Head
        private OppointmentApiHeader GetOppointmentApiHeader()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            string timeSpan = Convert.ToInt64(ts.TotalSeconds).ToString();
            string token = Encrypt.Md5Encrypt(apiOptions.SecretKey + apiOptions.FromType + timeSpan);

            OppointmentApiHeader headerInput = new OppointmentApiHeader
            {
                Token = token,
                Version = apiOptions.Version,
                Fromtype = apiOptions.FromType,
                Sessionid = apiOptions.FromType + timeSpan,
                Time = timeSpan
            };

            return headerInput;
        }

        //HIS提供接口，Token验证
        private bool VaildToken(OppointmentApiHeader header)
        {
            bool b = false;
            string token = Encrypt.Md5Encrypt(apiOptions.SecretKey + header.Fromtype + header.Time);
            if (String.Equals(token, header.Token, StringComparison.CurrentCultureIgnoreCase))
            {
                b = true;
            }
            return b;
        }

        #region 平台提供接口

        #region 对账单接口
        /// <summary>
        /// 对账单接口
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost("DownPayBill")]
        [ProducesResponseType(typeof(OppointmentApiBody), 200)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(ValidationError), 422)]
        [ProducesResponseType(typeof(void), 500)]
        public async Task<IActionResult> DownPayBill([FromBody]DownPayBillParam param)
        {
            var header = GetOppointmentApiHeader();
            var billInput = new DownPayBillInput()
            {
                Hospid = apiOptions.HospitalId,
                Tradedate = param.TradeDate
            };

            string head = Encrypt.Base64Encode(JsonConvert.SerializeObject(header, Formatting.Indented, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            }));
            string body = Encrypt.UrlEncode(Encrypt.Base64Encode(JsonConvert.SerializeObject(billInput, Formatting.Indented, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            })));

            OppointmentApi api = new OppointmentApi();
            string strResult = await api.DoPostAsync(apiOptions.BaseUri2, "api/downpaybill", head, body);

            OppointmentApiResult result = JsonConvert.DeserializeObject<OppointmentApiResult>(strResult);
            OppointmentApiBody resultBody = JsonConvert.DeserializeObject<OppointmentApiBody>(Encrypt.Base64Decode(result.Body.Contains("%") ? Encrypt.UrlDecode(result.Body) : result.Body));

            return new ObjectResult(resultBody);
        }
        #endregion

        #region 预约支付成功短信接口
        /// <summary>
        /// 预约支付成功短信接口
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost("OrderSms")]
        [ProducesResponseType(typeof(OppointmentApiBody), 200)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(ValidationError), 422)]
        [ProducesResponseType(typeof(void), 500)]
        public async Task<IActionResult> OrderSms([FromBody]OrderSmsParam param)
        {
            var header = GetOppointmentApiHeader();
            var smsInput = new OrderSmsInput()
            {
                Hospid = apiOptions.HospitalId,
                Phone = param.Phone,
                Oid = param.Oid,
                Smscontent = param.SmsContent
            };

            string head = Encrypt.Base64Encode(JsonConvert.SerializeObject(header, Formatting.Indented, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            }));
            string body = Encrypt.UrlEncode(Encrypt.Base64Encode(JsonConvert.SerializeObject(smsInput, Formatting.Indented, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            })));

            OppointmentApi api = new OppointmentApi();
            string strResult = await api.DoPostAsync(apiOptions.BaseUri2, "orderinfo/sms", head, body);

            OppointmentApiResult result = JsonConvert.DeserializeObject<OppointmentApiResult>(strResult);
            OppointmentApiBody resultBody = JsonConvert.DeserializeObject<OppointmentApiBody>(Encrypt.Base64Decode(result.Body.Contains("%") ? Encrypt.UrlDecode(result.Body) : result.Body));

            return new ObjectResult(resultBody);
        }
        #endregion

        #endregion

        #region HIS提供接口

        #endregion
    }
}