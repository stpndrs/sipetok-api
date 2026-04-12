namespace sipetok_api.dto
{
    public class TenantDto
    {
        public int id { get; set; }

        public string name { get; set; } = string.Empty;

        public string address { get; set; } = string.Empty;

        public string phoneNumber { get; set; } = string.Empty;

        public int user_id { get; set; }

        public virtual UserDto? user { get; set; }

        public TenantDto() { } 

        public TenantDto(int id, string name, string address, string phoneNumber, int user_id)
        {
            this.id = id;
            this.name = name;
            this.address = address;
            this.phoneNumber = phoneNumber;
            this.user_id = user_id;
        }
    }
}