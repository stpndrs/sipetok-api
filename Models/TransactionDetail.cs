using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace sipetok_api.Models
{
    public class TransactionDetail : BaseEntity
    {
        public int Id { get; set; }

        public int TransactionId { get; set; }

        [ForeignKey("TransactionId")]
        public virtual Transaction? Transaction { get; set; }

        [MaxLength(50)]
        public string CategoryName { get; set; } = string.Empty;
        public double Quantity { get; set; }
        public double Price { get; set; }
        public decimal Subtotal { get; set; } // Ubah ke decimal

        public TransactionDetail() { }

        public TransactionDetail(int Id, int TransactionId, string CategoryName, double Quantity, double Price, decimal Subtotal)
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
