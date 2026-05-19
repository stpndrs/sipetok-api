using System.ComponentModel.DataAnnotations;

namespace sipetok_api.dto.Request
{
    public class TransactionDetailDto
    {
        // [Required(ErrorMessage = "Transaction ID is required!")]
        // public int transaction_id { get; set; }

        [Required(ErrorMessage = "Category name is required!")]
        public string CategoryName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Quantity is required!")]
        public double Quantity { get; set; }

        [Required(ErrorMessage = "Subtotal is required!")]
        public decimal Subtotal { get; set; }
        public double Price { get; set; }

        public TransactionDetailDto() { }

        public TransactionDetailDto(string CategoryName, double Quantity, decimal Subtotal)
        {
            // this.transaction_id = transaction_id;
            this.CategoryName = CategoryName;
            this.Quantity = Quantity;
            this.Price = Price;
            this.Subtotal = Subtotal;
        }
    }
}