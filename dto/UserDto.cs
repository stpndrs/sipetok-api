using sipetok_api.Utilis;

namespace sipetok_api.dto
{
    public class UserDto
    {

        public int id { get; set; }

        public string username { get; set; } = string.Empty;

        public string password { get; set; } = string.Empty;

        public string email { get; set; } = string.Empty;
        public Role role { get; set; }
        public Status status { get; set; }

        public UserDto() {}
    }
}