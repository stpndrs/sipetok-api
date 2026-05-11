namespace sipetok_api.dto.Respon
{
    public class TransactionDetailRespon
    {
        public int id {get; set;}
        public int transaction_id { get; set; }
        public string category_name { get; set; } = string.Empty;
        public double quantity { get; set; }
        public double price { get; set; }
        public decimal subtotal { get; set; } // Ubah ke decimal

        public TransactionDetailRespon() { }

        public TransactionDetailRespon(int id, int transaction_id, string category_name, double quantity, double price, decimal subtotal)
        {
            this.id = id;
            this.transaction_id = transaction_id;
            this.category_name = category_name;
            this.quantity = quantity;
            this.price = price;
            this.subtotal = subtotal;
        }
    }
}