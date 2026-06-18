namespace sipetok_api.dto.Response
{
    public class OperationalResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string OperationalCost { get; set; } = string.Empty;
        public int TenantId { get; set; }
        public DateTime OperationalDate { get; set; }
        public OperationalResponseDto() { }
        public OperationalResponseDto(int Id, string Name, string OperationalCost, int TenantId, DateTime OperationalDate)
        {
            this.Id = Id;
            this.Name = Name;
            this.OperationalCost = OperationalCost;
            this.TenantId = TenantId;
            this.OperationalDate = OperationalDate;
        }
    }
}