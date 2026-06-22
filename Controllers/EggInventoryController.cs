using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using sipetok_api.Controllers.Factories;
using sipetok_api.Controllers.Products;
using sipetok_api.Data;
using sipetok_api.dto.Request; // Disesuaikan jika Dto berada di subfolder ini
using sipetok_api.Models;
using sipetok_api.Repositories;
using sipetok_api.Response;
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
        private readonly EggInventory _eggInventory = new EggInventory();
        private readonly EggInventoryResponseDto _response = new EggInventoryResponseDto();

        public EggInventoryController(AppDbContext context, IMapper mapper)
        {
            _dbContext = context;
            _factory = new EggInventoryFactory(context, mapper);
            _dbContext = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllEggs()
        {
            var tenant = await getExistingTenant();
            var worker = _factory.CreateMethod("get");

            return await worker.ActionAsync<EggInventory, EggInventoryResponseDto>(
                model: _eggInventory, 
                response: _response,
                id:null, 
                userId:null,
                includes: new[] { "Category" },
                searchQuery: new[] { $"Category.tenantId : {tenant.Id}" }
            );
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetEggById(int id)
        {
            IStevanMethod worker = _factory.CreateMethod("get");

            return await worker.ActionAsync<EggInventory, EggInventoryResponseDto>(
                model: _eggInventory, 
                response: _response, 
                id: id
            );
        }

        [HttpPost]
        public async Task<IActionResult> AddEgg([FromBody] EggInventoryRequestDto request)
        {
            IStevanMethod worker = _factory.CreateMethod("save");

            var category = await _dbContext.EggCategories.FindAsync(request.CategoryId);
            if(category == null)
            {
                return NotFound(new { message = "Kategori telur tidak ditemukan" });
            }
            if (category.TenantId != getExistingTenant().Result.Id)
            {
                return NotFound(new { message = "Kategori telur tidak ditemukan" });
            }

            return await worker.ActionAsync<EggInventory, EggInventoryResponseDto, EggInventoryRequestDto>(
                model: _eggInventory, 
                response: _response, 
                request: request, 
                httpMethod: "POST"
            );
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateEgg(int id, [FromBody] EggInventoryRequestDto request)
        {
            IStevanMethod worker = _factory.CreateMethod("save");

            return await worker.ActionAsync<EggInventory, EggInventoryResponseDto, EggInventoryRequestDto>(
                model: _eggInventory, 
                response: _response, 
                request: request, 
                httpMethod: "PUT", 
                id: id
            );
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteEgg(int id)
        {
            IStevanMethod worker = _factory.CreateMethod("save");
            EggInventory eggModel = new EggInventory();
            EggInventoryResponseDto response = new EggInventoryResponseDto();

            return await worker.ActionAsync<EggInventory, EggInventoryResponseDto, object>(
                model: _eggInventory, 
                response: _response, 
                request: null, 
                httpMethod: "DELETE", 
                id: id
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