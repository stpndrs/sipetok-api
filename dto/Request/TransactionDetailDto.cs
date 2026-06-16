using System.ComponentModel.DataAnnotations;

namespace sipetok_api.dto.Request
{
    public class TransactionDetailDto
    {
        // [Required(ErrorMessage = "Transaction ID is required!")]
        // public int transaction_id { get; set; }

        [Required(ErrorMessage = "Category name is required!")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Quantity is required!")]
        public double Quantity { get; set; }

        [Required(ErrorMessage = "Subtotal is required!")]
        public double Subtotal { get; set; }
        public double PriceAtPurchase { get; set; }

        public TransactionDetailDto() { }

        public TransactionDetailDto(int CategoryId, double Quantity)
        {
            // this.transaction_id = transaction_id;
            this.CategoryId = CategoryId;
            this.Quantity = Quantity;
            this.PriceAtPurchase = PriceAtPurchase;
            this.Subtotal = Subtotal;
        }
    }
}