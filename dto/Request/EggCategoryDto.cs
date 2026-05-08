using System.ComponentModel.DataAnnotations;

namespace sipetok_api.dto.Request
{
    public class EggCategoryDto
    {
        [Required(ErrorMessage = "Price is required!")]
        public decimal price { get; set; }

        [Required(ErrorMessage = "Description is required!")]
        public string description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Name is required!")]
        public string name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Tenant ID is required!")]
        public int tenant_id { get; set; }
        
        public EggCategoryDto() { }

        public EggCategoryDto(decimal price, string description, string name, int tenant_id)
        {
            this.price = price;
            this.description = description;
            this.name = name;
            this.tenant_id = tenant_id;
        }
    }
}