namespace sipetok_api.dto.Request
{
    public class OperationalDto
    {
        public string name { get; set; } = string.Empty;
        public string operational_cost { get; set; } = string.Empty; 
        public int tenant_id { get; set; }
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