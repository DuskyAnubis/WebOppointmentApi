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

        #endregion

        #region 检验报告同步

        #endregion

        #region 费用清单同步

        #endregion

        #endregion

        #region HIS提供接口

        #endregion
    }
}