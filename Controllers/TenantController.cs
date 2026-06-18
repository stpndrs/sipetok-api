using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using sipetok_api.Controllers.Factories;
using sipetok_api.Controllers.Products;
using sipetok_api.Data;
using sipetok_api.dto.Request;
using sipetok_api.dto.Response;
using sipetok_api.dto;
using sipetok_api.Models;
using System.Threading.Tasks;

namespace sipetok_api.Controllers
{
    [Authorize]
    [Route("api/tenants")]
    [ApiController]
    public class TenantController : ControllerBase
    {
        private readonly IConfiguration appConfig;
        private readonly StevanModuleFactory _factory;
        private readonly AppDbContext _dbContext;

        public TenantController(AppDbContext context, IConfiguration config, IMapper mapper)
        {
            _dbContext = context;
            _factory = new TenantFactory(context, appConfig, mapper);
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GetAllTenant()
        {
            var worker = _factory.CreateMethod("get");
            Tenant tenantModel = new Tenant();
            TenantResponseDto response = new TenantResponseDto();

            return await worker.ActionAsync<Tenant, TenantResponseDto>(tenantModel, response);
        }

        [HttpGet]
        [Route("{id:int}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GetTenantById(int id)
        {
            IStevanMethod worker = _factory.CreateMethod("get");
            Tenant tenantModel = new Tenant();
            TenantResponseDto response = new TenantResponseDto();
            return await worker.ActionAsync<Tenant, TenantResponseDto>(tenantModel, response, id);
        }

        [HttpGet]
        [Route("myprofile")]
        [Authorize(Roles = "TENANT")]
        public async Task<IActionResult> GetMyProfile()
        {
            int userId = int.Parse(User.FindFirst("userId")?.Value ?? "0");

            IStevanMethod worker = _factory.CreateMethod("get");
            Tenant tenantModel = new Tenant();
            TenantResponseDto response = new TenantResponseDto();

            return await worker.ActionAsync<Tenant, TenantResponseDto>(tenantModel, response, null, userId);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> AddTenant([FromBody] TenantRequestDto request)
        {
            IStevanMethod worker = _factory.CreateMethod("save");
            Tenant tenantModel = new Tenant();
            TenantResponseDto response = new TenantResponseDto();

            if (request.User == null || string.IsNullOrWhiteSpace(request.User.Password))
            {
                return new BadRequestObjectResult(new ResponData<object?>(false, "Password is required"));
            }
            string hashedPassword = Bcrypt.HashPassword(request.User.Password);
            request.User.Password = hashedPassword;

            return await worker.ActionAsync<Tenant, TenantResponseDto, TenantRequestDto>(tenantModel, response, request, "POST");
        }

        [HttpPut]
        [Route("{id:int}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> UpdateTenant(int id, [FromBody] TenantRequestDto request)
        {
            IStevanMethod worker = _factory.CreateMethod("save");
            Tenant tenantModel = new Tenant();
            TenantResponseDto response = new TenantResponseDto();

            if (request.User?.Password != null)
            {
                string hashedPassword = Bcrypt.HashPassword(request.User.Password);
                request.User.Password = hashedPassword;
            }

            return await worker.ActionAsync<Tenant, TenantResponseDto, TenantRequestDto>(
                model: tenantModel,
                response: response,
                request: request,
                httpMethod: "PUT",
                id: id
            );
        }

        [HttpPut]
        [Route("updatemyprofile")]
        [Authorize(Roles = "TENANT")]
        public async Task<IActionResult> UpdateMyProfile([FromBody] TenantRequestDto request)
        {
            int userId = int.Parse(User.FindFirst("userId")?.Value ?? "0");
            int tenantId = _dbContext.Tenants.Where(t => t.UserId == userId).Select(t => t.Id).FirstOrDefault();

            IStevanMethod worker = _factory.CreateMethod("save");
            Tenant tenantModel = new Tenant();
            TenantResponseDto response = new TenantResponseDto();

            if (request.User?.Password != null)
            {
                string hashedPassword = Bcrypt.HashPassword(request.User.Password);
                request.User.Password = hashedPassword;
            }

            return await worker.ActionAsync<Tenant, TenantResponseDto, TenantRequestDto>(tenantModel, response, request, "PUT", tenantId);
        }

        [HttpDelete]
        [Route("{id:int}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> DeleteTenant(int id)
        {
            IStevanMethod worker = _factory.CreateMethod("save");
            Tenant tenantModel = new Tenant();
            TenantResponseDto response = new TenantResponseDto();

            var userId = _dbContext.Tenants.Where(t => t.Id == id).Select(t => t.UserId).FirstOrDefault();
            if (userId != 0)
            {
                await DeleteUserTenant(userId);
            }


            return await worker.ActionAsync<Tenant, TenantResponseDto, object>(
                model: tenantModel,
                response: response,
                request: null,
                httpMethod: "DELETE",
                id: id
            );
        }

        public async Task<IActionResult> DeleteUserTenant(int id)
        {
            IStevanMethod worker = _factory.CreateMethod("save");
            User userModel = new User();
            UserResponseDto response = new UserResponseDto();

            return await worker.ActionAsync<User, UserResponseDto, object>(
                model: userModel,
                response: response,
                request: null,
                httpMethod: "DELETE",
                id: id
            );
        }
    }
}