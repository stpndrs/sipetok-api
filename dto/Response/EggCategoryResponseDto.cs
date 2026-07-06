namespace sipetok_api.dto.Response
{
    public class EggCategoryResponseDto
    {
        public int Id { get; set; }
        public double Price { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int TenantId { get; set; }
        public double TotalEgg { get; set; }
        public EggCategoryResponseDto() { }

        public EggCategoryResponseDto(int Id, double Price, string Description, string Name, int TenantId, double TotalEgg)
        {
            this.Id = Id;
            this.Price = Price;
            this.Description = Description;
            this.Name = Name;
            this.TenantId = TenantId;
            this.TotalEgg = TotalEgg;
        }
    }
}