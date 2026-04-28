using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using sipetok_api.Models;
using sipetok_api.dto.Request;
using sipetok_api.Data;
using AutoMapper;
using sipetok_api.dto.Respon;

[Route("api/[controller]")]
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
    public IActionResult GetAllEggCategory()
    {
        try
        {
            var allEggCategory = _mapper.Map<EggCategoryRespon>(dbContext.EggCategories.ToList());

            var respon = new ResponData<EggCategoryRespon>
            {
                status = true,
                data = allEggCategory,
                message = new List<string> {"Berhasil mengambil semua data egg category"}
            };
            return Ok(respon);
        }
        catch (Exception ex)
        {
            var respon = new ResponData<EggCategoryRespon>
            {
                status = false,
                message = new List<string> { ex.Message }
            };
            
            return StatusCode(500, respon);
        }
    }

    [HttpGet]
    [Route("{id:int}")]
    public IActionResult GetEggCategoryById(int id)
    {
        try
        {
            var eggCategory = _mapper.Map<EggCategoryRespon>(dbContext.EggCategories.Find(id));

            if (eggCategory is null)
            {
                return NotFound(new ResponData<EggCategoryRespon>
                {
                    status = false,
                    message = new List<string> { $"Data customer dengan id {id} tidak ditemukan" }
                });
            }

            var respon = new ResponData<EggCategoryRespon>
            {
                status = true,
                data = eggCategory,
                message = new List<string> { $"Berhasil mengambil data egg category pada id {id}" }
            };

            return Ok(respon);
        }
        catch (Exception ex)
        {
            var respon = new ResponData<EggCategoryRespon>
            {
                status = true,
                message = new List<string> { ex.Message }
            };

            return StatusCode(500, respon);
        }
    }

    [HttpPost]
    public IActionResult AddEggCategory(EggCategoryDto eggCategoryDto)
    {
        try
        {
            var eggCategory = _mapper.Map<EggCategory>(eggCategoryDto);

            dbContext.EggCategories.Add(eggCategory);
            dbContext.SaveChanges();

            var respon = new ResponData<EggCategoryRespon>
            {
                status = true,
                data = _mapper.Map<EggCategoryRespon>(eggCategory),
                message = new List<string> { "Berhasil menambahkan data egg category" }
            };

            return Ok(respon);
        }
        catch (Exception ex)
        {
            var respon = new ResponData<EggCategoryRespon>
            {
                status = true,
                message = new List<string> { ex.Message }
            };

            return BadRequest(respon);
        }
    }

    [HttpPut]
    [Route("{id:int}")]
    public IActionResult UpdateEggCategory(int id, EggCategoryDto eggCategoryDto)
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

            dbContext.SaveChanges();

            var respon = new ResponData<EggCategoryRespon>
            {
                status = true,
                data = _mapper.Map<EggCategoryRespon>(eggCategory),
                message = new List<string>{"Berhasil memperbarui data"}
            };

            return Ok(respon);
        }
        catch (Exception ex)
        {
            var respon = new ResponData<EggCategoryRespon>
            {
                status = true,
                message = new List<string>{ex.Message}
            };

            return BadRequest(respon);
        }
    }
}