using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using sipetok_api.Models;
using sipetok_api.dto.Request;
using sipetok_api.Data;
using AutoMapper;
using sipetok_api.dto.Respon;
using sipetok_api.Respon;
using System.Net.Http.Headers;

[Route("api/[controller]")]
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
    public IActionResult GetAllEggs()
    {
        try
        {
            var allEgg = _mapper.Map<List<EggRespon>>(dbContext.Eggs.Include(e => e.tenant).Include(e => e.category).ToList());

            var respon = new ResponData<List<EggRespon>>
            {
                status = true,
                data = allEgg,
                message = new List<string> { "Berhasil mengambil semua data egg " }
            };

            return Ok(respon);
        }
        catch (Exception ex)
        {
            var respon = new ResponData<List<EggRespon>>
            {
                status = false,
                message = new List<string> { ex.Message }
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
                    status = false,
                    message = new List<string> { $"Data egg dengan id {id} tidak ditemukan" }
                });
            }

            var respon = new ResponData<EggRespon>
            {
                status = true,
                data = egg,
                message = new List<string> { $"Berhasil mengambil data egg pada id {id}" }
            };
            return Ok(respon);
        }
        catch (Exception ex)
        {
            var respon = new ResponData<EggRespon>
            {
                status = false,
                message = new List<string> { ex.Message }
            };
            return StatusCode(500, respon);
        }
    }

    [HttpGet("tenant/{tenantId:int}")]
    public IActionResult GetEggByTenantId(int tenantId)
    {
        try
        {
            var egg = _mapper.Map<List<EggRespon>>(dbContext.Eggs.Include(e => e.tenant).Include(e => e.category).Where(e => e.tenant_id == tenantId).ToList());

            if (egg == null)
            {
                return NotFound(new ResponData<List<EggRespon>>
                {
                    status = false,
                    message = new List<string> { $"Data egg dengan id tenant {tenantId} tidak ditemukan" }
                });
            }

            var respon = new ResponData<List<EggRespon>>
            {
                status = true,
                data = egg,
                message = new List<string> { $"Berhasil mengambil data customer pada id tenant {tenantId}" }
            };

            return Ok(respon);
        }
        catch (Exception ex)
        {
            var respon = new ResponData<List<EggRespon>>
            {
                status = false,
                message = new List<string> { ex.Message }
            };

            return StatusCode(500, respon);
        }
    }

    [HttpGet("tenant/total/{tenantId:int}")]
    public IActionResult GetTotalEggByTenantId(int tenantId)
    {
        try
        {
            var totalStock = dbContext.Eggs
        .Where(e => e.tenant_id == tenantId)
        .Sum(e => e.stock);

            var respon = new ResponData<int>
            {
                status = true,
                data = totalStock,
                message = new List<string> { $"Berhasil mengambil total stok telur dari id tenant {tenantId}" }
            };

            return Ok(respon);
        }
        catch (Exception ex)
        {
            var respon = new ResponData<int>
            {
                status = true,
                message = new List<string> { ex.Message }
            };

            return StatusCode(500, respon);
        }
    }

    [HttpPost]
    public IActionResult AddEgg(EggDto eggDto)
    {
        try
        {
            var tenant = dbContext.Tenants.Find(eggDto.tenant_id);
            var eggCategory = dbContext.EggCategories.Find(eggDto.category_id);

            if (tenant == null)
            {
                return BadRequest(new ResponData<EggRespon>
                {
                    status = false,
                    message = new List<string> { "Data tenant tidak ditemukan" }
                });
            }
            if (eggCategory == null)
            {
                return BadRequest(new ResponData<EggRespon>
                {
                    status = false,
                    message = new List<string> { "Data category tidak ditemukan" }
                });
            }

            var egg = _mapper.Map<Egg>(eggDto);

            egg.tenant = tenant;
            egg.category = eggCategory;

            dbContext.Eggs.Add(egg);
            dbContext.SaveChanges();

            var respon = new ResponData<EggRespon>
            {
                status = true,
                data = _mapper.Map<EggRespon>(egg),
                message = new List<string> { "Berhasil menambahkan data egg" }
            };

            return Ok(respon);
        }
        catch (Exception ex)
        {
            var respon = new ResponData<EggRespon>
            {
                status = false,
                message = new List<string> { ex.Message }
            };

            return BadRequest(respon);
        }
    }

    [HttpPut]
    [Route("{id:int}")]
    public IActionResult UpdateEgg(int id, EggDto eggDto)
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

            dbContext.SaveChanges();

            var respon = new ResponData<EggRespon>
            {
                status = true,
                data = _mapper.Map<EggRespon>(egg),
                message = new List<string> { "Berhasil memperbarui data" }
            };

            return Ok(respon);
        }
        catch (Exception ex)
        {
            var respon = new ResponData<EggRespon>
            {
                status = false,
                message = new List<string> { ex.Message }
            };

            return BadRequest(respon);
        }
    }

    [HttpPut("kurangi/{idTenant:int}")]
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
                    status = false,
                    message = new List<string> { $"Stok tidak mencukupi. Total stok: {totalStokTersedia}, Permintaan: {jumlah}" }
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
                status = true,
                message = new List<string> { $"Berhasil mengurangi {jumlah} telur dari tenant {idTenant}" }
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ResponData<string>
            {
                status = false,
                message = new List<string> { ex.Message }
            });
        }
    }
}