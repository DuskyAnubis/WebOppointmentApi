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
    [Route("api/v1/Inpatients")]
    public class InpatientController : Controller
    {
        private readonly ApiContext dbContext;
        private readonly GhContext ghContext;
        private readonly HisContext hisContext;
        private readonly IMapper mapper;
        private readonly OppointmentApiOptions apiOptions;

        public InpatientController(ApiContext dbContext, GhContext ghContext, HisContext hisContext, IMapper mapper, OppointmentApiOptions apiOptions)
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

        #region 住院患者信息同步
        /// <summary>
        /// 住院患者信息同步
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost("SynchronizingInpatient")]
        [ProducesResponseType(typeof(OppointmentApiBody), 200)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(ValidationError), 422)]
        [ProducesResponseType(typeof(void), 500)]
        public async Task<IActionResult> SynchronizingInpatient([FromBody]SynchronizingInpatientParam param)
        {
            var header = GetOppointmentApiHeader();
            var patient = await hisContext.Zy病案库.FirstOrDefaultAsync(p => p.住院号 == Convert.ToInt32(param.InpatientNum));
            if (patient == null)
            {
                return NotFound(Json(new { Error = "同步信息失败，病人信息不存在!" }));
            }

            var bed = await hisContext.Zy病区床位.FirstOrDefaultAsync(b => b.病人编号 == patient.病人编号);
            var doctor = await hisContext.医师代码.FirstOrDefaultAsync(d => d.医师姓名.Equals(patient.医师代码));

            var synchronizingInpatientInput = new SynchronizingInpatientInput()
            {
                Pid = patient.病人编号.ToString(),
                Adnum = patient.住院号.ToString(),
                Bednum = bed != null ? bed.床位号.ToString() : "0",
                Name = patient.姓名,
                Date = patient.出院标志 == 4 ? Convert.ToDateTime(patient.出院日期).ToString("yyyy-MM-dd hh:mm:ss") : patient.入院日期.ToString("yyyy-MM-dd hh:mm:ss"),
                State = patient.出院标志 == 4 ? 2 : 1,
                Idcard = patient.身份证号,
                Phone = patient.电话,
                Hospid = apiOptions.HospitalId,
                Hospname = apiOptions.HospitalName,
                Deptid = doctor.KsId.ToString(),
                Deptname = patient.科室,
                Doctid = doctor.医师代码1,
                Docname = doctor.医师姓名,
                Sex = patient.性别 == "男" ? 1 : 2,
                Age = DateTime.Now.Year - Convert.ToDateTime(patient.出生日期).Year,
                Mtype = 3
            };

            string head = Encrypt.Base64Encode(JsonConvert.SerializeObject(header, Formatting.Indented, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            }));
            string body = Encrypt.UrlEncode(Encrypt.Base64Encode(JsonConvert.SerializeObject(synchronizingInpatientInput, Formatting.Indented, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            })));

            OppointmentApi api = new OppointmentApi();
            string strResult = await api.DoPostAsync(apiOptions.BaseUri4, "user/get", head, body);

            OppointmentApiResult result = JsonConvert.DeserializeObject<OppointmentApiResult>(strResult);
            OppointmentApiBody resultBody = JsonConvert.DeserializeObject<OppointmentApiBody>(Encrypt.Base64Decode(result.Body.Contains("%") ? Encrypt.UrlDecode(result.Body) : result.Body));

            return new ObjectResult(resultBody);
        }
        #endregion

        #region 邀请绑定住院服务
        /// <summary>
        /// 邀请绑定住院服务
        /// </summary>
        /// <returns></returns>
        [HttpPost("InviteBindInpatient")]
        [ProducesResponseType(typeof(OppointmentApiBody), 200)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(ValidationError), 422)]
        [ProducesResponseType(typeof(void), 500)]
        public async Task<IActionResult> InviteBindInpatient([FromBody]InviteBindInpatientParam param)
        {
            var header = GetOppointmentApiHeader();
            var patient = await hisContext.Zy病案库.FirstOrDefaultAsync(p => p.住院号 == Convert.ToInt32(param.InpatientNum));
            if (patient == null)
            {
                return NotFound(Json(new { Error = "邀请绑定信息失败，病人信息不存在!" }));
            }

            var inviteBindInpatientInput = new InviteBindInpatientInput()
            {
                Hospid = apiOptions.HospitalId,
                Pid = patient.病人编号.ToString()
            };

            string head = Encrypt.Base64Encode(JsonConvert.SerializeObject(header, Formatting.Indented, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            }));
            string body = Encrypt.UrlEncode(Encrypt.Base64Encode(JsonConvert.SerializeObject(inviteBindInpatientInput, Formatting.Indented, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            })));

            OppointmentApi api = new OppointmentApi();
            string strResult = await api.DoPostAsync(apiOptions.BaseUri4, "invite/bind", head, body);

            OppointmentApiResult result = JsonConvert.DeserializeObject<OppointmentApiResult>(strResult);
            OppointmentApiBody resultBody = JsonConvert.DeserializeObject<OppointmentApiBody>(Encrypt.Base64Decode(result.Body.Contains("%") ? Encrypt.UrlDecode(result.Body) : result.Body));

            return new ObjectResult(resultBody);
        }
        #endregion

        #region 解除绑定住院服务
        /// <summary>
        /// 解除绑定住院服务
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost("RelieveBindInpatient")]
        [ProducesResponseType(typeof(OppointmentApiBody), 200)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(ValidationError), 422)]
        [ProducesResponseType(typeof(void), 500)]
        public async Task<IActionResult> RelieveBindInpatient([FromBody]RelieveBindInpatientParam param)
        {
            var header = GetOppointmentApiHeader();
            var patient = await hisContext.Zy病案库.FirstOrDefaultAsync(p => p.住院号 == Convert.ToInt32(param.InpatientNum));
            if (patient == null)
            {
                return NotFound(Json(new { Error = "解除绑定信息失败，病人信息不存在!" }));
            }

            var relieveBindInpatientInput = new RelieveBindInpatientInput()
            {
                Hospid = apiOptions.HospitalId,
                Pid = patient.病人编号.ToString(),
                Userid = ""
            };

            string head = Encrypt.Base64Encode(JsonConvert.SerializeObject(header, Formatting.Indented, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            }));
            string body = Encrypt.UrlEncode(Encrypt.Base64Encode(JsonConvert.SerializeObject(relieveBindInpatientInput, Formatting.Indented, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            })));

            OppointmentApi api = new OppointmentApi();
            string strResult = await api.DoPostAsync(apiOptions.BaseUri4, "relieve/bind", head, body);

            OppointmentApiResult result = JsonConvert.DeserializeObject<OppointmentApiResult>(strResult);
            OppointmentApiBody resultBody = JsonConvert.DeserializeObject<OppointmentApiBody>(Encrypt.Base64Decode(result.Body.Contains("%") ? Encrypt.UrlDecode(result.Body) : result.Body));

            return new ObjectResult(resultBody);
        }
        #endregion

        #region 检验报告同步

        #endregion

        #region 费用清单同步
        /// <summary>
        /// 费用清单同步
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost("SynchronizingCostList")]
        [ProducesResponseType(typeof(OppointmentApiBody), 200)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(ValidationError), 422)]
        [ProducesResponseType(typeof(void), 500)]
        public async Task<IActionResult> SynchronizingCostList([FromBody]SynchronizingCostListParam param)
        {
            var header = GetOppointmentApiHeader();
            var patient = await hisContext.Zy病案库.FirstOrDefaultAsync(p => p.住院号 == Convert.ToInt32(param.InpatientNum));
            if (patient == null)
            {
                return NotFound(Json(new { Error = "同步费用清单失败，病人信息不存在!" }));
            }

            List<DateTime> times;
            if (string.IsNullOrEmpty(param.Date))
            {
                times = new List<DateTime>();
                DateTime beginDate = Convert.ToDateTime(patient.入院日期.ToString("yyyy-MM-dd") + " 00:00:00");
                while (beginDate < DateTime.Now.AddDays(-1))
                {
                    times.Add(beginDate);
                    beginDate = beginDate.AddDays(1);
                }
            }
            else
            {
                times = new List<DateTime>
                {
                    Convert.ToDateTime(param.Date + " 00:00:00")
                };
            }

            SynchronizingCostItemTitle costItemTitle = new SynchronizingCostItemTitle
            {
                Name = "品名",
                Spec = "规格",
                Count = "数量",
                Sum = "金额",
                Ext1 = "",
                Ext2 = ""
            };
            List<SynchronizingCostItem> costItems = new List<SynchronizingCostItem>();
            foreach (var date in times)
            {
                decimal totalPrie = await hisContext.Zy记帐流水帐.Where(p => p.病人编号 == patient.病人编号 && p.日期 >= date && p.日期 < date.AddDays(1)).Select(p => p.实交金额).SumAsync();
                List<Zy记帐流水帐> payments = await hisContext.Zy记帐流水帐.Where(p => p.病人编号 == patient.病人编号 && p.日期 >= date && p.日期 < date.AddDays(1)).ToListAsync();
                SynchronizingCostItem costItem = new SynchronizingCostItem
                {
                    Time = date.ToString("yyyy-MM-dd"),
                    Total = totalPrie.ToString("0.00"),
                    Title = costItemTitle
                };
                List<SynchronizingCostItemContent> costItemContents = new List<SynchronizingCostItemContent>();
                foreach (var item in payments)
                {
                    SynchronizingCostItemContent costItemContent = new SynchronizingCostItemContent
                    {
                        Id = item.Id,
                        Time = item.日期.ToString("yyyy-MM-dd"),
                        Name = item.名称,
                        Spec = item.规格,
                        Count = item.数量.ToString("0"),
                        Sum = item.实交金额.ToString("0.00"),
                        Ext1 = "",
                        Ext2 = ""
                    };
                    costItemContents.Add(costItemContent);
                }
                costItem.Content = costItemContents;
                costItems.Add(costItem);
            }

            var synchronizingCostListInput = new SynchronizingCostListInput()
            {
                Hospid = apiOptions.HospitalId,
                Pid = patient.病人编号.ToString(),
                Item = costItems
            };

            string head = Encrypt.Base64Encode(JsonConvert.SerializeObject(header, Formatting.Indented, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            }));
            string body = Encrypt.UrlEncode(Encrypt.Base64Encode(JsonConvert.SerializeObject(synchronizingCostListInput, Formatting.Indented, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            })));

            OppointmentApi api = new OppointmentApi();
            string strResult = await api.DoPostAsync(apiOptions.BaseUri4, "cost/get", head, body);

            OppointmentApiResult result = JsonConvert.DeserializeObject<OppointmentApiResult>(strResult);
            OppointmentApiBody resultBody = JsonConvert.DeserializeObject<OppointmentApiBody>(Encrypt.Base64Decode(result.Body.Contains("%") ? Encrypt.UrlDecode(result.Body) : result.Body));

            return new ObjectResult(resultBody);
        }
        #endregion

        #endregion

        #region HIS提供接口

        #region 同步绑定关系
        /// <summary>
        /// 同步绑定关系
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpPost("/api/syn/bind")]
        [ProducesResponseType(typeof(SynchronizingBindOutput), 200)]
        [ProducesResponseType(typeof(void), 500)]
        public async Task<IActionResult> SynchronizingBind([FromForm]OppointmentApiQuery query)
        {
            OppointmentApiHeader header = JsonConvert.DeserializeObject<OppointmentApiHeader>(Encrypt.Base64Decode(query.Head));
            SynchronizingBindParam param = JsonConvert.DeserializeObject<SynchronizingBindParam>(Encrypt.Base64Decode(Encrypt.UrlDecode(query.Body)), new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

            if (!VaildToken(header))
            {
                return new ObjectResult("Token验证失败，请检查身份验证信息!");
            }

            var patient = await hisContext.Zy病案库.FirstOrDefaultAsync(p => p.病人编号 == Convert.ToInt32(param.Pid));
            if (patient == null)
            {
                var synchronizingBindOutput = new SynchronizingBindOutput
                {
                    Code = 0,
                    Msg = "同步失败!未查到该病人信息!",
                    Result = ""
                };

                return new ObjectResult(new
                {
                    head = Encrypt.Base64Encode(JsonConvert.SerializeObject(header, Formatting.Indented, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() })),
                    body = Encrypt.UrlEncode(Encrypt.Base64Encode(JsonConvert.SerializeObject(synchronizingBindOutput, Formatting.Indented, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() })))
                });
            }
            else
            {
                patient.UserId = param.Userid;
                patient.Phone = param.Phone;
                patient.IsBind = param.Opcode == 1 ? true : false;
                hisContext.Zy病案库.Update(patient);
                hisContext.SaveChanges();

                var synchronizingBindOutput = new SynchronizingBindOutput
                {
                    Code = 1,
                    Msg = "同步成功!",
                    Result = ""
                };

                return new ObjectResult(new
                {
                    head = Encrypt.Base64Encode(JsonConvert.SerializeObject(header, Formatting.Indented, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() })),
                    body = Encrypt.UrlEncode(Encrypt.Base64Encode(JsonConvert.SerializeObject(synchronizingBindOutput, Formatting.Indented, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() })))
                });
            }
        }
        #endregion

        #region 获取检验报告明细

        #endregion

        #region 获取费用清单
        /// <summary>
        /// 获取费用清单
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpPost("/api/cost/update")]
        [ProducesResponseType(typeof(UpdateCostListOutput), 200)]
        [ProducesResponseType(typeof(void), 500)]
        public async Task<IActionResult> UpdateCostList([FromForm]OppointmentApiQuery query)
        {
            OppointmentApiHeader header = JsonConvert.DeserializeObject<OppointmentApiHeader>(Encrypt.Base64Decode(query.Head));
            UpdateCostListParam param = JsonConvert.DeserializeObject<UpdateCostListParam>(Encrypt.Base64Decode(Encrypt.UrlDecode(query.Body)), new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

            if (!VaildToken(header))
            {
                return new ObjectResult("Token验证失败，请检查身份验证信息!");
            }

            var patient = await hisContext.Zy病案库.FirstOrDefaultAsync(p => p.病人编号 == Convert.ToInt32(param.Pid));
            if (patient == null)
            {
                var synchronizingBindOutput = new SynchronizingBindOutput
                {
                    Code = 0,
                    Msg = "获取费用清单失败!未查到该病人信息!",
                    Result = ""
                };

                return new ObjectResult(new
                {
                    head = Encrypt.Base64Encode(JsonConvert.SerializeObject(header, Formatting.Indented, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() })),
                    body = Encrypt.UrlEncode(Encrypt.Base64Encode(JsonConvert.SerializeObject(synchronizingBindOutput, Formatting.Indented, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() })))
                });
            }
            else if (!patient.IsBind)
            {
                var synchronizingBindOutput = new SynchronizingBindOutput
                {
                    Code = 2,
                    Msg = "获取费用清单失败!该病人未绑定或已解绑!",
                    Result = ""
                };

                return new ObjectResult(new
                {
                    head = Encrypt.Base64Encode(JsonConvert.SerializeObject(header, Formatting.Indented, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() })),
                    body = Encrypt.UrlEncode(Encrypt.Base64Encode(JsonConvert.SerializeObject(synchronizingBindOutput, Formatting.Indented, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() })))
                });
            }
            else
            {
                List<DateTime> times;
                if (string.IsNullOrEmpty(param.Time))
                {
                    times = new List<DateTime>();
                    DateTime beginDate = Convert.ToDateTime(patient.入院日期.ToString("yyyy-MM-dd") + " 00:00:00");
                    while (beginDate < DateTime.Now.AddDays(-1))
                    {
                        times.Add(beginDate);
                        beginDate = beginDate.AddDays(1);
                    }
                }
                else
                {
                    times = new List<DateTime>
                    {
                        Convert.ToDateTime(param.Time + " 00:00:00")
                    };
                }
                UpdateCostItemTitle costItemTitle = new UpdateCostItemTitle
                {
                    Name = "品名",
                    Spec = "规格",
                    Count = "数量",
                    Sum = "金额",
                    Ext1 = "",
                    Ext2 = ""
                };
                List<UpdateCostItem> costItems = new List<UpdateCostItem>();
                foreach (var date in times)
                {
                    decimal totalPrie = await hisContext.Zy记帐流水帐.Where(p => p.病人编号 == patient.病人编号 && p.日期 >= date && p.日期 < date.AddDays(1)).Select(p => p.实交金额).SumAsync();
                    List<Zy记帐流水帐> payments = await hisContext.Zy记帐流水帐.Where(p => p.病人编号 == patient.病人编号 && p.日期 >= date && p.日期 < date.AddDays(1)).ToListAsync();
                    UpdateCostItem costItem = new UpdateCostItem
                    {
                        Time = date.ToString("yyyy-MM-dd"),
                        Total = totalPrie.ToString("0.00"),
                        Title = costItemTitle
                    };
                    List<UpdateCostItemContent> costItemContents = new List<UpdateCostItemContent>();
                    foreach (var item in payments)
                    {
                        UpdateCostItemContent costItemContent = new UpdateCostItemContent
                        {
                            Id = item.Id,
                            Time = item.日期.ToString("yyyy-MM-dd"),
                            Name = item.名称,
                            Spec = item.规格,
                            Count = item.数量.ToString("0"),
                            Sum = item.实交金额.ToString("0.00"),
                            Ext1 = "",
                            Ext2 = ""
                        };
                        costItemContents.Add(costItemContent);
                    }
                    costItem.Content = costItemContents;
                    costItems.Add(costItem);
                }

                var updateCostListOutput = new UpdateCostListOutput()
                {
                    Code = 1,
                    Msg = "获取费用清单信息成功!",
                    Hospid = apiOptions.HospitalId,
                    Pid = patient.病人编号.ToString(),
                    Item = costItems
                };

                return new ObjectResult(new
                {
                    head = Encrypt.Base64Encode(JsonConvert.SerializeObject(header, Formatting.Indented, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() })),
                    body = Encrypt.UrlEncode(Encrypt.Base64Encode(JsonConvert.SerializeObject(updateCostListOutput, Formatting.Indented, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() })))
                });
            }
        }
        #endregion

        #endregion
    }
}