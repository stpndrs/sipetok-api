namespace sipetok_api.dto.Respon
{
    public class EggCategoryResponseDto
    {
        public int Id { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int TenantId { get; set; }
        public EggCategoryResponseDto() { }

        public EggCategoryResponseDto(int Id, decimal Price, string Description, string Name, int TenantId)
        {
            this.Id = Id;
            this.Price = Price;
            this.Description = Description;
            this.Name = Name;
            this.TenantId = TenantId;
        }
    }
}