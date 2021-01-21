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
    [Route("api/v1/Lis")]
    public class LisController : Controller
    {
        private readonly ApiContext dbContext;
        private readonly GhContext ghContext;
        private readonly HisContext hisContext;
        private readonly IMapper mapper;
        private readonly OppointmentApiOptions apiOptions;

        public LisController(ApiContext dbContext, GhContext ghContext, HisContext hisContext, IMapper mapper, OppointmentApiOptions apiOptions)
        {
            this.dbContext = dbContext;
            this.ghContext = ghContext;
            this.hisContext = hisContext;
            this.mapper = mapper;
            this.apiOptions = apiOptions;
        }

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

        #region HIS提供接口

        #region 门诊患者查询
        /// <summary>
        /// 门诊患者查询
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpPost("/api/search/outpatient")]
        [ProducesResponseType(typeof(PatientOutput), 200)]
        [ProducesResponseType(typeof(void), 500)]
        public async Task<IActionResult> OutpatientGet([FromForm] OppointmentApiQuery query)
        {
            OppointmentApiHeader header = JsonConvert.DeserializeObject<OppointmentApiHeader>(Encrypt.Base64Decode(query.Head));
            PatientParam param = JsonConvert.DeserializeObject<PatientParam>(Encrypt.Base64Decode(Encrypt.UrlDecode(query.Body)), new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

            if (!VaildToken(header))
            {
                return new ObjectResult("Token验证失败，请检查身份验证信息!");
            }

            PatientOutput patientOutput;
            PatientResult patientResult;
            List<PatientResult> patientResults = new List<PatientResult>();

            List<门诊挂号> ghs = await ghContext.门诊挂号.Where(g => g.身份证.Equals(param.IdentityCard) && g.姓名.Equals(param.PatientName)).ToListAsync();
            foreach (门诊挂号 gh in ghs)
            {
                patientResult = new PatientResult
                {
                    PatientCode = gh.门诊号.ToString(),
                    PatientName = gh.姓名.Trim(),
                    IdentityCard = gh.身份证.Trim(),
                    Sex = gh.性别.Trim(),
                    Tel = gh.电话.Trim()
                };
                patientResults.Add(patientResult);
            }

            List<门诊挂号流水帐> ghlss = await ghContext.门诊挂号流水帐.Where(g => g.身份证.Equals(param.IdentityCard) && g.姓名.Equals(param.PatientName)).ToListAsync();
            foreach (门诊挂号流水帐 ghls in ghlss)
            {
                patientResult = new PatientResult
                {
                    PatientCode = ghls.门诊号.ToString(),
                    PatientName = ghls.姓名.Trim(),
                    IdentityCard = ghls.身份证.Trim(),
                    Sex = ghls.性别.Trim(),
                    Tel = ghls.电话.Trim()
                };
                patientResults.Add(patientResult);
            }

            if (patientResults.Count > 0)
            {
                patientOutput = new PatientOutput
                {
                    Code = 1,
                    Msg = "获取门诊患者信息成功!",
                    Results = patientResults
                };
            }
            else
            {
                patientOutput = new PatientOutput
                {
                    Code = 0,
                    Msg = "未查询到患者信息!",
                    Results = null
                };
            }

            return new ObjectResult(new
            {
                head = Encrypt.Base64Encode(JsonConvert.SerializeObject(header, Formatting.Indented, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() })),
                body = Encrypt.UrlEncode(Encrypt.Base64Encode(JsonConvert.SerializeObject(patientOutput, Formatting.Indented, new JsonSerializerSettings { ContractResolver = new DefaultContractResolver() })))
            });
        }
        #endregion

        #region 检验报告列表
        /// <summary>
        /// 检验报告列表
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpPost("/api/lab/report")]
        [ProducesResponseType(typeof(ReportOutput), 200)]
        [ProducesResponseType(typeof(void), 500)]
        public async Task<IActionResult> LisReport([FromForm] OppointmentApiQuery query)
        {
            OppointmentApiHeader header = JsonConvert.DeserializeObject<OppointmentApiHeader>(Encrypt.Base64Decode(query.Head));
            ReportParam param = JsonConvert.DeserializeObject<ReportParam>(Encrypt.Base64Decode(Encrypt.UrlDecode(query.Body)), new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

            if (!VaildToken(header))
            {
                return new ObjectResult("Token验证失败，请检查身份验证信息!");
            }

            ReportOutput reportOutput;
            ReportResult reportResult;
            List<ReportResult> reportResults = new List<ReportResult>();

            if (string.IsNullOrEmpty(param.Cpatientcode))
            {
                List<门诊挂号> ghs = await ghContext.门诊挂号.Where(g => g.身份证.Equals(param.Idcard) && g.姓名.Equals(param.Name)).ToListAsync();
                foreach (门诊挂号 gh in ghs)
                {
                    var reports = await hisContext.V_LisReport.Where(v => v.PatientCode.Equals(gh.卡号) && v.PatientName.Equals(param.Name)).Where(v => string.IsNullOrEmpty(param.Startdate) || v.ReportDate >= Convert.ToDateTime(param.Startdate + " 00:00:00")).Where(v => string.IsNullOrEmpty(param.Enddate) || v.ReportDate <= Convert.ToDateTime(param.Enddate + " 23:59:59")).ToListAsync();
                    foreach (var report in reports)
                    {
                        reportResult = new ReportResult
                        {
                            ApplyNo = report.ReqNo,
                            ReportTypeName = report.ReportName,
                            TestSample = report.Sampletype,
                            Cjgsj = Convert.ToDateTime(report.ReportDate).ToString("yyyy/MM/dd HH:mm:ss"),
                            OrderNo = (reportResults.Count + 1).ToString(),
                            MainID = report.ReportNo
                        };
                        reportResults.Add(reportResult);
                    }
                }

                List<门诊挂号流水帐> ghls = await ghContext.门诊挂号流水帐.Where(g => g.身份证.Equals(param.Idcard) && g.姓名.Equals(param.Name)).ToListAsync();
                foreach (门诊挂号流水帐 gh in ghls)
                {
                    var reports = await hisContext.V_LisReport.Where(v => v.PatientCode.Equals(gh.卡号) && v.PatientName.Equals(param.Name)).Where(v => string.IsNullOrEmpty(param.Startdate) || v.ReportDate >= Convert.ToDateTime(param.Startdate + " 00:00:00")).Where(v => string.IsNullOrEmpty(param.Enddate) || v.ReportDate <= Convert.ToDateTime(param.Enddate + " 23:59:59")).ToListAsync();
                    foreach (var report in reports)
                    {
                        reportResult = new ReportResult
                        {
                            ApplyNo = report.ReqNo,
                            ReportTypeName = report.ReportName,
                            TestSample = report.Sampletype,
                            Cjgsj = Convert.ToDateTime(report.ReportDate).ToString("yyyy/MM/dd HH:mm:ss"),
                            OrderNo = (reportResults.Count + 1).ToString(),
                            MainID = report.ReportNo
                        };
                        reportResults.Add(reportResult);
                    }
                }
            }
            else
            {
                门诊挂号 gh = await ghContext.门诊挂号.FirstOrDefaultAsync(g => g.门诊号 == Convert.ToInt32(param.Cpatientcode));
                if (gh != null)
                {
                    var reports = await hisContext.V_LisReport.Where(v => v.PatientCode.Equals(gh.卡号) && v.PatientName.Equals(gh.姓名.Trim())).Where(v => string.IsNullOrEmpty(param.Startdate) || v.ReportDate >= Convert.ToDateTime(param.Startdate + " 00:00:00")).Where(v => string.IsNullOrEmpty(param.Enddate) || v.ReportDate <= Convert.ToDateTime(param.Enddate + " 23:59:59")).ToListAsync();
                    foreach (var report in reports)
                    {
                        reportResult = new ReportResult
                        {
                            ApplyNo = report.ReqNo,
                            ReportTypeName = report.ReportName,
                            TestSample = report.Sampletype,
                            Cjgsj = Convert.ToDateTime(report.ReportDate).ToString("yyyy/MM/dd HH:mm:ss"),
                            OrderNo = (reportResults.Count + 1).ToString(),
                            MainID = report.ReportNo
                        };
                        reportResults.Add(reportResult);
                    }
                }
                else
                {
                    门诊挂号流水帐 ghls = await ghContext.门诊挂号流水帐.FirstOrDefaultAsync(g => g.门诊号 == Convert.ToInt32(param.Cpatientcode));
                    if (ghls != null)
                    {
                        var reports = await hisContext.V_LisReport.Where(v => v.PatientCode.Equals(ghls.卡号) && v.PatientName.Equals(ghls.姓名.Trim())).Where(v => string.IsNullOrEmpty(param.Startdate) || v.ReportDate >= Convert.ToDateTime(param.Startdate + " 00:00:00")).Where(v => string.IsNullOrEmpty(param.Enddate) || v.ReportDate <= Convert.ToDateTime(param.Enddate + " 23:59:59")).ToListAsync();
                        foreach (var report in reports)
                        {
                            reportResult = new ReportResult
                            {
                                ApplyNo = report.ReqNo,
                                ReportTypeName = report.ReportName,
                                TestSample = report.Sampletype,
                                Cjgsj = Convert.ToDateTime(report.ReportDate).ToString("yyyy/MM/dd HH:mm:ss"),
                                OrderNo = (reportResults.Count + 1).ToString(),
                                MainID = report.ReportNo
                            };
                            reportResults.Add(reportResult);
                        }
                    }
                }
            }

            if (reportResults.Count > 0)
            {
                reportOutput = new ReportOutput
                {
                    Code = 1,
                    Msg = "获取检验报告成功!",
                    Reportlist = reportResults
                };
            }
            else
            {
                reportOutput = new ReportOutput
                {
                    Code = 0,
                    Msg = "未获取到检验报告!",
                    Reportlist = null
                };
            }

            return new ObjectResult(new
            {
                head = Encrypt.Base64Encode(JsonConvert.SerializeObject(header, Formatting.Indented, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() })),
                body = Encrypt.UrlEncode(Encrypt.Base64Encode(JsonConvert.SerializeObject(reportOutput, Formatting.Indented, new JsonSerializerSettings { ContractResolver = new DefaultContractResolver() })))
            });

        }
        #endregion

        #region 检验报告明细
        /// <summary>
        /// 检验报告明细
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpPost("/api/lab/detail")]
        [ProducesResponseType(typeof(ReportDetailOutput), 200)]
        [ProducesResponseType(typeof(void), 500)]
        public async Task<IActionResult> LisReportDetail([FromForm] OppointmentApiQuery query)
        {
            OppointmentApiHeader header = JsonConvert.DeserializeObject<OppointmentApiHeader>(Encrypt.Base64Decode(query.Head));
            ReportDetailParam param = JsonConvert.DeserializeObject<ReportDetailParam>(Encrypt.Base64Decode(Encrypt.UrlDecode(query.Body)), new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

            if (!VaildToken(header))
            {
                return new ObjectResult("Token验证失败，请检查身份验证信息!");
            }

            ReportDetailOutput reportDetailOutput;
            ReportDetail reportDetail;
            List<ReportDetail> reportDetails = new List<ReportDetail>();

            V_LisReport report = await hisContext.V_LisReport.FirstOrDefaultAsync(r => r.ReportNo.Equals(param.Mainid));
            List<V_LisReportDetail> details = await hisContext.V_LisReportDetail.Where(d => d.ReportNo.Equals(param.Mainid)).ToListAsync();

            if (report != null)
            {
                foreach (var detail in details)
                {
                    reportDetail = new ReportDetail
                    {
                        TestItemChi = detail.ItemName,
                        TestResult = detail.Result,
                        Unit = detail.Unit,
                        ConsultChar = detail.Ckz,
                        TestResultSign = detail.ResultFlag,
                        Testmeans = "",
                        Mutualvalue = ""
                    };
                    reportDetails.Add(reportDetail);
                }

                reportDetailOutput = new ReportDetailOutput
                {
                    Code = 1,
                    Msg = "获取检验报告明细成功!",
                    Mainid = report.ReportNo,
                    StandardSampleNo = report.ReqNo,
                    SectionOffice = "化验室",
                    Jbsj = Convert.ToDateTime(report.Jssj).ToString("yyyy/MM/dd HH:mm:ss"),
                    Name = report.PatientName,
                    TestSample = report.Sampletype,
                    BedNo = "",
                    Jysj = Convert.ToDateTime(report.Jysj).ToString("yyyy/MM/dd HH:mm:ss"),
                    Sex = report.Sex,
                    SampleStatus = "正常",
                    Kdr = "",
                    Cjgsj = Convert.ToDateTime(report.ReportDate).ToString("yyyy/MM/dd HH:mm:ss"),
                    Age = report.Age.ToString(),
                    Jyr = report.Jyr,
                    Diagnose = "",
                    Cbsj = Convert.ToDateTime(report.Cysj).ToString("yyyy/MM/dd HH:mm:ss"),
                    Hdr = report.Shr,
                    Cbr = "",
                    ReportFormat = "检验报告标准样式",
                    ReportTypeName = report.ReportName,
                    MzZyNo = report.PatientCode,
                    Kdsj = "",
                    Details = reportDetails
                };
            }
            else
            {
                reportDetailOutput = new ReportDetailOutput
                {
                    Code = 0,
                    Msg = "获取检验报告明细失败!",
                    Details = null
                };
            }

            return new ObjectResult(new
            {
                head = Encrypt.Base64Encode(JsonConvert.SerializeObject(header, Formatting.Indented, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() })),
                body = Encrypt.UrlEncode(Encrypt.Base64Encode(JsonConvert.SerializeObject(reportDetailOutput, Formatting.Indented, new JsonSerializerSettings { ContractResolver = new DefaultContractResolver() })))
            });

        }
        #endregion

        #endregion
    }
}
