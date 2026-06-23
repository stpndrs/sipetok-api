using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using sipetok_api.Controllers.Factories;
using sipetok_api.Data;
using sipetok_api.dto.Request; 
using sipetok_api.Models;
using sipetok_api.Repositories;
using sipetok_api.Respon;
using System.Threading.Tasks;

namespace sipetok_api.Controllers
{
    [Authorize(Roles = "TENANT")]
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
        public async Task<IActionResult> AddEgg([FromBody] EggInventoryRequestDto request)
        {
            var worker = _factory.CreateMethod("save");
            return await worker.ActionAsync<EggInventory, EggInventoryResponseDto, EggInventoryRequestDto>(
                new EggInventory(), new EggInventoryResponseDto(), request, "POST");
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateEgg(int id, [FromBody] EggInventoryRequestDto request)
        {
            var worker = _factory.CreateMethod("save");
            return await worker.ActionAsync<EggInventory, EggInventoryResponseDto, EggInventoryRequestDto>(
                new EggInventory(), new EggInventoryResponseDto(), request, "PUT", id);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteEgg(int id)
        {
            var worker = _factory.CreateMethod("save");
            return await worker.ActionAsync<EggInventory, EggInventoryResponseDto, object>(
                new EggInventory(), new EggInventoryResponseDto(), null!, "DELETE", id);
        }
    }
}