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
        private readonly AuthFactory _factory;
        private readonly AppDbContext _dbContext;

        public AuthController(AppDbContext context, IConfiguration config, IMapper mapper)
        {
            _dbContext = context;
            // Memanggil constructor AuthFactory yang menerima 3 parameter
            _factory = new AuthFactory(context, config, mapper);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto req)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // 1. Hashing password
            string passwordHash = Bcrypt.BcryptPassword(req.Password);

            // 2. Gunakan nama 'newUser' agar tidak bentrok dengan keyword bawaan C#
            var newUser = new User
            {
                Username = req.Username,
                Email = req.Email,
                Password = passwordHash,
                Role = 3, // Default Customer
                IsActive = true
            };

            // 3. Panggil Factory
            IMethod handler = (IMethod)_factory.CreateMethod("save");

            // 4. PASTIKAN di sini menulis 'newUser', bukan 'user'
            return await handler.ActionAsync<User, AuthRespon>(
                subAction: "register",
                data: newUser, // <-- Perhatikan ini harus sama dengan nama di atas
                httpMethod: "POST"
            );
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto req)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // 1. Cari user langsung di Controller
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == req.Username);

            // 2. Validasi Password
            if (user == null || !Bcrypt.VerifyPassword(req.Password, user.Password))
            {
                return new BadRequestObjectResult(new ResponData<object>(false, "Wrong Username or Password"));
            }

            // 3. Validasi Keaktifan Akun
            if (!user.IsActive)
            {
                return new BadRequestObjectResult(new ResponData<object>(false, "Your account has been deactivated"));
            }

            // 4. Kirim objek user yang sudah VALID ke SaveData
            IMethod handler = (IMethod)_factory.CreateMethod("save");
            return await handler.ActionAsync<User, AuthRespon>(
                subAction: "login",
                data: user, // Yang dikirim adalah object User
                httpMethod: "POST"
            );
        }
    }
}