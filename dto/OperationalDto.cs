namespace sipetok_api.dto
{
    public class OperationalDto
    {
        public int id { get; set; }

        public string name { get; set; } = string.Empty;

        public string operational_cost { get; set; } = string.Empty; 

        public int tenant_id { get; set; }

        public virtual TenantDto? tenant { get; set; }
        public DateTime operational_date { get; set; }

        public OperationalDto() { }

        public OperationalDto(int id, string name, string operational_cost, int tenant_id, DateTime operational_date)
        {
            this.id = id;
            this.name = name;
            this.operational_cost = operational_cost;
            this.tenant_id = tenant_id;
            this.operational_date = operational_date;
        }
    }
}