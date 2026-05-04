using System.ComponentModel.DataAnnotations;

namespace sipetok_api.dto.Request
{
    public class TransactionDetailDto
    {
        [Required(ErrorMessage = "Transaction ID is required!")]
        public int transaction_id { get; set; }

        [Required(ErrorMessage = "Category name is required!")]
        public string category_name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Quantity is required!")]
        public double quantity { get; set; }
        
        [Required(ErrorMessage = "Subtotal is required!")]
        public decimal subtotal { get; set; }
        public double price { get; set; }

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