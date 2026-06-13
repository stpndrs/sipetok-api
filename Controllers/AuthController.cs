using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using AutoMapper;
using sipetok_api.Data;
using sipetok_api.Controllers.Factories;
using sipetok_api.Controllers.Products;
using sipetok_api.dto.Request;
using sipetok_api.dto.Respon;
using sipetok_api.Models;
using System.Threading.Tasks;
using System;

namespace sipetok_api.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthFactory _factory;

        public AuthController(AppDbContext context, IConfiguration config, IMapper mapper)
        {
            // Memanggil constructor AuthFactory yang menerima 3 parameter
            _factory = new AuthFactory(context, config, mapper);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto req)
        {
            var worker = (SaveData)_factory.CreateMethod("register");
            return await worker.ActionAsync<User, RegisterDto, AuthRespon>("register", req, "POST");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto req)
        {
            var worker = (SaveData)_factory.CreateMethod("login");
            return await worker.ActionAsync<User, LoginDto, AuthRespon>("login", req, "POST");
        }
    }
}