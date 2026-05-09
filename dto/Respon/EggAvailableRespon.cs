using sipetok_api.dto.Respon;

namespace sipetok_api.Respon
{
    public class EggAvailableRespon
    {
        public int stock { get; set; }
        public int tenant_id { get; set; }
        public int category_id { get; set; }
        public virtual EggCategoryRespon? category { get; set; }

        public EggAvailableRespon() { }

        public EggAvailableRespon(int stock, int tenant_id, int category_id)
        {
            this.stock = stock;
            this.tenant_id = tenant_id;
            this.category_id = category_id;
        }
    }
}