using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using sipetok_api.Controllers.Factories;
using sipetok_api.Data;
using sipetok_api.dto.Request;
using sipetok_api.dto.Response;
using sipetok_api.helper;
using sipetok_api.Models;
using System.Threading.Tasks;

namespace sipetok_api.Controllers
{
    [Authorize]
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IStevanModuleFactory _factory;
        private readonly AppDbContext _dbContext;

        private int CurrentUserId => int.Parse(User.FindFirst("userId")?.Value ?? "0");
        private readonly User _user = new User();
        private readonly UserResponseDto _response = new UserResponseDto();

        public UserController(UserFactory factory, AppDbContext dbContext)
        {
            _factory = factory;
            _dbContext = dbContext;
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GetAllUsers()
        {
            var worker = _factory.CreateMethod("get");
            return await worker.ActionAsync<User, UserResponseDto>(
                model: _user,
                response: _response
            );
        }

        [HttpGet("{id:int}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var worker = _factory.CreateMethod("get");
            return await worker.ActionAsync<User, UserResponseDto>(
                model: _user,
                response: _response,
                id: id
            );
        }

        [HttpGet("myaccount")]
        public async Task<IActionResult> GetMyAccount()
        {
            var worker = _factory.CreateMethod("get");
            return await worker.ActionAsync<User, UserResponseDto>(
                model: _user,
                response: _response,
                id: CurrentUserId
            );
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> AddUser([FromBody] UserRequestDto request)
        {
            HashUserPassword(request);

            var worker = _factory.CreateMethod("save");
            return await worker.ActionAsync<User, UserResponseDto, UserRequestDto>(
                model: _user,
                response: _response,
                request: request,
                httpMethod: "POST"
            );
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UserRequestDto request)
        {
            var existingUser = await GetExistingUser(id);
            if (request.Role == 0) request.Role = existingUser.Role;
            if (!string.IsNullOrWhiteSpace(request.Password))
            {
                request.Password = Bcrypt.HashPassword(request.Password);
            }
            else
            {
                request.Password = existingUser.Password;
            }

            var worker = _factory.CreateMethod("save");
            return await worker.ActionAsync<User, UserResponseDto, UserRequestDto>(
                model: _user,
                response: _response,
                request: request,
                httpMethod: "PUT",
                id: id
            );
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var worker = _factory.CreateMethod("delete");
            return await worker.ActionAsync<User, UserResponseDto, object>(
                model: _user,
                response: _response,
                request: null!,
                httpMethod: "DELETE",
                id: id);
        }

        private void HashUserPassword(UserRequestDto request)
        {
            if (!string.IsNullOrEmpty(request.Password))
            {
                request.Password = Bcrypt.HashPassword(request.Password);
            }
        }

        private async Task<User> GetExistingUser(int id)
        {
            var existingUser = await _dbContext.Users.FindAsync(id);
            if (existingUser is null)
            {
                throw new InvalidOperationException("User tidak ditemukan");
            }
            return existingUser;
        }
    }
}