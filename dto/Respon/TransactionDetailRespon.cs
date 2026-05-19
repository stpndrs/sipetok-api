namespace sipetok_api.dto.Respon
{
    public class TransactionDetailRespon
    {
        public int Id { get; set; }
        public int TransactionId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public double Quantity { get; set; }
        public double Price { get; set; }
        public decimal Subtotal { get; set; } // Ubah ke decimal

        public TransactionDetailRespon() { }

        public TransactionDetailRespon(int Id, int TransactionId, string CategoryName, double Quantity, double Price, decimal Subtotal)
        {
            this.Id = Id;
            this.TransactionId = TransactionId;
            this.CategoryName = CategoryName;
            this.Quantity = Quantity;
            this.Price = Price;
            this.Subtotal = Subtotal;
        }
    }
}