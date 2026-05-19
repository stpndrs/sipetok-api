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
        [Authorize(Roles = "ADMIN, CUSTOMER")]
        public IActionResult GetAllEggs()
        {
            try
            {
                var eggSummary = dbContext.Eggs
                .Include(e => e.category)
                .Include(e => e.tenant)
                .GroupBy(e => e.category_id)
                .Select(group => new EggAvailableRespon
                {
                    category_id = group.Key,
                    tenant_id = group.First().tenant_id,
                    stock = group.Sum(e => e.stock),
                    category = _mapper.Map<EggCategoryRespon>(group.First().category)
                })
                .ToList();

                var respon = new ResponData<List<EggAvailableRespon>>(true, eggSummary, "Successfully retrieved all egg data");

                return Ok(respon);
            }
            catch (Exception ex)
            {
                var respon = new ResponData<object?>(false, ex.Message);

                return StatusCode(500, respon);
            }
        }

        [HttpGet]
        [Route("{id:int}")]
        [Authorize(Roles = "ADMIN")]
        public IActionResult GetEggById(int id)
        {
            try
            {
                var egg = _mapper.Map<EggRespon>(dbContext.Eggs.Include(e => e.tenant).Include(e => e.category).FirstOrDefault(e => e.id == id));

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

        [HttpGet]
        [Route("myeggs")]
        [Authorize(Roles = "TENANT")]
        public IActionResult GetMyEggs()
        {
            try
            {
                int userId = int.Parse(User.FindFirst("userId")?.Value ?? "0");
                var tenant = dbContext.Tenants.Include(c => c.user).FirstOrDefault(c => c.user_id == userId);

                if (tenant is null)
                {
                    return BadRequest(new ResponData<object?>(false, "Data tenant not found"));
                }

                var availableEggs = dbContext.Eggs
                .Include(e => e.category)
                .Where(e => e.tenant_id == tenant.id)
                .GroupBy(e => e.category_id)
                .Select(group => new EggAvailableRespon
                {
                    category_id = group.Key,
                    tenant_id = tenant.id,
                    stock = group.Sum(e => e.stock),
                    category = _mapper.Map<EggCategoryRespon>(group.First().category)
                })
                .ToList();

                var respon = new ResponData<List<EggAvailableRespon>>(true, availableEggs, $"Successfully retrieved egg data with tenant id {tenant.id}");

                return Ok(respon);
            }
            catch (Exception ex)
            {
                var respon = new ResponData<object?>(false, ex.Message);

                return StatusCode(500, respon);
            }
        }

        [HttpGet]
        [Route("getalleggs")]
        [Authorize(Roles = "TENANT")]
        public IActionResult GetAllMyHistoryEggs()
        {
            try
            {
                int userId = int.Parse(User.FindFirst("userId")?.Value ?? "0");
                var tenant = dbContext.Tenants.Include(c => c.user).FirstOrDefault(c => c.user_id == userId);

                if (tenant is null)
                {
                    return BadRequest(new ResponData<object?>(false, "Data tenant not found"));
                }

                var myegg = dbContext.Eggs.Where(e => e.tenant_id == tenant.id).ToList();

                var respon = new ResponData<List<EggRespon>>(true, _mapper.Map<List<EggRespon>>(myegg), $"Successfully retrieved total egg stock for tenant {tenant.id}");

                return Ok(respon);
            }
            catch (Exception ex)
            {
                var respon = new ResponData<object?>(false, ex.Message);

                return StatusCode(500, respon);
            }
        }

        [HttpPost]
        [Route("addmyeggs")]
        [Authorize(Roles = "TENANT")]
        public IActionResult AddMyEgg([FromBody] EggDto eggDto)
        {
            try
            {
                int userId = int.Parse(User.FindFirst("userId")?.Value ?? "0");
                var tenant = dbContext.Tenants.Include(c => c.user).FirstOrDefault(c => c.user_id == userId);
                var eggCategory = dbContext.EggCategories.Find(eggDto.category_id);

                if (tenant is null)
                {
                    return BadRequest(new ResponData<object?>(false, "Data tenant not found"));
                }
                if (eggCategory is null)
                {
                    return BadRequest(new ResponData<object?>(false, "Data category not found"));
                }
                if (eggCategory.tenant_id != tenant.id)
                {
                    return BadRequest(new ResponData<object?>(false, "Category does not belong to your tenant"));
                }

                var egg = _mapper.Map<Egg>(eggDto);

                egg.tenant_id = tenant.id;
                egg.category_id = eggCategory.id;

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
        public IActionResult UpdateMyEgg(int id, [FromBody] EggDto eggDto)
        {
            try
            {
                int userId = int.Parse(User.FindFirst("userId")?.Value ?? "0");

                var egg = dbContext.Eggs
                    .Include(e => e.tenant)
                    .FirstOrDefault(e => e.id == id);

                if (egg == null)
                {
                    return NotFound(new ResponData<object?>(false, "Data egg not found"));
                }

                if (egg.tenant == null || egg.tenant.user_id != userId)
                {
                    return BadRequest(new ResponData<object?>(false, "Egg data does not have tenant information"));
                }
                egg.production_date = eggDto.production_date;
                egg.category_id = eggDto.category_id;
                egg.stock = eggDto.stock;
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
                    .Include(e => e.tenant)
                    .FirstOrDefault(e => e.id == id);

                if (egg is null)
                {
                    return NotFound(new ResponData<object?>(false, $"Egg data with id {id} not found"));
                }
                if (egg.tenant == null || egg.tenant.user_id != userId)
                {
                    return BadRequest(new ResponData<object?>(false, "Egg data does not have tenant information"));
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