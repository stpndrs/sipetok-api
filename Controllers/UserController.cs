using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using sipetok_api.Controllers.Factories;
using sipetok_api.Controllers.Products;
using sipetok_api.Data;
using sipetok_api.dto;
using sipetok_api.dto.Request;
using sipetok_api.dto.Response;
using sipetok_api.Models;
using System.Threading.Tasks;

namespace sipetok_api.Controllers
{
    [Authorize]
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly StevanModuleFactory _factory;

        public UserController(AppDbContext context, IMapper mapper)
        {
            _factory = new UserFactory(context, mapper);
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GetAllUsers()
        {
            var worker = _factory.CreateMethod("get");
            User userModel = new User();
            UserResponseDto response = new UserResponseDto();

            return await worker.ActionAsync<User, UserResponseDto>(userModel, response);
        }

        [HttpGet("{id:int}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GetUserById(int id)
        {
            IStevanMethod worker = _factory.CreateMethod("get");
            User userModel = new User();
            UserResponseDto response = new UserResponseDto();
            return await worker.ActionAsync<User, UserResponseDto>(userModel, response, id);
        }

        [HttpGet("myaccount")]
        public async Task<IActionResult> GetMyAccount()
        {
            int userId = int.Parse(User.FindFirst("userId")?.Value ?? "0");

            IStevanMethod worker = _factory.CreateMethod("get");
            User userModel = new User();
            UserResponseDto response = new UserResponseDto();

            return await worker.ActionAsync<User, UserResponseDto>(userModel, response, null, userId);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> AddUser([FromBody] UserRequestDto request)
        {
            IStevanMethod worker = _factory.CreateMethod("save");
            User userModel = new User();
            UserResponseDto response = new UserResponseDto();

            return await worker.ActionAsync<User, UserResponseDto, UserRequestDto>(userModel, response, request, "POST");
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UserRequestDto request)
        {
            IStevanMethod worker = _factory.CreateMethod("save");
            User userModel = new User();
            UserResponseDto response = new UserResponseDto();

            return await worker.ActionAsync<User, UserResponseDto, UserRequestDto>(userModel, response, request, "PUT", id);
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            IStevanMethod worker = _factory.CreateMethod("save");
            User userModel = new User();
            UserResponseDto response = new UserResponseDto();

            return await worker.ActionAsync<User, UserResponseDto, object>(userModel, response, null, "DELETE", id);
        }
    }
}