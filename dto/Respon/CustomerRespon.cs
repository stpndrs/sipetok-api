namespace sipetok_api.dto.Respon
{
    public class CustomerRespon
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int UserId { get; set; }
        public string Address { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public CustomerRespon() { }

        public CustomerRespon(int Id, string Name, int UserId, string Address, string PhoneNumber)
        {
            this.Id = Id;
            this.Name = Name;
            this.UserId = UserId;
            this.Address = Address;
            this.PhoneNumber = PhoneNumber;
        }
    }
}