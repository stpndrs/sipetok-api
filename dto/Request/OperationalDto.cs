using System.ComponentModel.DataAnnotations;

namespace sipetok_api.dto.Request
{
    public class OperationalDto
    {
        [Required(ErrorMessage = "Name is required!")]
        public string name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Operational cost is required!")]
        public string operational_cost { get; set; } = string.Empty;

        [Required(ErrorMessage = "Tenant ID is required!")]
        public int tenant_id { get; set; }

        [Required(ErrorMessage = "Operational date is required!")]
        public DateTime operational_date { get; set; }
        
        public OperationalDto() { }

        public OperationalDto(string name, string operational_cost, int tenant_id, DateTime operational_date)
        {
            this.name = name;
            this.operational_cost = operational_cost;
            this.tenant_id = tenant_id;
            this.operational_date = operational_date;
        }
    }
}