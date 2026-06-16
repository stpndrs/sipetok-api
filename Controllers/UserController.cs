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
using System.Threading.Tasks;

namespace sipetok_api.Controllers
{
    [Authorize]
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ModuleFactory _factory;

        public UserController(AppDbContext context, IMapper mapper)
        {
            _factory = new UserFactory(context, mapper);
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GetAllUsers()
        {
            IMethod worker = _factory.CreateMethod("get");
            return await worker.ActionAsync<User, UserRespon>("getall");
        }

        [HttpGet("{id:int}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GetUserById(int id)
        {
            IMethod worker = _factory.CreateMethod("get");
            return await worker.ActionAsync<User, UserRespon>("byid", id);
        }

        [HttpGet("myaccount")]
        public async Task<IActionResult> GetMyAccount()
        {
            int userId = int.Parse(User.FindFirst("userId")?.Value ?? "0");
            IMethod worker = _factory.CreateMethod("get");
            // Asumsikan kita buat action "get_by_id" di GetData untuk userId
            return await worker.ActionAsync<User, UserResponseDto>("byid", userId);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> AddUser([FromBody] UserRequestDto request)
        {
            IMethod worker = _factory.CreateMethod("save");
            return await worker.ActionAsync<User, UserRespon>("add_user", userDto, "POST");
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UserRequestDto request)
        {
            IMethod worker = _factory.CreateMethod("save");
            return await worker.ActionAsync<User, UserRespon>("update_user", userDto, "PUT", id);
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            IMethod worker = _factory.CreateMethod("delete");
            return await worker.ActionAsync<User, UserRespon>("delete_user", id);
        }
    }
}