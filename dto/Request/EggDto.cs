using System.ComponentModel.DataAnnotations;

namespace sipetok_api.dto.Request
{
    public class EggDto
    {
        [Required(ErrorMessage = "Production date is required!")]
        public DateTime production_date { get; set; }

        [Required(ErrorMessage = "Stock is required!")]
        public double stock { get; set; }
        public int tenant_id { get; set; }
        [Required(ErrorMessage = "Category is required!")]
        public int category_id { get; set; }

        public EggDto() { }

        public EggDto(DateTime production_date, double stock, int tenant_id, int category_id)
        {
            this.production_date = production_date;
            this.stock = stock;
            this.tenant_id = tenant_id;
            this.category_id = category_id;
        }
    }
}