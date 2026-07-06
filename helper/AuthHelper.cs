using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using sipetok_api.Services;
using sipetok_api.Models;

namespace sipetok_api.helper
{
    public class AuthHelper
    {
        public static string CreateToken(User user, IConfiguration configuration) 
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

                var jwtSection = configuration.GetSection("configProperties:JWT");
                var issuer = jwtSection["JWT_ISSUER"];
                var audience = jwtSection["JWT_AUDIENCE"];
                var keyString = jwtSection["JWT_KEY"];

                if (string.IsNullOrEmpty(keyString)) throw new Exception("JWT Key is missing");

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
            catch (Exception ex)
            {
                Console.WriteLine("JWT Error: " + ex.Message);
                return "false";
            }
        }
    }
}