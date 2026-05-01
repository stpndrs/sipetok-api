using System.ComponentModel.DataAnnotations;

namespace sipetok_api.dto.Request
{
    public class ChangePasswordDto
    {
        [Required(ErrorMessage = "Old password is required")]
        public string password_old { get; set; } = string.Empty;

        [Required(ErrorMessage = "New password is required")]
        public string password { get; set; } = string.Empty;
        
        public ChangePasswordDto() {}
    }
}