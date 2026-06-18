using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using sipetok_api.Controllers.Factories;
using sipetok_api.Controllers.Products;
using sipetok_api.Data;
using sipetok_api.dto;
using sipetok_api.dto.Request;
using sipetok_api.dto.Response;
using sipetok_api.Models;
using sipetok_api.Repositories;
using sipetok_api.Respon;
using System.Threading.Tasks;

namespace sipetok_api.Controllers
{
    [Authorize]
    [Route("api/eggs")]
    [ApiController]
    public class EggInventoryController : ControllerBase
    {
        private readonly StevanModuleFactory _factory;
        private readonly AppDbContext _dbContext;
        private int CurrentUserId => int.Parse(User.FindFirst("userId")?.Value ?? "0");

        public EggInventoryController(AppDbContext context, IMapper mapper)
        {
            _factory = new EggInventoryFactory(context, mapper);
            _dbContext = context;
        }

        [HttpGet]
        [Authorize(Roles = "TENANT")]
        public async Task<IActionResult> GetAllEggs()
        {
            var worker = _factory.CreateMethod("get");
            EggInventory eggModel = new EggInventory();
            EggInventoryResponseDto response = new EggInventoryResponseDto();

            var tenantRepository = new TenantRepository(_dbContext);
            var tenant = await tenantRepository.GetTenantByUserId(CurrentUserId);
            if (tenant == null) return Forbid();

            var eggCategoryRepository = new EggCategoryRepository(_dbContext);
            var eggCategory = await eggCategoryRepository.GetEggCategoryByTenantId(tenant.Id);
            if (tenant == null) return Forbid();

            var searchQuery = new[] { $"CategoryId:{eggCategory.Id}" };

            return await worker.ActionAsync<EggInventory, EggInventoryResponseDto>(eggModel, response, null, null, searchQuery, new[] { "Category" });
        }

        [HttpGet("{id:int}")]
        [Authorize(Roles = "TENANT")]
        public async Task<IActionResult> GetEggById(int id)
        {
            IStevanMethod worker = _factory.CreateMethod("get");
            EggInventory eggModel = new EggInventory();
            EggInventoryResponseDto response = new EggInventoryResponseDto();

            var tenantRepository = new TenantRepository(_dbContext);
            var tenant = await tenantRepository.GetTenantByUserId(CurrentUserId);
            if (tenant == null) return Forbid();

            var eggCategoryRepository = new EggCategoryRepository(_dbContext);
            var eggCategory = await eggCategoryRepository.GetEggCategoryByTenantId(tenant.Id);
            if (tenant == null) return Forbid();

            var searchQuery = new[] { $"CategoryId:{eggCategory.Id}" };

            return await worker.ActionAsync<EggInventory, EggInventoryResponseDto>(eggModel, response, id, null, searchQuery, new[] { "Category" });
        }

        [HttpPost]
        [Authorize(Roles = "TENANT")]
        public async Task<IActionResult> AddEgg([FromBody] EggInventoryRequestDto request)
        {
            IStevanMethod worker = _factory.CreateMethod("save");
            EggInventory eggModel = new EggInventory();
            EggInventoryResponseDto response = new EggInventoryResponseDto();

            return await worker.ActionAsync<EggInventory, EggInventoryResponseDto, EggInventoryRequestDto>(eggModel, response, request, "POST");
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "TENANT")]
        public async Task<IActionResult> UpdateEgg(int id, [FromBody] EggInventoryRequestDto request)
        {
            IStevanMethod worker = _factory.CreateMethod("save");
            EggInventory eggModel = new EggInventory();
            EggInventoryResponseDto response = new EggInventoryResponseDto();

            return await worker.ActionAsync<EggInventory, EggInventoryResponseDto, EggInventoryRequestDto>(eggModel, response, request, "PUT", id);
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "TENANT")]
        public async Task<IActionResult> DeleteEgg(int id)
        {
            IStevanMethod worker = _factory.CreateMethod("save");
            EggInventory eggModel = new EggInventory();
            EggInventoryResponseDto response = new EggInventoryResponseDto();

            return await worker.ActionAsync<EggInventory, EggInventoryResponseDto, object>(eggModel, response, null, "DELETE", id);
        }
    }
}