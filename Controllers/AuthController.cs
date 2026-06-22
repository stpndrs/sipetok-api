using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using sipetok_api.Controllers.Factories;
using sipetok_api.dto;
using sipetok_api.dto.Request;
using sipetok_api.dto.Response;
<<<<<<< HEAD
=======
using sipetok_api.helper;
>>>>>>> 66185cb9672652d715a413bd97d21b5b6f10fbf7
using sipetok_api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace sipetok_api.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly StevanModuleFactory _factory;
        private readonly IConfiguration _appConfig;

        public AuthController(AuthFactory factory, IConfiguration config)
        {
            _factory = factory;
            _appConfig = config;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest(new ResponData<object?>(false, "Password is required"));
            }

            request.Password = Bcrypt.HashPassword(request.Password);
            request.Role = 3;
            request.IsActive = true;

            var worker = _factory.CreateMethod("register");
            return await worker.ActionAsync<User, UserResponseDto, RegisterRequestDto>(
                new User(), new UserResponseDto(), request, "POST");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var getWorker = _factory.CreateMethod("get");
            var usernameCheckQuery = new[] { $"Username:{request.Username}" };

            var checkResult = await getWorker.ActionAsync<User, UserResponseDto>(
                new User(), new UserResponseDto(), searchQuery: usernameCheckQuery);

            UserResponseDto? targetUser = null;
            if (checkResult is OkObjectResult { Value: ResponData<List<UserResponseDto>> { Success: true, Data: { Count: > 0 } } responData })
            {
                targetUser = responData.Data[0];
            }

            if (targetUser == null || !Bcrypt.VerifyPassword(request.Password, targetUser.Password))
            {
                return BadRequest(new ResponData<object>(false, "Wrong Username or Password"));
            }

            if (targetUser.IsActive.key == 0)
            {
                return BadRequest(new ResponData<object>(false, "Your account has been deactivated"));
            }

            var userForToken = new User
            {
                Username = targetUser.Username,
                Role = targetUser.Role.key,
                Id = targetUser.Id
            };

            string token = AuthHelper.CreateToken(userForToken, _appConfig);

            var response = new ResponData<AuthResponseDto>(
                true,
                new AuthResponseDto(token != "false" ? token : null, userForToken.Username, userForToken.Role),
                "Login berhasil"
            );

            return Ok(response);
        }
    }
}