﻿using AutoMapper;
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
using System.Data;
using System.Data.SqlClient;

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
        public async Task<IActionResult> DownPayBill([FromBody] DownPayBillParam param)
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
        public async Task<IActionResult> OrderSms([FromBody] OrderSmsParam param)
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
        public async Task<IActionResult> BillingSms([FromBody] BillingSmsParam param)
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
                            Time = order.日期.ToString("yyyy-MM-dd HH:mm:ss"),
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
                        Time = order.日期.ToString("yyyy-MM-dd HH:mm:ss"),
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
        public async Task<IActionResult> SendPendingPayment([FromBody] SendPendingPaymentParam param)
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
                Time = orders[0].日期.ToString("yyyy-MM-dd HH:mm:ss"),
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
        public async Task<IActionResult> ScanCodePay([FromBody] ScanCodePayParam param)
        {
            var header = GetOppointmentApiHeader();
            ScanCodePayInput scanCodePayInput;

            string[] billNumArrary = param.BillNum.Split('|').Where(s => !string.IsNullOrEmpty(s)).ToArray();
            if (billNumArrary.Length == 0)
            {
                return NotFound(Json(new { Error = "付款失败，请输入划价号" }));
            }

            int count = await hisContext.划价临时库.Where(o => billNumArrary.Contains(o.划价号)).CountAsync();
            if (count <= 0)
            {
                return NotFound(Json(new { Error = "付款失败，划价信息不存在!" }));
            }

            string totalOId = ScanCodeTools.GetOrderString(apiOptions.PaymentId, "18", billNumArrary[0]);
            decimal totalSumPrice = await hisContext.划价临时库.Where(o => billNumArrary.Contains(o.划价号)).SumAsync(o => o.金额);

            List<ScanCodePayDetail> details = new List<ScanCodePayDetail>();
            foreach (var billNum in billNumArrary)
            {
                List<划价临时库> orders = await hisContext.划价临时库.Where(o => o.划价号.Equals(billNum)).ToListAsync();

                var doctor = await hisContext.医师代码.FirstOrDefaultAsync(d => d.医师姓名.Equals(orders[0].医师));
                decimal totalPrice = await hisContext.划价临时库.Where(o => o.划价号.Equals(billNum)).SumAsync(o => o.金额);

                ScanCodePayDetail detail = new ScanCodePayDetail
                {
                    Orderid = ScanCodeTools.GetOrderString(apiOptions.PaymentId, "1" + orders[0].CateId, billNum),
                    Price = totalPrice.ToString("0.00"),
                    Cateid = orders[0].CateId,
                    Name = orders[0].病人姓名,
                    Gender = orders[0].性别,
                    State = "1",
                    Dname = doctor.所在科室,
                    Docname = doctor.医师姓名,
                    Time = orders[0].日期.ToString("yyyy-MM-dd HH:mm:ss"),
                    Title = orders[0].名称 + "等"
                };
                details.Add(detail);

                foreach (var order in orders)
                {
                    order.ParentOrderCode = totalOId;
                    order.OrderCode = detail.Orderid;
                    hisContext.Update(order);
                }
            }

            hisContext.SaveChanges();

            scanCodePayInput = new ScanCodePayInput
            {
                Scantext = param.ScanCode,
                Userid = apiOptions.UserId,
                Hospid = apiOptions.HospitalId,
                Totaloid = totalOId,
                Totalprice = totalSumPrice.ToString("0.00"),
                Cateid = "2",
                Totaltitle = "患者门诊费用",
                State = "1",
                Orderids = details
            };

            string head = Encrypt.Base64Encode(JsonConvert.SerializeObject(header, Formatting.Indented, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            }));
            string body = Encrypt.UrlEncode(Encrypt.Base64Encode(JsonConvert.SerializeObject(scanCodePayInput, Formatting.Indented, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            })));

            OppointmentApi api = new OppointmentApi();
            string strResult = await api.DoPostAsync(apiOptions.BaseUri3, "api/pay/scanpay", head, body);

            OppointmentApiResult result = JsonConvert.DeserializeObject<OppointmentApiResult>(strResult);
            ScanCodePayBody resultBody = JsonConvert.DeserializeObject<ScanCodePayBody>(Encrypt.Base64Decode(result.Body.Contains("%") ? Encrypt.UrlDecode(result.Body) : result.Body));

            if (resultBody.Code == 1)
            {
                foreach (var item in resultBody.Result.Ounos)
                {
                    List<划价临时库> orders = await hisContext.划价临时库.Where(o => o.OrderCode.Equals(item.Oid)).ToListAsync();
                    foreach (var order in orders)
                    {
                        order.PlatformCode = item.Ouno;
                        hisContext.Update(order);
                    }
                }

                hisContext.SaveChanges();
            }

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
        public async Task<IActionResult> ScanCodeQuery([FromBody] ScanCodeQueryParam param)
        {
            var header = GetOppointmentApiHeader();
            ScanCodeQueryInput scanCodeQueryInput;

            string[] billNumArrary = param.BillNum.Split('|').Where(s => !string.IsNullOrEmpty(s)).ToArray();
            if (billNumArrary.Length == 0)
            {
                return NotFound(Json(new { Error = "查询失败，请输入划价号" }));
            }

            划价临时库 order = await hisContext.划价临时库.Where(o => billNumArrary.Contains(o.划价号)).FirstOrDefaultAsync();
            if (order == null)
            {
                return NotFound(Json(new { Error = "查询失败，划价信息不存在!" }));
            }

            scanCodeQueryInput = new ScanCodeQueryInput
            {
                Userid = apiOptions.UserId,
                Hospid = apiOptions.HospitalId,
                Orderid = order.ParentOrderCode
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
            string strResult = await api.DoPostAsync(apiOptions.BaseUri3, "api/pay/orderquery", head, body);

            OppointmentApiResult result = JsonConvert.DeserializeObject<OppointmentApiResult>(strResult);
            ScanCodePayBody resultBody = JsonConvert.DeserializeObject<ScanCodePayBody>(Encrypt.Base64Decode(result.Body.Contains("%") ? Encrypt.UrlDecode(result.Body) : result.Body));

            if (resultBody.Code == 1)
            {
                foreach (var item in resultBody.Result.Ounos)
                {
                    List<划价临时库> orders = await hisContext.划价临时库.Where(o => o.OrderCode.Equals(item.Oid)).ToListAsync();
                    foreach (var orderItem in orders)
                    {
                        orderItem.PlatformCode = item.Ouno;
                        hisContext.Update(orderItem);
                    }
                }

                hisContext.SaveChanges();
            }

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
        public async Task<IActionResult> ScanCodeComplete([FromBody] ScanCodeCompleteParam param)
        {
            var header = GetOppointmentApiHeader();
            ScanCodeCompleteInput scanCodeCompleteInput;

            string[] billNumArrary = param.BillNum.Split('|').Where(s => !string.IsNullOrEmpty(s)).ToArray();
            if (billNumArrary.Length == 0)
            {
                return NotFound(Json(new { Error = "查询失败，请输入划价号" }));
            }

            int count = await hisContext.划价临时库.Where(o => billNumArrary.Contains(o.划价号)).CountAsync();
            if (count <= 0)
            {
                return NotFound(Json(new { Error = "付款失败，划价信息不存在!" }));
            }

            门诊收费 payment = await hisContext.门诊收费.FirstOrDefaultAsync(p => p.收费id == Convert.ToInt32(param.CFlowCode));
            if (payment == null)
            {
                return NotFound(Json(new { Error = "付款完成失败，该划价单未收费!" }));
            }

            decimal totalSumPrice = await hisContext.划价临时库.Where(o => billNumArrary.Contains(o.划价号)).SumAsync(o => o.金额);

            List<ScanCodeCompleteDetail> details = new List<ScanCodeCompleteDetail>();
            foreach (var billNum in billNumArrary)
            {
                decimal totalPrice = await hisContext.划价临时库.Where(o => o.划价号.Equals(billNum)).SumAsync(o => o.金额);
                划价临时库 order = await hisContext.划价临时库.Where(o => o.划价号.Equals(billNum)).FirstOrDefaultAsync();

                ScanCodeCompleteDetail detail = new ScanCodeCompleteDetail
                {
                    Ouno = order.PlatformCode,
                    Price = totalPrice.ToString("0.00"),
                    Cflowcode = param.CFlowCode + billNum
                };
                details.Add(detail);
            }

            scanCodeCompleteInput = new ScanCodeCompleteInput
            {
                Touno = payment.ParentPlatformCode,
                Userid = apiOptions.UserId,
                Code = "1",
                Totalprice = totalSumPrice.ToString("0.00"),
                Tcflowcode = param.CFlowCode,
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
            string strResult = await api.DoPostAsync(apiOptions.BaseUri3, "api/pay/hispaynotice", head, body);

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
        public async Task<IActionResult> ScanCodeCancel([FromBody] ScanCodeCancelParam param)
        {
            var header = GetOppointmentApiHeader();
            ScanCodeCancelInput scanCodeCancelInput;

            string[] billNumArrary = param.BillNum.Split('|').Where(s => !string.IsNullOrEmpty(s)).ToArray();
            if (billNumArrary.Length == 0)
            {
                return NotFound(Json(new { Error = "查询失败，请输入划价号" }));
            }

            划价临时库 order = await hisContext.划价临时库.Where(o => billNumArrary.Contains(o.划价号)).FirstOrDefaultAsync();
            if (order == null)
            {
                return NotFound(Json(new { Error = "查询失败，划价信息不存在!" }));
            }

            scanCodeCancelInput = new ScanCodeCancelInput
            {
                Userid = apiOptions.UserId,
                Hospid = apiOptions.HospitalId,
                Orderid = order.ParentOrderCode
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
            string strResult = await api.DoPostAsync(apiOptions.BaseUri3, "api/pay/ordercancel", head, body);

            OppointmentApiResult result = JsonConvert.DeserializeObject<OppointmentApiResult>(strResult);
            OppointmentApiBody resultBody = JsonConvert.DeserializeObject<OppointmentApiBody>(Encrypt.Base64Decode(result.Body.Contains("%") ? Encrypt.UrlDecode(result.Body) : result.Body));

            return new ObjectResult(resultBody);
        }
        #endregion

        #region 扫码交预交金
        /// <summary>
        /// 面对面扫码预交金
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost("ScanCodePrepayment")]
        [ProducesResponseType(typeof(OppointmentApiBody), 200)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(ValidationError), 422)]
        [ProducesResponseType(typeof(void), 500)]
        public async Task<IActionResult> ScanCodePrepayment([FromBody] ScanCodePrepaymentParam param)
        {
            var header = GetOppointmentApiHeader();
            ScanCodePayInput scanCodePayInput;

            InpatientPrepayment prepayment = await hisContext.InpatientPrepayments.FirstOrDefaultAsync(p => p.SerialCode.Equals(param.SerialCode));
            if (prepayment == null)
            {
                return NotFound(Json(new { Error = "付款失败，预交金信息不存在!" }));
            }

            Zy病案库 patient = await hisContext.Zy病案库.FirstOrDefaultAsync(p => p.病人编号 == prepayment.PatientId);
            if (patient == null)
            {
                return NotFound(Json(new { Error = "付款失败，病人信息不存在!" }));
            }

            ScanCodePayDetail detail = new ScanCodePayDetail
            {
                Orderid = ScanCodeTools.GetOrderString(apiOptions.PaymentId, "17", prepayment.SerialCode),
                Price = prepayment.Money.ToString("0.00"),
                Cateid = "7",
                Name = patient.姓名,
                Gender = patient.性别,
                State = "1",
                Dname = "收款室",
                Docname = prepayment.UserName,
                Time = prepayment.TradeTime.ToString("yyyy-MM-dd HH:mm:ss"),
                Title = "住院预交金"
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
                Totaloid = ScanCodeTools.GetOrderString(apiOptions.PaymentId, "19", prepayment.SerialCode),
                Totalprice = prepayment.Money.ToString("0.00"),
                Cateid = "7",
                Totaltitle = "住院预交金",
                State = "1",
                Orderids = details
            };

            prepayment.ParentOrderCode = scanCodePayInput.Totaloid;
            prepayment.OrderCode = detail.Orderid;
            hisContext.InpatientPrepayments.Update(prepayment);
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
            string strResult = await api.DoPostAsync(apiOptions.BaseUri3, "api/pay/scanpay", head, body);

            OppointmentApiResult result = JsonConvert.DeserializeObject<OppointmentApiResult>(strResult);
            ScanCodePayBody resultBody = JsonConvert.DeserializeObject<ScanCodePayBody>(Encrypt.Base64Decode(result.Body.Contains("%") ? Encrypt.UrlDecode(result.Body) : result.Body));

            if (resultBody.Code == 1)
            {
                foreach (var item in resultBody.Result.Ounos)
                {
                    InpatientPrepayment pre = await hisContext.InpatientPrepayments.FirstOrDefaultAsync(p => p.OrderCode.Equals(item.Oid));
                    pre.PlatformCode = item.Ouno;
                    pre.ParentPlatformCode = resultBody.Result.Ouno;
                }

                hisContext.SaveChanges();
            }

            return new ObjectResult(resultBody);
        }
        #endregion

        #region 扫码交预交金状态查询
        /// <summary>
        /// 扫码交预交金状态查询
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost("ScanCodePrepaymentQuery")]
        [ProducesResponseType(typeof(OppointmentApiBody), 200)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(ValidationError), 422)]
        [ProducesResponseType(typeof(void), 500)]
        public async Task<IActionResult> ScanCodePrepaymentQuery([FromBody] ScanCodePrepaymentQueryParam param)
        {
            var header = GetOppointmentApiHeader();
            ScanCodeQueryInput scanCodeQueryInput;

            InpatientPrepayment prepayment = await hisContext.InpatientPrepayments.FirstOrDefaultAsync(p => p.SerialCode.Equals(param.SerialCode));
            if (prepayment == null)
            {
                return NotFound(Json(new { Error = "查询失败，预交金信息不存在!" }));
            }

            scanCodeQueryInput = new ScanCodeQueryInput
            {
                Userid = apiOptions.UserId,
                Hospid = apiOptions.HospitalId,
                Orderid = prepayment.ParentOrderCode
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
            string strResult = await api.DoPostAsync(apiOptions.BaseUri3, "api/pay/orderquery", head, body);

            OppointmentApiResult result = JsonConvert.DeserializeObject<OppointmentApiResult>(strResult);
            ScanCodePayBody resultBody = JsonConvert.DeserializeObject<ScanCodePayBody>(Encrypt.Base64Decode(result.Body.Contains("%") ? Encrypt.UrlDecode(result.Body) : result.Body));

            if (resultBody.Code == 1)
            {
                foreach (var item in resultBody.Result.Ounos)
                {
                    InpatientPrepayment pre = await hisContext.InpatientPrepayments.FirstOrDefaultAsync(p => p.OrderCode.Equals(item.Oid));
                    pre.PlatformCode = item.Ouno;
                    pre.ParentPlatformCode = resultBody.Result.Ouno;
                }

                hisContext.SaveChanges();
            }

            return new ObjectResult(resultBody);
        }
        #endregion

        #region 扫码交预交金完成
        /// <summary>
        /// 扫码交预交金完成
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost("ScanCodePrepaymentComplete")]
        [ProducesResponseType(typeof(OppointmentApiBody), 200)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(ValidationError), 422)]
        [ProducesResponseType(typeof(void), 500)]
        public async Task<IActionResult> ScanCodePrepaymentComplete([FromBody] ScanCodePrepaymentCompleteParam param)
        {
            var header = GetOppointmentApiHeader();
            ScanCodeCompleteInput scanCodeCompleteInput;

            InpatientPrepayment prepayment = await hisContext.InpatientPrepayments.FirstOrDefaultAsync(p => p.SerialCode.Equals(param.SerialCode));
            if (prepayment == null)
            {
                return NotFound(Json(new { Error = "付款完成失败，预交金信息不存在!" }));
            }

            ScanCodeCompleteDetail detail = new ScanCodeCompleteDetail
            {
                Ouno = prepayment.PlatformCode,
                Price = prepayment.Money.ToString("0.00"),
                Cflowcode = prepayment.PrepaymentId.ToString() + prepayment.SerialCode
            };
            List<ScanCodeCompleteDetail> details = new List<ScanCodeCompleteDetail>
            {
                detail
            };

            scanCodeCompleteInput = new ScanCodeCompleteInput
            {
                Touno = prepayment.ParentPlatformCode,
                Userid = apiOptions.UserId,
                Code = "1",
                Totalprice = prepayment.Money.ToString("0.00"),
                Tcflowcode = prepayment.PrepaymentId.ToString(),
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
            string strResult = await api.DoPostAsync(apiOptions.BaseUri3, "api/pay/hispaynotice", head, body);

            OppointmentApiResult result = JsonConvert.DeserializeObject<OppointmentApiResult>(strResult);
            OppointmentApiBody resultBody = JsonConvert.DeserializeObject<OppointmentApiBody>(Encrypt.Base64Decode(result.Body.Contains("%") ? Encrypt.UrlDecode(result.Body) : result.Body));

            return new ObjectResult(resultBody);
        }
        #endregion

        #region 扫码交预交金取消
        /// <summary>
        /// 扫码交预交金取消
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost("ScanCodePrepaymentCancel")]
        [ProducesResponseType(typeof(OppointmentApiBody), 200)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(ValidationError), 422)]
        [ProducesResponseType(typeof(void), 500)]
        public async Task<IActionResult> ScanCodePrepaymentCancel([FromBody] ScanCodePrepaymentCancelParam param)
        {
            var header = GetOppointmentApiHeader();
            ScanCodeCancelInput scanCodeCancelInput;

            InpatientPrepayment prepayment = await hisContext.InpatientPrepayments.FirstOrDefaultAsync(p => p.SerialCode.Equals(param.SerialCode));
            if (prepayment == null)
            {
                return NotFound(Json(new { Error = "付款取消失败，预交金信息不存在!" }));
            }

            scanCodeCancelInput = new ScanCodeCancelInput
            {
                Userid = apiOptions.UserId,
                Hospid = apiOptions.HospitalId,
                Orderid = prepayment.ParentOrderCode
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
            string strResult = await api.DoPostAsync(apiOptions.BaseUri3, "api/pay/ordercancel", head, body);

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
        public async Task<IActionResult> SearchPendingPayment([FromForm] OppointmentApiQuery query)
        {
            OppointmentApiHeader header = JsonConvert.DeserializeObject<OppointmentApiHeader>(Encrypt.Base64Decode(query.Head));
            SearchPendingPaymentParam param = JsonConvert.DeserializeObject<SearchPendingPaymentParam>(Encrypt.Base64Decode(Encrypt.UrlDecode(query.Body)), new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

            if (!VaildToken(header))
            {
                return new ObjectResult("Token验证失败，请检查身份验证信息!");
            }

            if (param.Indextype.Equals("0"))
            {
                List<划价临时库> orders = await hisContext.划价临时库.Where(o => o.接口码1.Equals(param.Orderid) && o.发票流水号 == -1).ToListAsync();
                if (orders == null || orders.Count == 0)
                {
                    var searchPendingPaymentOutPut = new SearchPendingPaymentOutput
                    {
                        Code = 0,
                        Msg = $"未能查询到订单号为{param.Orderid}的待缴费信息!",
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
                    decimal totalPrice = await hisContext.划价临时库.Where(o => o.接口码1.Equals(param.Orderid) && o.发票流水号 == -1).SumAsync(o => o.金额);
                    var details = mapper.Map<List<PendingPaymentDetails>>(orders);
                    PendingPayment payment = new PendingPayment
                    {
                        Name = orders[0].病人姓名,
                        Gender = orders[0].性别,
                        State = 1,
                        Dname = doctor.所在科室,
                        Docname = doctor.医师姓名,
                        Time = orders[0].日期.ToString("yyyy-MM-dd HH:mm:ss"),
                        Cateid = orders[0].CateId,
                        Title = orders[0].CateName,
                        Price = totalPrice.ToString(),
                        Orderid = orders[0].接口码1,
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
                var searchPendingPaymentOutPut = new SearchPendingPaymentOutput
                {
                    Code = 0,
                    Msg = $"医院未开通主索引模式!",
                    Items = null
                };

                return new ObjectResult(new
                {
                    head = Encrypt.Base64Encode(JsonConvert.SerializeObject(header, Formatting.Indented, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() })),
                    body = Encrypt.UrlEncode(Encrypt.Base64Encode(JsonConvert.SerializeObject(searchPendingPaymentOutPut, Formatting.Indented, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() })))
                });
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
        public async Task<IActionResult> PayOrder([FromForm] OppointmentApiQuery query)
        {
            OppointmentApiHeader header = JsonConvert.DeserializeObject<OppointmentApiHeader>(Encrypt.Base64Decode(query.Head));
            PayOrderParam param = JsonConvert.DeserializeObject<PayOrderParam>(Encrypt.Base64Decode(Encrypt.UrlDecode(query.Body)), new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

            if (!VaildToken(header))
            {
                return new ObjectResult("Token验证失败，请检查身份验证信息!");
            }
            List<划价临时库> orders;

            if (param.Indextype.Equals("0"))
            {
                orders = await hisContext.划价临时库.Where(o => o.接口码1.Equals(param.Orderid) && o.发票流水号 == -1).ToListAsync();
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
                    decimal totalPrice = await hisContext.划价临时库.Where(o => o.接口码1.Equals(param.Orderid) && o.发票流水号 == -1).SumAsync(o => o.金额);

                    门诊收费 payment = new 门诊收费
                    {
                        日期 = DateTime.Now,
                        操作员 = "健康山西",
                        病人姓名 = orders[0].病人姓名,
                        卡号 = orders[0].卡号,
                        总金额 = totalPrice,
                        优惠额 = Convert.ToDecimal(0.00),
                        账户支付 = totalPrice,
                        统筹支付 = Convert.ToDecimal(0.00),
                        补助金 = Convert.ToDecimal(0.00),
                        现金支付 = Convert.ToDecimal(0.00),
                        交班标志 = true,
                        结帐日期 = null,
                        门诊号 = "",
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
                        CzyId = 368,
                        PayMethod = "诊间支付",
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
                        Cidentitycard = "",
                        Cpatientname = orders[0].病人姓名,
                        Csex = orders[0].性别,
                        Ddiagnosetime = orders[0].日期.ToString("yyyy-MM-dd HH:mm:ss"),
                        Deptname = doctor.所在科室,
                        Doctorname = doctor.医师姓名,
                        Cdiagnosetypename = "",
                        Ndiagnosenum = orders[0].卡号,
                        Chousesectionname = "",
                        Chousename = "",
                        Clocation = "",
                        Nmoney = totalPrice.ToString("0.00"),
                        Diagnoseid = orders[0].卡号,
                        Windowmsg = "缴费成功!",
                        Windowname = "",
                        Crequestcode = orders[0].卡号
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
                var payOrderOutput = new PayOrderOutput
                {
                    Code = 0,
                    Msg = "医院未开通主索引模式!"
                };

                return new ObjectResult(new
                {
                    head = Encrypt.Base64Encode(JsonConvert.SerializeObject(header, Formatting.Indented, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() })),
                    body = Encrypt.UrlEncode(Encrypt.Base64Encode(JsonConvert.SerializeObject(payOrderOutput, Formatting.Indented, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() })))
                });
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
        public async Task<IActionResult> RefundOrder([FromForm] OppointmentApiQuery query)
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
                        PlatformCode = "",
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
                        OrderCode = "",
                        PlatformCode = ""
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
        public async Task<IActionResult> SearchWindowRefund([FromForm] OppointmentApiQuery query)
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
                        TradeDate = Convert.ToDateTime(payment.日期).ToString("yyyy-MM-dd HH:mm:ss")
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
        public async Task<IActionResult> FlagWindowRefund([FromForm] OppointmentApiQuery query)
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
        public async Task<IActionResult> SearchTradeFlow([FromForm] OppointmentApiQuery query)
        {
            OppointmentApiHeader header = JsonConvert.DeserializeObject<OppointmentApiHeader>(Encrypt.Base64Decode(query.Head));
            SearchTradeFlowParam param = JsonConvert.DeserializeObject<SearchTradeFlowParam>(Encrypt.Base64Decode(Encrypt.UrlDecode(query.Body)), new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

            if (!VaildToken(header))
            {
                return new ObjectResult("Token验证失败，请检查身份验证信息!");
            }

            DateTime begin = Convert.ToDateTime(param.Trade_beg + " 00:00:00");
            DateTime end = Convert.ToDateTime(param.Trade_end + " 23:59:59");

            var orderTemporary = hisContext.划价临时库.Where(o => o.日期 >= begin && o.日期 <= end && o.OrderCode != "" && o.PlatformCode != "");
            var orderTemporaryCodeList = (from db in orderTemporary
                                          group db by new { db.OrderCode, db.病人姓名, db.卡号, db.发票流水号 } into g
                                          select new { OrderCode = g.Key.OrderCode, Total = g.Sum(p => p.金额), CardNo = g.Key.卡号, PatientName = g.Key.病人姓名, TradeTime = g.Min(p => p.日期).ToString("yyyy-MM-dd HH:mm:ss"), InvoiceCode = g.Key.发票流水号, SerialCode = g.Min(p => p.划价号), CateId = g.Min(p => p.CateId) }).ToList();

            var orderSerial = hisContext.划价流水帐.Where(o => o.日期 >= begin && o.日期 <= end && o.OrderCode != "" && o.PlatformCode != "");
            var orderSerialCodeList = (from db in orderSerial
                                       group db by new { db.OrderCode, db.病人姓名, db.卡号, db.发票流水号 } into g
                                       select new { OrderCode = g.Key.OrderCode, Total = g.Sum(p => p.金额), CardNo = g.Key.卡号, PatientName = g.Key.病人姓名, TradeTime = g.Min(p => p.日期).Value.ToString("yyyy-MM-dd HH:mm:ss"), InvoiceCode = g.Key.发票流水号, SerialCode = g.Min(p => p.划价号), CateId = g.Min(p => p.CateId) }).ToList();

            var inpatientPrepaymentList = await hisContext.InpatientPrepayments.Where(p => p.PlatformCode != "" && p.TradeTime >= begin && p.TradeTime <= end).ToListAsync();

            var orderTemporaryOnline = hisContext.划价临时库.Where(o => o.日期 >= begin && o.日期 <= end && o.接口码1 != "" && o.发票流水号 != -1);
            var orderTemporaryOnlinePayList = (from db in orderTemporaryOnline
                                               group db by new { db.接口码1, db.病人姓名, db.卡号, db.发票流水号 } into g
                                               select new { OrderCode = g.Key.接口码1, Total = g.Sum(p => p.金额), CardNo = g.Key.卡号, PatientName = g.Key.病人姓名, TradeTime = g.Min(p => p.日期).ToString("yyyy-MM-dd HH:mm:ss"), InvoiceCode = g.Key.发票流水号, CateId = g.Min(p => p.CateId) }).ToList();

            var orderSerialOnline = hisContext.划价流水帐.Where(o => o.日期 >= begin && o.日期 <= end && o.接口码1 != "" && o.发票流水号 != -1);
            var orderSerialOnlinePayList = (from db in orderSerialOnline
                                            group db by new { db.OrderCode, db.病人姓名, db.卡号, db.发票流水号 } into g
                                            select new { OrderCode = g.Key.OrderCode, Total = g.Sum(p => p.金额), CardNo = g.Key.卡号, PatientName = g.Key.病人姓名, TradeTime = g.Min(p => p.日期).Value.ToString("yyyy-MM-dd HH:mm:ss"), InvoiceCode = g.Key.发票流水号, CateId = g.Min(p => p.CateId) }).ToList();

            if (orderTemporaryCodeList.Count == 0 && orderSerialCodeList.Count == 0 && inpatientPrepaymentList.Count == 0 && orderTemporaryOnlinePayList.Count == 0 && orderSerialOnlinePayList.Count == 0)
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
                foreach (var item in orderTemporaryCodeList)
                {
                    var searchTradeFlow = new SearchTradeFlow
                    {
                        Cflowcode = item.InvoiceCode.ToString() + item.SerialCode,
                        OrderCode = item.OrderCode,
                        Cateid = item.CateId,
                        Tradetype = "1",
                        Nmoney = item.Total.ToString("0.00"),
                        Tradedate = item.TradeTime,
                        Ls_cpscode = "10",
                        Cpatientname = item.PatientName,
                        Cpatientcode = item.CardNo,
                        Cidentitycard = "",
                        Tongchoumoney = "0",
                        Accountmoney = "0",
                        Factmoney = item.Total.ToString("0.00"),
                        Bdayend = "0",
                        Dayendtime = ""
                    };
                    tradeFlows.Add(searchTradeFlow);
                }

                foreach (var item in orderSerialCodeList)
                {
                    var searchTradeFlow = new SearchTradeFlow
                    {
                        Cflowcode = item.InvoiceCode.ToString() + item.SerialCode,
                        OrderCode = item.OrderCode,
                        Cateid = item.CateId,
                        Tradetype = "1",
                        Nmoney = Convert.ToDecimal(item.Total).ToString("0.00"),
                        Tradedate = item.TradeTime,
                        Ls_cpscode = "10",
                        Cpatientname = item.PatientName,
                        Cpatientcode = item.CardNo,
                        Cidentitycard = "",
                        Tongchoumoney = "0",
                        Accountmoney = "0",
                        Factmoney = Convert.ToDecimal(item.Total).ToString("0.00"),
                        Bdayend = "0",
                        Dayendtime = ""
                    };
                    tradeFlows.Add(searchTradeFlow);
                }

                foreach (var item in inpatientPrepaymentList)
                {
                    Zy病案库 patient = await hisContext.Zy病案库.FirstOrDefaultAsync(p => p.病人编号 == item.PatientId);
                    var searchTradeFlow = new SearchTradeFlow
                    {
                        Cflowcode = item.PrepaymentId.ToString() + item.SerialCode,
                        OrderCode = item.OrderCode,
                        Cateid = "7",
                        Tradetype = "1",
                        Nmoney = item.Money.ToString("0.00"),
                        Tradedate = item.TradeTime.ToString("yyyy-MM-dd HH:mm:ss"),
                        Ls_cpscode = "10",
                        Cpatientname = patient.姓名,
                        Cpatientcode = patient.病人编号.ToString(),
                        Cidentitycard = "",
                        Tongchoumoney = "0",
                        Accountmoney = "0",
                        Factmoney = item.Money.ToString("0.00"),
                        Bdayend = "1",
                        Dayendtime = ""
                    };
                    tradeFlows.Add(searchTradeFlow);
                }

                foreach (var item in orderTemporaryOnlinePayList)
                {
                    var searchTradeFlow = new SearchTradeFlow
                    {
                        Cflowcode = item.InvoiceCode.ToString(),
                        OrderCode = item.OrderCode,
                        Cateid = item.CateId,
                        Tradetype = "1",
                        Nmoney = item.Total.ToString("0.00"),
                        Tradedate = item.TradeTime,
                        Ls_cpscode = "10",
                        Cpatientname = item.PatientName,
                        Cpatientcode = item.CardNo,
                        Cidentitycard = "",
                        Tongchoumoney = "0",
                        Accountmoney = "0",
                        Factmoney = item.Total.ToString("0.00"),
                        Bdayend = "0",
                        Dayendtime = ""
                    };
                    tradeFlows.Add(searchTradeFlow);
                }

                foreach (var item in orderSerialOnlinePayList)
                {
                    var searchTradeFlow = new SearchTradeFlow
                    {
                        Cflowcode = item.InvoiceCode.ToString(),
                        OrderCode = item.OrderCode,
                        Cateid = item.CateId,
                        Tradetype = "1",
                        Nmoney = Convert.ToDecimal(item.Total).ToString("0.00"),
                        Tradedate = item.TradeTime,
                        Ls_cpscode = "10",
                        Cpatientname = item.PatientName,
                        Cpatientcode = item.CardNo,
                        Cidentitycard = "",
                        Tongchoumoney = "0",
                        Accountmoney = "0",
                        Factmoney = Convert.ToDecimal(item.Total).ToString("0.00"),
                        Bdayend = "0",
                        Dayendtime = ""
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

        ///// <summary>
        ///// 查询交易流水（总订单号版本） 2019-06-05作废
        ///// </summary>
        ///// <param name="query"></param>
        ///// <returns></returns>
        //[HttpPost("/api/search/tradeflow")]
        //[ProducesResponseType(typeof(SearchTradeFlowOutput), 200)]
        //[ProducesResponseType(typeof(void), 500)]
        //public async Task<IActionResult> SearchTradeFlow([FromForm]OppointmentApiQuery query)
        //{
        //    OppointmentApiHeader header = JsonConvert.DeserializeObject<OppointmentApiHeader>(Encrypt.Base64Decode(query.Head));
        //    SearchTradeFlowParam param = JsonConvert.DeserializeObject<SearchTradeFlowParam>(Encrypt.Base64Decode(Encrypt.UrlDecode(query.Body)), new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

        //    if (!VaildToken(header))
        //    {
        //        return new ObjectResult("Token验证失败，请检查身份验证信息!");
        //    }

        //    DateTime begin = Convert.ToDateTime(param.Trade_beg + " 00:00:00");
        //    DateTime end = Convert.ToDateTime(param.Trade_end + " 23:59:59");

        //    var payments = await hisContext.门诊收费.Where(p => p.PayFrom.Equals("健康山西") && p.日期 >= begin && p.日期 <= end).ToListAsync();
        //    var paymentsSerial = await hisContext.门诊收费流水帐.Where(p => p.PayFrom.Equals("健康山西") && p.日期 >= begin && p.日期 <= end).ToListAsync();

        //    if (payments.Count == 0 && paymentsSerial.Count == 0)
        //    {
        //        var searchTradeFlowOutput = new SearchTradeFlowOutput
        //        {
        //            Code = 0,
        //            Msg = $"查询流水失败，未能找到信息！"
        //        };

        //        return new ObjectResult(new
        //        {
        //            head = Encrypt.Base64Encode(JsonConvert.SerializeObject(header, Formatting.Indented, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() })),
        //            body = Encrypt.UrlEncode(Encrypt.Base64Encode(JsonConvert.SerializeObject(searchTradeFlowOutput, Formatting.Indented, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() })))
        //        });
        //    }
        //    else
        //    {
        //        List<SearchTradeFlow> tradeFlows = new List<SearchTradeFlow>();

        //        foreach (var item in payments)
        //        {
        //            var order = await hisContext.划价临时库.FirstOrDefaultAsync(o => o.发票流水号 == item.收费id);
        //            var searchTradeFlow = new SearchTradeFlow
        //            {
        //                Cflowcode = item.收费id.ToString(),
        //                OrderCode = order.ParentOrderCode,
        //                Cateid = order.CateId,
        //                Tradetype = "1",
        //                Nmoney = Convert.ToDecimal(item.总金额).ToString("0.00"),
        //                Tradedate = Convert.ToDateTime(item.日期).ToString("yyyy-MM-dd hh:mm:ss"),
        //                Ls_cpscode = "10",
        //                Cpatientname = item.病人姓名,
        //                Cpatientcode = item.卡号,
        //                Cidentitycard = "",
        //                Tongchoumoney = "0",
        //                Accountmoney = "0",
        //                Factmoney = Convert.ToDecimal(item.总金额).ToString("0.00"),
        //                Bdayend = "0",
        //                Dayendtime = ""
        //            };
        //            tradeFlows.Add(searchTradeFlow);
        //        }
        //        foreach (var item in paymentsSerial)
        //        {
        //            var order = await hisContext.划价流水帐.FirstOrDefaultAsync(o => o.发票流水号 == item.收费id);
        //            var searchTradeFlow = new SearchTradeFlow
        //            {
        //                Cflowcode = item.收费id.ToString(),
        //                OrderCode = order.ParentOrderCode,
        //                Cateid = order.CateId,
        //                Tradetype = "1",
        //                Nmoney = Convert.ToDecimal(item.总金额).ToString("0.00"),
        //                Tradedate = Convert.ToDateTime(item.日期).ToString("yyyy-MM-dd hh:mm:ss"),
        //                Ls_cpscode = "10",
        //                Cpatientname = item.病人姓名,
        //                Cpatientcode = item.卡号,
        //                Cidentitycard = "",
        //                Tongchoumoney = "0",
        //                Accountmoney = "0",
        //                Factmoney = Convert.ToDecimal(item.总金额).ToString("0.00"),
        //                Bdayend = "1",
        //                Dayendtime = Convert.ToDateTime(item.结帐日期).ToString("yyyy-MM-dd hh:mm:ss")
        //            };
        //            tradeFlows.Add(searchTradeFlow);
        //        }

        //        var searchTradeFlowOutput = new SearchTradeFlowOutput
        //        {
        //            Code = 1,
        //            Msg = $"查询成功！",
        //            Results = tradeFlows
        //        };

        //        return new ObjectResult(new
        //        {
        //            head = Encrypt.Base64Encode(JsonConvert.SerializeObject(header, Formatting.Indented, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() })),
        //            body = Encrypt.UrlEncode(Encrypt.Base64Encode(JsonConvert.SerializeObject(searchTradeFlowOutput, Formatting.Indented, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() })))
        //        });
        //    }
        //}

        #endregion

        #region 线上开单申请
        /// <summary>
        /// 线上开单申请
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpPost("/api/OnlineVisits/setrequest")]
        [ProducesResponseType(typeof(SetRequestOutput), 200)]
        [ProducesResponseType(typeof(void), 500)]
        public async Task<IActionResult> SetRequest([FromForm] OppointmentApiQuery query)
        {
            OppointmentApiHeader header = JsonConvert.DeserializeObject<OppointmentApiHeader>(Encrypt.Base64Decode(query.Head));
            SetRequestParam param = JsonConvert.DeserializeObject<SetRequestParam>(Encrypt.Base64Decode(Encrypt.UrlDecode(query.Body)), new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

            try
            {
                if (!VaildToken(header))
                {
                    return new ObjectResult("Token验证失败，请检查身份验证信息!");
                }

                SetRequestOutput setRequestOutput;
                划价临时库 order;
                医师代码 doctor;
                string billNum;

                Yf yf = await hisContext.Yf.FirstOrDefaultAsync(y => y.Id == Convert.ToInt32(param.Labgroup));
                if (yf != null)
                {
                    门诊挂号 gh = await ghContext.门诊挂号.OrderByDescending(g => g.门诊号).FirstOrDefaultAsync(g => g.门诊号 == Convert.ToInt32(param.Cpatientcode) && !string.IsNullOrEmpty(g.卡号));
                    if (gh != null)
                    {
                        doctor = await hisContext.医师代码.FirstOrDefaultAsync(d => d.医师代码1.Equals("999"));

                        if (doctor.划价号 < 99999)
                        {
                            doctor.划价号 += 1;
                        }
                        else
                        {
                            doctor.划价号 = 1;
                        }
                        hisContext.医师代码.Update(doctor);
                        hisContext.SaveChanges();

                        billNum = "1268" + doctor.划价号.ToString().PadLeft(5, '0');

                        order = new 划价临时库
                        {
                            日期 = DateTime.Now,
                            发票流水号 = -1,
                            代码 = yf.代码,
                            货号 = "自费",
                            名称 = yf.名称,
                            规格 = yf.规格,
                            单位 = yf.单位,
                            单价 = yf.单价,
                            数量 = 1,
                            金额 = yf.单价,
                            药品付数 = 1,
                            材质分类 = yf.材质分类,
                            收费科室 = "化验室",
                            科室id = 0,
                            物理分类 = "",
                            特殊药品 = false,
                            库存量 = 0,
                            日结帐日期 = null,
                            月结帐日期 = null,
                            发药标志 = 1,
                            发药日期 = DateTime.Now,
                            操作员 = "健康山西",
                            医保码 = "",
                            医保比例 = 0.00M,
                            医保金额 = 0.00M,
                            医师 = "健康山西",
                            划价号 = billNum,
                            病人姓名 = gh.姓名,
                            用法 = null,
                            用量 = null,
                            使用频率 = null,
                            性别 = gh.性别,
                            年龄 = gh.年龄.ToString() + gh.年龄单位,
                            地址 = "",
                            批号 = "",
                            YfId = yf.Id,
                            疾病诊断 = "",
                            接口码1 = ScanCodeTools.GetOrderString(apiOptions.PaymentId, "13", billNum),
                            接口码2 = "",
                            合疗分类 = "",
                            政府采购价 = null,
                            禁忌 = "",
                            卡号 = gh.卡号,
                            组别 = null,
                            分组标识 = "",
                            处方类别 = "",
                            套餐名称 = "",
                            农合卡号 = "",
                            一付量 = 0.00M,
                            CzyId = 368,
                            DwId = 1,
                            YsId = 268,
                            CateId = "3",
                            CateName = "检验",
                            OrderCode = "",
                            ParentOrderCode = "",
                            PlatformCode = ""
                        };

                        hisContext.划价临时库.Add(order);
                        await hisContext.SaveChangesAsync();

                        setRequestOutput = new SetRequestOutput
                        {
                            Code = 1,
                            Msg = "开单成功!",
                            Orderid = order.接口码1
                        };
                    }
                    else
                    {
                        门诊挂号流水帐 ghls = await ghContext.门诊挂号流水帐.OrderByDescending(g => g.门诊号).FirstOrDefaultAsync(g => g.门诊号 == Convert.ToInt32(param.Cpatientcode) && !string.IsNullOrEmpty(g.卡号));
                        if (ghls != null)
                        {
                            doctor = await hisContext.医师代码.FirstOrDefaultAsync(d => d.医师代码1.Equals("999"));

                            if (doctor.划价号 < 99999)
                            {
                                doctor.划价号 += 1;
                            }
                            else
                            {
                                doctor.划价号 = 1;
                            }
                            hisContext.医师代码.Update(doctor);
                            hisContext.SaveChanges();

                            billNum = "1268" + doctor.划价号.ToString().PadLeft(5, '0');

                            order = new 划价临时库
                            {
                                日期 = DateTime.Now,
                                发票流水号 = -1,
                                代码 = yf.代码,
                                货号 = "自费",
                                名称 = yf.名称,
                                规格 = yf.规格,
                                单位 = yf.单位,
                                单价 = yf.单价,
                                数量 = 1,
                                金额 = yf.单价,
                                药品付数 = 1,
                                材质分类 = yf.材质分类,
                                收费科室 = "化验室",
                                科室id = 0,
                                物理分类 = "",
                                特殊药品 = false,
                                库存量 = 0,
                                日结帐日期 = null,
                                月结帐日期 = null,
                                发药标志 = 1,
                                发药日期 = null,
                                操作员 = "健康山西",
                                医保码 = "",
                                医保比例 = 0.00M,
                                医保金额 = 0.00M,
                                医师 = "健康山西",
                                划价号 = billNum,
                                病人姓名 = ghls.姓名,
                                用法 = null,
                                用量 = null,
                                使用频率 = null,
                                性别 = ghls.性别,
                                年龄 = ghls.年龄.ToString() + ghls.年龄单位,
                                地址 = "",
                                批号 = "",
                                YfId = yf.Id,
                                疾病诊断 = "",
                                接口码1 = ScanCodeTools.GetOrderString(apiOptions.PaymentId, "13", billNum),
                                接口码2 = "",
                                合疗分类 = "",
                                政府采购价 = null,
                                禁忌 = "",
                                卡号 = ghls.卡号,
                                组别 = null,
                                分组标识 = "",
                                处方类别 = "",
                                套餐名称 = "",
                                农合卡号 = "",
                                一付量 = 0.00M,
                                CzyId = 368,
                                DwId = 1,
                                YsId = 268,
                                CateId = "3",
                                CateName = "检验",
                                OrderCode = "",
                                ParentOrderCode = "",
                                PlatformCode = ""
                            };

                            hisContext.划价临时库.Add(order);
                            await hisContext.SaveChangesAsync();

                            setRequestOutput = new SetRequestOutput
                            {
                                Code = 1,
                                Msg = "开单成功!",
                                Orderid = order.接口码1
                            };
                        }
                        else
                        {
                            setRequestOutput = new SetRequestOutput
                            {
                                Code = 0,
                                Msg = "开单失败，未查询到该病人信息!",
                                Orderid = ""
                            };
                        }
                    }
                }
                else
                {
                    setRequestOutput = new SetRequestOutput
                    {
                        Code = 0,
                        Msg = "开单失败，未查询到该检验项目!",
                        Orderid = ""
                    };
                }

                return new ObjectResult(new
                {
                    head = Encrypt.Base64Encode(JsonConvert.SerializeObject(header, Formatting.Indented, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() })),
                    body = Encrypt.UrlEncode(Encrypt.Base64Encode(JsonConvert.SerializeObject(setRequestOutput, Formatting.Indented, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() })))
                });
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
        #endregion

        #region 获取诊疗ID
        /// <summary>
        /// 获取诊疗ID
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpPost("/api/identity/get")]
        [ProducesResponseType(typeof(IdentityOutput), 200)]
        [ProducesResponseType(typeof(void), 500)]
        public async Task<IActionResult> IdentityGet([FromForm] OppointmentApiQuery query)
        {
            OppointmentApiHeader header = JsonConvert.DeserializeObject<OppointmentApiHeader>(Encrypt.Base64Decode(query.Head));
            IdentityParam param = JsonConvert.DeserializeObject<IdentityParam>(Encrypt.Base64Decode(Encrypt.UrlDecode(query.Body)), new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

            if (!VaildToken(header))
            {
                return new ObjectResult("Token验证失败，请检查身份验证信息!");
            }

            IdentityOutput identityOutput;
            IdentityResult identityResult;

            if (string.IsNullOrEmpty(param.IdentityCard.Trim()) || string.IsNullOrEmpty(param.PatientName.Trim()))
            {
                identityOutput = new IdentityOutput
                {
                    Code = 0,
                    Msg = "获取诊疗ID失败!病人姓名或身份证号不能为空",
                    Result = null
                };
            }
            else
            {
                门诊挂号 gh = await ghContext.门诊挂号.OrderByDescending(g => g.门诊号).FirstOrDefaultAsync(g => g.身份证.Equals(param.IdentityCard) && !string.IsNullOrEmpty(g.卡号));
                if (gh != null)
                {
                    gh.姓名 = param.PatientName;
                    gh.性别 = param.Sex;
                    gh.出生日期 = Convert.ToDateTime(param.Birthday);
                    gh.电话 = param.Tel;
                    gh.年龄 = Convert.ToInt16(param.Age);
                    gh.年龄单位 = "岁";
                    gh.通信地址 = param.Address;


                    ghContext.门诊挂号.Update(gh);
                    await ghContext.SaveChangesAsync();

                    identityResult = new IdentityResult
                    {
                        PatientCode = gh.门诊号.ToString(),
                        PatientName = gh.姓名,
                        IdentityCard = gh.身份证,
                        Birthday = gh.出生日期.ToString(),
                        Age = gh.年龄.ToString(),
                        Ageunit = "岁",
                        Sex = gh.性别,
                        Address = gh.通信地址,
                        Tel = gh.电话,
                        ZlkCardNum = gh.卡号
                    };

                    identityOutput = new IdentityOutput
                    {
                        Code = 1,
                        Msg = "获取诊疗ID成功!",
                        Result = identityResult
                    };
                }
                else
                {
                    门诊挂号流水帐 ghls = await ghContext.门诊挂号流水帐.OrderByDescending(g => g.门诊号).FirstOrDefaultAsync(g => g.身份证.Equals(param.IdentityCard) && !string.IsNullOrEmpty(g.卡号));
                    if (ghls != null)
                    {
                        ghls.姓名 = param.PatientName;
                        ghls.性别 = param.Sex;
                        ghls.出生日期 = Convert.ToDateTime(param.Birthday);
                        ghls.电话 = param.Tel;
                        ghls.年龄 = Convert.ToInt16(param.Age);
                        ghls.年龄单位 = "岁";
                        ghls.通信地址 = param.Address;

                        ghContext.门诊挂号流水帐.Update(ghls);
                        await ghContext.SaveChangesAsync();

                        identityResult = new IdentityResult
                        {
                            PatientCode = ghls.门诊号.ToString(),
                            PatientName = ghls.姓名,
                            IdentityCard = ghls.身份证,
                            Birthday = ghls.出生日期.ToString(),
                            Age = ghls.年龄.ToString(),
                            Ageunit = ghls.年龄单位,
                            Sex = ghls.性别,
                            Address = ghls.通信地址,
                            Tel = ghls.电话,
                            ZlkCardNum = ghls.卡号
                        };

                        identityOutput = new IdentityOutput
                        {
                            Code = 1,
                            Msg = "获取诊疗ID成功!",
                            Result = identityResult
                        };
                    }
                    else
                    {
                        gh = new 门诊挂号
                        {
                            姓名 = param.PatientName,
                            年龄 = Convert.ToInt16(param.Age),
                            性别 = param.Sex,
                            通信地址 = param.Address,
                            电话 = param.Tel,
                            日期 = DateTime.Now,
                            科室 = "化验室",
                            挂号类别 = "普通挂号",
                            操作员 = "健康山西",
                            医师 = "健康山西",
                            挂号费 = 0,
                            工本费 = 0,
                            金额 = 0,
                            作废标志 = 0,
                            卡号 = param.IdentityCard,
                            初诊 = 1,
                            复诊 = 0,
                            急诊 = 0,
                            交班 = true,
                            交班日期 = DateTime.Now,
                            退票号 = "0",
                            就诊标志 = false,
                            民族 = "",
                            接诊医师id = 268,
                            出生日期 = Convert.ToDateTime(param.Birthday),
                            过敏史 = "",
                            年龄单位 = "岁",
                            身份证 = param.IdentityCard,
                            总预存款 = 0,
                            总费用 = 0,
                            预存款支付 = 0,
                            现金支付 = 0,
                            PassWord = "",
                            来源 = "健康山西",
                            预存款余额 = 0,
                            状态 = 0,
                            退款 = 0,
                            DwId = 1,
                            CzyId = 368,
                            社保卡 = param.IdentityCard
                        };

                        ghContext.门诊挂号.Add(gh);
                        await ghContext.SaveChangesAsync();

                        identityResult = new IdentityResult
                        {
                            PatientCode = gh.门诊号.ToString(),
                            PatientName = gh.姓名,
                            IdentityCard = gh.身份证,
                            Birthday = gh.出生日期.ToString(),
                            Age = gh.年龄.ToString(),
                            Ageunit = gh.年龄单位,
                            Sex = gh.性别,
                            Address = gh.通信地址,
                            Tel = gh.电话,
                            ZlkCardNum = gh.卡号
                        };

                        identityOutput = new IdentityOutput
                        {
                            Code = 1,
                            Msg = "获取诊疗ID成功!",
                            Result = identityResult
                        };
                    }
                }
            }

            return new ObjectResult(new
            {
                head = Encrypt.Base64Encode(JsonConvert.SerializeObject(header, Formatting.Indented, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() })),
                body = Encrypt.UrlEncode(Encrypt.Base64Encode(JsonConvert.SerializeObject(identityOutput, Formatting.Indented, new JsonSerializerSettings { ContractResolver = new DefaultContractResolver() })))
            });
        }
        #endregion

        #region 非订单支付接口
        //[HttpPost("/api/pay/commonrate")]
        //[ProducesResponseType(typeof(CommonrateOutput), 200)]
        //[ProducesResponseType(typeof(void), 500)]
        //public async Task<IActionResult> Commonrate([FromBody] OppointmentApiQuery query)
        //{
        //    OppointmentApiHeader header = JsonConvert.DeserializeObject<OppointmentApiHeader>(Encrypt.Base64Decode(query.Head));
        //    CommonrateParam param = JsonConvert.DeserializeObject<CommonrateParam>(Encrypt.Base64Decode(Encrypt.UrlDecode(query.Body)), new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

        //    if (!VaildToken(header))
        //    {
        //        return new ObjectResult("Token验证失败，请检查身份验证信息!");
        //    }
        //    try
        //    {
        //        门诊收费 payment = new 门诊收费
        //        {
        //            日期 = DateTime.Now
        //        };
        //        await hisContext.门诊收费.AddAsync(payment);
        //        await hisContext.SaveChangesAsync();


        //        划价临时库 order = new 划价临时库
        //        {
        //            日期 = DateTime.Now
        //        };
        //        await hisContext.划价临时库.AddAsync(order);
        //        await hisContext.SaveChangesAsync();

        //        var commonrateOutput = new CommonrateOutput()
        //        {
        //            Code = 1,
        //            Msg = "非订单支付成功!",
        //            Cflowcode = payment.收费id.ToString()
        //        };

        //        return new ObjectResult(new
        //        {
        //            head = Encrypt.Base64Encode(JsonConvert.SerializeObject(header, Formatting.Indented, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() })),
        //            body = Encrypt.UrlEncode(Encrypt.Base64Encode(JsonConvert.SerializeObject(commonrateOutput, Formatting.Indented, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() })))
        //        });
        //    }
        //    catch (Exception)
        //    {
        //        var commonrateOutput = new CommonrateOutput()
        //        {
        //            Code = 0,
        //            Msg = "接口调用发生错误,非订单支付失败!"
        //        };
        //        return new ObjectResult(new
        //        {
        //            head = Encrypt.Base64Encode(JsonConvert.SerializeObject(header, Formatting.Indented, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() })),
        //            body = Encrypt.UrlEncode(Encrypt.Base64Encode(JsonConvert.SerializeObject(commonrateOutput, Formatting.Indented, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() })))
        //        });
        //    }
        //}
        #endregion

        #endregion
    }
}