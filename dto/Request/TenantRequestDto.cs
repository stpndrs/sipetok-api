using System.ComponentModel.DataAnnotations;

namespace sipetok_api.dto.Request
{
    public class TenantRequestDto
    {
        [Required(ErrorMessage = "Name is required!")]
        public string Name { get; set; } = string.Empty;
        [Required(ErrorMessage = "Address is required!")]
        public string Address { get; set; } = string.Empty;
        [Required(ErrorMessage = "Phone number is required!")]
        public string PhoneNumber { get; set; } = string.Empty;
        public bool IsValid { get; set; }

        public virtual UserRequestDto? User { get; set; }
        public TenantRequestDto() { }

        public TenantRequestDto(string Name, string Address, string PhoneNumber, bool IsValid)
        {
            this.Name = Name;
            this.Address = Address;
            this.PhoneNumber = PhoneNumber;
            this.IsValid = IsValid;
        }
    }
}