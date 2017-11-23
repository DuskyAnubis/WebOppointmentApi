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
    [Route("api/v1/Registered")]
    [Authorize]
    public class RegisteredController : Controller
    {
        private readonly ApiContext dbContext;
        private readonly HisContext hisContext;
        private readonly IMapper mapper;

        public RegisteredController(ApiContext dbContext, HisContext hisContext, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.hisContext = hisContext;
            this.mapper = mapper;
        }

        #region 基本操作
        /// <summary>
        /// 获得预约列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<RegisteredOutput>), 200)]
        [ProducesResponseType(typeof(void), 500)]
        public async Task<IEnumerable<RegisteredOutput>> GetRegistereds(RegisteredQueryInput input)
        {
            int pageIndex = input.Page - 1;
            int Per_Page = input.Per_Page;
            string sortBy = input.SortBy;

            IQueryable<Registered> query = dbContext.Registereds
                .Include(r => r.Scheduling)
                .Include(r => r.Scheduling.User)
                .Include(r => r.Scheduling.User.Organazition)
                .AsQueryable<Registered>();

            query = query.Where(q => input.OrganazitionId == 0 || q.Scheduling.User.OrganazitionId == input.OrganazitionId);
            query = query.Where(q => input.UserId == 0 || q.Scheduling.UserId == input.UserId);
            query = query.Where(q => !input.DoctorDate.HasValue || q.DoctorDate.Equals(input.DoctorDate));
            query = query.Where(q => string.IsNullOrEmpty(input.Name) || q.Name.Contains(input.Name));
            query = query.Where(q => string.IsNullOrEmpty(input.OrderId) || q.OrderId.Equals(input.OrderId));
            query = query.Where(q => string.IsNullOrEmpty(input.RegisteredStateCode) || q.RegisteredStateCode.Equals(input.RegisteredStateCode));
            query = query.OrderBy(sortBy);

            var totalCount = query.Count();
            var totalPages = (int)Math.Ceiling((double)totalCount / Per_Page);

            HttpContext.Response.Headers.Add("X-TotalCount", JsonConvert.SerializeObject(totalCount));
            HttpContext.Response.Headers.Add("X-TotalPage", JsonConvert.SerializeObject(totalPages));

            query = query.Skip(pageIndex * Per_Page).Take(Per_Page);

            List<Registered> registereds = await query.ToListAsync();
            List<RegisteredOutput> list = mapper.Map<List<RegisteredOutput>>(registereds);

            return list;
        }

        /// <summary>
        /// 获得预约信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}", Name = "GetRegistered")]
        [ProducesResponseType(typeof(RegisteredOutput), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(void), 500)]
        public async Task<IActionResult> GetRegistered([FromRoute]int id)
        {
            Registered registered = await dbContext.Registereds
              .Include(r => r.Scheduling)
              .Include(r => r.Scheduling.User)
              .Include(r => r.Scheduling.User.Organazition)
              .FirstOrDefaultAsync(r => r.Id == id);

            if (registered == null)
            {
                return NotFound(Json(new { Error = "该预约不存在" }));
            }

            RegisteredOutput output = mapper.Map<RegisteredOutput>(registered);

            return new ObjectResult(output);
        }

        /// <summary>
        /// 创建预约
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateModel]
        [ProducesResponseType(typeof(RegisteredOutput), 201)]
        [ProducesResponseType(typeof(ValidationError), 422)]
        [ProducesResponseType(typeof(void), 500)]
        public async Task<IActionResult> CreateRegistered([FromBody]RegisteredCreateInput input)
        {
            var scheduling = await dbContext.Schedulings.FirstOrDefaultAsync(s => s.Id == input.SchedulingId);
            if (scheduling == null)
            {
                return BadRequest(Json(new { Error = "不存在该预约班次" }));
            }

            if (input.DoctorDate <= DateTime.Now)
            {
                return BadRequest(Json(new { Error = "请至少提前一天预约" }));
            }

            var remainCount = scheduling.MaxCount - await dbContext.Registereds.CountAsync(r => r.SchedulingId == input.SchedulingId && r.RegisteredStateCode != "3");
            if (remainCount <= 0)
            {
                return BadRequest(Json(new { Error = "该预约班次预约已满" }));
            }

            var registered = mapper.Map<Registered>(input);
            registered.TransactionDate = DateTime.Now;
            registered.OrderId = "OPRE" + DateTime.Now.ToString("yyyyMMddhhmmss");

            dbContext.Registereds.Add(registered);
            await dbContext.SaveChangesAsync();

            return CreatedAtRoute("GetRegistered", new { id = registered.Id }, mapper.Map<RegisteredOutput>(registered));
        }

        /// <summary>
        /// 修改预约
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
        public async Task<IActionResult> UpdateRegistered([FromRoute]int id, [FromBody]RegisteredUpdateInput input)
        {
            if (input.Id != id)
            {
                return BadRequest(Json(new { Error = "请求参数错误" }));
            }
            var registered = await dbContext.Registereds.FirstOrDefaultAsync(r => r.Id == id);
            if (registered == null)
            {
                return NotFound(Json(new { Error = "该预约不存在" }));
            }

            dbContext.Entry(registered).CurrentValues.SetValues(input);
            await dbContext.SaveChangesAsync();

            return new NoContentResult();
        }

        /// <summary>
        /// 更新预约
        /// </summary>
        /// <param name="id"></param>
        /// <param name="patchDoc"></param>
        /// <returns></returns>
        [HttpPatch("{id}")]
        [ProducesResponseType(typeof(RegisteredOutput), 201)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(ValidationError), 422)]
        [ProducesResponseType(typeof(void), 500)]
        public async Task<IActionResult> PatchScheduling([FromRoute]int id, [FromBody]JsonPatchDocument<RegisteredUpdateInput> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest(Json(new { Error = "请求参数错误" }));
            }
            var registered = await dbContext.Registereds.FirstOrDefaultAsync(r => r.Id == id);
            if (registered == null)
            {
                return NotFound(Json(new { Error = "该预约不存在" }));
            }

            var input = mapper.Map<RegisteredUpdateInput>(registered);
            patchDoc.ApplyTo(input);

            TryValidateModel(input);
            if (!ModelState.IsValid)
            {
                return new ValidationFailedResult(ModelState);
            }

            dbContext.Entry(registered).CurrentValues.SetValues(input);
            await dbContext.SaveChangesAsync();

            return CreatedAtRoute("GetRegistered", new { id = registered.Id }, mapper.Map<RegisteredOutput>(registered));
        }

        /// <summary>
        /// 删除预约
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(void), 204)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(void), 500)]
        public async Task<IActionResult> DeleteScheduling([FromRoute]int id)
        {
            var registered = await dbContext.Registereds.FirstOrDefaultAsync(r => r.Id == id);
            if (registered == null)
            {
                return NotFound(Json(new { Error = "该预约不存在" }));
            }

            registered.Status = "删除";
            dbContext.Registereds.Update(registered);
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
                var registered = await dbContext.Registereds.FirstOrDefaultAsync(r => r.Id == ids[i]);
                if (registered != null)
                {
                    registered.Status = "删除";
                    dbContext.Registereds.Update(registered);
                }
            }

            await dbContext.SaveChangesAsync();

            return new NoContentResult();
        }
        #endregion

        #region 取消预约
        /// <summary>
        /// 取消预约
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("WithCancel/{id}")]
        [ValidateModel]
        [ProducesResponseType(typeof(void), 204)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(void), 500)]
        public async Task<IActionResult> CancelRegistered([FromRoute]int id)
        {
            var registered = await dbContext.Registereds.FirstOrDefaultAsync(r => r.Id == id);
            if (registered == null)
            {
                return NotFound(Json(new { Error = "该预约不存在" }));
            }

            registered.RegisteredStateCode = "3";
            registered.RegisteredStateName = "已取消";
            registered.Status = "正常";

            dbContext.Registereds.Update(registered);
            await dbContext.SaveChangesAsync();

            return new NoContentResult();
        }
        #endregion

        #region 爽约
        /// <summary>
        /// 爽约
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("WithBreakPromise/{id}")]
        [ValidateModel]
        [ProducesResponseType(typeof(void), 204)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(void), 500)]
        public async Task<IActionResult> BreakPromiseRegistered([FromRoute]int id)
        {
            var registered = await dbContext.Registereds.FirstOrDefaultAsync(r => r.Id == id);
            if (registered == null)
            {
                return NotFound(Json(new { Error = "该预约不存在" }));
            }

            registered.RegisteredStateCode = "2";
            registered.RegisteredStateName = "未取号（爽约)";

            dbContext.Registereds.Update(registered);
            await dbContext.SaveChangesAsync();

            return new NoContentResult();
        }
        #endregion

        #region 取号
        /// <summary>
        /// 取号
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("WithTake/{id}")]
        [ValidateModel]
        [ProducesResponseType(typeof(void), 204)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(void), 500)]
        public async Task<IActionResult> TakeRegistered([FromRoute]int id)
        {
            var registered = await dbContext.Registereds
                .Include(r => r.Scheduling)
                .Include(r => r.Scheduling.User)
                .Include(r => r.Scheduling.User.Organazition)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (registered == null)
            {
                return NotFound(Json(new { Error = "该预约不存在" }));
            }

            registered.RegisteredStateCode = "1";
            registered.RegisteredStateName = "取号";
            registered.RegisteredDate = DateTime.Now;

            dbContext.Registereds.Update(registered);
            await dbContext.SaveChangesAsync();

            系统数据 system = await hisContext.系统数据.FirstOrDefaultAsync(q => q.Name == "工本费");

            //挂号信息写入HIS系统
            门诊挂号 gh = new 门诊挂号
            {
                姓名 = registered.Name,
                年龄 = Convert.ToInt16(DateTime.Now.Year - Convert.ToDateTime(registered.Birth).Year),
                性别 = registered.GenderName,
                通信地址 = registered.Address,
                电话 = registered.Phone,
                日期 = registered.RegisteredDate,
                科室 = registered.Scheduling.User.Organazition.Name,
                挂号类别 = registered.Scheduling.User.RegisteredRankName,
                操作员 = "预约诊疗平台",
                医师 = registered.Scheduling.User.Name,
                挂号费 = registered.Scheduling.Price / 100,
                工本费 = Convert.ToDecimal(system.Word),
                金额 = registered.Scheduling.Price / 100 + Convert.ToDecimal(system.Word),
                作废标志 = 0,
                卡号 = "",
                初诊 = 1,
                复诊 = 0,
                急诊 = 0,
                交班 = false,
                退票号 = 0,
                就诊标志 = false,
                民族 = "",
                接诊医师id = 0,
                出生日期 = Convert.ToDateTime(registered.Birth),
                过敏史 = "",
                年龄单位 = "岁",
                身份证 = registered.IDCard,
                总预存款 = 0,
                总费用 = 0,
                预存款支付 = 0,
                现金支付 = 0,
                来源 = registered.FromType,
                预存款余额 = 0,
                状态 = 0,
                退款 = 0
            };

            hisContext.门诊挂号.Add(gh);
            await hisContext.SaveChangesAsync();

            return new NoContentResult();
        }
        #endregion
    }
}