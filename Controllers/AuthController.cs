using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using sipetok_api.Controllers.Factories;
using sipetok_api.Controllers.Products;
using sipetok_api.Data;
using sipetok_api.dto;
using sipetok_api.dto.Request;
using sipetok_api.dto.Respon;
using sipetok_api.helper;
using sipetok_api.Models;
using sipetok_api.Repositories;
using System;
using System.Threading.Tasks;

namespace sipetok_api.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly StevanModuleFactory _factory;
        private readonly IConfiguration appConfig;
        private readonly AppDbContext _dbContext;

        public AuthController(AppDbContext context, IConfiguration config, IMapper mapper)
        {
            _dbContext = context;
            appConfig = config;
            _factory = new AuthFactory(context, appConfig, mapper);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (string.IsNullOrWhiteSpace(request.Password))
            {
                return new BadRequestObjectResult(new ResponData<object?>(false, "Password is required"));
            }

            User userModel = new User();
            UserResponseDto response = new UserResponseDto();

            string hashedPassword = Bcrypt.HashPassword(request.Password);

            request.Password = hashedPassword;
            request.Role = 3;
            request.IsActive = true;

            IStevanMethod worker = _factory.CreateMethod("register");

            return await worker.ActionAsync<User, UserResponseDto, RegisterRequestDto>(userModel, response, request, "POST");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            User userModel = new User();
            UserResponseDto userResponse = new UserResponseDto();

            IStevanMethod getWorker = _factory.CreateMethod("get");

            string[] usernameCheckQuery = new[] { $"Username:{request.Username}" };

            // 1. Lakukan pencarian dinamis via StevanGetData
            var checkResult = await getWorker.ActionAsync<User, UserResponseDto>(
                userModel,
                userResponse,
                searchQuery: usernameCheckQuery
            );

            UserResponseDto? targetUser = null;

            if (checkResult is OkObjectResult okResult && okResult.Value != null)
            {
                var responData = okResult.Value as ResponData<List<UserResponseDto>>;

                if (responData != null && responData.Success && responData.Data != null && responData.Data.Count > 0)
                {
                    targetUser = responData.Data[0];
                }
            }


            if (targetUser == null || !Bcrypt.VerifyPassword(request.Password, targetUser.Password))
            {
                return new BadRequestObjectResult(new ResponData<object>(false, "Wrong Username or Password"));
            }

            if (targetUser.IsActive.key == 0)
            {
                return new BadRequestObjectResult(new ResponData<object>(false, "Your account has been deactivated"));
            }

            User userForToken = new User
            {
                Username = targetUser.Username,
                Role = targetUser.Role.key,
                Id = targetUser.Id
            };

            string token = AuthHelper.CreateToken(userForToken, appConfig);

            var response = new ResponData<AuthResponseDto>
            (
                true,
                new AuthResponseDto(token != "false" ? token : null),
                "Login berhasil"
            );

            return Ok(response);
        }
    }
}