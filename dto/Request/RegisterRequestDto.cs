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
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required!")]
        [EmailAddress(ErrorMessage = "Format email salah")]
        public string Email { get; set; } = string.Empty;

        public int Role { get; set; }
        public bool IsActive { get; set; }
    }
}
