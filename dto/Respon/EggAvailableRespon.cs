using sipetok_api.dto.Respon;

namespace sipetok_api.Respon
{
    public class EggAvailableRespon
    {
        public double Stock { get; set; }
        public int TenantId { get; set; }
        public virtual TenantRespon? Tenant { get; set; }
        public int CategoryId { get; set; }
        public virtual EggCategoryRespon? Category { get; set; }

        public string CategoryName { get; set; }
        public string TenantName { get; set; }

        public EggAvailableRespon() { }

        public EggAvailableRespon(double Stock, int TenantId, int CategoryId, string CategoryName, string TenantName)
        {
            this.Stock = Stock;
            this.TenantId = TenantId;
            this.CategoryId = CategoryId;
            this.CategoryName = CategoryName;
            this.TenantName = TenantName;
        }
    }
}