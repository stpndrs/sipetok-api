using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace sipetok_api.dto.Request
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "Username wajib diisi")]
        [StringLength(50, ErrorMessage = "Panjang maksimal 50 karakter")]
        public string Username { get; set; }

        [Required]
        //[StringLength(100, MinimumLength = 8, ErrorMessage = "Password minimal 8 karakter")]
        //[RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$",
        //ErrorMessage = "Password harus mengandung huruf besar, huruf kecil, dan angka")]
        public string Password { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Format email salah")]
        public string Email { get; set; }
    }
}
