using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using sipetok_api.Models;
using sipetok_api.dto;
using sipetok_api.Data;
using AutoMapper;

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
        var allEggCategory = dbContext.EggCategories.ToList();
        return Ok(allEggCategory);
    }

    [HttpGet]
    [Route("{id:int}")]
    public IActionResult GetEggCategoryById(int id)
    {
        var eggCategory = dbContext.EggCategories.Find(id);

        if (eggCategory is null)
        {
            return NotFound();
        }

        return Ok(eggCategory);
    }

    [HttpPost]
    public IActionResult AddEggCategory(EggCategoryDto eggCategoryDto)
    {
        var eggCategory = _mapper.Map<EggCategory>(eggCategoryDto);

        dbContext.EggCategories.Add(eggCategory);
        dbContext.SaveChanges();

        return Ok(eggCategory);
    }

    [HttpPut]
    [Route("{id:int}")]
    public IActionResult UpdateEggCategory(int id, EggCategoryDto eggCategoryDto)
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
        return Ok(eggCategory);
    }
}