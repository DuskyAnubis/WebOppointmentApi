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
    [Route("api/v1/Hosptial")]
    //[Authorize]
    public class HosptialController : Controller
    {
        private readonly ApiContext dbContext;
        private readonly IMapper mapper;

        public HosptialController(ApiContext dbContext, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
        }

        #region 基本操作
        /// <summary>
        /// 获得医院列表
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<HosptialOutput>), 200)]
        [ProducesResponseType(typeof(void), 500)]
        public async Task<IEnumerable<HosptialOutput>> GetHosptials(HosptialQueryInput input)
        {
            int pageIndex = input.Page - 1;
            int Per_Page = input.Per_Page;
            string sortBy = input.SortBy;

            IQueryable<Hospital> query = dbContext.Hospitals.AsQueryable<Hospital>();

            query = query.Where(q => string.IsNullOrEmpty(input.Name) || q.Name.Contains(input.Name));
            query = query.OrderBy(sortBy);

            var totalCount = query.Count();
            var totalPages = (int)Math.Ceiling((double)totalCount / Per_Page);

            HttpContext.Response.Headers.Add("X-TotalCount", JsonConvert.SerializeObject(totalCount));
            HttpContext.Response.Headers.Add("X-TotalPage", JsonConvert.SerializeObject(totalPages));

            query = query.Skip(pageIndex * Per_Page).Take(Per_Page);

            List<Hospital> hospitals = await query.ToListAsync();
            List<HosptialOutput> list = mapper.Map<List<HosptialOutput>>(hospitals);

            return list;
        }

        /// <summary>
        /// 获得医院信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}", Name = "GetHosptial")]
        [ProducesResponseType(typeof(HosptialOutput), 200)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(void), 500)]
        public async Task<IActionResult> GetHosptial([FromRoute]int id)
        {
            Hospital hospital = await dbContext.Hospitals.FirstOrDefaultAsync(h => h.Id == id);
            if (hospital == null)
            {
                return NotFound(Json(new { Error = "该医院不存在" }));
            }

            HosptialOutput hosptialOutput = mapper.Map<HosptialOutput>(hospital);

            return new ObjectResult(hosptialOutput);
        }

        /// <summary>
        /// 创建医院
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateModel]
        [ProducesResponseType(typeof(HosptialOutput), 201)]
        [ProducesResponseType(typeof(ValidationError), 422)]
        [ProducesResponseType(typeof(void), 500)]
        public async Task<IActionResult> CreateHosptial([FromBody]HosptialCreateInput input)
        {
            Hospital hosptial = mapper.Map<Hospital>(input);

            dbContext.Hospitals.Add(hosptial);
            await dbContext.SaveChangesAsync();

            return CreatedAtRoute("GetHosptial", new { id = hosptial.Id }, mapper.Map<HosptialOutput>(hosptial));
        }

        /// <summary>
        /// 修改医院
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
        public async Task<IActionResult> UpdateHosptial([FromRoute]int id, [FromBody]HosptialUpdateInput input)
        {
            if (input.Id != id)
            {
                return BadRequest(Json(new { Error = "请求参数错误" }));
            }

            var hospital = await dbContext.Hospitals.FirstOrDefaultAsync(h => h.Id == id);
            if (hospital == null)
            {
                return NotFound(Json(new { Error = "该医院不存在" }));
            }

            dbContext.Entry(hospital).CurrentValues.SetValues(input);
            await dbContext.SaveChangesAsync();

            return new NoContentResult();
        }

        /// <summary>
        /// 更新医院
        /// </summary>
        /// <param name="id"></param>
        /// <param name="patchDoc"></param>
        /// <returns></returns>
        [HttpPatch("{id}")]
        [ProducesResponseType(typeof(HosptialOutput), 201)]
        [ProducesResponseType(typeof(string), 400)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(ValidationError), 422)]
        [ProducesResponseType(typeof(void), 500)]
        public async Task<IActionResult> PatchHosptial([FromRoute]int id, [FromBody]JsonPatchDocument<HosptialUpdateInput> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest(Json(new { Error = "请求参数错误" }));
            }

            var hospital = await dbContext.Hospitals.FirstOrDefaultAsync(h => h.Id == id);
            if (hospital == null)
            {
                return NotFound(Json(new { Error = "该医院不存在" }));
            }

            var input = mapper.Map<HosptialUpdateInput>(hospital);
            patchDoc.ApplyTo(input);

            TryValidateModel(input);
            if (!ModelState.IsValid)
            {
                return new ValidationFailedResult(ModelState);
            }

            dbContext.Entry(hospital).CurrentValues.SetValues(input);
            await dbContext.SaveChangesAsync();

            return CreatedAtRoute("GetHosptial", new { id = hospital.Id }, mapper.Map<HosptialOutput>(hospital));
        }

        /// <summary>
        /// 删除医院
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(void), 204)]
        [ProducesResponseType(typeof(void), 400)]
        [ProducesResponseType(typeof(string), 404)]
        [ProducesResponseType(typeof(void), 500)]
        public async Task<IActionResult> DeleteHosptial([FromRoute]int id)
        {
            var hospital = await dbContext.Hospitals.FirstOrDefaultAsync(h => h.Id == id);
            if (hospital == null)
            {
                return NotFound(Json(new { Error = "该医院不存在" }));
            }

            hospital.Status = "删除";
            dbContext.Hospitals.Update(hospital);
            await dbContext.SaveChangesAsync();

            return new NoContentResult();
        }

        [HttpDelete]
        [ProducesResponseType(typeof(void), 204)]
        [ProducesResponseType(typeof(void), 500)]
        public async Task<IActionResult> BatchDelete([FromBody]int[] ids)
        {
            for (int i = 0; i < ids.Length; i++)
            {
                var hospital = await dbContext.Hospitals.FirstOrDefaultAsync(h => h.Id == ids[i]);

                if (hospital != null)
                {
                    hospital.Status = "删除";
                    dbContext.Hospitals.Update(hospital);
                }
            }
            await dbContext.SaveChangesAsync();

            return new NoContentResult();
        }
        #endregion
    }
}