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
        private readonly UserFactory _factory;

        public UserController(AppDbContext context, IMapper mapper)
        {
            _factory = new UserFactory(context, mapper);
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GetAllUsers()
        {
            var worker = (GetData)_factory.CreateMethod("get");
            return await worker.ActionAsync<User, UserResponseDto>("getall");
        }

        [HttpGet("{id:int}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var worker = (GetData)_factory.CreateMethod("get");
            return await worker.ActionAsync<User, UserResponseDto>("byid", id);
        }

        [HttpGet("myaccount")]
        public async Task<IActionResult> GetMyAccount()
        {
            int userId = int.Parse(User.FindFirst("userId")?.Value ?? "0");
            var worker = (GetData)_factory.CreateMethod("get");
            // Asumsikan kita buat action "get_by_id" di GetData untuk userId
            return await worker.ActionAsync<User, UserResponseDto>("byid", userId);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> AddUser([FromBody] UserRequestDto request)
        {
            var worker = (SaveData)_factory.CreateMethod("save");
            return await worker.ActionAsync<User, UserResponseDto>("add_user", request, "POST");
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UserRequestDto request)
        {
            var worker = (SaveData)_factory.CreateMethod("save");
            return await worker.ActionAsync<User, UserResponseDto>("update_user", request, "PUT", id);
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var worker = (DeleteData)_factory.CreateMethod("delete");
            return await worker.ActionAsync<User, UserResponseDto>("delete_user", id);
        }
    }
}