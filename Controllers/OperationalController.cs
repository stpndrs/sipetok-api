using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using sipetok_api.Controllers.Factories;
using sipetok_api.Controllers.Products;
using sipetok_api.Data;
using sipetok_api.dto.Request;
using sipetok_api.dto.Response;
using sipetok_api.Models;
using sipetok_api.Repositories;
using System.Threading.Tasks;

namespace sipetok_api.Controllers
{
    [Authorize(Roles = "TENANT")]
    [Route("api/operationals")]
    [ApiController]
    public class OperationalController : ControllerBase
    {
        private readonly IStevanModuleFactory _factory;
        private readonly AppDbContext _dbContext;
        private int CurrentUserId => int.Parse(User.FindFirst("userId")?.Value ?? "0");
        private readonly Operational _operational = new Operational();
        private readonly OperationalResponseDto _response = new OperationalResponseDto();

        public OperationalController(OperationalFactory factory, AppDbContext context)
        {
            _factory = factory;
            _dbContext = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllOperationals()
        {
            var tenant = await GetExistingTenant();
            if (tenant == null) return Forbid();

            var searchQuery = new[] { $"TenantId : {tenant.Id}" };

            var worker = _factory.CreateMethod("get");

            return await worker.ActionAsync<Operational, OperationalResponseDto>(
                model: _operational,
                response: _response,
                id: null,
                userId: CurrentUserId,
                searchQuery: searchQuery,
                includes: null
            );
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetOperationalById(int id)
        {
            IStevanMethod worker = _factory.CreateMethod("get");
            return await worker.ActionAsync<Operational, OperationalResponseDto>(
                model: _operational,
                response: _response,
                id: id,
                userId: CurrentUserId,
                searchQuery: null,
                includes: null
            );
        }

        [HttpPost]
        public async Task<IActionResult> AddOperational([FromBody] OperationalRequestDto request)
        {
            IStevanMethod worker = _factory.CreateMethod("save");

            Tenant tenant = await GetExistingTenant();
            if (tenant == null) return NotFound();

            request.TenantId = tenant!.Id;

            return await worker.ActionAsync<Operational, OperationalResponseDto, OperationalRequestDto>(
                model: _operational,
                response: _response,
                request: request,
                httpMethod: "POST"
            );
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateOperational(int id, [FromBody] OperationalRequestDto request)
        {
            IStevanMethod worker = _factory.CreateMethod("save");

            Tenant tenant = await GetExistingTenant();
            if (tenant == null) return NotFound();

            request.TenantId = tenant!.Id;

            return await worker.ActionAsync<Operational, OperationalResponseDto, OperationalRequestDto>(
                model: _operational,
                response: _response,
                request: request,
                httpMethod: "PUT",
                id: id,
                userId: CurrentUserId
            );
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteOperational(int id)
        {
            IStevanMethod worker = _factory.CreateMethod("save");

            var tenant = await GetExistingTenant();
            var operational = await GetExistingOperational(id);
            if (tenant.Id != operational.TenantId) return Forbid();

            return await worker.ActionAsync<Operational, OperationalResponseDto, object>(
                model: _operational,
                response: _response,
                request: null,
                httpMethod: "DELETE",
                id: id,
                userId: CurrentUserId
            );
        }

        private async Task<Tenant> GetExistingTenant()
        {
            var repository = new TenantRepository(_dbContext);
            var tenant = await repository.GetTenantByUserId(CurrentUserId);

            if (tenant == null)
            {
                throw new InvalidOperationException("Tenant tidak ditemukan");
            }

            return tenant;
        }

        private async Task<Operational> GetExistingOperational(int id)
        {
            var repository = new OperationalRepository(_dbContext);
            var operational = await repository.GetOperationalById(id);

            if (operational == null)
            {
                throw new InvalidOperationException("Operational tidak ditemukan");
            }

            return operational;
        }
    }
}