using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using sipetok_api.Models;
using sipetok_api.dto.Request;
using sipetok_api.Data;
using AutoMapper;
using sipetok_api.dto.Respon;

[Route("api/tenants")]
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
                success = true,
                data = allCustomer,
                message = "Berhasil mengambil semua data tenant"
            };

            return Ok(respon);
        }
        catch (Exception ex)
        {
            var respon = new ResponData<List<TenantRespon>>
            {
                success = true,
                message = ex.Message
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
                    success = false,
                    message = $"Tenant data with id {id} not found"
                });
            }

            var respon = new ResponData<TenantRespon>
            {
                success = true,
                data = tenant,
                message = $"Successfully retrieved tenant data with id {id}"
            };

            return Ok(respon);
        }
        catch (Exception ex)
        {
            var respon = new ResponData<TenantRespon>
            {
                success = true,
            };
            respon.message = ex.Message;

            return StatusCode(500, respon);
        }
    }

    [HttpPost]
    public IActionResult AddTenant([FromBody] TenantDto tenantDto)
    {
        try
        {
            if (tenantDto.user == null)
            {
                return BadRequest(new ResponData<TenantRespon>
                {
                    success = false,
                    message = "User is required"
                });
            }
            if (string.IsNullOrWhiteSpace(tenantDto.user.password))
            {
                return BadRequest(new ResponData<TenantRespon>
                {
                    success = false,
                    message = "Password is required"
                });
            }

            var user = _mapper.Map<User>(tenantDto.user);
            user.password = Bcrypt.BcryptPassword(user.password);
            user.role = 2;
            user.status = 1;

            var tenant = _mapper.Map<Tenant>(tenantDto);
            tenant.user = user;

            dbContext.Tenants.Add(tenant);
            dbContext.SaveChanges();

            var respon = new ResponData<TenantRespon>
            {
                success = true,
                data = _mapper.Map<TenantRespon>(tenant)
            };
            respon.message = $"Successfully added tenant data";

            return Ok(respon);
        }
        catch (Exception ex)
        {
            var respon = new ResponData<TenantRespon>
            {
                success = false,
                message = ex.Message
            };

            return BadRequest(respon);
        }
    }

    [HttpPut]
    [Route("{id:int}")]
    public IActionResult UpdateTenant(int id, [FromBody] TenantDto tenantDto)
    {
        try
        {
            var tenant = dbContext.Tenants.Find(id);

            if (tenant is null)
            {
                return NotFound(new ResponData<TenantRespon>
                {
                    success = false,
                    message = $"Tenant data with id {id} not found"
                });
            }

            tenant.name = tenantDto.name;
            tenant.address = tenantDto.address;
            tenant.phoneNumber = tenantDto.phoneNumber;

            if (tenantDto.user != null)
            {
                var user = dbContext.Users.Find(tenant.user_id);
                if (user != null)
                {
                    user.username = tenantDto.user.username;
                    if (!string.IsNullOrWhiteSpace(tenantDto.user.password))
                    {
                        user.password = Bcrypt.BcryptPassword(tenantDto.user.password);
                    }
                    user.email = tenantDto.user.email;
                    user.status = tenantDto.user.status;
                }
            }

            dbContext.SaveChanges();

            var respon = new ResponData<TenantRespon>
            {
                success = true,
                data = _mapper.Map<TenantRespon>(tenant),
                message = $"Successfully updated tenant data with id {id}"
            };

            return Ok(respon);
        }
        catch (Exception ex)
        {
            var respon = new ResponData<TenantRespon>
            {
                success = false,
                message = ex.Message
            };

            return BadRequest(respon);
        }
    }

    [HttpPost]
    [Route("validate/{id:int}")]
    public IActionResult Validation(int id)
    {
        try
        {
            var tenant = dbContext.Tenants.Find(id);

            if (tenant is null)
            {
                return NotFound(new ResponData<TenantRespon>
                {
                    success = false,
                    message = $"Tenant data with id {id} not found"
                });
            }

            tenant.isValid = true;

            var respon = new ResponData<TenantRespon>
            {
                success = true,
                data = _mapper.Map<TenantRespon>(tenant),
                message = $"Successfully validated tenant data with id {id}"
            };

            dbContext.SaveChanges();
            return Ok(respon);

        }catch (Exception ex)
        {
            var respon = new ResponData<TenantRespon>
            {
                success = false,
                message = ex.Message
            };

            return BadRequest(respon);
        }
    }
}