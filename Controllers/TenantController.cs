using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using sipetok_api.Controllers.Factories;
using sipetok_api.Data;
using sipetok_api.dto.Request;
using sipetok_api.dto.Response;
using sipetok_api.dto;
using sipetok_api.Models;
using sipetok_api.helper; 
using System.Linq;
using System.Threading.Tasks;

namespace sipetok_api.Controllers
{
    [Authorize]
    [Route("api/tenants")]
    [ApiController]
    public class TenantController : ControllerBase
    {
        private readonly StevanModuleFactory _factory;
        private readonly AppDbContext _dbContext;

        private int CurrentUserId => int.Parse(User.FindFirst("userId")?.Value ?? "0");

        public TenantController(AppDbContext context, IConfiguration config, IMapper mapper)
        {
            _dbContext = context;
            _factory = new TenantFactory(context, config, mapper);
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GetAllTenant()
        {
            var worker = _factory.CreateMethod("get");
            return await worker.ActionAsync<Tenant, TenantResponseDto>(new Tenant(), new TenantResponseDto());
        }

        [HttpGet("{id:int}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GetTenantById(int id)
        {
            var worker = _factory.CreateMethod("get");
            return await worker.ActionAsync<Tenant, TenantResponseDto>(new Tenant(), new TenantResponseDto(), id);
        }

        [HttpGet("myprofile")]
        [Authorize(Roles = "TENANT")]
        public async Task<IActionResult> GetMyProfile()
        {
            var worker = _factory.CreateMethod("get");
            return await worker.ActionAsync<Tenant, TenantResponseDto>(new Tenant(), new TenantResponseDto(), null, CurrentUserId);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> AddTenant([FromBody] TenantRequestDto request)
        {
            if (request.User == null || string.IsNullOrWhiteSpace(request.User.Password))
            {
                return BadRequest(new ResponData<object?>(false, "Password is required"));
            }

            request.User.Password = Bcrypt.HashPassword(request.User.Password);

            var worker = _factory.CreateMethod("save");
            return await worker.ActionAsync<Tenant, TenantResponseDto, TenantRequestDto>(
                new Tenant(), new TenantResponseDto(), request, "POST");
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> UpdateTenant(int id, [FromBody] TenantRequestDto request)
        {
            HashUserPasswordIfPresent(request);

            var worker = _factory.CreateMethod("save");
            return await worker.ActionAsync<Tenant, TenantResponseDto, TenantRequestDto>(
                new Tenant(), new TenantResponseDto(), request, "PUT", id);
        }

        [HttpPut("updatemyprofile")]
        [Authorize(Roles = "TENANT")]
        public async Task<IActionResult> UpdateMyProfile([FromBody] TenantRequestDto request)
        {
            int tenantId = _dbContext.Tenants.Where(t => t.UserId == CurrentUserId).Select(t => t.Id).FirstOrDefault();
            HashUserPasswordIfPresent(request);

            var worker = _factory.CreateMethod("save");
            return await worker.ActionAsync<Tenant, TenantResponseDto, TenantRequestDto>(
                new Tenant(), new TenantResponseDto(), request, "PUT", tenantId);
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> DeleteTenant(int id)
        {
            var associatedUserId = _dbContext.Tenants.Where(t => t.Id == id).Select(t => t.UserId).FirstOrDefault();
            if (associatedUserId != 0)
            {
                await DeleteUserTenant(associatedUserId);
            }

            var worker = _factory.CreateMethod("save");
            return await worker.ActionAsync<Tenant, TenantResponseDto, object>(
                new Tenant(), new TenantResponseDto(), null!, "DELETE", id);
        }

        private async Task<IActionResult> DeleteUserTenant(int id)
        {
            var worker = _factory.CreateMethod("save");
            return await worker.ActionAsync<User, UserResponseDto, object>(
                new User(), new UserResponseDto(), null!, "DELETE", id);
        }

        private void HashUserPasswordIfPresent(TenantRequestDto request)
        {
            if (!string.IsNullOrEmpty(request.User?.Password))
            {
                request.User.Password = Bcrypt.HashPassword(request.User.Password);
            }
        }
    }
}