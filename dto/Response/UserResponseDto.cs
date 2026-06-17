using System.ComponentModel;
using sipetok_api.Services;
using sipetok_api.Utils;

namespace sipetok_api.dto
{
    public class UserResponseDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        // [Browsable(false)]
        public string Password { get; set; } = string.Empty;
        public TabelDriven Role { get; set; } = null!;
        public TabelDriven IsActive { get; set; } = null!;
        public UserResponseDto() { }

        public UserResponseDto(int Id, string Username, string Email, int Role, int IsActive)
        {
            var roleLogic = new AccountRoleTableDriven();
            var statusLogic = new AccountStatusTableDriven();
            this.Id = Id;
            this.Username = Username;
            this.Email = Email;
            this.Role = new TabelDriven(Role, roleLogic.GetRoleName(Role));
            this.IsActive = new TabelDriven(IsActive, statusLogic.GetStatusName(IsActive));
        }
    }
}