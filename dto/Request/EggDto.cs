using System.ComponentModel.DataAnnotations;

namespace sipetok_api.dto.Request
{
    public class EggDto
    {
        [Required(ErrorMessage = "Production date is required!")]
        public DateTime ProductionDate { get; set; }

        [Required(ErrorMessage = "Stock is required!")]
        public double Stock { get; set; }
        public int TenantId { get; set; }
        [Required(ErrorMessage = "Category is required!")]
        public int CategoryId { get; set; }

        public EggDto() { }

        public EggDto(DateTime ProductionDate, double Stock, int TenantId, int CategoryId)
        {
            this.ProductionDate = ProductionDate;
            this.Stock = Stock;
            this.TenantId = TenantId;
            this.CategoryId = CategoryId;
        }
    }
}