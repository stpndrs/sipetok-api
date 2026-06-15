using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using sipetok_api.dto.Request;
using sipetok_api.dto.Respon;
using sipetok_api.Controllers.Products;
using sipetok_api.Controllers.Factories;
using sipetok_api.Models;

namespace sipetok_api.Controllers
{
    [Authorize(Roles = "TENANT")]
    [Route("api/operationals")]
    [ApiController]
    public class OperationalController : ControllerBase
    {
        private readonly OperationalFactory _factory;

        public OperationalController(OperationalFactory factory)
        {
            _factory = factory;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllOperationals()
        {
            int userId = int.Parse(User.FindFirst("userId")?.Value ?? "0");
            var handler = (GetData)_factory.CreateMethod("get");
            return await handler.ActionAsync<Operational, OperationalRespon>("get_all_op", userId: userId);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetOperationalById(int id)
        {
            int userId = int.Parse(User.FindFirst("userId")?.Value ?? "0");
            var handler = (GetData)_factory.CreateMethod("get");
            return await handler.ActionAsync<Operational, OperationalRespon>("op_byid", id: id, userId: userId);
        }

        [HttpPost]
        public async Task<IActionResult> CreateOperational([FromBody] OperationalDto operationalDto)
        {
            int userId = int.Parse(User.FindFirst("userId")?.Value ?? "0");
            var handler = (SaveData)_factory.CreateMethod("save");

            return await handler.ActionAsync<Operational, OperationalDto, OperationalRespon>(
                subAction: "add_op",
                data: operationalDto,
                httpMethod: "POST",
                userId: userId
            );
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateOperational(int id, [FromBody] OperationalDto operationalDto)
        {
            int userId = int.Parse(User.FindFirst("userId")?.Value ?? "0");
            var handler = (SaveData)_factory.CreateMethod("save");

            return await handler.ActionAsync<Operational, OperationalDto, OperationalRespon>(
                subAction: "update_op",
                data: operationalDto,
                httpMethod: "PUT",
                id: id,
                userId: userId
            );
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteOperational(int id)
        {
            int userId = int.Parse(User.FindFirst("userId")?.Value ?? "0");
            var handler = (DeleteData)_factory.CreateMethod("delete");
            return await handler.ActionAsync<Operational>("delete_op", id: id, userId: userId);
        }
    }
}