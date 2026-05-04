using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace sipetok_api.Models
{
    public class TransactionDetail : BaseEntity
    {
        public int id { get; set; }

        public int transaction_id { get; set; }

        [ForeignKey("transaction_id")]
        public virtual Transaction? transaction { get; set; }

        [MaxLength(50)]
        public string category_name { get; set; } = string.Empty;
        public double quantity { get; set; }
        public double price { get; set; }
        public decimal subtotal { get; set; } // Ubah ke decimal

        public TransactionDetail() { }

        public TransactionDetail(int id, int transaction_id, string category_name, double quantity, double price, decimal subtotal)
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
