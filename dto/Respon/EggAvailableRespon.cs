using sipetok_api.dto.Respon;

namespace sipetok_api.Respon
{
    public class EggAvailableRespon
    {
        public double Stock { get; set; }
        public int TenantId { get; set; }
        public int CategoryId { get; set; }
        public virtual EggCategoryRespon? Category { get; set; }

        public EggAvailableRespon() { }

        public EggAvailableRespon(double Stock, int TenantId, int CategoryId)
        {
            this.Stock = Stock;
            this.TenantId = TenantId;
            this.CategoryId = CategoryId;
        }
    }
}