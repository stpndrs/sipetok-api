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
    [Authorize(Roles = "TENANT")]
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
        public IActionResult GetAllOperationals()
        {
            try
            {
                int userId = int.Parse(User.FindFirst("userId")?.Value ?? "0");

                var operationals = dbContext.Operationals
                    .Include(o => o.Tenant)
                    .Where(o => o.Tenant!.UserId == userId)
                    .ToList();

                var dataRespon = _mapper.Map<List<OperationalRespon>>(operationals);
                return Ok(new ResponData<List<OperationalRespon>>(true, dataRespon, "Successfully retrieved all your operational data"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponData<object?>(false, ex.Message));
            }
        }

        [HttpGet("{id:int}")]
        public IActionResult GetOperationalById(int id)
        {
            try
            {
                int userId = int.Parse(User.FindFirst("userId")?.Value ?? "0");

                var operational = dbContext.Operationals
                    .Include(c => c.Tenant)
                    .FirstOrDefault(c => c.Id == id);

                // Jika data tidak ada, atau ada tapi milik tenant lain
                if (operational == null || operational.Tenant!.UserId != userId)
                {
                    return NotFound(new ResponData<object?>(false, $"Operational data with id {id} not found"));
                }

                var dataRespon = _mapper.Map<OperationalRespon>(operational);
                return Ok(new ResponData<OperationalRespon>(true, dataRespon, $"Successfully retrieved operational data"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponData<object?>(false, ex.Message));
            }
        }

        [HttpPost]
        public IActionResult CreateOperational([FromBody] OperationalDto operationalDto)
        {
            try
            {
                int userId = int.Parse(User.FindFirst("userId")?.Value ?? "0");
                var tenant = dbContext.Tenants.FirstOrDefault(c => c.UserId == userId);

                if (tenant is null)
                {
                    return BadRequest(new ResponData<object?>(false, "Tenant profile not found"));
                }

                var operational = _mapper.Map<Operational>(operationalDto);
                operational.TenantId = tenant.Id; // Otomatis diset dari sistem, bukan dari body request

                dbContext.Operationals.Add(operational);
                dbContext.SaveChanges();

                var dataRespon = _mapper.Map<OperationalRespon>(operational);
                return Ok(new ResponData<OperationalRespon>(true, dataRespon, "Successfully added operational data"));
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponData<object?>(false, ex.Message));
            }
        }

        [HttpPut("{id:int}")]
        public IActionResult UpdateOperational(int id, [FromBody] OperationalDto operationalDto)
        {
            try
            {
                int userId = int.Parse(User.FindFirst("userId")?.Value ?? "0");

                var operational = dbContext.Operationals
                    .Include(o => o.Tenant)
                    .FirstOrDefault(o => o.Id == id);

                if (operational is null || operational.Tenant!.UserId != userId)
                {
                    return NotFound(new ResponData<object?>(false, $"Operational data with id {id} not found"));
                }

                _mapper.Map(operationalDto, operational);
                operational.UpdateTimestamps();

                dbContext.SaveChanges();

                var dataRespon = _mapper.Map<OperationalRespon>(operational);
                return Ok(new ResponData<OperationalRespon>(true, dataRespon, "Successfully updated operational data"));
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponData<object?>(false, ex.Message));
            }
        }

        [HttpDelete("{id:int}")]
        public IActionResult DeleteOperational(int id)
        {
            try
            {
                int userId = int.Parse(User.FindFirst("userId")?.Value ?? "0");

                var operational = dbContext.Operationals
                    .Include(o => o.Tenant)
                    .FirstOrDefault(o => o.Id == id);

                if (operational is null || operational.Tenant!.UserId != userId)
                {
                    return NotFound(new ResponData<object?>(false, $"Operational data with id {id} not found"));
                }

                operational.SoftDelete();
                dbContext.SaveChanges();

                return Ok(new ResponData<object?>(true, "Successfully deleted operational data"));
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponData<object?>(false, ex.Message));
            }
        }
    }
}