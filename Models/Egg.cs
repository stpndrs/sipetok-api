using System.ComponentModel.DataAnnotations.Schema;

namespace sipetok_api.Models
{
    public class Egg : BaseEntity
    {
        public int id { get; set; }
        public DateTime production_date { get; set; }
        public double stock { get; set; }

        public int tenant_id { get; set; }

        [ForeignKey("tenant_id")]
        public virtual Tenant? tenant { get; set; }
        public int category_id { get; set; }

        [ForeignKey("category_id")]
        public virtual EggCategory? category { get; set; }

        public Egg() { }

        public Egg(int id, DateTime production_date, double stock, int tenant_id, int category_id)
        {
            this.id = id;
            this.production_date = production_date;
            this.stock = stock;
            this.tenant_id = tenant_id;
            this.category_id = category_id;
        }
    }
}
