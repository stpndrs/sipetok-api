namespace sipetok_api.dto.Request
{
    public class EggDto
    {
        public DateTime production_date { get; set; }
        public int stock { get; set; }
        public int tenant_id { get; set; }
        public virtual TenantDto? tenant { get; set; }
        public int category_id { get; set; }

        public EggDto() { }

        public EggDto(DateTime production_date, int stock, int tenant_id, int category_id)
        {
            this.production_date = production_date;
            this.stock = stock;
            this.tenant_id = tenant_id;
            this.category_id = category_id;
        }
    }
}