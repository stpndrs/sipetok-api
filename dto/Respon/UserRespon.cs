using sipetok_api.Services;
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

        public UserRespon(int id, string username, string email, int role, int status)
        {
            var roleLogic = new AccountRoleTableDriven();
            var statusLogic = new AccountStatusTableDriven();
            this.id = id;
            this.username = username;
            this.email = email;
            this.role = new TabelDriven(role, roleLogic.GetRoleName(role));
            this.status = new TabelDriven(status, statusLogic.GetStatusName(status));
        }
    }
}