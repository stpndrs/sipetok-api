using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using sipetok_api.Models;
using sipetok_api.dto;
using sipetok_api.Data;
using AutoMapper;
using sipetok_api.Utilis;

[Route("api/[controller]")]
[ApiController]
public class TenantController : ControllerBase
{
    private readonly AppDbContext dbContext;
    private readonly IMapper _mapper;

    public TenantController(AppDbContext context, IMapper mapper)
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
        if (tenantDto.user == null)
        {
            return BadRequest("User wajib diisi");
        }

        var user = _mapper.Map<User>(tenantDto.user);

        user.password = Bcrypt.BcryptPassword(user.password);
        user.role = Role.TENANT;
        user.status = Status.ACTIVE;

        var tenant = _mapper.Map<Tenant>(tenantDto);
        
        tenant.user = user;

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

        if (!string.IsNullOrEmpty(tenantDto.name))
            tenant.name = tenantDto.name;

        if (!string.IsNullOrEmpty(tenantDto.address))
            tenant.address = tenantDto.address;

        if (!string.IsNullOrEmpty(tenantDto.phoneNumber))
            tenant.phoneNumber = tenantDto.phoneNumber;

        dbContext.SaveChanges();
        return Ok(tenant);
    }
}