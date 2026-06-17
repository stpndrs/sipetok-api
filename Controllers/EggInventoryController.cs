using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using sipetok_api.Controllers.Factories;
using sipetok_api.Data;
using sipetok_api.dto.Request; // Disesuaikan jika Dto berada di subfolder ini
using sipetok_api.Models;
using sipetok_api.Respon;
using System.Threading.Tasks;

namespace sipetok_api.Controllers
{
    [Authorize(Roles = "TENANT")]
    [Route("api/eggs")]
    [ApiController]
    public class EggInventoryController : ControllerBase
    {
        private readonly StevanModuleFactory _factory;

        public EggInventoryController(AppDbContext context, IMapper mapper)
        {
            _factory = new EggInventoryFactory(context, mapper);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllEggs()
        {
            var worker = _factory.CreateMethod("get");
            return await worker.ActionAsync<EggInventory, EggInventoryResponseDto>(
                new EggInventory(), new EggInventoryResponseDto());
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetEggById(int id)
        {
            var worker = _factory.CreateMethod("get");
            return await worker.ActionAsync<EggInventory, EggInventoryResponseDto>(
                new EggInventory(), new EggInventoryResponseDto(), id);
        }

        [HttpPost]
        public async Task<IActionResult> AddEgg([FromBody] EggInventoryRequestDto request)
        {
            var worker = _factory.CreateMethod("save");
            return await worker.ActionAsync<EggInventory, EggInventoryResponseDto, EggInventoryRequestDto>(
                new EggInventory(), new EggInventoryResponseDto(), request, "POST");
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateEgg(int id, [FromBody] EggInventoryRequestDto request)
        {
            var worker = _factory.CreateMethod("save");
            return await worker.ActionAsync<EggInventory, EggInventoryResponseDto, EggInventoryRequestDto>(
                new EggInventory(), new EggInventoryResponseDto(), request, "PUT", id);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteEgg(int id)
        {
            var worker = _factory.CreateMethod("save");
            return await worker.ActionAsync<EggInventory, EggInventoryResponseDto, object>(
                new EggInventory(), new EggInventoryResponseDto(), null!, "DELETE", id);
        }
    }
}