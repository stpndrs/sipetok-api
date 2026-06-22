using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using sipetok_api.Controllers.Factories;
using sipetok_api.dto.Request;
using sipetok_api.dto.Response;
using sipetok_api.Models;
using sipetok_api.Data;
using System;
using System.Threading.Tasks;
using sipetok_api.Repositories;

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
        private int CurrentUserId => int.Parse(User.FindFirst("userId")?.Value ?? "0");
        private readonly EggCategory _eggCategory = new EggCategory();
        private readonly EggCategoryResponseDto _response = new EggCategoryResponseDto();

        public EggCategoryController(AppDbContext context, IConfiguration appConfig, IMapper mapper)
        {
            _dbContext = context;
            _factory = new EggCategoryFactory(context, appConfig, mapper);
        }

        [HttpGet]
        [Authorize(Roles = "TENANT")]
        public async Task<IActionResult> GetAllEggCategory()
        {
            var tenant = await getExistingTenant();

            var worker = _factory.CreateMethod("get");

            var repository = new TenantRepository(_dbContext);
            if (tenant == null) return Forbid();

            var searchQuery = new[] { $"TenantId : {tenant.Id}" };

            return await worker.ActionAsync<EggCategory, EggCategoryResponseDto>(
                model: _eggCategory,
                response: _response,
                id: null,
                userId: tenant.Id,
                searchQuery: new[] { $"TenantId : {tenant.Id}" }
            );
        }

        [HttpGet("{id:int}")]
        [Authorize(Roles = "TENANT")]
        public async Task<IActionResult> GetEggCategoryById(int id)
        {
            var tenant = await getExistingTenant();

            var worker = _factory.CreateMethod("get");

            var repository = new TenantRepository(_dbContext);
            if (tenant == null) return Forbid();

            return await worker.ActionAsync<EggCategory, EggCategoryResponseDto>(
                model: _eggCategory,
                response: _response,
                id: id,
                userId: tenant.Id,
                searchQuery: new[] { $"TenantId : {tenant.Id}" }
            );
        }

        [HttpPost]
        [Authorize(Roles = "TENANT")]
        public async Task<IActionResult> AddEggCategory([FromBody] EggCategoryRequestDto request)
        {
            var tenant = await getExistingTenant();
            if (tenant == null) return Forbid();

            var eggCategoryData = new EggCategory
            {
                Name = request.Name,
                Price = request.Price,
                TenantId = tenant.Id
            };

            var worker = _factory.CreateMethod("save");

            return await worker.ActionAsync<EggCategory, EggCategoryResponseDto, EggCategory>(
                model: _eggCategory,
                response: _response,
                request: eggCategoryData,
                httpMethod: "POST"
            );
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "TENANT")]
        public async Task<IActionResult> UpdateEggCategory(int id, [FromBody] EggCategoryRequestDto request)
        {
            var tenant = await getExistingTenant();

            var existingCategory = await _dbContext.EggCategories.Include(c => c.Tenant)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (existingCategory == null) return NotFound();
            if (existingCategory.Tenant?.UserId != tenant.Id) return Forbid();

            var worker = _factory.CreateMethod("save");

            return await worker.ActionAsync<EggCategory, EggCategoryResponseDto, EggCategoryRequestDto>(
                model: _eggCategory,
                response: _response,
                request: request,
                httpMethod: "PUT",
                id: id,
                userId: tenant.Id
            );
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "TENANT")]
        public async Task<IActionResult> DeleteEggCategory(int id)
        {
            var tenant = await getExistingTenant();

            var existingCategory = await _dbContext.EggCategories.Include(c => c.Tenant)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (existingCategory == null) return NotFound();
            if (existingCategory.Tenant?.UserId != tenant.Id) return Forbid();

            var worker = _factory.CreateMethod("delete");

            return await worker.ActionAsync<EggCategory, EggCategoryResponseDto, object>(
                model: _eggCategory,
                response: _response,
                request: null!,
                httpMethod: "DELETE",
                id: id, userId:
                tenant.Id
            );
        }

        private async Task<Tenant> getExistingTenant()
        {
            var repository = new TenantRepository(_dbContext);
            var tenant = await repository.GetTenantByUserId(CurrentUserId);

            if (tenant == null)
            {
                throw new InvalidOperationException("Tenant tidak ditemukan");
            }

            return tenant;
        }
    }
}