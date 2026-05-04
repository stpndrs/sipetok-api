using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace sipetok_api.Models
{
    public class Tenant : BaseEntity
    {
        public int id { get; set; }

        [Required, MaxLength(100)]
        public string name { get; set; } = string.Empty;

        [MaxLength(255)]
        public string address { get; set; } = string.Empty;

        [MaxLength(20)]
        public string phoneNumber { get; set; } = string.Empty;

        public bool isValid {get; set;} = false;

        public int user_id { get; set; }

        [ForeignKey("user_id")]
        public virtual User? user { get; set; }

        public Tenant() { } // Constructor Kosong

        public Tenant(int id, string name, string address, string phoneNumber, int user_id)
        {
            this.id = id;
            this.name = name;
            this.address = address;
            this.phoneNumber = phoneNumber;
            this.user_id = user_id;
        }
    }
}
