namespace sipetok_api.dto.Request
{
    public class CustomerDto
    {
        public string name { get; set; } = string.Empty;
        public virtual UserDto? user { get; set; }
        public string address { get; set; } = string.Empty;
        public string phone_number { get; set; } = string.Empty;
        public CustomerDto() { }
        public CustomerDto(string name, string address, string phone_number)
        {
            this.name = name;
            this.address = address;
            this.phone_number = phone_number;
        }
    }
}