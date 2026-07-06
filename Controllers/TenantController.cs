using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using sipetok_api.Controllers.Factories;
using sipetok_api.Data;
using sipetok_api.dto.Request;
using sipetok_api.dto.Response;
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
        private readonly IStevanModuleFactory _factory;
        private readonly AppDbContext _dbContext;

        private int CurrentUserId => int.Parse(User.FindFirst("userId")?.Value ?? "0");
        private readonly Tenant _tenant = new Tenant();
        private readonly TenantResponseDto _response = new TenantResponseDto();

        public TenantController(TenantFactory factory, AppDbContext context)
        {
            _factory = factory;
            _dbContext = context;
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GetAllTenant()
        {
            var worker = _factory.CreateMethod("get");
            return await worker.ActionAsync<Tenant, TenantResponseDto>(
                model: _tenant,
                response: _response
            );
        }

        [HttpGet("{id:int}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GetTenantById(int id)
        {
            var worker = _factory.CreateMethod("get");
            return await worker.ActionAsync<Tenant, TenantResponseDto>(
                model: _tenant,
                response: _response,
                id: id
            );
        }

        [HttpGet("myprofile")]
        [Authorize(Roles = "TENANT")]
        public async Task<IActionResult> GetMyProfile()
        {
            var worker = _factory.CreateMethod("get");

            int tenantId = _dbContext.Tenants.Where(t => t.UserId == CurrentUserId).Select(t => t.Id).FirstOrDefault();

            return await worker.ActionAsync<Tenant, TenantResponseDto>(
                model: _tenant,
                response: _response,
                id: null,
                userId: CurrentUserId,
                searchQuery: new[] { $"UserId : {CurrentUserId}" }
            );
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
                model: _tenant,
                response: _response,
                request: request,
                httpMethod: "POST"
            );
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> UpdateTenant(int id, [FromBody] TenantRequestDto request)
        {
            HashUserPasswordIfPresent(request);

            var worker = _factory.CreateMethod("save");
            return await worker.ActionAsync<Tenant, TenantResponseDto, TenantRequestDto>(
                model: _tenant,
                response: _response,
                request: request,
                httpMethod: "PUT",
                id: id
            );
        }

        [HttpPut("updatemyprofile")]
        [Authorize(Roles = "TENANT")]
        public async Task<IActionResult> UpdateMyProfile([FromBody] TenantRequestDto request)
        {
            HashUserPasswordIfPresent(request);

            var worker = _factory.CreateMethod("save");
            return await worker.ActionAsync<Tenant, TenantResponseDto, TenantRequestDto>(
                model: _tenant,
                response: _response,
                request: request,
                httpMethod: "PUT",
                id: null,
                userId: CurrentUserId,
                searchQuery: new[] { $"UserId : {CurrentUserId}" }
            );
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
                model: _tenant,
                response: _response,
                request: null,
                httpMethod: "DELETE",
                id: id
            );
        }

        private async Task<IActionResult> DeleteUserTenant(int id)
        {
            var worker = _factory.CreateMethod("save");
            return await worker.ActionAsync<User, UserResponseDto, object>(
                model: new User(),
                response: new UserResponseDto(),
                request: null,
                httpMethod: "DELETE",
                id: id
            );
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