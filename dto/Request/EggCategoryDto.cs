using System.ComponentModel.DataAnnotations;

namespace sipetok_api.dto.Request
{
    public class EggCategoryDto
    {
        [Required(ErrorMessage = "Price is required!")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Description is required!")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Name is required!")]
        public string Name { get; set; } = string.Empty;

        // [Required(ErrorMessage = "Tenant ID is required!")]
        public int TenantId { get; set; }

        public EggCategoryDto() { }

        public EggCategoryDto(decimal Price, string Description, string Name, int TenantId)
        {
            this.Price = Price;
            this.Description = Description;
            this.Name = Name;
            this.TenantId = TenantId;
        }
    }
}