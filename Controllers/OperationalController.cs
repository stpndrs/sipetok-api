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
    [Route("api/operationals")]
    [ApiController]
    public class OperationalController : ControllerBase
    {
        private readonly StevanModuleFactory _factory;

        public OperationalController(AppDbContext context, IMapper mapper)
        {
            _factory = new OperationalFactory(context, mapper);
        }

        [HttpGet]
        [Authorize(Roles = "TENANT")]
        public async Task<IActionResult> GetAllOperationals()
        {
            var worker = _factory.CreateMethod("get");
            Operational operationalModel = new Operational();
            OperationalRespon response = new OperationalRespon();

            return await worker.ActionAsync<Operational, OperationalRespon>(operationalModel, response);
        }

        [HttpGet("{id:int}")]
        [Authorize(Roles = "TENANT")]
        public async Task<IActionResult> GetOperationalById(int id)
        {
            IStevanMethod worker = _factory.CreateMethod("get");
            Operational operationalModel = new Operational();
            OperationalRespon response = new OperationalRespon();
            return await worker.ActionAsync<Operational, OperationalRespon>(operationalModel, response, id);
        }

        [HttpPost]
        [Authorize(Roles = "TENANT")]
        public async Task<IActionResult> AddOperational([FromBody] OperationalDto request)
        {
            IStevanMethod worker = _factory.CreateMethod("save");
            Operational operationalModel = new Operational();
            OperationalRespon response = new OperationalRespon();

            return await worker.ActionAsync<Operational, OperationalRespon, OperationalDto>(operationalModel, response, request, "POST");
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "TENANT")]
        public async Task<IActionResult> UpdateOperational(int id, [FromBody] OperationalDto request)
        {
            IStevanMethod worker = _factory.CreateMethod("save");
            Operational operationalModel = new Operational();
            OperationalRespon response = new OperationalRespon();

            return await worker.ActionAsync<Operational, OperationalRespon, OperationalDto>(operationalModel, response, request, "PUT", id);
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "TENANT")]
        public async Task<IActionResult> DeleteOperational(int id)
        {
            IStevanMethod worker = _factory.CreateMethod("save");
            Operational operationalModel = new Operational();
            OperationalRespon response = new OperationalRespon();

            return await worker.ActionAsync<Operational, OperationalRespon, object>(operationalModel, response, null, "DELETE", id);
        }
    }
}