using System.ComponentModel.DataAnnotations.Schema;

namespace sipetok_api.Models
{
    public class Egg : BaseEntity
    {
        public int Id { get; set; }
        public DateTime ProductionDate { get; set; }
        public double Stock { get; set; }

        public int TenantId { get; set; }

        [ForeignKey("TenantId")]
        public virtual Tenant? Tenant { get; set; }
        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public virtual EggCategory? Category { get; set; }

        public Egg() { }

        public Egg(int Id, DateTime ProductionDate, double Stock, int TenantId, int CategoryId)
        {
            this.Id = Id;
            this.ProductionDate = ProductionDate;
            this.Stock = Stock;
            this.TenantId = TenantId;
            this.CategoryId = CategoryId;
        }
    }
}
