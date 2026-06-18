using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using sipetok_api.Controllers.Factories;
using sipetok_api.Data;
using sipetok_api.dto;
using sipetok_api.dto.Request;
using sipetok_api.dto.Response;
<<<<<<< HEAD
=======
using sipetok_api.helper;
>>>>>>> 66185cb9672652d715a413bd97d21b5b6f10fbf7
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
        private readonly AppDbContext _dbContext;

        private int CurrentUserId => int.Parse(User.FindFirst("userId")?.Value ?? "0");

        public UserController(UserFactory factory)
        {
            _factory = factory;
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GetAllUsers()
        {
            var worker = _factory.CreateMethod("get");
            return await worker.ActionAsync<User, UserResponseDto>(new User(), new UserResponseDto());
        }

        [HttpGet("{id:int}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var worker = _factory.CreateMethod("get");
            return await worker.ActionAsync<User, UserResponseDto>(new User(), new UserResponseDto(), id);
        }

        [HttpGet("myaccount")]
        public async Task<IActionResult> GetMyAccount()
        {
            var worker = _factory.CreateMethod("get");
            return await worker.ActionAsync<User, UserResponseDto>(new User(), new UserResponseDto(), null, CurrentUserId);
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> AddUser([FromBody] UserRequestDto request)
        {
            HashUserPassword(request);

            var worker = _factory.CreateMethod("save");
            return await worker.ActionAsync<User, UserResponseDto, UserRequestDto>(
                new User(), new UserResponseDto(), request, "POST");
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UserRequestDto request)
        {
            HashUserPassword(request);

            var worker = _factory.CreateMethod("save");
            return await worker.ActionAsync<User, UserResponseDto, UserRequestDto>(
                new User(), new UserResponseDto(), request, "PUT", id);
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var worker = _factory.CreateMethod("delete");
            return await worker.ActionAsync<User, UserResponseDto, object>(
                new User(), new UserResponseDto(), null!, "DELETE", id);
        }

        private void HashUserPassword(UserRequestDto request)
        {
            if (!string.IsNullOrEmpty(request.Password))
            {
                request.Password = Bcrypt.HashPassword(request.Password);
            }
        }
    }
}