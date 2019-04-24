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
        private readonly GhContext ghContext;
        private readonly HisContext hisContext;
        private readonly IMapper mapper;
        private readonly OppointmentApiOptions apiOptions;

        public PaymentController(ApiContext dbContext, GhContext ghContext, HisContext hisContext, IMapper mapper, OppointmentApiOptions apiOptions)
        {
            this.dbContext = dbContext;
            this.ghContext = ghContext;
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
            string strResult = await api.DoPostAsync(apiOptions.BaseUri3, "api/downpaybill", head, body);

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

        #region 开单短信通知
        /// <summary>
        /// 开单短信通知
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost("BillingSms")]
        [ProducesResponseType(typeof(OppointmentApiBody), 200)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(ValidationError), 422)]
        [ProducesResponseType(typeof(void), 500)]
        public async Task<IActionResult> BillingSms([FromBody]BillingSmsParam param)
        {
            var header = GetOppointmentApiHeader();
            BillingSmsInput smsInput;

            划价临时库 order = await hisContext.划价临时库.FirstOrDefaultAsync(o => o.划价号.Equals(param.BillNum));
            if (order == null)
            {
                return NotFound(Json(new { Error = "发送短信失败，划价信息不存在!" }));
            }

            if (order.卡号 == null || order.卡号 == "")
            {
                return NotFound(Json(new { Error = "发送短信失败，未查询到病人卡号!" }));
            }
            var doctor = await hisContext.医师代码.FirstOrDefaultAsync(d => d.医师姓名.Equals(order.医师));
            var registered = await ghContext.门诊挂号.FirstOrDefaultAsync(r => r.卡号.Equals(order.卡号));
            if (registered == null)
            {
                var registeredLs = await ghContext.门诊挂号流水帐.FirstOrDefaultAsync(r => r.卡号.Equals(order.卡号));
                if (registeredLs == null)
                {
                    return NotFound(Json(new { Error = "发送短信失败，未查询到病人信息!" }));
                }
                else
                {
                    if (registeredLs.电话 == null || registeredLs.电话 == "")
                    {
                        return NotFound(Json(new { Error = "发送短信失败，未查询到病人电话!" }));
                    }
                    else
                    {
                        smsInput = new BillingSmsInput
                        {
                            Hospid = apiOptions.HospitalId,
                            Hospname = apiOptions.HospitalName,
                            Deptname = doctor.所在科室,
                            Doctorname = doctor.医师姓名,
                            Time = order.日期.ToString("yyyy-MM-dd hh:mm:ss"),
                            Pname = order.病人姓名,
                            Phone = registeredLs.电话,
                            Cateid = order.CateId,
                            Ccheckcode = param.BillNum,
                            Leadplace = "请去缴费处缴费",
                            Userid = order.CzyId.ToString()
                        };
                    }
                }
            }
            else
            {
                if (registered.电话 == null || registered.电话 == "")
                {
                    return NotFound(Json(new { Error = "发送短信失败，未查询到病人电话!" }));
                }
                else
                {
                    smsInput = new BillingSmsInput
                    {
                        Hospid = apiOptions.HospitalId,
                        Hospname = apiOptions.HospitalName,
                        Deptname = doctor.所在科室,
                        Doctorname = doctor.医师姓名,
                        Time = order.日期.ToString("yyyy-MM-dd hh:mm:ss"),
                        Pname = order.病人姓名,
                        Phone = registered.电话,
                        Cateid = order.CateId,
                        Ccheckcode = param.BillNum,
                        Leadplace = "请去缴费处缴费",
                        Userid = order.CzyId.ToString()
                    };
                }
            }

            string head = Encrypt.Base64Encode(JsonConvert.SerializeObject(header, Formatting.Indented, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            }));
            string body = Encrypt.UrlEncode(Encrypt.Base64Encode(JsonConvert.SerializeObject(smsInput, Formatting.Indented, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            })));

            OppointmentApi api = new OppointmentApi();
            string strResult = await api.DoPostAsync(apiOptions.BaseUri2, "billing/sms", head, body);

            OppointmentApiResult result = JsonConvert.DeserializeObject<OppointmentApiResult>(strResult);
            OppointmentApiBody resultBody = JsonConvert.DeserializeObject<OppointmentApiBody>(Encrypt.Base64Decode(result.Body.Contains("%") ? Encrypt.UrlDecode(result.Body) : result.Body));

            return new ObjectResult(resultBody);
        }
        #endregion

        #region 推送待缴费信息
        /// <summary>
        /// 推送待缴费信息
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost("SendPendingPayment")]
        [ProducesResponseType(typeof(OppointmentApiBody), 200)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(ValidationError), 422)]
        [ProducesResponseType(typeof(void), 500)]
        public async Task<IActionResult> SendPendingPayment([FromBody]SendPendingPaymentParam param)
        {
            var header = GetOppointmentApiHeader();
            SendPendingPaymentinput paymentInput;

            List<划价临时库> orders = await hisContext.划价临时库.Where(o => o.划价号.Equals(param.BillNum)).ToListAsync();
            if (orders == null || orders.Count == 0)
            {
                return NotFound(Json(new { Error = "推送失败，划价信息不存在!" }));
            }
            if (orders[0].卡号 == null || orders[0].卡号 == "")
            {
                return NotFound(Json(new { Error = "推送失败，未查询到病人卡号!" }));
            }

            var registereds = await dbContext.Registereds.FirstOrDefaultAsync(r => r.Name.Equals(orders[0].病人姓名) && r.DoctorDate == Convert.ToDateTime(orders[0].日期.ToString("yyyy-MM-dd")));
            if (registereds == null)
            {
                return NotFound(Json(new { Error = "推送失败，该病人未在平台预约!" }));
            }

            var doctor = await hisContext.医师代码.FirstOrDefaultAsync(d => d.医师姓名.Equals(orders[0].医师));
            decimal totalPrice = await hisContext.划价临时库.Where(o => o.划价号.Equals(param.BillNum)).SumAsync(o => o.金额);
            var details = mapper.Map<List<PendingPaymentDetails>>(orders);
            paymentInput = new SendPendingPaymentinput
            {
                Hospid = apiOptions.HospitalId,
                Oid = registereds.OrderId,
                Name = orders[0].病人姓名,
                Gender = orders[0].性别,
                Cpatientcode = orders[0].卡号,
                Clinicno = "",
                state = 1,
                Dname = doctor.所在科室,
                Docname = doctor.医师姓名,
                Time = orders[0].日期.ToString("yyyy-MM-dd hh:mm:ss"),
                Cateid = orders[0].CateId,
                Title = orders[0].CateName,
                Price = totalPrice.ToString("0.00"),
                Type = "1",
                Orderid = param.BillNum,
                Detail = details
            };

            string head = Encrypt.Base64Encode(JsonConvert.SerializeObject(header, Formatting.Indented, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            }));
            string body = Encrypt.UrlEncode(Encrypt.Base64Encode(JsonConvert.SerializeObject(paymentInput, Formatting.Indented, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            })));

            OppointmentApi api = new OppointmentApi();
            string strResult = await api.DoPostAsync(apiOptions.BaseUri2, "payorder/get", head, body);

            OppointmentApiResult result = JsonConvert.DeserializeObject<OppointmentApiResult>(strResult);
            OppointmentApiBody resultBody = JsonConvert.DeserializeObject<OppointmentApiBody>(Encrypt.Base64Decode(result.Body.Contains("%") ? Encrypt.UrlDecode(result.Body) : result.Body));

            return new ObjectResult(resultBody);
        }
        #endregion

        #region 面对面扫码付款
        /// <summary>
        /// 面对面扫码付款
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost("ScanCodePay")]
        [ProducesResponseType(typeof(OppointmentApiBody), 200)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(ValidationError), 422)]
        [ProducesResponseType(typeof(void), 500)]
        public async Task<IActionResult> ScanCodePay([FromBody]ScanCodePayParam param)
        {
            var header = GetOppointmentApiHeader();
            ScanCodePayInput scanCodePayInput;

            List<划价临时库> orders = await hisContext.划价临时库.Where(o => o.划价号.Equals(param.BillNum)).ToListAsync();
            if (orders == null || orders.Count == 0)
            {
                return NotFound(Json(new { Error = "付款失败，划价信息不存在!" }));
            }

            var doctor = await hisContext.医师代码.FirstOrDefaultAsync(d => d.医师姓名.Equals(orders[0].医师));
            decimal totalPrice = await hisContext.划价临时库.Where(o => o.划价号.Equals(param.BillNum)).SumAsync(o => o.金额);

            ScanCodePayDetail detail = new ScanCodePayDetail
            {
                Orderid = ScanCodeTools.GetOrderString(apiOptions.PaymentId, "1" + orders[0].CateId, param.BillNum),
                Price = totalPrice.ToString("0.00"),
                Cateid = orders[0].CateId,
                Name = orders[0].病人姓名,
                Gender = orders[0].性别,
                State = "1",
                Dname = doctor.所在科室,
                Docname = doctor.医师姓名,
                Time = orders[0].日期.ToString("yyyy-MM-dd hh:mm:ss"),
                Title = orders[0].名称 + "等"
            };

            List<ScanCodePayDetail> details = new List<ScanCodePayDetail>
            {
                detail
            };

            scanCodePayInput = new ScanCodePayInput
            {
                Scantext = param.ScanCode,
                Userid = apiOptions.UserId,
                Hospid = apiOptions.HospitalId,
                Totaloid = ScanCodeTools.GetOrderString(apiOptions.PaymentId, "18", param.BillNum),
                Totalprice = totalPrice.ToString("0.00"),
                Cateid = orders[0].CateId,
                Totaltitle = orders[0].CateName + "费用",
                State = "1",
                Orderids = details
            };

            foreach (var order in orders)
            {
                order.ParentOrderCode = scanCodePayInput.Totaloid;
                order.OrderCode = detail.Orderid;
                hisContext.Update(order);
            }
            hisContext.SaveChanges();

            string head = Encrypt.Base64Encode(JsonConvert.SerializeObject(header, Formatting.Indented, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            }));
            string body = Encrypt.UrlEncode(Encrypt.Base64Encode(JsonConvert.SerializeObject(scanCodePayInput, Formatting.Indented, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            })));

            OppointmentApi api = new OppointmentApi();
            string strResult = await api.DoPostAsync(apiOptions.BaseUri2, "pay/scanpay", head, body);

            OppointmentApiResult result = JsonConvert.DeserializeObject<OppointmentApiResult>(strResult);
            OppointmentApiBody resultBody = JsonConvert.DeserializeObject<OppointmentApiBody>(Encrypt.Base64Decode(result.Body.Contains("%") ? Encrypt.UrlDecode(result.Body) : result.Body));

            return new ObjectResult(resultBody);
        }
        #endregion

        #region 面对面扫码付款状态查询
        /// <summary>
        /// 面对面扫码付款状态查询
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost("ScanCodeQuery")]
        [ProducesResponseType(typeof(OppointmentApiBody), 200)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(ValidationError), 422)]
        [ProducesResponseType(typeof(void), 500)]
        public async Task<IActionResult> ScanCodeQuery([FromBody]ScanCodeQueryParam param)
        {
            var header = GetOppointmentApiHeader();
            ScanCodeQueryInput scanCodeQueryInput;

            List<划价临时库> orders = await hisContext.划价临时库.Where(o => o.划价号.Equals(param.BillNum)).ToListAsync();
            if (orders == null || orders.Count == 0)
            {
                return NotFound(Json(new { Error = "查询失败，划价信息不存在!" }));
            }

            scanCodeQueryInput = new ScanCodeQueryInput
            {
                Userid = apiOptions.UserId,
                Hospid = apiOptions.HospitalId,
                Orderid = orders[0].ParentOrderCode
            };

            string head = Encrypt.Base64Encode(JsonConvert.SerializeObject(header, Formatting.Indented, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            }));
            string body = Encrypt.UrlEncode(Encrypt.Base64Encode(JsonConvert.SerializeObject(scanCodeQueryInput, Formatting.Indented, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            })));

            OppointmentApi api = new OppointmentApi();
            string strResult = await api.DoPostAsync(apiOptions.BaseUri2, "pay/orderquery", head, body);

            OppointmentApiResult result = JsonConvert.DeserializeObject<OppointmentApiResult>(strResult);
            OppointmentApiBody resultBody = JsonConvert.DeserializeObject<OppointmentApiBody>(Encrypt.Base64Decode(result.Body.Contains("%") ? Encrypt.UrlDecode(result.Body) : result.Body));

            return new ObjectResult(resultBody);
        }
        #endregion

        #region 面对面扫码付款完成
        /// <summary>
        /// 面对面扫码付款完成
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost("ScanCodeComplete")]
        [ProducesResponseType(typeof(OppointmentApiBody), 200)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(ValidationError), 422)]
        [ProducesResponseType(typeof(void), 500)]
        public async Task<IActionResult> ScanCodeComplete([FromBody]ScanCodeCompleteParam param)
        {
            var header = GetOppointmentApiHeader();
            ScanCodeCompleteInput scanCodeCompleteInput;

            List<划价临时库> orders = await hisContext.划价临时库.Where(o => o.划价号.Equals(param.BillNum)).ToListAsync();
            if (orders == null || orders.Count == 0)
            {
                return NotFound(Json(new { Error = "付款完成失败，划价信息不存在!" }));
            }

            门诊收费 payment = await hisContext.门诊收费.FirstOrDefaultAsync(p => p.收费id == orders[0].发票流水号);
            if (payment == null)
            {
                return NotFound(Json(new { Error = "付款完成失败，该划价单未收费!" }));
            }

            decimal totalPrice = await hisContext.划价临时库.Where(o => o.划价号.Equals(param.BillNum)).SumAsync(o => o.金额);

            ScanCodeCompleteDetail detail = new ScanCodeCompleteDetail
            {
                Ouno = payment.PlatformCode,
                Price = totalPrice.ToString("0.00"),
                Cflowcode = orders[0].发票流水号.ToString()
            };

            List<ScanCodeCompleteDetail> details = new List<ScanCodeCompleteDetail>
            {
                detail
            };

            scanCodeCompleteInput = new ScanCodeCompleteInput
            {
                Touno = payment.ParentPlatformCode,
                Userid = apiOptions.UserId,
                Code = "1",
                Totalprice = totalPrice.ToString("0.00"),
                Tcflowcode = orders[0].发票流水号.ToString(),
                Ounos = details
            };

            string head = Encrypt.Base64Encode(JsonConvert.SerializeObject(header, Formatting.Indented, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            }));
            string body = Encrypt.UrlEncode(Encrypt.Base64Encode(JsonConvert.SerializeObject(scanCodeCompleteInput, Formatting.Indented, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            })));

            OppointmentApi api = new OppointmentApi();
            string strResult = await api.DoPostAsync(apiOptions.BaseUri2, "pay/hispaynotice", head, body);

            OppointmentApiResult result = JsonConvert.DeserializeObject<OppointmentApiResult>(strResult);
            OppointmentApiBody resultBody = JsonConvert.DeserializeObject<OppointmentApiBody>(Encrypt.Base64Decode(result.Body.Contains("%") ? Encrypt.UrlDecode(result.Body) : result.Body));

            return new ObjectResult(resultBody);
        }
        #endregion

        #region 面对面扫码付款取消
        /// <summary>
        /// 面对面扫码付款取消
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost("ScanCodeCancel")]
        [ProducesResponseType(typeof(OppointmentApiBody), 200)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(ValidationError), 422)]
        [ProducesResponseType(typeof(void), 500)]
        public async Task<IActionResult> ScanCodeCancel([FromBody]ScanCodeCancelParam param)
        {
            var header = GetOppointmentApiHeader();
            ScanCodeCancelInput scanCodeCancelInput;

            List<划价临时库> orders = await hisContext.划价临时库.Where(o => o.划价号.Equals(param.BillNum)).ToListAsync();
            if (orders == null || orders.Count == 0)
            {
                return NotFound(Json(new { Error = "付款取消失败，划价信息不存在!" }));
            }

            scanCodeCancelInput = new ScanCodeCancelInput
            {
                Userid = apiOptions.UserId,
                Hospid = apiOptions.HospitalId,
                Orderid = orders[0].ParentOrderCode
            };

            string head = Encrypt.Base64Encode(JsonConvert.SerializeObject(header, Formatting.Indented, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            }));
            string body = Encrypt.UrlEncode(Encrypt.Base64Encode(JsonConvert.SerializeObject(scanCodeCancelInput, Formatting.Indented, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            })));

            OppointmentApi api = new OppointmentApi();
            string strResult = await api.DoPostAsync(apiOptions.BaseUri2, "pay/ordercancel", head, body);

            OppointmentApiResult result = JsonConvert.DeserializeObject<OppointmentApiResult>(strResult);
            OppointmentApiBody resultBody = JsonConvert.DeserializeObject<OppointmentApiBody>(Encrypt.Base64Decode(result.Body.Contains("%") ? Encrypt.UrlDecode(result.Body) : result.Body));

            return new ObjectResult(resultBody);
        }
        #endregion

        #endregion

        #region HIS提供接口

        #region 查询待缴费项
        /// <summary>
        /// 查询待缴费项
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpPost("/api/search/notpay")]
        [ProducesResponseType(typeof(SearchPendingPaymentOutput), 200)]
        [ProducesResponseType(typeof(void), 500)]
        public async Task<IActionResult> SearchPendingPayment([FromForm]OppointmentApiQuery query)
        {
            OppointmentApiHeader header = JsonConvert.DeserializeObject<OppointmentApiHeader>(Encrypt.Base64Decode(query.Head));
            SearchPendingPaymentParam param = JsonConvert.DeserializeObject<SearchPendingPaymentParam>(Encrypt.Base64Decode(Encrypt.UrlDecode(query.Body)), new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

            if (!VaildToken(header))
            {
                return new ObjectResult("Token验证失败，请检查身份验证信息!");
            }

            if (param.Indextype.Equals("0"))
            {
                List<划价临时库> orders = await hisContext.划价临时库.Where(o => o.划价号.Equals(param.Orderid) && o.CateId.Equals(param.Cateid) && o.发票流水号 == 0).ToListAsync();
                if (orders == null || orders.Count == 0)
                {
                    var searchPendingPaymentOutPut = new SearchPendingPaymentOutput
                    {
                        Code = 0,
                        Msg = $"未能查询到院内订单Id为{param.Orderid}的待缴费信息!",
                        Items = null
                    };

                    return new ObjectResult(new
                    {
                        head = Encrypt.Base64Encode(JsonConvert.SerializeObject(header, Formatting.Indented, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() })),
                        body = Encrypt.UrlEncode(Encrypt.Base64Encode(JsonConvert.SerializeObject(searchPendingPaymentOutPut, Formatting.Indented, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() })))
                    });
                }
                else
                {
                    var doctor = await hisContext.医师代码.FirstOrDefaultAsync(d => d.医师姓名.Equals(orders[0].医师));
                    decimal totalPrice = await hisContext.划价临时库.Where(o => o.划价号.Equals(param.Orderid) && o.发票流水号 == 0).SumAsync(o => o.金额);
                    var details = mapper.Map<List<PendingPaymentDetails>>(orders);
                    PendingPayment payment = new PendingPayment
                    {
                        Name = orders[0].病人姓名,
                        Gender = orders[0].性别,
                        State = orders[0].发票流水号 == 0 ? 1 : 3,
                        Dname = doctor.所在科室,
                        Docname = doctor.医师姓名,
                        Time = orders[0].日期.ToString("yyyy-MM-dd hh:mm:ss"),
                        Cateid = orders[0].CateId,
                        Title = orders[0].CateName,
                        Price = totalPrice.ToString("0.00"),
                        Orderid = orders[0].划价号,
                        Detail = details
                    };
                    List<PendingPayment> payments = new List<PendingPayment>
                    {
                        payment
                    };

                    var searchPendingPaymentOutPut = new SearchPendingPaymentOutput
                    {
                        Code = 1,
                        Msg = "查询待缴费信息成功!",
                        Items = payments
                    };
                    return new ObjectResult(new
                    {
                        head = Encrypt.Base64Encode(JsonConvert.SerializeObject(header, Formatting.Indented, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() })),
                        body = Encrypt.UrlEncode(Encrypt.Base64Encode(JsonConvert.SerializeObject(searchPendingPaymentOutPut, Formatting.Indented, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() })))
                    });
                }
            }
            else
            {
                List<string> orderIds = await hisContext.划价临时库.Where(o => o.卡号.Equals(param.Cpatientcode) && o.发票流水号 == 0).Select(o => o.划价号).Distinct().ToListAsync();
                if (orderIds == null || orderIds.Count == 0)
                {
                    var searchPendingPaymentOutPut = new SearchPendingPaymentOutput
                    {
                        Code = 0,
                        Msg = $"未能查询到诊疗Id为{param.Cpatientcode}的待缴费信息!",
                        Items = null
                    };

                    return new ObjectResult(new
                    {
                        head = Encrypt.Base64Encode(JsonConvert.SerializeObject(header, Formatting.Indented, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() })),
                        body = Encrypt.UrlEncode(Encrypt.Base64Encode(JsonConvert.SerializeObject(searchPendingPaymentOutPut, Formatting.Indented, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() })))
                    });
                }
                else
                {
                    List<PendingPayment> payments = new List<PendingPayment>();
                    foreach (var item in orderIds)
                    {
                        List<划价临时库> orders = await hisContext.划价临时库.Where(o => o.划价号.Equals(item) && o.发票流水号 == 0).ToListAsync();
                        var doctor = await hisContext.医师代码.FirstOrDefaultAsync(d => d.医师姓名.Equals(orders[0].医师));
                        decimal totalPrice = await hisContext.划价临时库.Where(o => o.划价号.Equals(item) && o.发票流水号 == 0).SumAsync(o => o.金额);
                        var details = mapper.Map<List<PendingPaymentDetails>>(orders);
                        PendingPayment payment = new PendingPayment
                        {
                            Name = orders[0].病人姓名,
                            Gender = orders[0].性别,
                            State = orders[0].发票流水号 == 0 ? 1 : 3,
                            Dname = doctor.所在科室,
                            Docname = doctor.医师姓名,
                            Time = orders[0].日期.ToString("yyyy-MM-dd hh:mm:ss"),
                            Cateid = orders[0].CateId,
                            Title = orders[0].CateName,
                            Price = totalPrice.ToString("0.00"),
                            Orderid = orders[0].划价号,
                            Detail = details
                        };
                        payments.Add(payment);
                    }

                    var searchPendingPaymentOutPut = new SearchPendingPaymentOutput
                    {
                        Code = 1,
                        Msg = "查询待缴费信息成功!",
                        Items = payments
                    };
                    return new ObjectResult(new
                    {
                        head = Encrypt.Base64Encode(JsonConvert.SerializeObject(header, Formatting.Indented, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() })),
                        body = Encrypt.UrlEncode(Encrypt.Base64Encode(JsonConvert.SerializeObject(searchPendingPaymentOutPut, Formatting.Indented, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() })))
                    });
                }
            }
        }
        #endregion

        #region 订单支付
        /// <summary>
        /// 订单支付
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpPost("/api/pay/result")]
        [ProducesResponseType(typeof(PayOrderOutput), 200)]
        [ProducesResponseType(typeof(void), 500)]
        public async Task<IActionResult> PayOrder([FromForm]OppointmentApiQuery query)
        {
            OppointmentApiHeader header = JsonConvert.DeserializeObject<OppointmentApiHeader>(Encrypt.Base64Decode(query.Head));
            PayOrderParam param = JsonConvert.DeserializeObject<PayOrderParam>(Encrypt.Base64Decode(Encrypt.UrlDecode(query.Body)), new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

            if (!VaildToken(header))
            {
                return new ObjectResult("Token验证失败，请检查身份验证信息!");
            }

            string IdCard = "";
            string OutpatientNum = "";
            List<划价临时库> orders;

            if (param.Indextype.Equals("0"))
            {
                orders = await hisContext.划价临时库.Where(o => o.划价号.Equals(param.Orderid) && o.CateId.Equals(param.Cateid) && o.发票流水号 == 0).ToListAsync();
                if (orders == null || orders.Count == 0)
                {
                    var payOrderOutput = new PayOrderOutput
                    {
                        Code = 0,
                        Msg = "支付失败!未查到该病人订单或已支付!"
                    };

                    return new ObjectResult(new
                    {
                        head = Encrypt.Base64Encode(JsonConvert.SerializeObject(header, Formatting.Indented, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() })),
                        body = Encrypt.UrlEncode(Encrypt.Base64Encode(JsonConvert.SerializeObject(payOrderOutput, Formatting.Indented, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() })))
                    });
                }
                else
                {
                    var doctor = await hisContext.医师代码.FirstOrDefaultAsync(d => d.医师姓名.Equals(orders[0].医师));
                    decimal totalPrice = await hisContext.划价临时库.Where(o => o.划价号.Equals(param.Orderid) && o.CateId.Equals(param.Cateid) && o.发票流水号 == 0).SumAsync(o => o.金额);
                    var user = await hisContext.工作人员.FirstOrDefaultAsync(u => u.代码.Equals(param.Userid));
                    var registered = await ghContext.门诊挂号.FirstOrDefaultAsync(r => r.卡号.Equals(orders[0].卡号));
                    if (registered == null)
                    {
                        var registeredLs = await ghContext.门诊挂号流水帐.FirstOrDefaultAsync(r => r.卡号.Equals(orders[0].卡号));
                        if (registeredLs != null)
                        {
                            if (!string.IsNullOrEmpty(registeredLs.身份证))
                            {
                                IdCard = registeredLs.身份证;
                            }
                        }
                    }
                    else
                    {
                        OutpatientNum = registered.门诊号.ToString();
                        if (!string.IsNullOrEmpty(registered.身份证))
                        {
                            IdCard = registered.身份证;
                        }
                    }

                    门诊收费 payment = new 门诊收费
                    {
                        日期 = DateTime.Now,
                        操作员 = user.姓名,
                        病人姓名 = orders[0].病人姓名,
                        卡号 = orders[0].卡号,
                        总金额 = totalPrice,
                        优惠额 = Convert.ToDecimal(0.00),
                        账户支付 = Convert.ToDecimal(0.00),
                        统筹支付 = Convert.ToDecimal(0.00),
                        补助金 = Convert.ToDecimal(0.00),
                        现金支付 = totalPrice,
                        交班标志 = false,
                        结帐日期 = null,
                        门诊号 = OutpatientNum,
                        发票号 = "",
                        PInfo = "",
                        退票 = null,
                        费别 = "自费",
                        折扣率 = Convert.ToDecimal(0.00),
                        单据流水号 = "",
                        医疗保险号 = "",
                        基金支付额 = Convert.ToDecimal(0.00),
                        帐户余额 = Convert.ToDecimal(0.00),
                        公补基金支付 = Convert.ToDecimal(0.00),
                        医保 = "",
                        性别 = orders[0].性别,
                        DwId = 1,
                        CzyId = user.Id,
                        PayMethod = param.Ls_cpscode,
                        PayFrom = "健康山西",
                        IsWindowRefund = false,
                        WindowRefundFlag = 0,
                        ParentPlatformCode = "",
                        PlatformCode = "",
                    };
                    hisContext.Add(payment);
                    hisContext.SaveChanges();
                    foreach (var order in orders)
                    {
                        order.发票流水号 = payment.收费id;
                        hisContext.Update(order);
                    }
                    hisContext.SaveChanges();

                    var payOrderOutput = new PayOrderOutput
                    {
                        Code = 1,
                        Msg = "订单支付成功!",
                        Cflowcode = payment.收费id.ToString(),
                        Cpatientcode = orders[0].卡号,
                        Cidentitycard = IdCard,
                        Cpatientname = orders[0].病人姓名,
                        Csex = orders[0].性别,
                        Ddiagnosetime = orders[0].日期.ToString("yyyy-MM-dd hh:mm:ss"),
                        Deptname = doctor.所在科室,
                        Doctorname = doctor.医师姓名,
                        Cdiagnosetypename = "",
                        Ndiagnosenum = OutpatientNum,
                        Chousesectionname = "",
                        Chousename = "",
                        Clocation = "",
                        Nmoney = totalPrice.ToString("0.00"),
                        Diagnoseid = OutpatientNum,
                        Windowmsg = "",
                        Windowname = ""
                    };

                    return new ObjectResult(new
                    {
                        head = Encrypt.Base64Encode(JsonConvert.SerializeObject(header, Formatting.Indented, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() })),
                        body = Encrypt.UrlEncode(Encrypt.Base64Encode(JsonConvert.SerializeObject(payOrderOutput, Formatting.Indented, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() })))
                    });
                }
            }
            else
            {
                orders = await hisContext.划价临时库.Where(o => o.卡号.Equals(param.Cpatientcode) && o.CateId.Equals(param.Cateid) && o.发票流水号 == 0).ToListAsync();
                if (orders == null || orders.Count == 0)
                {
                    var payOrderOutput = new PayOrderOutput
                    {
                        Code = 0,
                        Msg = "支付失败!未查到该病人订单或已支付!"
                    };

                    return new ObjectResult(new
                    {
                        head = Encrypt.Base64Encode(JsonConvert.SerializeObject(header, Formatting.Indented, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() })),
                        body = Encrypt.UrlEncode(Encrypt.Base64Encode(JsonConvert.SerializeObject(payOrderOutput, Formatting.Indented, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() })))
                    });
                }
                else
                {
                    var doctor = await hisContext.医师代码.FirstOrDefaultAsync(d => d.医师姓名.Equals(orders[0].医师));
                    decimal totalPrice = await hisContext.划价临时库.Where(o => o.卡号.Equals(param.Cpatientcode) && o.CateId.Equals(param.Cateid) && o.发票流水号 == 0).SumAsync(o => o.金额);
                    var user = await hisContext.工作人员.FirstOrDefaultAsync(u => u.代码.Equals(param.Userid));
                    var registered = await ghContext.门诊挂号.FirstOrDefaultAsync(r => r.卡号.Equals(orders[0].卡号));
                    if (registered == null)
                    {
                        var registeredLs = await ghContext.门诊挂号流水帐.FirstOrDefaultAsync(r => r.卡号.Equals(orders[0].卡号));
                        if (registeredLs != null)
                        {
                            if (!string.IsNullOrEmpty(registeredLs.身份证))
                            {
                                IdCard = registeredLs.身份证;
                            }
                        }
                    }
                    else
                    {
                        OutpatientNum = registered.门诊号.ToString();
                        if (!string.IsNullOrEmpty(registered.身份证))
                        {
                            IdCard = registered.身份证;
                        }
                    }

                    门诊收费 payment = new 门诊收费
                    {
                        日期 = DateTime.Now,
                        操作员 = user.姓名,
                        病人姓名 = orders[0].病人姓名,
                        卡号 = orders[0].卡号,
                        总金额 = totalPrice,
                        优惠额 = Convert.ToDecimal(0.00),
                        账户支付 = Convert.ToDecimal(0.00),
                        统筹支付 = Convert.ToDecimal(0.00),
                        补助金 = Convert.ToDecimal(0.00),
                        现金支付 = totalPrice,
                        交班标志 = false,
                        结帐日期 = null,
                        门诊号 = OutpatientNum,
                        发票号 = "",
                        PInfo = "",
                        退票 = null,
                        费别 = "自费",
                        折扣率 = Convert.ToDecimal(0.00),
                        单据流水号 = "",
                        医疗保险号 = "",
                        基金支付额 = Convert.ToDecimal(0.00),
                        帐户余额 = Convert.ToDecimal(0.00),
                        公补基金支付 = Convert.ToDecimal(0.00),
                        医保 = "",
                        性别 = orders[0].性别,
                        DwId = 1,
                        CzyId = user.Id,
                        PayMethod = param.Ls_cpscode,
                        PayFrom = "健康山西",
                        IsWindowRefund = false,
                        WindowRefundFlag = 0,
                        ParentPlatformCode = "",
                        PlatformCode = ""
                    };
                    hisContext.Add(payment);
                    hisContext.SaveChanges();
                    foreach (var order in orders)
                    {
                        order.发票流水号 = payment.收费id;
                        hisContext.Update(order);
                    }
                    hisContext.SaveChanges();

                    var payOrderOutput = new PayOrderOutput
                    {
                        Code = 1,
                        Msg = "订单支付成功!",
                        Cflowcode = payment.收费id.ToString(),
                        Cpatientcode = orders[0].卡号,
                        Cidentitycard = IdCard,
                        Cpatientname = orders[0].病人姓名,
                        Csex = orders[0].性别,
                        Ddiagnosetime = orders[0].日期.ToString("yyyy-MM-dd hh:mm:ss"),
                        Deptname = doctor.所在科室,
                        Doctorname = doctor.医师姓名,
                        Cdiagnosetypename = "",
                        Ndiagnosenum = OutpatientNum,
                        Chousesectionname = "",
                        Chousename = "",
                        Clocation = "",
                        Nmoney = totalPrice.ToString("0.00"),
                        Diagnoseid = OutpatientNum,
                        Windowmsg = "",
                        Windowname = ""
                    };

                    return new ObjectResult(new
                    {
                        head = Encrypt.Base64Encode(JsonConvert.SerializeObject(header, Formatting.Indented, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() })),
                        body = Encrypt.UrlEncode(Encrypt.Base64Encode(JsonConvert.SerializeObject(payOrderOutput, Formatting.Indented, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() })))
                    });
                }
            }
        }
        #endregion

        #region 退费
        /// <summary>
        /// 退费
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpPost("/api/register/fade")]
        [ProducesResponseType(typeof(RefundOrderOutput), 200)]
        [ProducesResponseType(typeof(void), 500)]
        public async Task<IActionResult> RefundOrder([FromForm]OppointmentApiQuery query)
        {
            OppointmentApiHeader header = JsonConvert.DeserializeObject<OppointmentApiHeader>(Encrypt.Base64Decode(query.Head));
            RefundOrderParam param = JsonConvert.DeserializeObject<RefundOrderParam>(Encrypt.Base64Decode(Encrypt.UrlDecode(query.Body)), new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

            if (!VaildToken(header))
            {
                return new ObjectResult("Token验证失败，请检查身份验证信息!");
            }

            int paymentCount = await hisContext.门诊收费.CountAsync(p => p.收费id == Convert.ToInt32(param.Cflowcode));
            int paymentSerialCount = await hisContext.门诊收费流水帐.CountAsync(p => p.收费id == Convert.ToInt32(param.Cflowcode));

            if (paymentCount == 0 && paymentSerialCount == 0)
            {
                var refundOrderOutput = new RefundOrderOutput
                {
                    Code = 0,
                    Msg = $"退费失败！未查询到流水号为{param.Cflowcode}的收费信息！"
                };

                return new ObjectResult(new
                {
                    head = Encrypt.Base64Encode(JsonConvert.SerializeObject(header, Formatting.Indented, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() })),
                    body = Encrypt.UrlEncode(Encrypt.Base64Encode(JsonConvert.SerializeObject(refundOrderOutput, Formatting.Indented, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() })))
                });
            }

            if (hisContext.门诊收费.Count(p => p.退票 == Convert.ToInt32(param.Cflowcode)) > 0 || (hisContext.门诊收费流水帐.Count(p => p.退票 == Convert.ToInt32(param.Cflowcode)) > 0))
            {
                var refundOrderOutput = new RefundOrderOutput
                {
                    Code = -2,
                    Msg = $"退费失败！流水号为{param.Cflowcode}的收费信息已退费！"
                };

                return new ObjectResult(new
                {
                    head = Encrypt.Base64Encode(JsonConvert.SerializeObject(header, Formatting.Indented, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() })),
                    body = Encrypt.UrlEncode(Encrypt.Base64Encode(JsonConvert.SerializeObject(refundOrderOutput, Formatting.Indented, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() })))
                });
            }

            if (hisContext.划价临时库.Count(p => p.发药标志 == 1 && p.发票流水号 == Convert.ToInt32(param.Cflowcode)) > 0 || (hisContext.划价流水帐.Count(p => p.发药标志 == 1 && p.发票流水号 == Convert.ToInt32(param.Cflowcode)) > 0))
            {
                var refundOrderOutput = new RefundOrderOutput
                {
                    Code = 0,
                    Msg = $"退费失败！流水号为{param.Cflowcode}的收费信息已发药，无法退费！"
                };

                return new ObjectResult(new
                {
                    head = Encrypt.Base64Encode(JsonConvert.SerializeObject(header, Formatting.Indented, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() })),
                    body = Encrypt.UrlEncode(Encrypt.Base64Encode(JsonConvert.SerializeObject(refundOrderOutput, Formatting.Indented, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() })))
                });
            }

            var user = await hisContext.工作人员.FirstOrDefaultAsync(u => u.代码.Equals(param.Userid));
            if (paymentCount > 0)
            {
                var payment = await hisContext.门诊收费.FirstOrDefaultAsync(p => p.收费id == Convert.ToInt32(param.Cflowcode));
                var orders = await hisContext.划价临时库.Where(o => o.发票流水号 == Convert.ToInt32(param.Cflowcode)).ToListAsync();

                var paymentRefund = new 门诊收费
                {
                    日期 = DateTime.Now,
                    操作员 = user.姓名,
                    病人姓名 = payment.病人姓名,
                    卡号 = payment.卡号,
                    总金额 = payment.总金额 * -1,
                    优惠额 = payment.优惠额 * -1,
                    账户支付 = payment.账户支付 * -1,
                    统筹支付 = payment.统筹支付 * -1,
                    补助金 = payment.补助金 * -1,
                    现金支付 = payment.现金支付 * -1,
                    交班标志 = false,
                    结帐日期 = null,
                    门诊号 = payment.门诊号,
                    发票号 = "",
                    PInfo = "",
                    退票 = payment.收费id,
                    费别 = "自费",
                    折扣率 = payment.折扣率,
                    单据流水号 = "",
                    医疗保险号 = "",
                    基金支付额 = payment.基金支付额 * -1,
                    帐户余额 = payment.帐户余额,
                    公补基金支付 = payment.公补基金支付 * -1,
                    医保 = "",
                    性别 = payment.性别,
                    DwId = 1,
                    CzyId = user.Id,
                    PayMethod = payment.PayMethod,
                    PayFrom = payment.PayFrom,
                    IsWindowRefund = false,
                    WindowRefundFlag = 0,
                    ParentPlatformCode = "",
                    PlatformCode = ""
                };
                hisContext.Add(paymentRefund);
                hisContext.SaveChanges();

                foreach (var order in orders)
                {
                    var orderRefund = new 划价临时库
                    {
                        日期 = DateTime.Now,
                        发票流水号 = paymentRefund.收费id,
                        代码 = order.代码,
                        货号 = order.货号,
                        名称 = order.名称,
                        规格 = order.规格,
                        单位 = order.单位,
                        单价 = order.单价,
                        数量 = order.数量 * -1,
                        金额 = order.金额 * -1,
                        药品付数 = order.药品付数,
                        材质分类 = order.材质分类,
                        收费科室 = order.收费科室,
                        科室id = order.科室id,
                        物理分类 = order.物理分类,
                        特殊药品 = order.特殊药品,
                        库存量 = order.库存量,
                        日结帐日期 = null,
                        月结帐日期 = null,
                        发药标志 = order.发药标志,
                        发药日期 = order.发药日期,
                        操作员 = user.姓名,
                        医保码 = order.医保码,
                        医保比例 = order.医保比例,
                        医保金额 = order.医保金额,
                        医师 = order.医师,
                        划价号 = order.划价号,
                        病人姓名 = order.病人姓名,
                        用法 = order.用法,
                        用量 = order.用量,
                        使用频率 = order.使用频率,
                        性别 = order.性别,
                        年龄 = order.年龄,
                        地址 = order.地址,
                        批号 = order.批号,
                        YfId = order.YfId,
                        疾病诊断 = order.疾病诊断,
                        接口码1 = order.接口码1,
                        接口码2 = order.接口码2,
                        合疗分类 = order.合疗分类,
                        政府采购价 = order.政府采购价,
                        禁忌 = order.禁忌,
                        卡号 = order.卡号,
                        组别 = order.组别,
                        分组标识 = order.分组标识,
                        处方类别 = order.处方类别,
                        套餐名称 = order.套餐名称,
                        农合卡号 = order.农合卡号,
                        一付量 = order.一付量,
                        CzyId = user.Id,
                        DwId = 1,
                        YsId = order.YsId,
                        CateId = order.CateId,
                        CateName = order.CateName,
                        ParentOrderCode = "",
                        OrderCode = "",
                    };
                    hisContext.Add(orderRefund);
                }
                hisContext.SaveChanges();

                var refundOrderOutput = new RefundOrderOutput
                {
                    Code = 1,
                    Msg = $"退费成功!"
                };

                return new ObjectResult(new
                {
                    head = Encrypt.Base64Encode(JsonConvert.SerializeObject(header, Formatting.Indented, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() })),
                    body = Encrypt.UrlEncode(Encrypt.Base64Encode(JsonConvert.SerializeObject(refundOrderOutput, Formatting.Indented, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() })))
                });
            }
            else
            {
                var payment = await hisContext.门诊收费流水帐.FirstOrDefaultAsync(o => o.收费id == Convert.ToInt32(param.Cflowcode));
                var orders = await hisContext.划价流水帐.Where(o => o.发票流水号 == Convert.ToInt32(param.Cflowcode)).ToListAsync();

                var paymentRefund = new 门诊收费
                {
                    日期 = DateTime.Now,
                    操作员 = user.姓名,
                    病人姓名 = payment.病人姓名,
                    卡号 = payment.卡号,
                    总金额 = payment.总金额 * -1,
                    优惠额 = payment.优惠额 * -1,
                    账户支付 = payment.账户支付 * -1,
                    统筹支付 = payment.统筹支付 * -1,
                    补助金 = payment.补助金 * -1,
                    现金支付 = payment.现金支付 * -1,
                    交班标志 = false,
                    结帐日期 = null,
                    门诊号 = payment.门诊号,
                    发票号 = "",
                    PInfo = "",
                    退票 = payment.收费id,
                    费别 = "自费",
                    折扣率 = payment.折扣率,
                    单据流水号 = "",
                    医疗保险号 = "",
                    基金支付额 = payment.基金支付额 * -1,
                    帐户余额 = payment.帐户余额,
                    公补基金支付 = payment.公补基金支付 * -1,
                    医保 = "",
                    性别 = payment.性别,
                    DwId = 1,
                    CzyId = user.Id,
                    PayMethod = payment.PayMethod,
                    PayFrom = payment.PayFrom,
                    IsWindowRefund = false,
                    WindowRefundFlag = 0,
                    ParentPlatformCode = "",
                    PlatformCode = ""
                };
                hisContext.Add(paymentRefund);
                hisContext.SaveChanges();

                foreach (var order in orders)
                {
                    var orderRefund = new 划价临时库
                    {
                        日期 = DateTime.Now,
                        发票流水号 = paymentRefund.收费id,
                        代码 = order.代码,
                        货号 = order.货号,
                        名称 = order.名称,
                        规格 = order.规格,
                        单位 = order.单位,
                        单价 = order.单价 == null ? Convert.ToDecimal(0.00) : Convert.ToDecimal(order.单价),
                        数量 = order.数量 == null ? Convert.ToDecimal(0.00) : Convert.ToDecimal(order.数量) * -1,
                        金额 = order.金额 == null ? Convert.ToDecimal(0.00) : Convert.ToDecimal(order.金额) * -1,
                        药品付数 = 1,
                        材质分类 = order.材质分类,
                        收费科室 = order.收费科室,
                        科室id = order.科室id == null ? Convert.ToInt32(0) : Convert.ToInt32(order.科室id),
                        物理分类 = order.物理分类,
                        特殊药品 = order.特殊药品 == null ? false : Convert.ToBoolean(order.特殊药品),
                        库存量 = order.库存量 == null ? Convert.ToDecimal(0.00) : Convert.ToDecimal(order.库存量),
                        日结帐日期 = null,
                        月结帐日期 = null,
                        发药标志 = 0,
                        发药日期 = order.发药日期,
                        操作员 = user.姓名,
                        医保码 = order.医保码,
                        医保比例 = order.医保比例 == null ? Convert.ToDecimal(0.00) : Convert.ToDecimal(order.医保比例),
                        医保金额 = order.医保金额 == null ? Convert.ToDecimal(0.00) : Convert.ToDecimal(order.医保金额),
                        医师 = order.医师,
                        划价号 = order.划价号,
                        病人姓名 = order.病人姓名,
                        用法 = order.用法,
                        用量 = order.用量,
                        使用频率 = order.使用频率,
                        性别 = order.性别,
                        年龄 = order.年龄,
                        地址 = order.地址,
                        批号 = order.批号,
                        YfId = order.YfId == null ? Convert.ToDecimal(0.00) : Convert.ToDecimal(order.YfId),
                        疾病诊断 = order.疾病诊断,
                        接口码1 = order.接口码1,
                        接口码2 = order.接口码2,
                        合疗分类 = order.合疗分类,
                        政府采购价 = order.政府采购价,
                        禁忌 = order.禁忌,
                        卡号 = order.卡号,
                        组别 = order.组别,
                        分组标识 = order.分组标识,
                        处方类别 = order.处方类别,
                        套餐名称 = order.套餐名称,
                        农合卡号 = order.农合卡号,
                        一付量 = order.一付量,
                        CzyId = user.Id,
                        DwId = 1,
                        YsId = order.YsId,
                        CateId = order.CateId,
                        CateName = order.CateName,
                        ParentOrderCode = "",
                        OrderCode = ""
                    };
                    hisContext.Add(orderRefund);
                }
                hisContext.SaveChanges();

                var refundOrderOutput = new RefundOrderOutput
                {
                    Code = 1,
                    Msg = $"退费成功!"
                };

                return new ObjectResult(new
                {
                    head = Encrypt.Base64Encode(JsonConvert.SerializeObject(header, Formatting.Indented, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() })),
                    body = Encrypt.UrlEncode(Encrypt.Base64Encode(JsonConvert.SerializeObject(refundOrderOutput, Formatting.Indented, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() })))
                });
            }
        }
        #endregion

        #region 人工窗口退费查询
        /// <summary>
        /// 人工窗口退费查询
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpPost("/api/search/windowfadefare")]
        [ProducesResponseType(typeof(SearchWindowRefundOutput), 200)]
        [ProducesResponseType(typeof(void), 500)]
        public async Task<IActionResult> SearchWindowRefund([FromForm]OppointmentApiQuery query)
        {
            OppointmentApiHeader header = JsonConvert.DeserializeObject<OppointmentApiHeader>(Encrypt.Base64Decode(query.Head));
            SearchWindowRefundParam param = JsonConvert.DeserializeObject<SearchWindowRefundParam>(Encrypt.Base64Decode(Encrypt.UrlDecode(query.Body)), new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

            if (!VaildToken(header))
            {
                return new ObjectResult("Token验证失败，请检查身份验证信息!");
            }

            var payments = await hisContext.门诊收费.Where(p => string.IsNullOrEmpty(param.CFlowCode) || p.收费id == Convert.ToInt32(param.CFlowCode))
                .Where(p => p.退票 != null && p.PayFrom.Equals("健康山西") && p.IsWindowRefund == true && p.WindowRefundFlag == 0).ToListAsync();

            if (payments == null || payments.Count == 0)
            {
                var searchWindowRefundOutput = new SearchWindowRefundOutput
                {
                    Code = 0,
                    Msg = $"查询窗口退费失败，未能找到信息！"
                };

                return new ObjectResult(new
                {
                    head = Encrypt.Base64Encode(JsonConvert.SerializeObject(header, Formatting.Indented, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() })),
                    body = Encrypt.UrlEncode(Encrypt.Base64Encode(JsonConvert.SerializeObject(searchWindowRefundOutput, Formatting.Indented, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() })))
                });
            }
            else
            {
                List<SearchWindowRefund> SearchWindowRefunds = new List<SearchWindowRefund>();
                foreach (var payment in payments)
                {
                    var order = await hisContext.划价临时库.FirstOrDefaultAsync(o => o.发票流水号 == payment.收费id);
                    var searchWindowRefund = new SearchWindowRefund
                    {
                        CFlowCode = payment.收费id.ToString(),
                        CRateType = order.CateId,
                        NMoney = payment.总金额.ToString(),
                        CPatientCode = payment.卡号,
                        CPatientName = payment.病人姓名,
                        Ordercode = order.划价号,
                        TradeDate = Convert.ToDateTime(payment.日期).ToString("yyyy-MM-dd hh:mm:ss")
                    };
                    SearchWindowRefunds.Add(searchWindowRefund);
                }

                var searchWindowRefundOutput = new SearchWindowRefundOutput
                {
                    Code = 1,
                    Msg = $"查询窗口退费成功!",
                    Result = SearchWindowRefunds
                };

                return new ObjectResult(new
                {
                    head = Encrypt.Base64Encode(JsonConvert.SerializeObject(header, Formatting.Indented, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() })),
                    body = Encrypt.UrlEncode(Encrypt.Base64Encode(JsonConvert.SerializeObject(searchWindowRefundOutput, Formatting.Indented, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() })))
                });
            }
        }
        #endregion

        #region 人工窗口退费置标志
        /// <summary>
        /// 人工窗口退费置标志
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpPost("/api/search/setwindowfadefareflag")]
        [ProducesResponseType(typeof(FlagWindowRefundOutput), 200)]
        [ProducesResponseType(typeof(void), 500)]
        public async Task<IActionResult> FlagWindowRefund([FromForm]OppointmentApiQuery query)
        {
            OppointmentApiHeader header = JsonConvert.DeserializeObject<OppointmentApiHeader>(Encrypt.Base64Decode(query.Head));
            FlagWindowRefundParam param = JsonConvert.DeserializeObject<FlagWindowRefundParam>(Encrypt.Base64Decode(Encrypt.UrlDecode(query.Body)), new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

            if (!VaildToken(header))
            {
                return new ObjectResult("Token验证失败，请检查身份验证信息!");
            }
            var payment = await hisContext.门诊收费.Where(p => p.退票 != null && p.收费id == Convert.ToInt32(param.CFlowCode) && p.PayFrom.Equals("健康山西") && p.IsWindowRefund == true && p.WindowRefundFlag == 0).FirstOrDefaultAsync();

            if (payment == null)
            {
                var flagWindowRefundOutput = new FlagWindowRefundOutput
                {
                    Code = 0,
                    Msg = $"查询窗口退费失败，未能找到信息！"
                };

                return new ObjectResult(new
                {
                    head = Encrypt.Base64Encode(JsonConvert.SerializeObject(header, Formatting.Indented, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() })),
                    body = Encrypt.UrlEncode(Encrypt.Base64Encode(JsonConvert.SerializeObject(flagWindowRefundOutput, Formatting.Indented, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() })))
                });
            }
            else
            {
                payment.WindowRefundFlag = 1;
                hisContext.Update(payment);
                hisContext.SaveChanges();

                var flagWindowRefundOutput = new FlagWindowRefundOutput
                {
                    Code = 0,
                    Msg = $"操作成功！"
                };

                return new ObjectResult(new
                {
                    head = Encrypt.Base64Encode(JsonConvert.SerializeObject(header, Formatting.Indented, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() })),
                    body = Encrypt.UrlEncode(Encrypt.Base64Encode(JsonConvert.SerializeObject(flagWindowRefundOutput, Formatting.Indented, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() })))
                });
            }
        }
        #endregion

        #region 查询交易流水
        /// <summary>
        /// 查询交易流水
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpPost("/api/search/tradeflow")]
        [ProducesResponseType(typeof(SearchTradeFlowOutput), 200)]
        [ProducesResponseType(typeof(void), 500)]
        public async Task<IActionResult> SearchTradeFlow([FromForm]OppointmentApiQuery query)
        {
            OppointmentApiHeader header = JsonConvert.DeserializeObject<OppointmentApiHeader>(Encrypt.Base64Decode(query.Head));
            SearchTradeFlowParam param = JsonConvert.DeserializeObject<SearchTradeFlowParam>(Encrypt.Base64Decode(Encrypt.UrlDecode(query.Body)), new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

            if (!VaildToken(header))
            {
                return new ObjectResult("Token验证失败，请检查身份验证信息!");
            }

            DateTime begin = Convert.ToDateTime(param.Trade_beg + " 00:00:00");
            DateTime end = Convert.ToDateTime(param.Trade_end + " 23:59:59");

            var payments = await hisContext.门诊收费.Where(p => p.PayFrom.Equals("健康山西") && p.日期 >= begin && p.日期 <= end).ToListAsync();
            var paymentsSerial = await hisContext.门诊收费流水帐.Where(p => p.PayFrom.Equals("健康山西") && p.日期 >= begin && p.日期 <= end).ToListAsync();

            if (payments.Count == 0 && paymentsSerial.Count == 0)
            {
                var searchTradeFlowOutput = new SearchTradeFlowOutput
                {
                    Code = 0,
                    Msg = $"查询流水失败，未能找到信息！"
                };

                return new ObjectResult(new
                {
                    head = Encrypt.Base64Encode(JsonConvert.SerializeObject(header, Formatting.Indented, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() })),
                    body = Encrypt.UrlEncode(Encrypt.Base64Encode(JsonConvert.SerializeObject(searchTradeFlowOutput, Formatting.Indented, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() })))
                });
            }
            else
            {
                List<SearchTradeFlow> tradeFlows = new List<SearchTradeFlow>();

                foreach (var item in payments)
                {
                    var idCard = "";
                    var registered = await ghContext.门诊挂号.FirstOrDefaultAsync(r => r.卡号.Equals(item.卡号));
                    if (registered == null)
                    {
                        var registeredLs = await ghContext.门诊挂号流水帐.FirstOrDefaultAsync(r => r.卡号.Equals(item.卡号));
                        if (registeredLs != null)
                        {
                            if (!string.IsNullOrEmpty(registeredLs.身份证))
                            {
                                idCard = registeredLs.身份证;
                            }
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(registered.身份证))
                        {
                            idCard = registered.身份证;
                        }
                    }
                    var order = await hisContext.划价临时库.FirstOrDefaultAsync(o => o.发票流水号 == item.收费id);
                    var searchTradeFlow = new SearchTradeFlow
                    {
                        Cflowcode = item.收费id.ToString(),
                        OrderCode = order.划价号,
                        Cateid = order.CateId,
                        Tradetype = item.退票 == null ? "缴费" : "退费",
                        Nmoney = Convert.ToDecimal(item.总金额).ToString("0.00"),
                        Tradedate = Convert.ToDateTime(item.日期).ToString("yyyy-MM-dd hh:mm:ss"),
                        Ls_cpscode = item.PayMethod,
                        Cpatientname = item.病人姓名,
                        Cpatientcode = item.卡号,
                        Cidentitycard = idCard,
                        Tongchoumoney = "0",
                        Accountmoney = "0",
                        Factmoney = Convert.ToDecimal(item.总金额).ToString("0.00"),
                        Bdayend = "0",
                        Dayendtime = ""
                    };
                    tradeFlows.Add(searchTradeFlow);
                }
                foreach (var item in paymentsSerial)
                {
                    var idCard = "";
                    var registered = await ghContext.门诊挂号.FirstOrDefaultAsync(r => r.卡号.Equals(item.卡号));
                    if (registered == null)
                    {
                        var registeredLs = await ghContext.门诊挂号流水帐.FirstOrDefaultAsync(r => r.卡号.Equals(item.卡号));
                        if (registeredLs != null)
                        {
                            if (!string.IsNullOrEmpty(registeredLs.身份证))
                            {
                                idCard = registeredLs.身份证;
                            }
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(registered.身份证))
                        {
                            idCard = registered.身份证;
                        }
                    }
                    var order = await hisContext.划价流水帐.FirstOrDefaultAsync(o => o.发票流水号 == item.收费id);
                    var searchTradeFlow = new SearchTradeFlow
                    {
                        Cflowcode = item.收费id.ToString(),
                        OrderCode = order.划价号,
                        Cateid = order.CateId,
                        Tradetype = item.退票 == null ? "缴费" : "退费",
                        Nmoney = Convert.ToDecimal(item.总金额).ToString("0.00"),
                        Tradedate = Convert.ToDateTime(item.日期).ToString("yyyy-MM-dd hh:mm:ss"),
                        Ls_cpscode = item.PayMethod,
                        Cpatientname = item.病人姓名,
                        Cpatientcode = item.卡号,
                        Cidentitycard = idCard,
                        Tongchoumoney = "0",
                        Accountmoney = "0",
                        Factmoney = Convert.ToDecimal(item.总金额).ToString("0.00"),
                        Bdayend = "1",
                        Dayendtime = Convert.ToDateTime(item.结帐日期).ToString("yyyy-MM-dd hh:mm:ss")
                    };
                    tradeFlows.Add(searchTradeFlow);
                }

                var searchTradeFlowOutput = new SearchTradeFlowOutput
                {
                    Code = 1,
                    Msg = $"查询成功！",
                    Results = tradeFlows
                };

                return new ObjectResult(new
                {
                    head = Encrypt.Base64Encode(JsonConvert.SerializeObject(header, Formatting.Indented, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() })),
                    body = Encrypt.UrlEncode(Encrypt.Base64Encode(JsonConvert.SerializeObject(searchTradeFlowOutput, Formatting.Indented, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() })))
                });
            }
        }

        #endregion

        #endregion
    }
}