using System.ComponentModel.DataAnnotations;

namespace sipetok_api.dto.Request
{
    public class EggInventoryDto
    {
        [Required(ErrorMessage = "Production date is required!")]
        public DateTime ProductionDate { get; set; }

        [Required(ErrorMessage = "Stock is required!")]
        public double Stock { get; set; }

        [Required(ErrorMessage = "Category is required!")]
        public int CategoryId { get; set; }

        public EggInventoryDto() { }

        public EggInventoryDto(DateTime ProductionDate, double Stock, int CategoryId)
        {
            this.ProductionDate = ProductionDate;
            this.Stock = Stock;
            this.CategoryId = CategoryId;
        }
    }
}