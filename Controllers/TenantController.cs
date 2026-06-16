using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using sipetok_api.dto.Request;
using sipetok_api.dto.Respon;
using sipetok_api.Controllers.Products;
using sipetok_api.Controllers.Factories;
using sipetok_api.Models;

namespace sipetok_api.Controllers
{
    [Authorize]
    [Route("api/tenants")]
    [ApiController]
    public class TenantController : ControllerBase
    {
        private readonly ModuleFactory _factory;

        public TenantController(TenantFactory factory)
        {
            _factory = factory;
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GetAllTenant()
        {
            IMethod handler = _factory.CreateMethod("get");
            return await handler.ActionAsync<Tenant, TenantRespon>("getall");
        }

        [HttpGet]
        [Route("{id:int}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GetTenantById(int id)
        {
            IMethod handler = _factory.CreateMethod("get");
            return await handler.ActionAsync<Tenant, TenantRespon>("tenant_byid", id: id);
        }

        [HttpGet]
        [Route("myprofile")]
        [Authorize(Roles = "TENANT")]
        public async Task<IActionResult> GetMyProfile()
        {
            int userId = int.Parse(User.FindFirst("userId")?.Value ?? "0");
            IMethod handler = _factory.CreateMethod("get");
            return await handler.ActionAsync<Tenant, TenantRespon>("tenant_myprofile", userId: userId);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> AddTenant([FromBody] TenantDto tenantDto)
        {
            IMethod handler = _factory.CreateMethod("save");
            return await handler.ActionAsync<Tenant, TenantRespon>(
                subAction: "add_tenant",
                data: tenantDto,
                httpMethod: "POST"
            );
        }

        [HttpPut]
        [Route("{id:int}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> UpdateTenant(int id, [FromBody] TenantDto tenantDto)
        {
            IMethod handler = _factory.CreateMethod("save");
            return await handler.ActionAsync<Tenant, TenantRespon>(
                subAction: "update_tenant",
                data: tenantDto,
                httpMethod: "PUT",
                id: id
            );
        }

        [HttpPut]
        [Route("updatemyprofile")]
        [Authorize(Roles = "TENANT")]
        public async Task<IActionResult> UpdateMyProfile([FromBody] TenantDto tenantDto)
        {
            int userId = int.Parse(User.FindFirst("userId")?.Value ?? "0");
            IMethod handler = _factory.CreateMethod("save");
            return await handler.ActionAsync<Tenant, TenantRespon>(
                subAction: "update_myprofile",
                data: tenantDto,
                httpMethod: "PUT",
                userId: userId
            );
        }

        [HttpPost]
        [Route("validate/{id:int}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> Validation(int id)
        {
            IMethod handler = _factory.CreateMethod("save");
            // Karena tidak mengirim object body, kita isi data dengan object kosong / dummy
            return await handler.ActionAsync<Tenant, TenantRespon>(
                subAction: "validate_tenant",
                data: new object(),
                httpMethod: "POST",
                id: id
            );
        }

        [HttpDelete]
        [Route("{id:int}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> DeleteTenant(int id)
        {
            IMethod handler = _factory.CreateMethod("delete");
            return await handler.ActionAsync<Tenant, TenantRespon>("delete_tenant", id: id);
        }
    }
}