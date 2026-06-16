using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using sipetok_api.Controllers.Factories;
using sipetok_api.Controllers.Products;
using sipetok_api.Data;
using sipetok_api.dto.Request;
using sipetok_api.dto.Respon;
using sipetok_api.Models;
using System;
using System.Threading.Tasks;

namespace sipetok_api.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ModuleFactory _factory;
        private readonly AppDbContext _dbContext;

        public AuthController(AppDbContext context, IConfiguration config, IMapper mapper)
        {
            _dbContext = context;
            // Memanggil constructor AuthFactory yang menerima 3 parameter
            _factory = new AuthFactory(context, config, mapper);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // 1. Hashing password
            string hashedPassword = Bcrypt.HashPassword(request.Password);

            // 2. Gunakan nama 'user' agar tidak bentrok dengan keyword bawaan C#
            var user = new User
            {
                Username = request.Username,
                Email = request.Email,
                Password = hashedPassword,
                Role = 3, // Default Customer
                IsActive = true
            };

            // 3. Panggil Factory
            IMethod handler = _factory.CreateMethod("save");

            // 4. PASTIKAN di sini menulis 'user', bukan 'user'
            return await handler.ActionAsync<User, AuthResponseDto>(
                subAction: "register",
                data: user, // <-- Perhatikan ini harus sama dengan nama di atas
                httpMethod: "POST"
            );
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // 1. Cari user langsung di Controller
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == request.Username);

            // 2. Validasi Password
            if (user == null || !Bcrypt.VerifyPassword(request.Password, user.Password))
            {
                return new BadRequestObjectResult(new ResponData<object>(false, "Wrong Username or Password"));
            }

            // 3. Validasi Keaktifan Akun
            if (!user.IsActive)
            {
                return new BadRequestObjectResult(new ResponData<object>(false, "Your account has been deactivated"));
            }

            // 4. Kirim objek user yang sudah VALID ke SaveData
            IMethod handler = _factory.CreateMethod("save");
            return await handler.ActionAsync<User, AuthRespon>(
                subAction: "login",
                data: user, // Yang dikirim adalah object User
                httpMethod: "POST"
            );
        }
    }
}