namespace sipetok_api.dto.Respon
{
    public class EggCategoryRespon
    {
        public int id {get; set;}
        public decimal price { get; set; }
        public string description { get; set; } = string.Empty;
        public string name { get; set; } = string.Empty;
        public int tenant_id { get; set; }
        public EggCategoryRespon() { }

        public EggCategoryRespon(int id, decimal price, string description, string name, int tenant_id)
        {
            this.id = id;
            this.price = price;
            this.description = description;
            this.name = name;
            this.tenant_id = tenant_id;
        }
    }
}