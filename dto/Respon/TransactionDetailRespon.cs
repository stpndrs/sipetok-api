namespace sipetok_api.dto.Respon
{
    public class TransactionDetailRespon
    {
        public int Id { get; set; }
        public int TransactionId { get; set; }
        public string Category { get; set; }
        public double Quantity { get; set; }
        public double PriceAtPurchase { get; set; }
        public decimal Subtotal { get; set; } // Ubah ke decimal

        // public TransactionDetailRespon() { }

        public TransactionDetailRespon(int Id, int TransactionId, EggCategoryRespon Category, double Quantity, double PriceAtPurchase, decimal Subtotal)
        {
            this.Id = Id;
            this.TransactionId = TransactionId;
            this.Category = Category.Name;
            this.Quantity = Quantity;
            this.PriceAtPurchase = PriceAtPurchase;
            this.Subtotal = Subtotal;
        }
    }
}