using System.ComponentModel.DataAnnotations;

namespace sipetok_api.dto.Request
{
    public class EggDto
    {
        [Required(ErrorMessage = "Production date is required!")]
        public DateTime production_date { get; set; }

        [Required(ErrorMessage = "Stock is required!")]
        public int stock { get; set; }

        [Required(ErrorMessage = "Tenant is required!")]
        public int tenant_id { get; set; }
        public virtual TenantDto? tenant { get; set; }

        [Required(ErrorMessage = "Category is required!")]
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