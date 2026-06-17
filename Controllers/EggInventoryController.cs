using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using sipetok_api.Controllers.Factories;
using sipetok_api.Controllers.Products;
using sipetok_api.dto.Request;
using sipetok_api.dto.Respon;
using sipetok_api.Models;
using sipetok_api.Respon;

namespace sipetok_api.Controllers
{
    [Authorize]
    [Route("api/eggs")]
    [ApiController]
    public class EggInventoryController : ControllerBase
    {
        private readonly ModuleFactory _factory;

        // Constructor sudah diperbaiki untuk menyuntikkan EggFactory
        public EggInventoryController(EggFactory factory)
        {
            _factory = factory;
        }

        [HttpGet]
        [Authorize(Roles = "TENANT")]
        public async Task<IActionResult> GetAllEggs()
        {
            int userId = int.Parse(User.FindFirst("userId")?.Value ?? "0");
            IMethod handler = _factory.CreateMethod("get");

            // Memanggil sub-action khusus untuk mengambil stock telur berdasarkan Tenant si user
            return await handler.ActionAsync<EggInventory, EggRespon>("egg_all_tenant", userId: userId);
        }

        [HttpGet]
        [Route("{id:int}")]
        [Authorize(Roles = "TENANT")]
        public async Task<IActionResult> GetEggById(int id)
        {
            int userId = int.Parse(User.FindFirst("userId")?.Value ?? "0");
            IMethod handler = _factory.CreateMethod("get");
            return await handler.ActionAsync<EggInventory, EggRespon>("get_egg_by_id", id: id, userId: userId);
        }

        [HttpPost]
        [Authorize(Roles = "TENANT")]
        public async Task<IActionResult> AddEgg([FromBody] EggDto eggDto)
        {
            int userId = int.Parse(User.FindFirst("userId")?.Value ?? "0");
            IMethod handler = _factory.CreateMethod("save");

            return await handler.ActionAsync<EggInventory, EggRespon>(
                subAction: "add_egg",
                data: eggDto,
                httpMethod: "POST",
                userId: userId
            );
        }

        [HttpPut]
        [Route("{id:int}")]
        [Authorize(Roles = "TENANT")]
        public async Task<IActionResult> UpdateEgg(int id, [FromBody] EggDto eggDto)
        {
            IMethod handler = _factory.CreateMethod("save");
            var userId = int.Parse(User.FindFirst("userId")?.Value ?? "0");

            return await handler.ActionAsync<EggInventory, EggRespon>(
                subAction: "update_egg",
                data: eggDto,
                httpMethod: "PUT",
                id: id,
                userId: userId
            );
        }

        [HttpDelete]
        [Route("{id:int}")]
        [Authorize(Roles = "TENANT")]
        public async Task<IActionResult> DeleteEgg(int id)
        {
            var userId = int.Parse(User.FindFirst("userId")?.Value ?? "0");
            IMethod handler = _factory.CreateMethod("delete");

            return await handler.ActionAsync<EggInventory, EggRespon>(
                subAction: "delete_egg",
                data: null,
                httpMethod: "DELETE",
                id: id,
                userId: userId
            );
        }
    }
}