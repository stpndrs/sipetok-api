namespace sipetok_api.dto.Request
{
    public class TransactionDetailDto
    {
        public int transaction_id { get; set; }
        public string category_name { get; set; } = string.Empty;
        public double quantity { get; set; }
        public double price { get; set; }
        public decimal subtotal { get; set; } // Ubah ke decimal

        public TransactionDetailDto() { }

        public TransactionDetailDto(int transaction_id, string category_name, double quantity, double price, decimal subtotal)
        {
            this.transaction_id = transaction_id;
            this.category_name = category_name;
            this.quantity = quantity;
            this.price = price;
            this.subtotal = subtotal;
        }
    }
}