using System.ComponentModel.DataAnnotations;

namespace sipetok_api.dto.Request
{
    public class OperationalDto
    {
        [Required(ErrorMessage = "Name is required!")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Operational cost is required!")]
        public string OperationalCost { get; set; } = string.Empty;

        // [Required(ErrorMessage = "Tenant ID is required!")]
        public int TenantId { get; set; }

        [Required(ErrorMessage = "Operational date is required!")]
        public DateTime OperationalDate { get; set; }

        public OperationalDto() { }
        public OperationalDto(string Name, string OperationalCost, DateTime OperationalDate)
        {
            this.Name = Name;
            this.OperationalCost = OperationalCost;
            this.OperationalDate = OperationalDate;
        }
    }
}