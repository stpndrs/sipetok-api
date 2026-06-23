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
    [Authorize(Roles = "TENANT")]
    [Route("api/operationals")]
    [ApiController]
    public class OperationalController : ControllerBase
    {
        private readonly StevanModuleFactory _factory;
        private readonly AppDbContext _dbContext;

        public OperationalController(AppDbContext context, IMapper mapper)
        {
            _factory = new OperationalFactory(context, mapper);
            _dbContext = context;
        }

        [HttpGet]
        [Authorize(Roles = "TENANT")]
        public async Task<IActionResult> GetAllOperationals()
        {
            int userId = int.Parse(User.FindFirst("userId")?.Value ?? "0");

            var worker = _factory.CreateMethod("get");
            Operational operationalModel = new Operational();
            OperationalResponseDto response = new OperationalResponseDto();

            var repository = new TenantRepository(_dbContext);
            var tenant = await repository.GetTenantByUserId(userId);
            if (tenant == null) return Forbid();

            var searchQuery = new[] { $"TenantId : {tenant.Id}" };

            return await worker.ActionAsync<Operational, OperationalResponseDto>(operationalModel, response, null, null, searchQuery);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetOperationalById(int id)
        {
            int userId = int.Parse(User.FindFirst("userId")?.Value ?? "0");

            var worker = _factory.CreateMethod("get");
            Operational operationalModel = new Operational();
            OperationalResponseDto response = new OperationalResponseDto();

            var repository = new TenantRepository(_dbContext);
            var tenant = await repository.GetTenantByUserId(userId);
            if (tenant == null) return Forbid();

            var searchQuery = new[] { $"TenantId : {tenant.Id}" };

            return await worker.ActionAsync<Operational, OperationalResponseDto>(operationalModel, response, id, null, searchQuery);
        }

        [HttpPost]
        public async Task<IActionResult> AddOperational([FromBody] OperationalRequestDto request)
        {
            int userId = int.Parse(User.FindFirst("userId")?.Value ?? "0");
            IStevanMethod worker = _factory.CreateMethod("save");
            Operational operationalModel = new Operational();
            OperationalResponseDto response = new OperationalResponseDto();
            var repository = new TenantRepository(_dbContext);
            var tenant = await repository.GetTenantByUserId(userId);
            request.TenantId = tenant!.Id;

            return await worker.ActionAsync<Operational, OperationalResponseDto, OperationalRequestDto>(operationalModel, response, request, "POST");
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateOperational(int id, [FromBody] OperationalRequestDto request)
        {
            IStevanMethod worker = _factory.CreateMethod("save");
            Operational operationalModel = new Operational();
            OperationalResponseDto response = new OperationalResponseDto();

            return await worker.ActionAsync<Operational, OperationalResponseDto, OperationalRequestDto>(operationalModel, response, request, "PUT", id);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteOperational(int id)
        {
            IStevanMethod worker = _factory.CreateMethod("save");
            Operational operationalModel = new Operational();
            OperationalResponseDto response = new OperationalResponseDto();

            return await worker.ActionAsync<Operational, OperationalResponseDto, object>(operationalModel, response, null, "DELETE", id);
        }
    }
}