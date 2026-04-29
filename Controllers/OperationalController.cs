using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using sipetok_api.dto.Request;
using sipetok_api.Models;
using sipetok_api.Data;
using AutoMapper;
using sipetok_api.dto.Respon;

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
        try
        {
            var allOperational = _mapper.Map<List<OperationalRespon>>(dbContext.Operationals.Include(c => c.tenant).ToList());

            var respon = new ResponData<List<OperationalRespon>>
            {
                status = true,
                data = allOperational,
                message = new List<string> { "Berhasil mengambil semua data Operational" }
            };

            return Ok(respon);
        }
        catch (Exception ex)
        {
            var respon = new ResponData<List<OperationalRespon>>
            {
                status = false,
                message = new List<string> { ex.Message }
            };

            return StatusCode(500, respon);
        }
    }

    [HttpGet]
    [Route("{id:int}")]
    public IActionResult GetOperationalById(int id)
    {
        try
        {
            var operational = _mapper.Map<OperationalRespon>(dbContext.Operationals.Include(c => c.tenant).FirstOrDefault(c => c.id == id));

            if (operational == null)
            {
                return NotFound(new ResponData<OperationalRespon>
                {
                    status = false,
                    message = new List<string> { $"Data operational dengan id {id} tidak ditemukan" }
                });
            }

            var respon = new ResponData<OperationalRespon>
            {
                status = true,
                data = operational,
                message = new List<string> { $"Berhasil mengambil data operational pada id {id}" }
            };

            return Ok(respon);
        }
        catch (Exception ex)
        {
            var respon = new ResponData<OperationalRespon>
            {
                status = false,
                message = new List<string> { ex.Message }
            };

            return StatusCode(500, respon);
        }
    }

    [HttpGet("tenant/{tenantId:int}")]
    public IActionResult GetOperationalByTenantId(int tenantId)
    {
        try
        {
            var operational = _mapper.Map<List<OperationalRespon>>(dbContext.Operationals.Include(o => o.tenant).Where(o => o.tenant_id == tenantId).ToList());

            if (operational == null)
            {
                return NotFound(new ResponData<OperationalRespon>
                {
                    status = false,
                    message = new List<string> { $"Data operational dengan id tenant {tenantId} tidak ditemukan" }
                });
            }

            var respon = new ResponData<List<OperationalRespon>>
            {
                status = true,
                data = operational,
                message = new List<string> { $"Berhasil mengambil data operational pada id tenant {tenantId}" }
            };

            return Ok(respon);
        }
        catch (Exception ex)
        {
            var respon = new ResponData<OperationalRespon>
            {
                status = false,
                message = new List<string> { ex.Message }
            };

            return StatusCode(500, respon);
        }
    }

    [HttpPost]
    public IActionResult AddOperational(OperationalDto operationalDto)
    {
        try
        {
            var tenant = dbContext.Tenants.Find(operationalDto.tenant_id);
            if (tenant is null)
            {
                return BadRequest("Tenant tidak ditemukan");
            }

            var operational = _mapper.Map<Operational>(operationalDto);
            operational.tenant = tenant;
            operational.tenant_id = tenant.id;

            dbContext.Operationals.Add(operational);
            dbContext.SaveChanges();

            var respon = new ResponData<OperationalRespon>
            {
                status = true,
                data = _mapper.Map<OperationalRespon>(operational),
                message = new List<string> { "Berhasil menambahkan data operational" }
            };

            return Ok(respon);
        }
        catch (Exception ex)
        {
            var respon = new ResponData<OperationalRespon>
            {
                status = false,
                message = new List<string> { ex.Message }
            };

            return Ok(respon);
        }
    }

    [HttpPut]
    [Route("{id:int}")]
    public IActionResult UpdateOperational(int id, OperationalDto operationalDto)
    {
        try
        {
            var operational = dbContext.Operationals.Find(id);

            if (operational is null)
            {
                return NotFound();
            }

            operational.name = operationalDto.name;
            operational.operational_cost = operationalDto.operational_cost;
            operational.operational_date = operationalDto.operational_date;

            dbContext.SaveChanges();

            var respon = new ResponData<OperationalRespon>
            {
                status = true,
                data = _mapper.Map<OperationalRespon>(operational),
                message = new List<string>{"Berhasil memperbarui data"}
            };

            return Ok(respon);
        }
        catch (Exception ex)
        {
            var respon = new ResponData<OperationalRespon>
            {
                status = false,
                message = new List<string>{ex.Message}
            };

            return Ok(respon);
        }
    }
}