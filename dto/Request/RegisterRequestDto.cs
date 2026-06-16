using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace sipetok_api.dto.Request
{
    public class RegisterRequestDto
    {
        [Required(ErrorMessage = "Name is required!")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Username is required!")]
        [StringLength(50, ErrorMessage = "Panjang maksimal 50 karakter")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required!")]
        //[StringLength(100, MinimumLength = 8, ErrorMessage = "Password minimal 8 karakter")]
        //[RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$",
        //ErrorMessage = "Password harus mengandung huruf besar, huruf kecil, dan angka")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required!")]
        [EmailAddress(ErrorMessage = "Format email salah")]
        public string Email { get; set; } = string.Empty;
    }
}
