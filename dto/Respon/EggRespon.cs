using sipetok_api.dto.Respon;

namespace sipetok_api.Respon
{
    public class EggRespon
    {
        public int id {get; set;}
        public DateTime production_date { get; set; }
        public int stock { get; set; }
        public int tenant_id { get; set; }
        public int category_id { get; set; }
        public virtual EggCategoryRespon? category { get; set; }

        public EggRespon() { }

        public EggRespon(int id, DateTime production_date, int stock, int tenant_id, int category_id)
        {
            this.id = id;
            this.production_date = production_date;
            this.stock = stock;
            this.tenant_id = tenant_id;
            this.category_id = category_id;
        }
    }
}