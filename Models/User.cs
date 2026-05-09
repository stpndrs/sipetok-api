using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace sipetok_api.Models
{
    public class User : BaseEntity
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
    }
}
