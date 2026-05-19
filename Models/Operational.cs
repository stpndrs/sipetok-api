using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace sipetok_api.Models
{
    public class Operational : BaseEntity
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(50)]
        public string OperationalCost { get; set; } = string.Empty;

        public int TenantId { get; set; }

        [ForeignKey("TenantId")]
        public virtual Tenant? Tenant { get; set; }
        public DateTime OperationalDate { get; set; }

        public Operational() { }

        public Operational(int Id, string Name, string OperationalCost, int TenantId, DateTime OperationalDate)
        {
            this.Id = Id;
            this.Name = Name;
            this.OperationalCost = OperationalCost;
            this.TenantId = TenantId;
            this.OperationalDate = OperationalDate;
        }
    }
}
