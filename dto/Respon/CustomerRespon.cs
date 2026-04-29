namespace sipetok_api.dto.Respon
{
    public class CustomerRespon
    {
        public int id {get; set;}
        public string name { get; set; } = string.Empty;
        public int user_id { get; set; }
        public string address { get; set; } = string.Empty;
        public string phone_number { get; set; } = string.Empty;
        public CustomerRespon() { }

        public CustomerRespon(int id, string name, int user_id, string address, string phone_number)
        {
            this.id = id;
            this.name = name;
            this.user_id = user_id;
            this.address = address;
            this.phone_number = phone_number;
        }
    }
}