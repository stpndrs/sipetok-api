using sipetok_api.Utils;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace sipetok_api.Models
{
    public class User
    {
        [Key]
        public int id { get; set; }

        [Required, MaxLength(50)]
        public string username { get; set; } = string.Empty;

        [Required, MaxLength(255)]
        [JsonIgnore]
        public string password { get; set; } = string.Empty;

        [MaxLength(100)]
        public string email { get; set; } = string.Empty;
        public int role { get; set; }
        public int status { get; set; }

        public User()
        {
            
        }

        public User(int id, string username, string password, string email, int role, int status)
        {
            this.id = id;
            this.username = username;
            this.password = password;
            this.email = email;
            this.role = role;
            this.email = email;
        }
    }
}
