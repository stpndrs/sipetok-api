namespace sipetok_api.dto.Request
{
    public class TenantDto
    {
        public string name { get; set; } = string.Empty;
        public string address { get; set; } = string.Empty;
        public string phoneNumber { get; set; } = string.Empty;
        public virtual UserDto? user { get; set; }
        public TenantDto() { } 

        public TenantDto(string name, string address, string phoneNumber)
        {
            this.name = name;
            this.address = address;
            this.phoneNumber = phoneNumber;
        }
    }
}