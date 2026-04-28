using sipetok_api.Utils;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Role role { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Status status { get; set; }
    }
}
