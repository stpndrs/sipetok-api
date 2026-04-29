using System.ComponentModel.DataAnnotations;

namespace sipetok_api.dto.Request
{
    public class ChangePasswordDto
    {
        [Required(ErrorMessage = "Password lama wajib diisi")]
        public string password_old { get; set; } = string.Empty;
        [Required(ErrorMessage = "Password baru wajib diisi")]
        public string password { get; set; } = string.Empty;
        public ChangePasswordDto() {}
    }
}