using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using AutoMapper;

using sipetok_api.dto.Request;
using sipetok_api.Models;
using sipetok_api.Data;
using sipetok_api.dto.Respon;

namespace sipetok_api.Controllers
{
    [Authorize]
    [Route("api/operationals")]
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
        [Authorize(Roles = "ADMIN")]
        public IActionResult GetAllOperationals()
        {
            try
            {
                var allOperational = _mapper.Map<List<OperationalRespon>>(dbContext.Operationals.Include(c => c.tenant).ToList());

                var respon = new ResponData<List<OperationalRespon>>(true, allOperational, "Successfully retrieved all Operational data");

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
        [Authorize(Roles = "ADMIN, TENANT")]
        public IActionResult GetOperationalById(int id)
        {
            try
            {
                var operational = _mapper.Map<OperationalRespon>(dbContext.Operationals.Include(c => c.tenant).FirstOrDefault(c => c.id == id));

                if (operational == null)
                {
                    return NotFound(new ResponData<object?>(false, $"Operational data with id {id} not found"));
                }

                var respon = new ResponData<OperationalRespon>(true, operational, $"Successfully retrieved operational data with id {id}");

                return Ok(respon);
            }
            catch (Exception ex)
            {
                var respon = new ResponData<object?>(false, ex.Message);

                return StatusCode(500, respon);
            }
        }

        [HttpGet]
        [Route("tenant/{tenantId:int}")]
        [Authorize(Roles = "ADMIN")]
        public IActionResult GetOperationalByTenantId(int tenantId)
        {
            try
            {
                var operational = _mapper.Map<List<OperationalRespon>>(
                    dbContext.Operationals.Include(o => o.tenant).Where(o => o.tenant_id == tenantId).ToList()
                );

                if (operational == null)
                {
                    return NotFound(new ResponData<object?>(false, $"Operational data with tenant id {tenantId} not found"));
                }

                var respon = new ResponData<List<OperationalRespon>>(true, operational, $"Successfully retrieved operational data with tenant id {tenantId}");

                return Ok(respon);
            }
            catch (Exception ex)
            {
                var respon = new ResponData<object?>(false, ex.Message);

                return StatusCode(500, respon);
            }
        }

        [HttpGet]
        [Route("myoperational")]
        [Authorize(Roles = "TENANT")]
        public IActionResult GetMyOperational()
        {
            try
            {
                int userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == "userId")?.Value ?? "0");
                var operational = _mapper.Map<List<OperationalRespon>>
                (
                    (from o in dbContext.Operationals
                     join t in dbContext.Tenants on o.tenant_id equals t.id
                     where t.user_id == userId
                     select o
                     ).ToList()
                );

                if (operational == null)
                {
                    return NotFound(new ResponData<object?>(false, $"Operational data with user id {userId} not found"));
                }

                var respon = new ResponData<List<OperationalRespon>>(true, operational, $"Successfully retrieved operational data with user id {userId}");

                return Ok(respon);
            }
            catch (Exception ex)
            {
                var respon = new ResponData<object?>(false, ex.Message);

                return StatusCode(500, respon);
            }
        }

        [HttpPost]
        [Route("addmyoperational")]
        [Authorize(Roles = "TENANT")]
        public IActionResult AddMyOperational([FromBody] OperationalDto operationalDto)
        {
            try
            {
                int userId = int.Parse(User.FindFirst("userId")?.Value ?? "0");
                var tenant = _mapper.Map<TenantRespon>(dbContext.Tenants.Include(c => c.user).FirstOrDefault(c => c.user_id == userId));
                if (tenant is null)
                {
                    return BadRequest(new ResponData<object?>(false, "Tenant not found"));
                }

                var operational = _mapper.Map<Operational>(operationalDto);
                operational.tenant_id = tenant.id;

                dbContext.Operationals.Add(operational);
                dbContext.SaveChanges();

                var respon = new ResponData<OperationalRespon>(true, _mapper.Map<OperationalRespon>(operational), "Successfully added operational data");

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
        [Authorize(Roles = "TENANT")]
        public IActionResult UpdateOperational(int id, [FromBody] OperationalDto operationalDto)
        {
            try
            {
                var operational = dbContext.Operationals.Find(id);
                int userId = int.Parse(User.FindFirst("userId")?.Value ?? "0");
                var tenant = _mapper.Map<TenantRespon>(dbContext.Tenants.Include(c => c.user).FirstOrDefault(c => c.user_id == userId));

                if (tenant is null)
                {
                    return BadRequest(new ResponData<object?>(false, "Tenant not found"));
                }
                if (operational is null)
                {
                    return NotFound(new ResponData<object?>(false, $"Operational data with id {id} not found"));
                }
                if (operational.tenant_id != tenant.id)
                {
                    return BadRequest(new ResponData<object?>(false, "You are not authorized to update this operational data"));
                }

                _mapper.Map(operationalDto, operational);
                operational.UpdateTimestamps();

                dbContext.SaveChanges();

                var respon = new ResponData<OperationalRespon>(true, _mapper.Map<OperationalRespon>(operational), "Successfully updated operational data");

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
        [Authorize(Roles = "TENANT")]
        public IActionResult DeleteOperational(int id)
        {
            try
            {
                var operational = dbContext.Operationals.Find(id);
                int userId = int.Parse(User.FindFirst("userId")?.Value ?? "0");
                var tenant = _mapper.Map<TenantRespon>(dbContext.Tenants.Include(c => c.user).FirstOrDefault(c => c.user_id == userId));

                if (tenant is null)
                {
                    return BadRequest(new ResponData<object?>(false, "Tenant not found"));
                }
                if (operational is null)
                {
                    return NotFound(new ResponData<object?>(false, $"Operational data with id {id} not found"));
                }
                if (operational.tenant_id != tenant.id)
                {
                    return BadRequest(new ResponData<object?>(false, "You are not authorized to update this operational data"));
                }

                operational.SoftDelete();
                dbContext.SaveChanges();

                var respon = new ResponData<object?>(true, "Successfully deleted operational data");

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