namespace sipetok_api.dto.Response
{
    public class TenantResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public bool IsValid { get; set; }
        public int UserId { get; set; }
        public TenantResponseDto() { }

        public TenantResponseDto(int Id, string Name, string Address, string PhoneNumber, int UserId, bool IsValid)
        {
            this.Id = Id;
            this.Name = Name;
            this.Address = Address;
            this.PhoneNumber = PhoneNumber;
            this.IsValid = IsValid;
            this.UserId = UserId;
        }
    }
}