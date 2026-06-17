using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using sipetok_api.Controllers.Factories;
using sipetok_api.Controllers.Products;
using sipetok_api.Data;
using sipetok_api.dto;
using sipetok_api.dto.Request;
using sipetok_api.dto.Respon;
using sipetok_api.Models;
using sipetok_api.Respon;
using System.Threading.Tasks;

namespace sipetok_api.Controllers
{
    [Authorize]
    [Route("api/eggs")]
    [ApiController]
    public class EggInventoryController : ControllerBase
    {
        private readonly StevanModuleFactory _factory;

        public EggInventoryController(AppDbContext context, IMapper mapper)
        {
            _factory = new EggInventoryFactory (context, mapper);
        }

        [HttpGet]
        [Authorize(Roles = "TENANT")]
        public async Task<IActionResult> GetAllEggs()
        {
            var worker = _factory.CreateMethod("get");
            EggInventory eggModel = new EggInventory();
            EggInventoryRespon response = new EggInventoryRespon();

            return await worker.ActionAsync<EggInventory, EggInventoryRespon>(eggModel, response);
        }

        [HttpGet("{id:int}")]
        [Authorize(Roles = "TENANT")]
        public async Task<IActionResult> GetEggById(int id)
        {
            IStevanMethod worker = _factory.CreateMethod("get");
            EggInventory eggModel = new EggInventory();
            EggInventoryRespon response = new EggInventoryRespon();
            return await worker.ActionAsync<EggInventory, EggInventoryRespon>(eggModel, response, id);
        }

        [HttpPost]
        [Authorize(Roles = "TENANT")]
        public async Task<IActionResult> AddEgg([FromBody] EggInventoryDto request)
        {
            IStevanMethod worker = _factory.CreateMethod("save");
            EggInventory eggModel = new EggInventory();
            EggInventoryRespon response = new EggInventoryRespon();

            return await worker.ActionAsync<EggInventory, EggInventoryRespon, EggInventoryDto>(eggModel, response, request, "POST");
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "TENANT")]
        public async Task<IActionResult> UpdateEgg(int id, [FromBody] EggInventoryDto request)
        {
            IStevanMethod worker = _factory.CreateMethod("save");
            EggInventory eggModel = new EggInventory();
            EggInventoryRespon response = new EggInventoryRespon();

            return await worker.ActionAsync<EggInventory, EggInventoryRespon, EggInventoryDto>(eggModel, response, request, "PUT", id);
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "TENANT")]
        public async Task<IActionResult> DeleteEgg(int id)
        {
            IStevanMethod worker = _factory.CreateMethod("save");
            EggInventory eggModel = new EggInventory();
            EggInventoryRespon response = new EggInventoryRespon();

            return await worker.ActionAsync<EggInventory, EggInventoryRespon, object>(eggModel, response, null, "DELETE", id);
        }
    }
}