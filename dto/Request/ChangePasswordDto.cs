using System.ComponentModel.DataAnnotations;

namespace sipetok_api.dto.Request
{
    public class ChangePasswordDto
    {
        [Required(ErrorMessage = "Old password is required")]
        public string PasswordOld { get; set; } = string.Empty;

        [Required(ErrorMessage = "New password is required")]
        public string Password { get; set; } = string.Empty;
        
        public ChangePasswordDto() {}
    }
}