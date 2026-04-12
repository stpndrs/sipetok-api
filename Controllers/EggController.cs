using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using sipetok_api.Models;
using sipetok_api.dto;
using sipetok_api.Data;
using AutoMapper;

[Route("api/[controller]")]
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
    public IActionResult GetAllEggs()
    {
        var allEgg = dbContext.Eggs.Include(e => e.tenant).Include(e => e.category).ToList();
        return Ok(allEgg);
    }

    [HttpGet]
    [Route("{id:int}")]
    public IActionResult GetEggById(int id)
    {
        var egg = dbContext.Eggs.Include(e => e.tenant).Include(e => e.category).FirstOrDefault(e => e.id == id);
        if (egg is null)
        {
            return NotFound();
        }

        var result = _mapper.Map<EggDto>(egg);
        return Ok(result);
    }

    [HttpGet("tenant/{tenantId:int}")]
    public IActionResult GetEggByTenantId(int tenantId)
    {
        var egg = dbContext.Eggs.Include(e => e.tenant).Include(e => e.category).Where(e => e.tenant_id == tenantId).ToList();

        if (egg.Count == 0)
        {
            return NotFound();
        }

        return Ok(egg);
    }

    [HttpPost]
    public IActionResult AddEgg(EggDto eggDto)
    {
        var tenant = dbContext.Tenants.Find(eggDto.tenant_id);
        var eggCategory = dbContext.EggCategories.Find(eggDto.category_id);
        if (tenant == null)
        {
            return BadRequest("Tenant tidak ditemukan");
        }

        var egg = _mapper.Map<Egg>(eggDto);
        egg.tenant_id = tenant.id;
        egg.category_id = eggCategory.id;

        egg.tenant = tenant;
        egg.category = eggCategory;

        dbContext.Eggs.Add(egg);
        dbContext.SaveChanges();

        return Ok(egg);
    }

    [HttpPut]
    [Route("{id:int}")]
    public IActionResult UpdateEgg(int id, EggDto eggDto)
    {
        var egg = dbContext.Eggs.Find(id);

        if (egg == null)
        {
            return NotFound();
        }

        egg.production_date = eggDto.production_date;
        egg.category_id = eggDto.category_id;
        egg.stock = eggDto.stock;
        egg.tenant_id = eggDto.tenant_id;

        dbContext.SaveChanges();
        return Ok(egg);
    }
}