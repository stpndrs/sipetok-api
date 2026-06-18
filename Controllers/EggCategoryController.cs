using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using sipetok_api.Controllers.Factories;
using sipetok_api.Data;
using sipetok_api.dto.Request;
using sipetok_api.dto.Response;
using sipetok_api.dto.Response;
using sipetok_api.Models;
using System.Threading.Tasks;

namespace sipetok_api.Controllers
{
    [Authorize(Roles = "TENANT")]
    [ApiController]
    [Route("api/egg/categories")]
    public class EggCategoryController : ControllerBase
    {
        private readonly StevanModuleFactory _factory;
        private readonly AppDbContext _dbContext;

        
        private int CurrentUserId => int.Parse(User.FindFirst("userId")?.Value ?? "0");

        public EggCategoryController(AppDbContext context, IConfiguration config, IMapper mapper)
        {
            _dbContext = context;
            _factory = new EggCategoryFactory(context, config, mapper);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllEggCategory()
        {
            var worker = _factory.CreateMethod("get");
            return await worker.ActionAsync<EggCategory, EggCategoryResponseDto>(
                new EggCategory(), new EggCategoryResponseDto(), null, CurrentUserId, null, new[] { "Tenant" });
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetEggCategoryById(int id)
        {
            var worker = _factory.CreateMethod("get");
            return await worker.ActionAsync<EggCategory, EggCategoryResponseDto>(
                new EggCategory(), new EggCategoryResponseDto(), id, CurrentUserId, null, new[] { "Tenant" });
        }

        [HttpPost]
        public async Task<IActionResult> AddEggCategory([FromBody] EggCategoryRequestDto request)
        {
            var tenant = await _dbContext.Tenants.FirstOrDefaultAsync(a => a.UserId == CurrentUserId);
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
        public async Task<IActionResult> UpdateEggCategory(int id, [FromBody] EggCategoryRequestDto request)
        {
            var authResult = await ValidateOwnershipAsync(id);
            if (authResult != null) return authResult;

            var worker = _factory.CreateMethod("save");
            return await worker.ActionAsync<EggCategory, EggCategoryResponseDto, EggCategoryRequestDto>(
                new EggCategory(), new EggCategoryResponseDto(), request, "PUT", id, CurrentUserId);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteEggCategory(int id)
        {
            var authResult = await ValidateOwnershipAsync(id);
            if (authResult != null) return authResult;

            var worker = _factory.CreateMethod("delete");
            return await worker.ActionAsync<EggCategory, EggCategoryResponseDto, object>(
                new EggCategory(), new EggCategoryResponseDto(), null!, "DELETE", id, CurrentUserId);
        }

        private async Task<IActionResult?> ValidateOwnershipAsync(int id)
        {
            var existingCategory = await _dbContext.EggCategories
                .Include(c => c.Tenant)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (existingCategory == null) return NotFound();
            if (existingCategory.Tenant?.UserId != CurrentUserId) return Forbid();

            return null;
        }
    }
}