namespace sipetok_api.dto.Request
{
    public class EggCategoryDto
    {
        public decimal price { get; set; }
        public string description { get; set; } = string.Empty;
        public string name { get; set; } = string.Empty;
        public EggCategoryDto() { }

        public EggCategoryDto(decimal price, string description, string name)
        {
            this.price = price;
            this.description = description;
            this.name = name;
        }
    }
}