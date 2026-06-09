using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace sipetok_api.Models
{
    public class EggCategory : BaseEntity
    {
        public int Id { get; set; }

        // Gunakan decimal untuk harga
        public decimal Price { get; set; }

        [MaxLength(255)]
        public string Description { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        public int TenantId { get; set; }

        [ForeignKey("TenantId")]
        public virtual Tenant? Tenant { get; set; }

        public virtual ICollection<Egg> Eggs { get; set; } = new List<Egg>();

        public EggCategory() { }

        public EggCategory(int Id, decimal Price, string Description, string Name)
        {
            this.Id = Id;
            this.Price = Price;
            this.Description = Description;
            this.Name = Name;
        }
    }
}
