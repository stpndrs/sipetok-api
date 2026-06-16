using System.ComponentModel.DataAnnotations;

namespace sipetok_api.dto.Request
{
    public class EggCategoryRequestDto
    {
        [Required(ErrorMessage = "Price is required!")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Description is required!")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Name is required!")]
        public string Name { get; set; } = string.Empty;

        public EggCategoryRequestDto() { }

        public EggCategoryRequestDto(decimal Price, string Description, string Name)
        {
            this.Price = Price;
            this.Description = Description;
            this.Name = Name;
        }
    }
}