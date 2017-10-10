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

        #region 同步医院信息
        /// <summary>
        /// 同步医院信息
        /// </summary>
        /// <returns></returns>
        [HttpPost("SynchronizingHospital")]
        //[Authorize]
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
        #endregion

        #region 同步科室信息
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
                var orgs = await dbContext.Orgnazitions.Where(q => q.OrgTypeCode.Equals("01")).ToListAsync();
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
                    var org = await dbContext.Orgnazitions.FirstOrDefaultAsync(o => o.Id == id && o.OrgTypeCode.Equals("01"));
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

        #region 同步医生信息
        /// <summary>
        /// 同步医生信息
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost("SynchronizingDoctor")]
        //[Authorize]
        [ValidateModel]
        [ProducesResponseType(typeof(OppointmentApiBodyOutput), 201)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(ValidationError), 422)]
        [ProducesResponseType(typeof(void), 500)]
        public async Task<IActionResult> SynchronizingDoctor([FromBody]SynchronizingDoctorParam param)
        {
            var header = GetOppointmentApiHeader();
            if (param.Opcode == 10)
            {
                var users = await dbContext.Users.Include(u => u.Organazition).Where(q => param.OrgId == 0 || q.OrganazitionId == param.OrgId).Where(q => q.UserTypeCode.Equals("01")).ToListAsync();
                if (users == null || users.Count == 0)
                {
                    return NotFound(Json(new { Error = "同步失败，医生信息不存在" }));
                }
                var doctors = mapper.Map<List<SynchronizingDoctor>>(users);
                var doctorsInput = new SynchronizingDoctorInput
                {
                    Hospid = apiOptions.HospitalId,
                    Opcode = param.Opcode,
                    Doctors = doctors
                };
                var input = new { head = header, body = doctorsInput };
                return new ObjectResult(input);
            }
            else
            {
                if (param.Ids.Length == 0)
                {
                    return BadRequest(Json(new { Error = "请求参数错误" }));
                }
                int[] ids = param.Ids;
                var users = new List<User>();
                foreach (var id in ids)
                {
                    var user = await dbContext.Users.Include(u => u.Organazition).FirstOrDefaultAsync(u => u.Id == id && u.UserTypeCode.Equals("01"));
                    if (user != null)
                    {
                        users.Add(user);
                    }
                }
                if (users.Count == 0)
                {
                    return NotFound(Json(new { Error = "同步失败，医生信息不存在" }));
                }
                var doctors = mapper.Map<List<SynchronizingDoctor>>(users);
                var doctorsInput = new SynchronizingDoctorInput
                {
                    Hospid = apiOptions.HospitalId,
                    Opcode = param.Opcode,
                    Doctors = doctors
                };
                var input = new { head = header, body = doctorsInput };
                return new ObjectResult(input);
            }
        }
        #endregion

        #region 同步排班信息
        /// <summary>
        /// 同步排班信息
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost("SynchronizingWork")]
        //[Authorize]
        [ValidateModel]
        [ProducesResponseType(typeof(OppointmentApiBodyOutput), 201)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(ValidationError), 422)]
        [ProducesResponseType(typeof(void), 500)]
        public async Task<IActionResult> SynchronizingWork([FromBody]SynchronizingWorkParam param)
        {
            var header = GetOppointmentApiHeader();
            if (param.Opcode == 10)
            {
                var schedulings = await dbContext.Schedulings
                    .Include(s => s.User)
                    .Include(s => s.User.Organazition)
                    .Include(s => s.Registereds)
                    .Where(q => q.User.OrganazitionId == param.OrgId)
                    .Where(q => q.SurgeryDate >= Convert.ToDateTime(param.Date))
                    .ToListAsync();
                if (schedulings == null || schedulings.Count == 0)
                {
                    return NotFound(Json(new { Error = "同步失败，排班信息不存在" }));
                }
                var wroks = mapper.Map<List<SynchronizingWork>>(schedulings);
                var wroksInput = new SynchronizingWorkInput
                {
                    Hospid = apiOptions.HospitalId,
                    Opcode = param.Opcode,
                    Atype = 0,
                    Deptid = param.OrgId,
                    Works = wroks
                };
                var input = new { head = header, body = wroksInput };
                return new ObjectResult(input);
            }
            else
            {
                if (param.Ids.Length == 0)
                {
                    return BadRequest(Json(new { Error = "请求参数错误" }));
                }
                int[] ids = param.Ids;
                var schedulings = new List<Scheduling>();
                foreach (var id in ids)
                {
                    var scheduling = await dbContext.Schedulings
                        .Include(s => s.User)
                        .Include(s => s.User.Organazition)
                        .Include(s => s.Registereds)
                        .FirstOrDefaultAsync(s => s.Id == id && s.User.OrganazitionId == param.OrgId);
                    if (scheduling != null)
                    {
                        schedulings.Add(scheduling);
                    }
                }
                if (schedulings.Count == 0)
                {
                    return NotFound(Json(new { Error = "同步失败，该排班信息不存在" }));
                }
                var wroks = mapper.Map<List<SynchronizingWork>>(schedulings);
                var doctorsInput = new SynchronizingWorkInput
                {
                    Hospid = apiOptions.HospitalId,
                    Opcode = param.Opcode,
                    Atype = 0,
                    Deptid = param.OrgId,
                    Works = wroks
                };
                var input = new { head = header, body = doctorsInput };
                return new ObjectResult(input);
            }
        }
        #endregion

        #endregion

        #region HIS提供接口

        #endregion

    }
}