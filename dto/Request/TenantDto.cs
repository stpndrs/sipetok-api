using System.ComponentModel.DataAnnotations;

namespace sipetok_api.dto.Request
{
    public class TenantDto
    {
        [Required(ErrorMessage = "Name is required!")]
        public string name { get; set; } = string.Empty;
        [Required(ErrorMessage = "Address is required!")]
        public string address { get; set; } = string.Empty;
        [Required(ErrorMessage = "Phone number is required!")]
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