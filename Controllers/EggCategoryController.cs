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
    [Route("api/egg/categories")]
    [ApiController]
    public class EggCategoryController : ControllerBase
    {
        private readonly ModuleFactory _factory;

        // Inject EggCategoryFactory langsung ke dalam Controller
        public EggCategoryController(EggCategoryFactory factory)
        {
            _factory = factory;
        }

        [HttpGet]
        [Authorize(Roles = "TENANT, CUSTOMER")]
        public async Task<IActionResult> GetAllEggCategory()
        {
            int userId = int.Parse(User.FindFirst("userId")?.Value ?? "0");

            // Panggil factory untuk mendapatkan objek GetData
            IMethod handler = _factory.CreateMethod("get");

            if (User.IsInRole("CUSTOMER"))
            {
                return await handler.ActionAsync<EggCategory, EggAvailableRespon>("customer_all", userId: userId);
            }
            else if (User.IsInRole("TENANT"))
            {
                return await handler.ActionAsync<EggCategory, EggCategoryResponseDto>("category_all_tenant", userId: userId);
            }
            return Forbid();
        }

        [HttpGet]
        [Route("{id:int}")]
        [Authorize(Roles = "TENANT")]
        public async Task<IActionResult> GetEggCategoryById(int id)
        {
            var userId = int.Parse(User.FindFirst("userId")?.Value ?? "0");
            IMethod handler = _factory.CreateMethod("get");
            return await handler.ActionAsync<EggCategory, EggCategoryResponseDto>("get_category_by_id", id: id, userId: userId);
        }

        [HttpPost]
        [Authorize(Roles = "TENANT")]
        public async Task<IActionResult> AddEggCategory([FromBody] EggCategoryRequestDto eggCategoryDto)
        {
            int userId = int.Parse(User.FindFirst("userId")?.Value ?? "0");

            // Panggil factory untuk mendapatkan objek SaveData
            IMethod handler = _factory.CreateMethod("save");

            return await handler.ActionAsync<EggCategory, EggCategoryResponseDto>(
                subAction: "add_category",
                data: eggCategoryDto,
                httpMethod: "POST",
                userId: userId
            );
        }

        [HttpPut]
        [Route("{id:int}")]
        [Authorize(Roles = "TENANT")]
        public async Task<IActionResult> UpdateEggCategory(int id, [FromBody] EggCategoryRequestDto eggCategoryDto)
        {
            int userId = int.Parse(User.FindFirst("userId")?.Value ?? "0");
            IMethod handler = _factory.CreateMethod("save");

            return await handler.ActionAsync<EggCategory, EggCategoryResponseDto>(
                subAction: "update_category",
                data: eggCategoryDto,
                httpMethod: "PUT",
                id: id,
                userId: userId
            );
        }

        [HttpDelete]
        [Route("{id:int}")]
        [Authorize(Roles = "TENANT")]
        public async Task<IActionResult> DeleteEggCategory(int id)
        {
            var userId = int.Parse(User.FindFirst("userId")?.Value ?? "0");
            IMethod handler = _factory.CreateMethod("delete");

            return await handler.ActionAsync<EggCategory, EggCategoryResponseDto>(
                subAction: "delete_category",
                data: null,
                httpMethod: "DELETE",
                id: id,
                userId: userId
            );
        }
    }
}