using sipetok_api.Utils;
using System.ComponentModel.DataAnnotations;

namespace sipetok_api.dto.Request
{
    public class UserRequestDto
    {
        [Required(ErrorMessage = "Username is required!")]
        public string Username { get; set; } = string.Empty;

        // [Required(ErrorMessage = "Password is required!")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required!")]
        public string Email { get; set; } = string.Empty;

        // [Required(ErrorMessage = "Role is required!")]
        public int Role { get; set; }

        // [Required(ErrorMessage = "Status is required!")]
        public bool IsActive { get; set; }
        public UserRequestDto() { }

        public UserRequestDto(string Username, string Password, string Email, int Role, bool IsActive)
        {
            this.Username = Username;
            this.Password = Password;
            this.Email = Email;
            this.Role = Role;
            this.IsActive = IsActive;
        }
    }
}