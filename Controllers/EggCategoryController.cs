using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using sipetok_api.Models;
using sipetok_api.dto.Request;
using sipetok_api.Data;
using AutoMapper;
using sipetok_api.dto.Respon;
using Microsoft.EntityFrameworkCore;

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
    [Authorize(Roles = "1")]
    public IActionResult GetAllEggCategory()
    {
        try
        {
            var allEggCategory = _mapper.Map<List<EggCategoryRespon>>(dbContext.EggCategories.ToList());

            var respon = new ResponData<List<EggCategoryRespon>>
            {
                success = true,
                data = allEggCategory,
                message = "Successfully retrieved all egg category data"
            };
            return Ok(respon);
        }
        catch (Exception ex)
        {
            var respon = new ResponData<EggCategoryRespon>
            {
                success = false,
                message = ex.Message
            };
            
            return StatusCode(500, respon);
        }
    }

    [HttpGet]
    [Route("myeggcategory")]
    [Authorize(Roles = "2")]
    public IActionResult GetMyEggCategories()
    {
        try
        {
            int userId = int.Parse(User.FindFirst("userId")?.Value ?? "0");
            var allEggCategory = _mapper.Map<List<EggCategoryRespon>>(
                (from u in dbContext.Users
                join t in dbContext.Tenants on u.id equals t.user_id
                join ec in dbContext.EggCategories on t.id equals ec.tenant_id
                where u.id == userId
                select ec).ToList()
            );

            var respon = new ResponData<List<EggCategoryRespon>>
            {
                success = true,
                data = allEggCategory,
                message = "Successfully retrieved my egg category data"
            };
            return Ok(respon);
        }
        catch (Exception ex)
        {
            var respon = new ResponData<EggCategoryRespon>
            {
                success = false,
                message = ex.Message
            };
            
            return StatusCode(500, respon);
        }
    }

    [HttpGet]
    [Route("{id:int}")]
    [Authorize(Roles = "1")]
    public IActionResult GetEggCategoryById(int id)
    {
        try
        {
            var eggCategory = _mapper.Map<EggCategoryRespon>(dbContext.EggCategories.Find(id));

            if (eggCategory is null)
            {
                return NotFound(new ResponData<EggCategoryRespon>
                {
                    success = false,
                    message = $"Egg category data with id {id} not found"
                });
            }

            var respon = new ResponData<EggCategoryRespon>
            {
                success = true,
                data = eggCategory,
                message = $"Successfully retrieved egg category data with id {id}"
            };

            return Ok(respon);
        }
        catch (Exception ex)
        {
            var respon = new ResponData<EggCategoryRespon>
            {
                success = false,
                message = ex.Message
            };

            return StatusCode(500, respon);
        }
    }

    [HttpPost]
    [Authorize(Roles = "2")]
    public IActionResult AddEggCategory([FromBody] EggCategoryDto eggCategoryDto)
    {
        try
        {
            int userId = int.Parse(User.FindFirst("userId")?.Value ?? "0");
            var tenant = dbContext.Tenants.Include(c => c.user).FirstOrDefault(c => c.user_id == userId);
            if (tenant is null)
            {
                return BadRequest("Tenant not found");
            }

            var eggCategory = _mapper.Map<EggCategory>(eggCategoryDto);
            eggCategory.tenant_id = tenant.id;
            dbContext.EggCategories.Add(eggCategory);
            dbContext.SaveChanges();

            var respon = new ResponData<EggCategoryRespon>
            {
                success = true,
                data = _mapper.Map<EggCategoryRespon>(eggCategory),
                message = "Successfully added egg category data"
            };

            return Ok(respon);
        }
        catch (Exception ex)
        {
            var respon = new ResponData<EggCategoryRespon>
            {
                success = false,
                message = ex.Message
            };

            return BadRequest(respon);
        }
    }

    [HttpPut]
    [Route("{id:int}")]
    [Authorize(Roles = "1, 2")]
    public IActionResult UpdateEggCategory(int id, [FromBody] EggCategoryDto eggCategoryDto)
    {
        try
        {
            var eggCategory = dbContext.EggCategories.Find(id);
            if (eggCategory is null)
            {
                return NotFound();
            }

            eggCategory.name = eggCategoryDto.name;
            eggCategory.price = eggCategoryDto.price;
            eggCategory.description = eggCategoryDto.description;
            eggCategory.UpdateTimestamps();

            dbContext.SaveChanges();

            var respon = new ResponData<EggCategoryRespon>
            {
                success = true,
                data = _mapper.Map<EggCategoryRespon>(eggCategory),
                message = "Successfully updated egg category data"
            };

            return Ok(respon);
        }
        catch (Exception ex)
        {
            var respon = new ResponData<EggCategoryRespon>
            {
                success = false,
                message = ex.Message
            };

            return BadRequest(respon);
        }
    }

    [HttpDelete]
    [Route("{id:int}")]
    [Authorize(Roles = "1, 2")]
    public IActionResult DeleteEggCategory(int id)
    {
        try
        {
            var eggCategory = dbContext.EggCategories.Find(id);

            if (eggCategory is null)
            {
                return NotFound(new ResponData<EggCategoryRespon>
                {
                    success = false,
                    message = $"Egg category data with id {id} not found"
                });
            }

            eggCategory.SoftDelete();
            dbContext.SaveChanges();

            var respon = new ResponData<EggCategoryRespon>
            {
                success = true,
                message = "Successfully deleted egg category data"
            };

            return Ok(respon);
        }
        catch (Exception ex)
        {
            var respon = new ResponData<EggCategoryRespon>
            {
                success = false,
                message = ex.Message
            };

            return BadRequest(respon);
        }
    }
}