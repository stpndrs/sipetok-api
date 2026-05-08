using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

using sipetok_api.Models;
using sipetok_api.dto.Request;
using sipetok_api.Data;
using AutoMapper;
using sipetok_api.dto.Respon;
using sipetok_api.Respon;

[Authorize]
[Route("api/eggs")]
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
    [Authorize(Roles = "1")]
    public IActionResult GetAllEggs()
    {
        try
        {
            var allEgg = _mapper.Map<List<EggRespon>>(dbContext.Eggs.Include(e => e.tenant).Include(e => e.category).ToList());

            var respon = new ResponData<List<EggRespon>>
            {
                success = true,
                data = allEgg,
                message = "Successfully retrieved all egg data"
            };

            return Ok(respon);
        }
        catch (Exception ex)
        {
            var respon = new ResponData<List<EggRespon>>
            {
                success = false,
                message = ex.Message
            };

            return StatusCode(500, respon);
        }
    }

    [HttpGet]
    [Route("{id:int}")]
    public IActionResult GetEggById(int id)
    {
        try
        {
            var egg = _mapper.Map<EggRespon>(dbContext.Eggs.Include(e => e.tenant).Include(e => e.category).FirstOrDefault(e => e.id == id));

            if (egg is null)
            {
                return NotFound(new ResponData<EggRespon>
                {
                    success = false,
                    message = $"Egg data with id {id} not found"
                });
            }

            var respon = new ResponData<EggRespon>
            {
                success = true,
                data = egg,
                message = $"Successfully retrieved egg data with id {id}"
            };
            return Ok(respon);
        }
        catch (Exception ex)
        {
            var respon = new ResponData<EggRespon>
            {
                success = false,
                message = ex.Message
            };
            return StatusCode(500, respon);
        }
    }

    [HttpGet]
    [Route("tenant/{id:int}")]
    public IActionResult GetEggByTenantId(int tenantId)
    {
        try
        {
            var egg = _mapper.Map<List<EggRespon>>(dbContext.Eggs.Include(e => e.tenant).Include(e => e.category).Where(e => e.tenant_id == tenantId).ToList());

            if (egg == null)
            {
                return NotFound(new ResponData<List<EggRespon>>
                {
                    success = false,
                    message = $"Egg data with tenant id {tenantId} not found"
                });
            }

            var respon = new ResponData<List<EggRespon>>
            {
                success = true,
                data = egg,
                message = $"Successfully retrieved egg data with tenant id {tenantId}"
            };

            return Ok(respon);
        }
        catch (Exception ex)
        {
            var respon = new ResponData<List<EggRespon>>
            {
                success = false,
                message = ex.Message
            };

            return StatusCode(500, respon);
        }
    }

    [HttpGet]
    [Route("tenant/total/{tenantId:int}")]
    public IActionResult GetTotalEggByTenantId(int tenantId)
    {
        try
        {
            var totalStock = dbContext.Eggs
        .Where(e => e.tenant_id == tenantId)
        .Sum(e => e.stock);

            var respon = new ResponData<int>
            {
                success = true,
                data = totalStock,
                message = $"Successfully retrieved total egg stock for tenant {tenantId}"
            };

            return Ok(respon);
        }
        catch (Exception ex)
        {
            var respon = new ResponData<int>
            {
                success = true,
                message = ex.Message
            };

            return StatusCode(500, respon);
        }
    }

    [HttpPost]
    [Authorize(Roles = "2")]
    public IActionResult AddEgg([FromBody] EggDto eggDto)
    {
        try
        {
            int userId = int.Parse(User.FindFirst("userId")?.Value ?? "0");
            var tenant = dbContext.Tenants.Include(c => c.user).FirstOrDefault(c => c.user_id == userId);
            var eggCategory = dbContext.EggCategories.Find(eggDto.category_id);

            if (tenant is null)
            {
                return BadRequest(new ResponData<EggRespon>
                {
                    success = false,
                    message = "Data tenant not found"
                });
            }
            if (eggCategory is null)
            {
                return BadRequest(new ResponData<EggRespon>
                {
                    success = false,
                    message = "Data category not found"
                });
            }

            var egg = _mapper.Map<Egg>(eggDto);

            egg.tenant = tenant;
            egg.category = eggCategory;

            dbContext.Eggs.Add(egg);
            dbContext.SaveChanges();

            var respon = new ResponData<EggRespon>
            {
                success = true,
                data = _mapper.Map<EggRespon>(egg),
                message = "Successfully added egg data"
            };

            return Ok(respon);
        }
        catch (Exception ex)
        {
            var respon = new ResponData<EggRespon>
            {
                success = false,
                message = ex.Message
            };

            return BadRequest(respon);
        }
    }

    [HttpPut]
    [Route("{id:int}")]
    public IActionResult UpdateEgg(int id, [FromBody] EggDto eggDto)
    {
        try
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
            egg.UpdateTimestamps();

            dbContext.SaveChanges();

            var respon = new ResponData<EggRespon>
            {
                success = true,
                data = _mapper.Map<EggRespon>(egg),
                message = "Successfully updated egg data"
            };

            return Ok(respon);
        }
        catch (Exception ex)
        {
            var respon = new ResponData<EggRespon>
            {
                success = false,
                message = ex.Message
            };

            return BadRequest(respon);
        }
    }

    [HttpPut]
    [Route("kurangi/{idTenant:int}")]
    public IActionResult KurangiEggByTenant(int idTenant, [FromQuery] int jumlah)
    {
        try
        {
            var listEgg = dbContext.Eggs
                .Where(e => e.tenant_id == idTenant && e.stock > 0)
                .OrderBy(e => e.production_date)
                .ToList();

            int totalStokTersedia = listEgg.Sum(e => e.stock);
            if (totalStokTersedia < jumlah)
            {
                return BadRequest(new ResponData<string>
                {
                    success = false,
                    message = $"Stock insufficient. Total stock: {totalStokTersedia}, Request: {jumlah}"
                });
            }

            int sisaYangHarusDikurangi = jumlah;

            foreach (var egg in listEgg)
            {
                if (sisaYangHarusDikurangi <= 0) break;

                if (egg.stock >= sisaYangHarusDikurangi)
                {
                    egg.stock -= sisaYangHarusDikurangi;
                    sisaYangHarusDikurangi = 0;
                }
                else
                {
                    sisaYangHarusDikurangi -= egg.stock;
                    egg.stock = 0;
                }
            }

            dbContext.SaveChanges();

            return Ok(new ResponData<string>
            {
                success = true,
                message = $"Successfully reduced {jumlah} eggs from tenant {idTenant}"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ResponData<string>
            {
                success = false,
                message = ex.Message
            });
        }
    }

    [HttpDelete]
    [Route("{id:int}")]
    public IActionResult DeleteEgg(int id)
    {
        try
        {
            var egg = dbContext.Eggs.Find(id);

            if (egg is null)
            {
                return NotFound(new ResponData<EggRespon>
                {
                    success = false,
                    message = $"Egg data with id {id} not found"
                });
            }

            egg.SoftDelete();
            dbContext.SaveChanges();

            var respon = new ResponData<EggRespon>
            {
                success = true,
                message = "Successfully deleted egg data"
            };

            return Ok(respon);
        }
        catch (Exception ex)
        {
            var respon = new ResponData<EggRespon>
            {
                success = false,
                message = ex.Message
            };

            return BadRequest(respon);
        }
    }
}