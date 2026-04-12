using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using sipetok_api.Models;
using sipetok_api.dto;
using sipetok_api.Data;
using AutoMapper;

[Route("api/[controller]")]
[ApiController]
public class TenantController  : ControllerBase
{
    private readonly AppDbContext dbContext;
    private readonly IMapper _mapper;

    public TenantController (AppDbContext context, IMapper mapper)
    {
        dbContext = context;
        _mapper = mapper;
    }

    [HttpGet]
    public IActionResult GetAllTenant()
    {
        var allTenant = dbContext.Tenants.Include(c => c.user).ToList();
        return Ok(allTenant);
    }

    [HttpGet]
    [Route("{id:int}")]
    public IActionResult GetTenantById(int id)
    {
        var tenant = dbContext.Tenants.Include(c => c.user).FirstOrDefault(c => c.id == id);

        if (tenant is null)
        {
            return NotFound();
        }

        return Ok(tenant);
    }

    [HttpPost]
    public IActionResult AddTenant(TenantDto tenantDto)
    {
        var tenant = _mapper.Map<Tenant>(tenantDto);

        dbContext.Tenants.Add(tenant);
        dbContext.SaveChanges();

        return Ok(tenant);
    }

    [HttpPut]
    [Route("{id:int}")]
    public IActionResult UpdateTenant(int id, TenantDto tenantDto)
    {
        var tenant = dbContext.Tenants.Find(id);
        if (tenant is null)
        {
            return NotFound();
        }
        tenant.user = dbContext.Users.Find(tenant.user_id);
        tenant.name = tenantDto.name;
        tenant.address = tenantDto.address;
        tenant.phoneNumber = tenantDto.phoneNumber;
        tenant.user_id = tenantDto.user_id;

        dbContext.SaveChanges();
        return Ok(tenant);
    }
}