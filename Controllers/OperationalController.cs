using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using sipetok_api.Controllers.Factories;
using sipetok_api.Data;
using sipetok_api.dto.Request;
using sipetok_api.dto.Response;
using sipetok_api.Models;
using sipetok_api.Respon;
using System.Threading.Tasks;

namespace sipetok_api.Controllers
{
    [Authorize(Roles = "TENANT")]
    [ApiController]
    [Route("api/operationals")]
    public class OperationalController : ControllerBase
    {
        private readonly StevanModuleFactory _factory;
        private readonly AppDbContext _dbContext;

        private int CurrentUserId => int.Parse(User.FindFirst("userId")?.Value ?? "0");

        public OperationalController(AppDbContext context, IMapper mapper)
        {
            _factory = new OperationalFactory(context, mapper);
            _dbContext = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllOperationals()
        {
            var worker = _factory.CreateMethod("get");
            return await worker.ActionAsync<Operational, OperationalResponseDto>(
                new Operational(), new OperationalResponseDto());
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetOperationalById(int id)
        {
            var worker = _factory.CreateMethod("get");
            return await worker.ActionAsync<Operational, OperationalResponseDto>(
                new Operational(), new OperationalResponseDto(), id);
        }

        [HttpPost]
        public async Task<IActionResult> AddOperational([FromBody] OperationalRequestDto request)
        {
            var tenant = await _dbContext.Tenants.FirstOrDefaultAsync(a => a.UserId == CurrentUserId);
            if (tenant == null) return Forbid();

            request.TenantId = tenant.Id;

            var worker = _factory.CreateMethod("save");
            return await worker.ActionAsync<Operational, OperationalResponseDto, OperationalRequestDto>(
                new Operational(), new OperationalResponseDto(), request, "POST");
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateOperational(int id, [FromBody] OperationalRequestDto request)
        {
            var tenant = await _dbContext.Tenants.FirstOrDefaultAsync(a => a.UserId == CurrentUserId);
            if (tenant == null) return Forbid();

            request.TenantId = tenant.Id;

            var worker = _factory.CreateMethod("save");
            return await worker.ActionAsync<Operational, OperationalResponseDto, OperationalRequestDto>(
                new Operational(), new OperationalResponseDto(), request, "PUT", id);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteOperational(int id)
        {
            var worker = _factory.CreateMethod("save");
            return await worker.ActionAsync<Operational, OperationalResponseDto, object>(
                new Operational(), new OperationalResponseDto(), null!, "DELETE", id);
        }
    }
}