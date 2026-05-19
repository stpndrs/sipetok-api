using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace sipetok_api.Models
{
    public class Customer : BaseEntity
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual User? User { get; set; }

        [MaxLength(255)]
        public string Address { get; set; } = string.Empty;

        [MaxLength(20)]
        public string PhoneNumber { get; set; } = string.Empty;

        public Customer() { }
        public Customer(int Id, string Name, int UserId, string Address, string PhoneNumber)
        {
            this.Id = Id;
            this.Name = Name;
            this.UserId = UserId;
            this.Address = Address;
            this.PhoneNumber = PhoneNumber;
        }
    }
}
