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

namespace WebOppointmentApi.Controllers
{
    [Produces("application/json")]
    [Route("api/v1/Oppointments")]
    public class OppointmentController : Controller
    {
        private readonly ApiContext dbContext;
        private readonly IMapper mapper;
        private readonly OppointmentApiOptions apiOptions;

        public OppointmentController(ApiContext dbContext, IMapper mapper, OppointmentApiOptions apiOptions)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
            this.apiOptions = apiOptions;
        }

        private OppointmentApiHeaderInput GetOppointmentApiHeader()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            string timeSpan = Convert.ToInt64(ts.TotalSeconds).ToString();
            string token = Encrypt.Md5Encrypt(apiOptions.SecretKey + apiOptions.FromType + timeSpan);

            OppointmentApiHeaderInput headerInput = new OppointmentApiHeaderInput
            {
                Token = token,
                Version = apiOptions.Version,
                Fromtype = apiOptions.FromType,
                Sessionid = apiOptions.FromType + timeSpan,
                Time = timeSpan
            };

            return headerInput;
        }

        #region 平台提供接口
        /// <summary>
        /// 同步医院信息
        /// </summary>
        /// <returns></returns>
        [HttpPost("SynchronizingHospital")]
        [Authorize]
        [ProducesResponseType(typeof(OppointmentApiBodyOutput), 201)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(void), 500)]
        public async Task<IActionResult> SynchronizingHospital()
        {
            var header = GetOppointmentApiHeader();
            var hospital = await dbContext.Hospitals.FirstOrDefaultAsync();

            if (hospital == null)
            {
                return NotFound(Json(new { Error = "同步失败，医院信息不存在" }));
            }

            var hospitalInput = mapper.Map<SynchronizingHospitalInput>(hospital);
            hospitalInput.Id = apiOptions.HospitalId;

            var input = new { head = header, body = hospitalInput };

            return new ObjectResult(input);
        }

        /// <summary>
        /// 同步科室信息
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost("SynchronizingDept")]
        //[Authorize]
        [ValidateModel]
        [ProducesResponseType(typeof(OppointmentApiBodyOutput), 201)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(ValidationError), 422)]
        [ProducesResponseType(typeof(void), 500)]
        public async Task<IActionResult> SynchronizingDept([FromBody]SynchronizingDeptParam param)
        {
            var header = GetOppointmentApiHeader();
            if (param.Opcode == 10)
            {
                var orgs = await dbContext.Orgnazitions.ToListAsync();
                if (orgs == null || orgs.Count == 0)
                {
                    return NotFound(Json(new { Error = "同步失败，科室信息不存在" }));
                }
                var depts = mapper.Map<List<SynchronizingDept>>(orgs);
                var deptsInput = new SynchronizingDeptInput
                {
                    Hospid = apiOptions.HospitalId,
                    Opcode = param.Opcode,
                    Depts = depts
                };
                var input = new { head = header, body = deptsInput };
                return new ObjectResult(input);
            }
            else
            {
                if (param.Ids.Length == 0)
                {
                    return BadRequest(Json(new { Error = "请求参数错误" }));
                }
                int[] ids = param.Ids;
                var orgs = new List<Orgnazition>();
                foreach (var id in ids)
                {
                    var org = await dbContext.Orgnazitions.FirstOrDefaultAsync(o => o.Id == id);
                    if (org != null)
                    {
                        orgs.Add(org);
                    }
                }
                if (orgs.Count == 0)
                {
                    return NotFound(Json(new { Error = "同步失败，科室信息不存在" }));
                }
                var depts = mapper.Map<List<SynchronizingDept>>(orgs);
                var deptsInput = new SynchronizingDeptInput
                {
                    Hospid = apiOptions.HospitalId,
                    Opcode = param.Opcode,
                    Depts = depts
                };
                var input = new { head = header, body = deptsInput };
                return new ObjectResult(input);
            }
        }

        #endregion

        #region HIS提供接口

        #endregion

    }
}