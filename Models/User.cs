using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace sipetok_api.Models
{
    public class User : BaseEntity
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required, MaxLength(255)]
        [JsonIgnore]
        public string Password { get; set; } = string.Empty;

        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;
        public int Role { get; set; }
        public int Status { get; set; }

        public User()
        {

        }

        public User(int Id, string Username, string Password, string Email, int Role, int Status)
        {
            this.Id = Id;
            this.Username = Username;
            this.Password = Password;
            this.Email = Email;
            this.Role = Role;
            this.Status = Status;
        }
    }
}
