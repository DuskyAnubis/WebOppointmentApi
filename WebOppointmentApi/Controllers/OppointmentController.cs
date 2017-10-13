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

        #region 平台提供接口

        #region 同步医院信息
        /// <summary>
        /// 同步医院信息
        /// </summary>
        /// <returns></returns>
        [HttpPost("SynchronizingHospital")]
        //[Authorize]
        [ProducesResponseType(typeof(OppointmentApiBody), 201)]
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
        [ProducesResponseType(typeof(OppointmentApiBody), 201)]
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
        [ProducesResponseType(typeof(OppointmentApiBody), 201)]
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
        [ProducesResponseType(typeof(OppointmentApiBody), 201)]
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
                var works = mapper.Map<List<SynchronizingWork>>(schedulings);
                var worksInput = new SynchronizingWorkInput
                {
                    Hospid = apiOptions.HospitalId,
                    Opcode = param.Opcode,
                    Atype = 0,
                    Deptid = param.OrgId.ToString(),
                    Works = works
                };
                var input = new { head = header, body = worksInput };
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
                var works = mapper.Map<List<SynchronizingWork>>(schedulings);
                var doctorsInput = new SynchronizingWorkInput
                {
                    Hospid = apiOptions.HospitalId,
                    Opcode = param.Opcode,
                    Atype = 0,
                    Deptid = param.OrgId.ToString(),
                    Works = works
                };
                var input = new { head = header, body = doctorsInput };
                return new ObjectResult(input);
            }
        }
        #endregion

        #region 同步预约执行情况
        /// <summary>
        /// 同步预约执行情况
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost("SynchronizingOrderStates")]
        //[Authorize]
        [ValidateModel]
        [ProducesResponseType(typeof(OppointmentApiBody), 201)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(ValidationError), 422)]
        [ProducesResponseType(typeof(void), 500)]
        public async Task<IActionResult> SynchronizingOrderStates([FromBody]SynchronizingOrderStateParam param)
        {
            var header = GetOppointmentApiHeader();
            var registereds = await dbContext.Registereds.Where(r => r.DoctorDate == Convert.ToDateTime(param.Date)).ToListAsync();

            if (registereds == null || registereds.Count == 0)
            {
                return NotFound(Json(new { Error = "同步失败，预约信息不存在" }));
            }

            var orders = mapper.Map<List<SynchronizingStateOrder>>(registereds);
            var ordersInput = new SynchronizingOrderStateInput
            {
                Hospid = apiOptions.HospitalId,
                Orders = orders
            };
            var input = new { head = header, body = ordersInput };

            return new ObjectResult(input);
        }
        #endregion

        #region 同步停诊状况
        /// <summary>
        /// 同步停/复诊情况
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost("SynchronizingStop")]
        //[Authorize]
        [ValidateModel]
        [ProducesResponseType(typeof(OppointmentApiBody), 201)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(ValidationError), 422)]
        [ProducesResponseType(typeof(void), 500)]
        public async Task<IActionResult> SynchronizingStop([FromBody]SynchronizingsStopParam param)
        {
            var header = GetOppointmentApiHeader();
            var scheduling = await dbContext.Schedulings
                .Include(s => s.User)
                .Include(s => s.User.Organazition)
                .FirstOrDefaultAsync(s => s.Id == param.Id && s.Status.Equals("正常") && s.EndTreatDate != null);

            if (scheduling == null)
            {
                return NotFound(Json(new { Error = "同步失败，不存在或已同步该信息" }));
            }

            var stop = mapper.Map<SynchronizingsStop>(scheduling);
            if (stop.Endtreat == 1)
            {
                stop.Reason = "";
            }
            var stops = new List<SynchronizingsStop>
            {
                stop
            };
            var stopsInput = new SynchronizingsStopInput
            {
                Hospid = apiOptions.HospitalId,
                Values = stops
            };
            var input = new { head = header, body = stopsInput };

            return new ObjectResult(input);
        }

        /// <summary>
        /// 同步所有未同步停/复诊信息
        /// </summary>
        /// <returns></returns>
        [HttpPost("SynchronizingStops")]
        //[Authorize]
        [ValidateModel]
        [ProducesResponseType(typeof(OppointmentApiBody), 201)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(ValidationError), 422)]
        [ProducesResponseType(typeof(void), 500)]
        public async Task<IActionResult> SynchronizingStops()
        {
            var header = GetOppointmentApiHeader();
            var schedulings = await dbContext.Schedulings
                .Include(s => s.User)
                .Include(s => s.User.Organazition)
                .Where(s => s.EndTreatDate != null && s.Status == "正常").ToListAsync();

            if (schedulings == null || schedulings.Count == 0)
            {
                return NotFound(Json(new { Error = "同步失败，不存在未同步的停/复诊信息" }));
            }

            var stops = mapper.Map<List<SynchronizingsStop>>(schedulings);

            var stopsInput = new SynchronizingsStopInput
            {
                Hospid = apiOptions.HospitalId,
                Values = stops
            };
            var input = new { head = header, body = stopsInput };

            return new ObjectResult(input);
        }
        #endregion

        #region 同步医院预约挂号
        /// <summary>
        /// 同步医院预约挂号
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost("SynchronizingOrder")]
        //[Authorize]
        [ValidateModel]
        [ProducesResponseType(typeof(OppointmentApiBody), 201)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(ValidationError), 422)]
        [ProducesResponseType(typeof(void), 500)]
        public async Task<IActionResult> SynchronizingOrder([FromBody]SynchronizingsOrderParam param)
        {
            var header = GetOppointmentApiHeader();
            var registered = await dbContext.Registereds
                .Include(r => r.Scheduling)
                .Include(r => r.Scheduling.User)
                .Include(r => r.Scheduling.User.Organazition)
                .Include(r => r.Scheduling.Registereds)
                .FirstOrDefaultAsync(r => r.Id == param.Id && r.Status.Equals("正常"));

            if (registered == null)
            {
                return NotFound(Json(new { Error = "同步失败，不存在或已同步该信息" }));
            }

            var order = mapper.Map<SynchronizingsOrder>(registered);
            order.Hospid = apiOptions.HospitalId;
            order.Hospname = apiOptions.HospitalName;

            var input = new { head = header, body = order };

            return new ObjectResult(input);
        }

        /// <summary>
        /// 同步所有未同步的医院预约挂号
        /// </summary>
        /// <returns></returns>
        [HttpPost("SynchronizingOrders")]
        //[Authorize]
        [ValidateModel]
        [ProducesResponseType(typeof(OppointmentApiBody), 201)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(ValidationError), 422)]
        [ProducesResponseType(typeof(void), 500)]
        public async Task<IActionResult> SynchronizingOrders()
        {
            var header = GetOppointmentApiHeader();
            var registereds = await dbContext.Registereds
                .Include(r => r.Scheduling)
                .Include(r => r.Scheduling.User)
                .Include(r => r.Scheduling.User.Organazition)
                .Include(r => r.Scheduling.Registereds)
                .Where(r => r.Status.Equals("正常")).ToListAsync();

            if (registereds == null || registereds.Count == 0)
            {
                return NotFound(Json(new { Error = "同步失败，不存在未同步的医院预约挂号信息" }));
            }

            var orders = mapper.Map<List<SynchronizingsOrder>>(registereds);
            foreach (var order in orders)
            {
                order.Hospid = apiOptions.HospitalId;
                order.Hospname = apiOptions.HospitalName;
            }

            var input = new { head = header, body = orders };

            return new ObjectResult(input);
        }
        #endregion

        #region 同步医院实际取号量
        /// <summary>
        /// 同步医院实际取号量
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost("SynchronizingMed")]
        //[Authorize]
        [ValidateModel]
        [ProducesResponseType(typeof(OppointmentApiBody), 201)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(ValidationError), 422)]
        [ProducesResponseType(typeof(void), 500)]
        public async Task<IActionResult> SynchronizingMed([FromBody]SynchronizingsMedParam param)
        {
            var header = GetOppointmentApiHeader();
            var schedulings = await dbContext.Schedulings
                .Include(s => s.Registereds)
                .Where(s => s.SurgeryDate == Convert.ToDateTime(param.Date))
                .ToListAsync();

            if (schedulings == null || schedulings.Count == 0)
            {
                return NotFound(Json(new { Error = "同步失败，排班信息不存在" }));
            }

            var meds = mapper.Map<List<SynchronizingsMed>>(schedulings);

            var medsInput = new SynchronizingsMedInput
            {
                Hospid = apiOptions.HospitalId,
                Works = meds
            };
            var input = new { head = header, body = medsInput };

            return new ObjectResult(input);
        }
        #endregion

        #endregion

        #region HIS提供接口

        #region 更新科室信息
        /// <summary>
        /// 更新科室信息
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpPost("/api/v1/dept/update")]
        [ProducesResponseType(typeof(UpdateDeptOutput), 201)]
        [ProducesResponseType(typeof(void), 500)]
        public async Task<IActionResult> UpdateDept([FromBody]OppointmentApiQuery query)
        {
            OppointmentApiHeader header = JsonConvert.DeserializeObject<OppointmentApiHeader>(Encrypt.Base64Decode(query.Head));
            UpdateDeptParam param = JsonConvert.DeserializeObject<UpdateDeptParam>(Encrypt.Base64Decode(Encrypt.UrlDecode(query.Body)));

            var orgs = await dbContext.Orgnazitions.Where(o => o.Id == Convert.ToInt32(param.Id)).ToListAsync();
            var depts = mapper.Map<List<UpdateDept>>(orgs);
            var deptsOutput = new UpdateDeptOutput
            {
                Hospid = apiOptions.HospitalId,
                Depts = depts
            };
            var input = new { head = header, body = deptsOutput };
            return new ObjectResult(input);
        }
        #endregion

        #region 更新医生信息
        /// <summary>
        /// 更新医生信息
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpPost("/api/v1/doctor/update")]
        [ProducesResponseType(typeof(UpdateDeptOutput), 201)]
        [ProducesResponseType(typeof(void), 500)]
        public async Task<IActionResult> UpdateDoctor([FromBody]OppointmentApiQuery query)
        {
            OppointmentApiHeader header = JsonConvert.DeserializeObject<OppointmentApiHeader>(Encrypt.Base64Decode(query.Head));
            UpdateDoctorParam param = JsonConvert.DeserializeObject<UpdateDoctorParam>(Encrypt.Base64Decode(Encrypt.UrlDecode(query.Body)));

            var users = await dbContext.Users.Include(u => u.Organazition).Where(u => u.Id == Convert.ToInt32(param.Id)).ToListAsync();
            var doctors = mapper.Map<List<UpdateDoctor>>(users);
            var doctorsOutput = new UpdateDoctorOutput
            {
                Hospid = apiOptions.HospitalId,
                Doctors = doctors
            };
            var input = new { head = header, body = doctorsOutput };
            return new ObjectResult(input);
        }
        #endregion

        #region 更新排班信息
        /// <summary>
        /// 更新排班信息
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpPost("/api/v1/work/update")]
        [ProducesResponseType(typeof(UpdateDeptOutput), 201)]
        [ProducesResponseType(typeof(void), 500)]
        public async Task<IActionResult> UpdateWork([FromBody]OppointmentApiQuery query)
        {
            OppointmentApiHeader header = JsonConvert.DeserializeObject<OppointmentApiHeader>(Encrypt.Base64Decode(query.Head));
            UpdateWorkParam param = JsonConvert.DeserializeObject<UpdateWorkParam>(Encrypt.Base64Decode(Encrypt.UrlDecode(query.Body)));

            if (param.Optype == 1)
            {
                string sql = "";
                if (param.Dates == "")
                {
                    sql = "select * from Schedulings";
                }
                else
                {
                    string[] datesArray = param.Dates.Split(',');
                    string dates = "";
                    foreach (string str in datesArray)
                    {
                        dates += "'" + str + "',";
                    }
                    dates = dates.Substring(0, dates.Length - 1);
                    sql = $"select * from Schedulings where SurgeryDate in ({dates})";
                }
                var schedulings = await dbContext.Schedulings
                    .Include(s => s.User)
                    .Include(s => s.User.Organazition)
                    .Include(s => s.Registereds)
                    .FromSql(sql)
                    .Where(s => s.User.Organazition.Id == Convert.ToInt32(param.Deptid))
                    .Where(s => s.UserId == Convert.ToInt32(param.Docid))
                    .ToListAsync();

                var works = mapper.Map<List<UpdateWork>>(schedulings);
                var worksOutout = new UpdateWorkOutput
                {
                    Hospid = apiOptions.HospitalId,
                    Opcode = param.Optype,
                    Atype = 0,
                    Deptid = param.Deptid,
                    Works = works
                };

                var input = new { head = header, body = worksOutout };
                return new ObjectResult(input);
            }
            else
            {
                string sql = $"select * from Schedulings where Id in ({param.Ids})";

                var schedulings = await dbContext.Schedulings
                    .Include(s => s.User)
                    .Include(s => s.User.Organazition)
                    .Include(s => s.Registereds)
                    .FromSql(sql)
                    .ToListAsync();

                var works = mapper.Map<List<UpdateWork>>(schedulings);
                var worksOutout = new UpdateWorkOutput
                {
                    Hospid = apiOptions.HospitalId,
                    Opcode = param.Optype,
                    Atype = 0,
                    Deptid = param.Deptid,
                    Works = works
                };

                var input = new { head = header, body = worksOutout };
                return new ObjectResult(input);
            }
        }
        #endregion

        #endregion

    }
}