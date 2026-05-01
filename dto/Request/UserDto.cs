using sipetok_api.Utils;
using System.ComponentModel.DataAnnotations;

namespace sipetok_api.dto.Request
{
    public class UserDto
    {
        [Required(ErrorMessage = "Username is required!")]
        public string username { get; set; } = string.Empty;

        // [Required(ErrorMessage = "Password is required!")]
        public string password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required!")]
        public string email { get; set; } = string.Empty;

        // [Required(ErrorMessage = "Role is required!")]
        public int role { get; set; }

        [Required(ErrorMessage = "Status is required!")]
        public int status { get; set; }
        public UserDto() {}

        public UserDto(string username, string password, string email, int role, int status)
        {
            this.username = username;
            this.password = password;
            this.email = email;
            this.role = role;
            this.status = status;
        }
    }
}