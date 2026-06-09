using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using AutoMapper;

using sipetok_api.Models;
using sipetok_api.dto.Request;
using sipetok_api.Data;
using sipetok_api.dto.Respon;
using sipetok_api.Respon;

namespace sipetok_api.Controllers
{
    [Authorize]
    [Route("api/eggs")]
    [ApiController]
    public class EggController : ControllerBase
    {
        private readonly AppDbContext dbContext;
        private readonly IMapper _mapper;

        public EggController(AppDbContext context, IMapper mapper)
        {
            dbContext = context;
            _mapper = mapper;
        }

        [HttpGet]
        [Authorize(Roles = "CUSTOMER")]
        public IActionResult GetAllEggs()
        {
            try
            {
                int userId = int.Parse(User.FindFirst("userId")?.Value ?? "0");
                var Tenant = dbContext.Tenants.Include(c => c.User).FirstOrDefault(c => c.UserId == userId);

                var eggs = dbContext.Eggs
                    .Include(e => e.Category)
                    .Where(e => e.Category != null && e.Category.TenantId == Tenant!.Id)
                    .ToList();

                var respon = new ResponData<List<EggRespon>>(true, _mapper.Map<List<EggRespon>>(eggs), $"Successfully retrieved total egg Stock");

                return Ok(respon);
            }
            catch (Exception ex)
            {
                string detailError = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                var respon = new ResponData<object?>(false, detailError);
                return StatusCode(500, respon);
            }
        }

        [HttpGet]
        [Route("{id:int}")]
        [Authorize(Roles = "TENANT")]
        public IActionResult GetEggById(int id)
        {
            try
            {
                var egg = _mapper.Map<EggRespon>(dbContext.Eggs.Include(e => e.Category).FirstOrDefault(e => e.Id == id));

                if (egg is null)
                {
                    return NotFound(new ResponData<object?>(false, $"Egg data with id {id} not found"));
                }

                var respon = new ResponData<EggRespon>(true, egg, $"Successfully retrieved egg data with id {id}");
                return Ok(respon);
            }
            catch (Exception ex)
            {
                var respon = new ResponData<object?>(false, ex.Message);
                return StatusCode(500, respon);
            }
        }

        [HttpPost]
        [Authorize(Roles = "TENANT")]
        public IActionResult AddEgg([FromBody] EggDto eggDto)
        {
            try
            {
                int userId = int.Parse(User.FindFirst("userId")?.Value ?? "0");
                var Tenant = dbContext.Tenants.Include(c => c.User).FirstOrDefault(c => c.UserId == userId);
                var eggCategory = dbContext.EggCategories.Find(eggDto.CategoryId);

                if (Tenant is null)
                {
                    return BadRequest(new ResponData<object?>(false, "Data Tenant not found"));
                }
                if (eggCategory is null)
                {
                    return BadRequest(new ResponData<object?>(false, "Data Category not found"));
                }
                if (eggCategory.TenantId != Tenant.Id)
                {
                    return BadRequest(new ResponData<object?>(false, "Category does not belong to your Tenant"));
                }

                var egg = _mapper.Map<Egg>(eggDto);

                egg.CategoryId = eggCategory.Id;

                dbContext.Eggs.Add(egg);
                dbContext.SaveChanges();

                var respon = new ResponData<EggRespon>(true, _mapper.Map<EggRespon>(egg), "Successfully added egg data");

                return Ok(respon);
            }
            catch (Exception ex)
            {
                var respon = new ResponData<object?>(false, ex.Message);

                return BadRequest(respon);
            }
        }

        [HttpPut]
        [Route("{id:int}")]
        [Authorize(Roles = "TENANT")]
        public IActionResult UpdateEgg(int id, [FromBody] EggDto eggDto)
        {
            try
            {
                int userId = int.Parse(User.FindFirst("userId")?.Value ?? "0");

                var egg = dbContext.Eggs
                    .FirstOrDefault(e => e.Id == id);

                if (egg == null)
                {
                    return NotFound(new ResponData<object?>(false, "Data egg not found"));
                }

                egg.ProductionDate = eggDto.ProductionDate;
                egg.CategoryId = eggDto.CategoryId;
                egg.Stock = eggDto.Stock;
                egg.UpdateTimestamps();

                dbContext.SaveChanges();

                var respon = new ResponData<EggRespon>(true, _mapper.Map<EggRespon>(egg), "Successfully updated egg data");

                return Ok(respon);
            }
            catch (Exception ex)
            {
                var respon = new ResponData<object?>(false, ex.Message);

                return BadRequest(respon);
            }
        }

        [HttpDelete]
        [Route("{id:int}")]
        [Authorize(Roles = "TENANT")]
        public IActionResult DeleteEgg(int id)
        {
            try
            {
                var userId = int.Parse(User.FindFirst("userId")?.Value ?? "0");
                var egg = dbContext.Eggs
                    .FirstOrDefault(e => e.Id == id);

                if (egg is null)
                {
                    return NotFound(new ResponData<object?>(false, $"Egg data with id {id} not found"));
                }

                egg.SoftDelete();
                dbContext.SaveChanges();

                var respon = new ResponData<object?>(true, "Successfully deleted egg data");

                return Ok(respon);
            }
            catch (Exception ex)
            {
                var respon = new ResponData<object?>(false, ex.Message);

                return BadRequest(respon);
            }
        }
    }
}