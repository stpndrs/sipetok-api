namespace sipetok_api.dto.Response
{
    public class TransactionDetailResponseDto
    {
        public int Id { get; set; }
        public int TransactionId { get; set; }
        public EggCategoryResponseDto Category { get; set; }
        public double Quantity { get; set; }
        public double PriceAtPurchase { get; set; }
        public double Subtotal { get; set; } 

        public TransactionDetailResponseDto(int Id, int TransactionId, EggCategoryResponseDto Category, double Quantity, double PriceAtPurchase, double Subtotal)
        {
            this.Id = Id;
            this.TransactionId = TransactionId;
            this.Category = Category;
            this.Quantity = Quantity;
            this.PriceAtPurchase = PriceAtPurchase;
            this.Subtotal = Subtotal;
        }
    }
}