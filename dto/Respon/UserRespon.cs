using sipetok_api.Utils;

namespace sipetok_api.dto
{
    public class UserRespon
    {
        public int id {get; set;}
        public string username { get; set; } = string.Empty;
        public string email { get; set; } = string.Empty;
        public int role { get; set; }
        public int status { get; set; }
        public UserRespon() {}

        public UserRespon(int id, string username, string email, int role, int status)
        {
            this.id = id;
            this.username = username;
            this.email = email;
            this.role = role;
            this.status = status;
        }
    }
}