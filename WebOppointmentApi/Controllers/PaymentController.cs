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
                            Cateid = param.BillType,
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
                        Cateid = param.BillType,
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
            if (orders == null && orders.Count == 0)
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
                Cateid = param.BillType,
                Title = " 缴费单",
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

        #endregion

        #region HIS提供接口

        #region 查询待缴费项
        /// <summary>
        /// 查询待缴费项
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpPost("/api/search/notpay")]
        [ProducesResponseType(typeof(UpdateDeptOutput), 200)]
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
                List<划价临时库> orders = await hisContext.划价临时库.Where(o => o.划价号.Equals(param.Orderid)).ToListAsync();
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
                    decimal totalPrice = await hisContext.划价临时库.Where(o => o.划价号.Equals(param.Orderid)).SumAsync(o => o.金额);
                    var details = mapper.Map<List<PendingPaymentDetails>>(orders);
                    PendingPayment payment = new PendingPayment
                    {
                        Name = orders[0].病人姓名,
                        Gender = orders[0].性别,
                        State = orders[0].发票流水号 == 0 ? 1 : 3,
                        Dname = doctor.所在科室,
                        Docname = doctor.医师姓名,
                        Time = orders[0].日期.ToString("yyyy-MM-dd hh:mm:ss"),
                        Cateid = param.Cateid,
                        Title = " 缴费单",
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
                List<string> orderIds = await hisContext.划价临时库.Where(o => o.卡号.Equals(param.Cpatientcode)).Select(o => o.划价号).Distinct().ToListAsync();
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
                        List<划价临时库> orders = await hisContext.划价临时库.Where(o => o.划价号.Equals(item)).ToListAsync();
                        var doctor = await hisContext.医师代码.FirstOrDefaultAsync(d => d.医师姓名.Equals(orders[0].医师));
                        decimal totalPrice = await hisContext.划价临时库.Where(o => o.划价号.Equals(param.Orderid)).SumAsync(o => o.金额);
                        var details = mapper.Map<List<PendingPaymentDetails>>(orders);
                        PendingPayment payment = new PendingPayment
                        {
                            Name = orders[0].病人姓名,
                            Gender = orders[0].性别,
                            State = orders[0].发票流水号 == 0 ? 1 : 3,
                            Dname = doctor.所在科室,
                            Docname = doctor.医师姓名,
                            Time = orders[0].日期.ToString("yyyy-MM-dd hh:mm:ss"),
                            Cateid = "2",
                            Title = " 缴费单",
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

        #endregion
    }
}