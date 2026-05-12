using sipetok_api.Utilis;
using sipetok_api.Utils;

namespace sipetok_api.dto
{
    public class UserRespon
    {
        public int id {get; set;}
        public string username { get; set; } = string.Empty;
        public string email { get; set; } = string.Empty;
        public TabelDriven role { get; set; } = null!;
        public TabelDriven status { get; set; } = null!;
        public UserRespon() {}

        public UserRespon(int id, string username, string email, TabelDriven role, TabelDriven status)
        {
            this.id = id;
            this.username = username;
            this.email = email;
            this.role = role;
            this.status = status;
        }
    }
}