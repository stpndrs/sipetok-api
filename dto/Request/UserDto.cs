using sipetok_api.Utilis;

namespace sipetok_api.dto.Request
{
    public class UserDto
    {
        public string username { get; set; } = string.Empty;
        public string password { get; set; } = string.Empty;
        public string email { get; set; } = string.Empty;
        public Role role { get; set; }
        public Status status { get; set; }
        public UserDto() {}

        public UserDto(string username, string password, string email, Role role, Status status)
        {
            this.username = username;
            this.password = password;
            this.email = email;
            this.role = role;
            this.status = status;
        }
    }
}