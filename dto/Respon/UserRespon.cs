using sipetok_api.Utilis;

namespace sipetok_api.dto
{
    public class UserRespon
    {
        public int id {get; set;}
        public string username { get; set; } = string.Empty;
        public string email { get; set; } = string.Empty;
        public Role role { get; set; }
        public Status status { get; set; }
        public UserRespon() {}

        public UserRespon(int id, string username, string email, Role role, Status status)
        {
            this.id = id;
            this.username = username;
            this.email = email;
            this.role = role;
            this.status = status;
        }
    }
}