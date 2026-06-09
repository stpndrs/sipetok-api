using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using sipetok_api.Data;
using sipetok_api.dto.Request;
using sipetok_api.dto.Respon;
using sipetok_api.Models;
using sipetok_api.Services;

namespace sipetok_api.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext dbContext;
        private readonly IConfiguration appConfig;
        private readonly IMapper _mapper;

        public AuthController(AppDbContext context, IConfiguration config, IMapper mapper)
        {
            dbContext = context;
            appConfig = config;
            _mapper = mapper;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto req)
        {
            try
            {
                string passwordHash = Bcrypt.BcryptPassword(req.Password);
                var user = new User
                {
                    Username = req.Username,
                    Email = req.Email,
                    Password = passwordHash,
                    Role = 3,
                    IsActive = true
                };

                dbContext.Users.Add(user);
                await dbContext.SaveChangesAsync();
                string token = CreateToken(user);

                // Menggunakan konstruktor: ResponData(bool success, T data, string message)
                ResponData<AuthRespon> respon = new ResponData<AuthRespon>
                (
                    true,
                    new AuthRespon(token != "false" ? token : null),
                    "Register berhasil"
                );

                return Ok(respon);
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException != null && (ex.InnerException.Message.Contains("Duplicate") || ex.InnerException.Message.Contains("unique")))
                {
                    // 1. Buat dictionary error manual untuk menentukan field mana yang error
                    var errorDetail = new Dictionary<string, string[]>
                    {
                        { "Account", new[] { "Email atau Username sudah terdaftar, silakan gunakan yang lain." } }
                    };

                    // 2. Masukkan ke dalam objek ResponValidation menggunakan konstruktornya
                    var responUnique = new ResponValidation(errorDetail);

                    return BadRequest(responUnique);
                }
                ResponData<object?> responDb = new ResponData<object?>
                (
                    false,
                    "Terjadi kesalahan saat menyimpan data ke database."
                );
                return StatusCode(500, responDb);
            }
            catch (Exception ex)
            {
                ResponData<object?> respon = new ResponData<object?>
                (
                    false,
                    ex.Message
                );

                return StatusCode(500, respon);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto req)
        {
            try
            {
                var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Username == req.Username);

                if (user == null || !Bcrypt.VerifyPassword(req.Password, user.Password))
                {
                    return BadRequest(new ResponData<object?>
                    (
                        false,
                        "Wrong Username or Password"
                    ));
                }

                if (!user.IsActive)
                {
                    return BadRequest(new ResponData<object?>
                    (
                        false,
                        "Your account has been deactivated"
                    ));
                }

                string token = CreateToken(user);

                var respon = new ResponData<AuthRespon>
                (
                    true,
                    new AuthRespon(token != "false" ? token : null),
                    "Login berhasil"
                );

                return Ok(respon);
            }
            catch (Exception ex)
            {
                var respon = new ResponData<object?>
                (
                    false,
                    ex.Message
                );

                return StatusCode(500, respon);
            }
        }

        private string CreateToken(User user)
        {
            try
            {
                var roleService = new AccountRoleTableDriven();
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Role, roleService.GetRoleName(user.Role)),
                    new Claim("userId", user.Id.ToString()),
                };

                var jwtSection = appConfig.GetSection("configProperties:JWT");
                var issuer = jwtSection["JWT_ISSUER"];
                var audience = jwtSection["JWT_AUDIENCE"];
                var keyString = jwtSection["JWT_KEY"];

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));

                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: issuer,
                    audience: audience,
                    claims: claims,
                    expires: DateTime.Now.AddDays(1),
                    signingCredentials: creds
                );

                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            catch (Exception)
            {
                return "false";
            }
        }
    }
}