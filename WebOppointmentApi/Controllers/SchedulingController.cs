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

namespace WebOppointmentApi.Controllers
{
    [Produces("application/json")]
    [Route("api/v1/Scheduling")]
    //[Authorize]
    public class SchedulingController : Controller
    {
        private readonly ApiContext dbContext;
        private readonly IMapper mapper;

        public SchedulingController(ApiContext dbContext, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
        }

        #region 基本操作
        /// <summary>
        /// 获得排班列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<OrgOutput>), 200)]
        [ProducesResponseType(typeof(void), 500)]
        public async Task<IEnumerable<SchedulingOutput>> GetSchedulings(SchedulingQueryInput input)
        {
            int pageIndex = input.Page - 1;
            int Per_Page = input.Per_Page;
            string sortBy = input.SortBy;

            IQueryable<Scheduling> query = dbContext.Schedulings
                .Include(q => q.User)
                .Include(q => q.User.Organazition)
                .AsQueryable<Scheduling>();

            query = query.Where(q => input.OrganazitionId == 0 || q.User.OrganazitionId == input.OrganazitionId);
            query = query.Where(q => input.UserId == 0 || q.UserId == input.UserId);
            query = query.Where(q => !input.SurgeryDate.HasValue || q.SurgeryDate.Equals(input.SurgeryDate));
            query = query.Where(q => string.IsNullOrEmpty(input.EndTreatCode) || q.EndTreatCode.Equals(input.EndTreatCode));
            query = query.OrderBy(sortBy);

            var totalCount = query.Count();
            var totalPages = (int)Math.Ceiling((double)totalCount / Per_Page);

            HttpContext.Response.Headers.Add("X-TotalCount", JsonConvert.SerializeObject(totalCount));
            HttpContext.Response.Headers.Add("X-TotalPage", JsonConvert.SerializeObject(totalPages));

            query = query.Skip(pageIndex * Per_Page).Take(Per_Page);

            List<Scheduling> schedulings = await query.ToListAsync();
            List<SchedulingOutput> list = mapper.Map<List<SchedulingOutput>>(schedulings);

            return list;
        }

        /// <summary>
        /// 获得排班信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}", Name = "GetScheduling")]
        [ProducesResponseType(typeof(UserOutput), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(void), 500)]
        public async Task<IActionResult> GetScheduling([FromRoute]int id)
        {
            Scheduling scheduling = await dbContext.Schedulings
              .Include(q => q.User)
              .Include(q => q.User.Organazition)
              .FirstOrDefaultAsync(s => s.Id == id);

            if (scheduling == null)
            {
                return NotFound(Json(new { Error = "该排班不存在" }));
            }

            SchedulingOutput output = mapper.Map<SchedulingOutput>(scheduling);

            return new ObjectResult(output);
        }

        /// <summary>
        /// 创建排班
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateModel]
        [ProducesResponseType(typeof(SchedulingOutput), 201)]
        [ProducesResponseType(typeof(ValidationError), 422)]
        [ProducesResponseType(typeof(void), 500)]
        public async Task<IActionResult> CreateScheduling([FromBody]SchedulingCreateInput input)
        {
            var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == input.UserId);
            if (user == null)
            {
                return BadRequest(Json(new { Error = "不存在该医生" }));
            }

            var scheduling = mapper.Map<Scheduling>(input);
            scheduling.SchedulingDate = DateTime.Now;

            dbContext.Schedulings.Add(scheduling);
            await dbContext.SaveChangesAsync();

            return CreatedAtRoute("GetScheduling", new { id = scheduling.Id }, mapper.Map<SchedulingOutput>(scheduling));
        }

        /// <summary>
        /// 修改排班
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [ValidateModel]
        [ProducesResponseType(typeof(void), 204)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(ValidationError), 422)]
        [ProducesResponseType(typeof(void), 500)]
        public async Task<IActionResult> UpdateScheduling([FromRoute]int id, [FromBody]SchedulingUpdateInput input)
        {
            if (input.Id != id)
            {
                return BadRequest(Json(new { Error = "请求参数错误" }));
            }
            var scheduling = await dbContext.Schedulings.FirstOrDefaultAsync(s => s.Id == id);
            if (scheduling == null)
            {
                return NotFound(Json(new { Error = "该排班不存在" }));
            }

            dbContext.Entry(scheduling).CurrentValues.SetValues(input);
            await dbContext.SaveChangesAsync();

            return new NoContentResult();
        }

        /// <summary>
        /// 更新排班
        /// </summary>
        /// <param name="id"></param>
        /// <param name="patchDoc"></param>
        /// <returns></returns>
        [HttpPatch("{id}")]
        [ProducesResponseType(typeof(SchedulingOutput), 201)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(ValidationError), 422)]
        [ProducesResponseType(typeof(void), 500)]
        public async Task<IActionResult> PatchScheduling([FromRoute]int id, [FromBody]JsonPatchDocument<SchedulingUpdateInput> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest(Json(new { Error = "请求参数错误" }));
            }
            var scheduling = await dbContext.Schedulings.FirstOrDefaultAsync(s => s.Id == id);
            if (scheduling == null)
            {
                return NotFound(Json(new { Error = "该排班不存在" }));
            }

            var input = mapper.Map<SchedulingUpdateInput>(scheduling);
            patchDoc.ApplyTo(input);

            TryValidateModel(input);
            if (!ModelState.IsValid)
            {
                return new ValidationFailedResult(ModelState);
            }

            dbContext.Entry(scheduling).CurrentValues.SetValues(input);
            await dbContext.SaveChangesAsync();

            return CreatedAtRoute("GetScheduling", new { id = scheduling.Id }, mapper.Map<SchedulingOutput>(scheduling));
        }

        /// <summary>
        /// 删除排班
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(void), 204)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(void), 500)]
        public async Task<IActionResult> DeleteScheduling([FromRoute]int id)
        {
            var scheduling = await dbContext.Schedulings.FirstOrDefaultAsync(s => s.Id == id);
            if (scheduling == null)
            {
                return NotFound(Json(new { Error = "该排班不存在" }));
            }

            scheduling.Status = "删除";
            dbContext.Schedulings.Update(scheduling);
            await dbContext.SaveChangesAsync();

            return new NoContentResult();
        }

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="ids">ID数组</param>
        /// <returns></returns>
        [HttpDelete]
        [ProducesResponseType(typeof(void), 204)]
        [ProducesResponseType(typeof(void), 500)]
        public async Task<IActionResult> BatchDelete([FromBody]int[] ids)
        {
            for (int i = 0; i < ids.Length; i++)
            {
                var scheduling = await dbContext.Schedulings.FirstOrDefaultAsync(s => s.Id == ids[i]);
                if (scheduling != null)
                {
                    scheduling.Status = "删除";
                    dbContext.Schedulings.Update(scheduling);
                }
            }

            await dbContext.SaveChangesAsync();

            return new NoContentResult();
        }
        #endregion

        #region 停诊与复诊
        /// <summary>
        ///  停诊
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPut("WithEndTreat/{id}")]
        [ValidateModel]
        [ProducesResponseType(typeof(void), 204)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(ValidationError), 422)]
        [ProducesResponseType(typeof(void), 500)]
        public async Task<IActionResult> EndTreatScheduling([FromRoute]int id, [FromBody]SchedulingEndTreatInput input)
        {
            if (input.Id != id)
            {
                return BadRequest(Json(new { Error = "请求参数错误" }));
            }
            var scheduling = await dbContext.Schedulings.FirstOrDefaultAsync(s => s.Id == id);
            if (scheduling == null)
            {
                return NotFound(Json(new { Error = "该排班不存在" }));
            }

            scheduling.EndTreatCode = "1";
            scheduling.EndTreatName = "已停诊";
            scheduling.EndTreatDate = DateTime.Now;
            scheduling.EndTreatReason = input.EndTreatReason;

            dbContext.Schedulings.Update(scheduling);
            await dbContext.SaveChangesAsync();

            return new NoContentResult();
        }

        /// <summary>
        ///  复诊
        /// </summary>
        /// <param name="id"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPut("WithRecoveryTreat/{id}")]
        [ValidateModel]
        [ProducesResponseType(typeof(void), 204)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(ValidationError), 422)]
        [ProducesResponseType(typeof(void), 500)]
        public async Task<IActionResult> RecoveryTreatScheduling([FromRoute]int id, [FromBody]SchedulingRecoveryTreatInput input)
        {
            if (input.Id != id)
            {
                return BadRequest(Json(new { Error = "请求参数错误" }));
            }
            var scheduling = await dbContext.Schedulings.FirstOrDefaultAsync(s => s.Id == id);
            if (scheduling == null)
            {
                return NotFound(Json(new { Error = "该排班不存在" }));
            }

            scheduling.EndTreatCode = "0";
            scheduling.EndTreatName = "未停诊";
            scheduling.RecoveryTreatDate = DateTime.Now;

            dbContext.Schedulings.Update(scheduling);
            await dbContext.SaveChangesAsync();

            return new NoContentResult();
        }

        #endregion
    }
}