using System.ComponentModel.DataAnnotations;

namespace sipetok_api.dto.Request
{
    public class TenantDto
    {
        [Required(ErrorMessage = "Name is required!")]
        public string Name { get; set; } = string.Empty;
        [Required(ErrorMessage = "Address is required!")]
        public string Address { get; set; } = string.Empty;
        [Required(ErrorMessage = "Phone number is required!")]
        public string PhoneNumber { get; set; } = string.Empty;
        public bool IsValid { get; set; }

        public virtual UserDto? User { get; set; }
        public TenantDto() { }

        public TenantDto(string Name, string Address, string PhoneNumber)
        {
            this.Name = Name;
            this.Address = Address;
            this.PhoneNumber = PhoneNumber;
        }
    }
}