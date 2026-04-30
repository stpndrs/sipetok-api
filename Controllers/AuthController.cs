using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using sipetok_api.Data;
using sipetok_api.dto.Request;
using sipetok_api.dto.Respon;
using sipetok_api.Models;
using sipetok_api.Respon;
using sipetok_api.Utils;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

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
            string passwordHash = Bcrypt.BcryptPassword(req.Password);
            var user = new User
            {
                username = req.Username,
                email = req.Email,
                password = passwordHash,
                role = 3,
                status = 1
            };

            dbContext.Users.Add(user);
            await dbContext.SaveChangesAsync();
            string token = CreateToken(user);

            var respon = new ResponData<AuthRespon>
            {
                success = true,
                data = new AuthRespon(token),
                message = $"Register berhasil"
            };

            return Ok(respon);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto req)
        {
            var user = await dbContext.Users.FirstOrDefaultAsync(u => u.username == req.Username);

            if (user == null || !Bcrypt.VerifyPassword(req.Password, user.password))
            {
                return BadRequest("Username atau password salah!");
            }

            string token = CreateToken(user);

            var respon = new ResponData<AuthRespon>
            {
                success = true,
                data = new AuthRespon(token),
                message = $"Login berhasil" 
            };

            return Ok(respon);
        }

        private string CreateToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.username),
                new Claim(ClaimTypes.Role, user.role.ToString())
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
    }
}