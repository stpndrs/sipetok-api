using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using sipetok_api.Models;
using sipetok_api.dto.Request;
using sipetok_api.Data;
using AutoMapper;
using sipetok_api.Utilis;
using sipetok_api.dto.Respon;

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
        try
        {
            var allCustomer = _mapper.Map<List<TenantRespon>>(dbContext.Tenants.Include(c => c.user).ToList());
            var respon = new ResponData<List<TenantRespon>>
            {
                status = true,
                data = allCustomer,
                message = new List<string>{"Berhasil mengambil semua data tenant"}
            };

            return Ok(respon);
        }
        catch (Exception ex)
        {
            var respon = new ResponData<List<TenantRespon>>
            {
                status = true,
                message = new List<string>{ex.Message}
            };

            return StatusCode(500, respon);
        }
    }

    [HttpGet]
    [Route("{id:int}")]
    public IActionResult GetTenantById(int id)
    {
        try
        {
            var tenant = _mapper.Map<TenantRespon>(dbContext.Tenants.Include(c => c.user).FirstOrDefault(c => c.id == id));

            if (tenant is null)
            {
                return NotFound(new ResponData<TenantRespon>
                {
                    status = false,
                    message = new List<string> {$"Data tenant dengan id {id} tidak ditemukan"}
                });
            }

            var respon = new ResponData<TenantRespon>
            {
                status = true,
                data = tenant,
                message = new List<string>{"Berhasil mengambil data customer pada id {id}"}
            };

            return Ok(respon);
        }
        catch (Exception ex)
        {
            var respon = new ResponData<TenantRespon>
            {
                status = true,
            };
            respon.message.Add(ex.Message);

            return StatusCode(500, respon);
        }
    }

    [HttpPost]
    public IActionResult AddTenant(TenantDto tenantDto)
    {
        try
        {
            if (tenantDto.user == null)
            {
                return BadRequest(new ResponData<TenantRespon>
                {
                    status = false,
                    message = new List<string>{"User wajib diisi"}
                });
            }

            var user = _mapper.Map<User>(tenantDto.user);

            user.password = Bcrypt.BcryptPassword(user.password);
            user.role = Role.TENANT;
            user.status = Status.ACTIVE;

            var tenant = _mapper.Map<Tenant>(tenantDto);
            
            tenant.user = user;

            dbContext.Tenants.Add(tenant);
            dbContext.SaveChanges();

            var respon = new ResponData<TenantRespon>
            {
                status = true,
                data = _mapper.Map<TenantRespon>(tenant)
            };
            respon.message.Add($"Berhasil menambahkan data tenant");
            
            return Ok(respon);
        }
        catch (Exception ex)
        {
            var respon = new ResponData<TenantRespon>
            {
                status = false,
                message = new List<string>{ex.Message}
            };
            
            return BadRequest(respon);
        }
    }

    [HttpPut]
    [Route("{id:int}")]
    public IActionResult UpdateTenant(int id, TenantDto tenantDto)
    {
        try
        {
            var tenant = dbContext.Tenants.Find(id);

            if (tenant is null)
            {
                return NotFound(new ResponData<TenantRespon>
                {
                    status = false,
                    message = new List<string> {$"Data tenant dengan id {id} tidak ditemukan"}
                });
            }
        
            if (!string.IsNullOrEmpty(tenantDto.name))
                tenant.name = tenantDto.name;

            if (!string.IsNullOrEmpty(tenantDto.address))
                tenant.address = tenantDto.address;

            if (!string.IsNullOrEmpty(tenantDto.phoneNumber))
                tenant.phoneNumber = tenantDto.phoneNumber;

            dbContext.SaveChanges();

            var respon = new ResponData<TenantRespon>
            {
                status = true,
                data = _mapper.Map<TenantRespon>(tenant),
                message = new List<string>{"Berhasil memperbarui data"}
            };
            
            return Ok(respon);
        }
        catch (Exception ex)
        {
            var respon = new ResponData<TenantRespon>
            {
                status = false,
                message = new List<string>{ex.Message}
            };
            
            return BadRequest(respon);
        }
    }
}