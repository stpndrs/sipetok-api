using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace sipetok_api.dto.Request
{
    public class LoginDto
    {
        [Required(ErrorMessage = "Username is required!")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required!")]
        public string Password { get; set; } = string.Empty;
    }
}
