using System.ComponentModel.DataAnnotations;

namespace sipetok_api.dto.Request
{
    public class EggCategoryRequestDto
    {
        [Required(ErrorMessage = "Price is required!")]
        public double Price { get; set; }

        [Required(ErrorMessage = "Description is required!")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Name is required!")]
        public string Name { get; set; } = string.Empty;

        public EggCategoryRequestDto() { }

        public EggCategoryRequestDto(double Price, string Description, string Name)
        {
            Price = Price;
            Description = Description;
            Name = Name;
        }
    }
}