using sipetok_api.Services;
using sipetok_api.Utils;

namespace sipetok_api.dto
{
    public class UserRespon
    {
        public int Id {get; set;}
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public TabelDriven Role { get; set; } = null!;
        public TabelDriven Status { get; set; } = null!;
        public UserRespon() {}

        public UserRespon(int Id, string Username, string Email, int Role, int Status)
        {
            var roleLogic = new AccountRoleTableDriven();
            var statusLogic = new AccountStatusTableDriven();
            this.Id = Id;
            this.Username = Username;
            this.Email = Email;
            this.Role = new TabelDriven(Role, roleLogic.GetRoleName(Role));
            this.Status = new TabelDriven(Status, statusLogic.GetStatusName(Status));
        }
    }
}