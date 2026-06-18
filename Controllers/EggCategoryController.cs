using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using sipetok_api.Controllers.Factories;
using sipetok_api.dto.Request;
using sipetok_api.dto.Response;
using sipetok_api.Models;
<<<<<<< HEAD
=======
using sipetok_api.Data;
using System;
using System.Threading.Tasks;
>>>>>>> 66185cb9672652d715a413bd97d21b5b6f10fbf7

namespace sipetok_api.Controllers
{
    [Authorize(Roles = "TENANT")]
    [ApiController]
    [Route("api/egg/categories")]
    public class EggCategoryController : ControllerBase
    {
        private readonly StevanModuleFactory _factory;
        private readonly IConfiguration appConfig;
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;

        public EggCategoryController(AppDbContext context, IConfiguration config, IMapper mapper)
        {
            _dbContext = context;
            _factory = new EggCategoryFactory(context, appConfig, mapper);
        }

        [HttpGet]
        [Authorize(Roles = "TENANT")]
        public async Task<IActionResult> GetAllEggCategory()
        {
            int userId = int.Parse(User.FindFirst("userId")?.Value ?? "0");

            var worker = _factory.CreateMethod("get");
            EggCategory model = new EggCategory();
            EggCategoryResponseDto response = new EggCategoryResponseDto();

            return await worker.ActionAsync<EggCategory, EggCategoryResponseDto>(
                model, response, null, userId, null, new[] { "Tenant" });
        }

        [HttpGet("{id:int}")]
        [Authorize(Roles = "TENANT")]
        public async Task<IActionResult> GetEggCategoryById(int id)
        {
            int userId = int.Parse(User.FindFirst("userId")?.Value ?? "0");

            var worker = _factory.CreateMethod("get");
            EggCategory model = new EggCategory();
            EggCategoryResponseDto response = new EggCategoryResponseDto();

            return await worker.ActionAsync<EggCategory, EggCategoryResponseDto>(
                model, response, id, userId, null, new[] { "Tenant" });
        }

        [HttpPost]
        [Authorize(Roles = "TENANT")]
        public async Task<IActionResult> AddEggCategory([FromBody] EggCategoryRequestDto request)
        {
            int userId = int.Parse(User.FindFirst("userId")?.Value ?? "0");

            // Ambil tenant berdasarkan userId
            Tenant tenant = await _dbContext.Tenants.FirstOrDefaultAsync(a => a.UserId == userId);
            if (tenant == null) return Forbid();

            var eggCategoryData = new EggCategory
            {
                Name = request.Name,
                Price = request.Price,
                TenantId = tenant.Id
            };

            var worker = _factory.CreateMethod("save");

            return await worker.ActionAsync<EggCategory, EggCategoryResponseDto, EggCategory>(
                new EggCategory(), new EggCategoryResponseDto(), eggCategoryData, "POST");
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "TENANT")]
        public async Task<IActionResult> UpdateEggCategory(int id, [FromBody] EggCategoryRequestDto request)
        {
            int userId = int.Parse(User.FindFirst("userId")?.Value ?? "0");

            var existingCategory = await _dbContext.EggCategories.Include(c => c.Tenant)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (existingCategory == null) return NotFound();
            if (existingCategory.Tenant?.UserId != userId) return Forbid();

            var worker = _factory.CreateMethod("save");

            return await worker.ActionAsync<EggCategory, EggCategoryResponseDto, EggCategoryRequestDto>(
                new EggCategory(), new EggCategoryResponseDto(), request, "PUT", id, userId);
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "TENANT")]
        public async Task<IActionResult> DeleteEggCategory(int id)
        {
            int userId = int.Parse(User.FindFirst("userId")?.Value ?? "0");

            // Validasi kepemilikan data sebelum didelete
            var existingCategory = await _dbContext.EggCategories.Include(c => c.Tenant)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (existingCategory == null) return NotFound();
            if (existingCategory.Tenant?.UserId != userId) return Forbid();

            var worker = _factory.CreateMethod("delete");

            return await worker.ActionAsync<EggCategory, EggCategoryResponseDto, object>(
                new EggCategory(), new EggCategoryResponseDto(), null!, "DELETE", id, userId);
        }
    }
}