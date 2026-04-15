using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using sipetok_api.dto;
using sipetok_api.Models;
using sipetok_api.Data;
using AutoMapper;

[Route("api/[controller]")]
[ApiController]
public class OperationalController : ControllerBase
{
    private readonly AppDbContext dbContext;
    private readonly IMapper _mapper;

    public OperationalController(AppDbContext context, IMapper mapper)
    {
        dbContext = context;
        _mapper = mapper;
    }

    [HttpGet]
    public IActionResult GetAllOperationals()
    {
        var allOperational = dbContext.Operationals.Include(c => c.tenant).ToList();
        return Ok(allOperational);
    }

    [HttpGet]
    [Route("{id:int}")]
    public IActionResult GetOperationalById(int id)
    {
        var operational = dbContext.Operationals.Include(c => c.tenant).FirstOrDefault(c => c.id == id);

        if (operational is null)
        {
            return NotFound();
        }

        return Ok(operational);
    }

    [HttpGet("tenant/{tenantId:int}")]
    public IActionResult GetOperationalByTenantId(int tenantId)
    {
        var operational = dbContext.Operationals.Include(o => o.tenant).Where(o => o.tenant_id == tenantId).ToList();

        if (operational.Count == 0)
        {
            return NotFound();
        }

        return Ok(operational);
    }

    [HttpPost]
    public IActionResult AddOperational(OperationalDto operationalDto)
    {
        var tenant = dbContext.Tenants.Find(operationalDto.tenant_id);
        if(tenant is null)
        {
            return BadRequest("Tenant tidak ditemukan");
        }

        var operational = _mapper.Map<Operational>(operationalDto);
        operational.tenant = tenant;
        operational.tenant_id = tenant.id;
        operational.operational_date = DateTime.Now;

        dbContext.Operationals.Add(operational);
        dbContext.SaveChanges();

        return Ok(operational);
    }

    [HttpPut]
    [Route("{id:int}")]
    public IActionResult UpdateOperational(int id, OperationalDto operationalDto)
    {
        var operational = dbContext.Operationals.Find(id);

        if (operational is null)
        {
            return NotFound();
        }

        operational.name = operationalDto.name;
        operational.operational_cost = operationalDto.operational_cost;
        // operational.tenant = operationalDto.tenant;
        operational.operational_date = operationalDto.operational_date;
        
        dbContext.SaveChanges();
        return Ok(operational);
    }
}