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
    public class EggController : ControllerBase
    {
        private readonly EggFactory _factory;

        // Constructor sudah diperbaiki untuk menyuntikkan EggFactory
        public EggController(EggFactory factory)
        {
            _factory = factory;
        }

        [HttpGet]
        [Authorize(Roles = "TENANT")]
        public async Task<IActionResult> GetAllEggs()
        {
            int userId = int.Parse(User.FindFirst("userId")?.Value ?? "0");
            var handler = (GetData)_factory.CreateMethod("get");

            // Memanggil sub-action khusus untuk mengambil stock telur berdasarkan Tenant si user
            return await handler.ActionAsync<Egg, EggRespon>("getall", userId: userId);
        }

        [HttpGet]
        [Route("{id:int}")]
        [Authorize(Roles = "TENANT")]
        public async Task<IActionResult> GetEggById(int id)
        {
            var handler = (GetData)_factory.CreateMethod("get");
            return await handler.ActionAsync<Egg, EggRespon>("egg_byid", id: id);
        }

        [HttpPost]
        [Authorize(Roles = "TENANT")]
        public async Task<IActionResult> AddEgg([FromBody] EggDto eggDto)
        {
            int userId = int.Parse(User.FindFirst("userId")?.Value ?? "0");
            var handler = (SaveData)_factory.CreateMethod("save");

            return await handler.ActionAsync<Egg, EggDto, EggRespon>(
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
            var handler = (SaveData)_factory.CreateMethod("save");

            return await handler.ActionAsync<Egg, EggDto, EggRespon>(
                subAction: "update_egg",
                data: eggDto,
                httpMethod: "PUT",
                id: id
            );
        }

        [HttpDelete]
        [Route("{id:int}")]
        [Authorize(Roles = "TENANT")]
        public async Task<IActionResult> DeleteEgg(int id)
        {
            var handler = (DeleteData)_factory.CreateMethod("delete");
            return await handler.ActionAsync<Egg>("delete_egg", id: id);
        }
    }
}