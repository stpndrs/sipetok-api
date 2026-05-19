using sipetok_api.Utils;
using System.ComponentModel.DataAnnotations;

namespace sipetok_api.dto.Request
{
    public class UserDto
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
        public int Status { get; set; }
        public UserDto() {}

        public UserDto(string Username, string Password, string Email, int Role, int Status)
        {
            this.Username = Username;
            this.Password = Password;
            this.Email = Email;
            this.Role = Role;
            this.Status = Status;
        }
    }
}