using System.ComponentModel.DataAnnotations.Schema;

namespace sipetok_api.Models
{
    public class EggInventory : BaseEntity
    {
        public int Id { get; set; }
        public DateTime ProductionDate { get; set; }
        public double Stock { get; set; }

        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public virtual EggCategory? Category { get; set; }

        public EggInventory() { }

        public EggInventory(int Id, DateTime ProductionDate, double Stock, int CategoryId)
        {
            this.Id = Id;
            this.ProductionDate = ProductionDate;
            this.Stock = Stock;
            this.CategoryId = CategoryId;
        }
    }
}
