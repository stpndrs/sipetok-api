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

        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public virtual EggCategory? Category { get; set; }
        public double Quantity { get; set; }
        public decimal PriceAtPurchase { get; set; }
        public decimal Subtotal { get; set; } // Ubah ke decimal

        public TransactionDetail() { }

        public TransactionDetail(int Id, int TransactionId, int CategoryId, double Quantity, decimal PriceAtPurchase, decimal Subtotal)
        {
            this.Id = Id;
            this.TransactionId = TransactionId;
            this.CategoryId = CategoryId;
            this.Quantity = Quantity;
            this.PriceAtPurchase = PriceAtPurchase;
            this.Subtotal = Subtotal;
        }
    }
}
