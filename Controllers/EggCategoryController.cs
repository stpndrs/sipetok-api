using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

using sipetok_api.Models;
using sipetok_api.dto.Request;
using sipetok_api.Data;
using sipetok_api.dto.Respon;

namespace sipetok_api.Controllers
{
    [Authorize]
    [Route("api/egg/categories")]
    [ApiController]
    public class EggCategoryController : ControllerBase
    {
        private readonly AppDbContext dbContext;
        private readonly IMapper _mapper;

        public EggCategoryController(AppDbContext context, IMapper mapper)
        {
            dbContext = context;
            _mapper = mapper;
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN")]
        public IActionResult GetAllEggCategory()
        {
            try
            {
                var allEggCategory = _mapper.Map<List<EggCategoryRespon>>(dbContext.EggCategories.ToList());

                var respon = new ResponData<List<EggCategoryRespon>>(true, allEggCategory, "Successfully retrieved all egg category data");

                return Ok(respon);
            }
            catch (Exception ex)
            {
                var respon = new ResponData<object?>(false, ex.Message);

                return StatusCode(500, respon);
            }
        }

        [HttpGet]
        [Route("myeggcategory")]
        [Authorize(Roles = "TENANT")]
        public IActionResult GetMyEggCategories()
        {
            try
            {
                int userId = int.Parse(User.FindFirst("userId")?.Value ?? "0");
                var allEggCategory = _mapper.Map<List<EggCategoryRespon>>(
                    (
                        from u in dbContext.Users
                        join t in dbContext.Tenants on u.Id equals t.UserId
                        join ec in dbContext.EggCategories on t.Id equals ec.TenantId
                        where u.Id == userId
                        select ec
                    ).ToList()
                );

                var respon = new ResponData<List<EggCategoryRespon>>(true, allEggCategory, "Successfully retrieved my egg category data");

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
        public IActionResult GetEggCategoryById(int id)
        {
            try
            {
                var eggCategory = _mapper.Map<EggCategoryRespon>(dbContext.EggCategories.Find(id));

                if (eggCategory is null)
                {
                    return NotFound(new ResponData<object?>(false, $"Egg category data with id {id} not found"));
                }

                var respon = new ResponData<EggCategoryRespon>(true, eggCategory, $"Successfully retrieved egg category data with id {id}");

                return Ok(respon);
            }
            catch (Exception ex)
            {
                var respon = new ResponData<object?>(false, ex.Message);

                return StatusCode(500, respon);
            }
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public IActionResult AddEggCategory([FromBody] EggCategoryDto eggCategoryDto)
        {
            try
            {
                var tenant = dbContext.Tenants.Find(eggCategoryDto.TenantId);
                if (tenant is null)
                {
                    return BadRequest(new ResponData<object?>(false, "Tenant not found"));
                }

                var eggCategory = _mapper.Map<EggCategory>(eggCategoryDto);
                eggCategory.TenantId = tenant.Id;
                dbContext.EggCategories.Add(eggCategory);
                dbContext.SaveChanges();

                var respon = new ResponData<EggCategoryRespon>(true, _mapper.Map<EggCategoryRespon>(eggCategory), "Successfully added egg category data");

                return Ok(respon);
            }
            catch (Exception ex)
            {
                var respon = new ResponData<object?>(false, ex.Message);

                return BadRequest(respon);
            }
        }

        [HttpPost]
        [Route("addmyeggcategory")]
        [Authorize(Roles = "TENANT")]
        public IActionResult AddMyEggCategory([FromBody] EggCategoryDto eggCategoryDto)
        {
            try
            {
                int userId = int.Parse(User.FindFirst("userId")?.Value ?? "0");
                var tenant = dbContext.Tenants.Include(c => c.User).FirstOrDefault(c => c.UserId == userId);
                if (tenant is null || tenant.UserId != userId)
                {
                    return BadRequest(new ResponData<object?>(false, "Egg category data does not have tenant information"));
                }

                var eggCategory = _mapper.Map<EggCategory>(eggCategoryDto);
                eggCategory.TenantId = tenant.Id;
                dbContext.EggCategories.Add(eggCategory);
                dbContext.SaveChanges();

                var respon = new ResponData<EggCategoryRespon>(true, _mapper.Map<EggCategoryRespon>(eggCategory), "Successfully added my egg category data");

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
        [Authorize(Roles = "ADMIN")]
        public IActionResult UpdateEggCategory(int id, [FromBody] EggCategoryDto eggCategoryDto)
        {
            try
            {
                var eggCategory = dbContext.EggCategories.Find(id);
                if (eggCategory is null)
                {
                    return NotFound(new ResponData<object?>(false, "Egg category data not found"));
                }

                eggCategory.Name = eggCategoryDto.Name;
                eggCategory.Price = eggCategoryDto.Price;
                eggCategory.Description = eggCategoryDto.Description;
                eggCategory.UpdateTimestamps();

                dbContext.SaveChanges();

                var respon = new ResponData<EggCategoryRespon>(true, _mapper.Map<EggCategoryRespon>(eggCategory), "Successfully updated egg category data");

                return Ok(respon);
            }
            catch (Exception ex)
            {
                var respon = new ResponData<object?>(false, ex.Message);

                return BadRequest(respon);
            }
        }

        [Authorize(Roles = "TENANT")]
        [HttpPut]
        [Route("myeggcategory/{id:int}")]
        public IActionResult UpdateMyEggCategory(int id, [FromBody] EggCategoryDto eggCategoryDto)
        {
            try
            {
                int userId = int.Parse(User.FindFirst("userId")?.Value ?? "0");
                var tenant = dbContext.Tenants.Include(c => c.User).FirstOrDefault(c => c.UserId == userId);
                var eggCategory = dbContext.EggCategories.Find(id);

                if (eggCategory is null)
                {
                    return NotFound(new ResponData<object?>(false, $"Egg category data with id {id} not found"));
                }
                if (tenant is null)
                {
                    return BadRequest(new ResponData<object?>(false, "Tenant not found"));
                }
                if (eggCategory.TenantId != tenant.Id)
                {
                    return BadRequest(new ResponData<object?>(false, "You are not authorized to update this egg category data"));
                }

                eggCategory.Name = eggCategoryDto.Name;
                eggCategory.Price = eggCategoryDto.Price;
                eggCategory.Description = eggCategoryDto.Description;
                eggCategory.UpdateTimestamps();

                dbContext.SaveChanges();

                var respon = new ResponData<EggCategoryRespon>(true, _mapper.Map<EggCategoryRespon>(eggCategory), "Successfully updated egg category data");

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
        public IActionResult DeleteEggCategory(int id)
        {
            try
            {
                int userId = int.Parse(User.FindFirst("userId")?.Value ?? "0");
                var tenant = dbContext.Tenants.Include(c => c.User).FirstOrDefault(c => c.UserId == userId);
                var eggCategory = dbContext.EggCategories.Find(id);

                if (eggCategory is null)
                {
                    return NotFound(new ResponData<object?>(false, $"Egg category data with id {id} not found"));
                }
                if (tenant is null)
                {
                    return BadRequest(new ResponData<object?>(false, "Tenant not found"));
                }
                if (eggCategory.TenantId != tenant.Id)
                {
                    return BadRequest(new ResponData<object?>(false, "You are not authorized to delete this egg category data"));
                }

                eggCategory.SoftDelete();
                dbContext.SaveChanges();

                var respon = new ResponData<object?>(true, "Successfully deleted egg category data");

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