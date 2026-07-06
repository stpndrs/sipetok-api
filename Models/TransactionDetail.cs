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
        public double PriceAtPurchase { get; set; }
        public double Subtotal { get; set; } 

        [NotMapped]
        public new DateTime? CreatedAt { get; set; }

        [NotMapped]
        public new DateTime? UpdatedAt { get; set; }

        [NotMapped]
        public new DateTime? DeletedAt { get; set; }

        public TransactionDetail() { }

        public TransactionDetail(int Id, int TransactionId, int CategoryId, double Quantity, double PriceAtPurchase, double Subtotal)
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
