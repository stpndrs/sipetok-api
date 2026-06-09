using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using AutoMapper;

using sipetok_api.Models;
using sipetok_api.dto.Request;
using sipetok_api.Data;
using sipetok_api.dto.Respon;

namespace sipetok_api.Controllers
{
    [Authorize]
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
        [Authorize(Roles = "ADMIN")]
        public IActionResult GetAllTenant()
        {
            try
            {
                var allCustomer = _mapper.Map<List<TenantRespon>>(dbContext.Tenants.Include(c => c.User).ToList());
                var respon = new ResponData<List<TenantRespon>>(true, allCustomer, "Berhasil mengambil semua data tenant");

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
        public IActionResult GetTenantById(int id)
        {
            try
            {
                var tenant = _mapper.Map<TenantRespon>(dbContext.Tenants.Include(c => c.User).FirstOrDefault(c => c.Id == id));

                if (tenant is null)
                {
                    return NotFound(new ResponData<object?>(false, $"Tenant data with id {id} not found"));
                }

                var respon = new ResponData<TenantRespon>(true, tenant, $"Successfully retrieved tenant data with id {id}");

                return Ok(respon);
            }
            catch (Exception ex)
            {
                var respon = new ResponData<object?>(false, ex.Message);

                return StatusCode(500, respon);
            }
        }

        [HttpGet]
        [Route("myprofile")]
        [Authorize(Roles = "TENANT")]
        public IActionResult GetMyProfile()
        {
            try
            {
                int userId = int.Parse(User.FindFirst("userId")?.Value ?? "0");
                var tenant = _mapper.Map<TenantRespon>(dbContext.Tenants.Include(c => c.User).FirstOrDefault(c => c.UserId == userId));

                if (tenant is null)
                {
                    return NotFound(new ResponData<object?>(false, $"Tenant data with id {userId} not found"));
                }

                var respon = new ResponData<TenantRespon>(true, tenant, $"Successfully retrieved tenant data with id {userId}");

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
        public IActionResult AddTenant([FromBody] TenantDto tenantDto)
        {
            try
            {
                if (tenantDto.User == null)
                {
                    return BadRequest(new ResponData<object?>(false, "User is required"));
                }
                if (string.IsNullOrWhiteSpace(tenantDto.User.Password))
                {
                    return BadRequest(new ResponData<object?>(false, "Password is required"));
                }

                var User = _mapper.Map<User>(tenantDto.User);
                User.Password = Bcrypt.BcryptPassword(User.Password);
                User.Role = 2;
                User.IsActive = true;
                tenantDto.IsValid = false;

                var tenant = _mapper.Map<Tenant>(tenantDto);
                tenant.User = User;

                dbContext.Tenants.Add(tenant);
                dbContext.SaveChanges();

                var respon = new ResponData<TenantRespon>(true, _mapper.Map<TenantRespon>(tenant), "Successfully added tenant data");

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
        public IActionResult UpdateTenant(int id, [FromBody] TenantDto tenantDto)
        {
            try
            {
                var tenant = dbContext.Tenants.Find(id);

                if (tenant is null)
                {
                    return NotFound(new ResponData<object?>(false, $"Tenant data with id {id} not found"));
                }

                _mapper.Map(tenantDto, tenant);
                tenant.UpdateTimestamps();

                if (tenantDto.User != null)
                {
                    var User = dbContext.Users.Find(tenant.UserId);
                    if (User != null)
                    {
                        _mapper.Map(tenantDto.User, User);
                        if (!string.IsNullOrWhiteSpace(tenantDto.User.Password))
                        {
                            User.Password = Bcrypt.BcryptPassword(tenantDto.User.Password);
                        }

                        User.UpdateTimestamps();
                    }
                }

                dbContext.SaveChanges();

                var respon = new ResponData<TenantRespon>(true, _mapper.Map<TenantRespon>(tenant), $"Successfully updated tenant data with id {id}");

                return Ok(respon);
            }
            catch (Exception ex)
            {
                var respon = new ResponData<object?>(false, ex.Message);

                return BadRequest(respon);
            }
        }

        [HttpPut]
        [Route("updatemyprofile")]
        [Authorize(Roles = "TENANT")]
        public IActionResult UpdateMyProfile([FromBody] TenantDto tenantDto)
        {
            try
            {
                int userId = int.Parse(User.FindFirst("userId")?.Value ?? "0");
                var tenant = dbContext.Tenants.FirstOrDefault(t => t.UserId == userId);

                if (tenant is null)
                {
                    return NotFound(new ResponData<object?>(false, $"Tenant data with id {userId} not found"));
                }

                _mapper.Map(tenantDto, tenant);
                tenant.UpdateTimestamps();

                if (tenantDto.User != null)
                {
                    var User = dbContext.Users.Find(tenant.UserId);
                    if (User != null)
                    {
                        if (!string.IsNullOrWhiteSpace(tenantDto.User.Password))
                        {
                            User.Password = Bcrypt.BcryptPassword(tenantDto.User.Password);
                        }
                        _mapper.Map(tenantDto.User, User);
                        User.Role = 2;
                        User.IsActive = true;
                        User.UpdateTimestamps();
                    }
                }

                dbContext.SaveChanges();

                var respon = new ResponData<TenantRespon>(true, _mapper.Map<TenantRespon>(tenant), $"Successfully updated tenant data with id {userId}");

                return Ok(respon);
            }
            catch (Exception ex)
            {
                var respon = new ResponData<object?>(false, ex.Message);

                return BadRequest(respon);
            }
        }

        [HttpPost]
        [Route("validate/{id:int}")]
        [Authorize(Roles = "ADMIN")]
        public IActionResult Validation(int id)
        {
            try
            {
                var tenant = dbContext.Tenants.Find(id);

                if (tenant is null)
                {
                    return NotFound(new ResponData<object?>(false, $"Tenant data with id {id} not found"));
                }

                tenant.IsValid = true;
                dbContext.SaveChanges();

                var respon = new ResponData<TenantRespon>(true, _mapper.Map<TenantRespon>(tenant), $"Successfully validated tenant data with id {id}");

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
        [Authorize(Roles = "ADMIN")]
        public IActionResult DeleteTenant(int id)
        {
            try
            {
                var tenant = dbContext.Tenants.Find(id);

                if (tenant is null)
                {
                    return NotFound(new ResponData<object?>(false, $"Tenant data with id {id} not found"));
                }

                if (tenant.UserId != 0)
                {
                    var User = dbContext.Users.Find(tenant.UserId);
                    if (User != null)
                    {
                        User.SoftDelete();
                    }
                }

                tenant.SoftDelete();
                dbContext.SaveChanges();

                var respon = new ResponData<object?>(true, "Successfully deleted tenant data");

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