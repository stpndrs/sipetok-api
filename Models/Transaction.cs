using sipetok_api.Utils;
using System.ComponentModel.DataAnnotations.Schema;

namespace sipetok_api.Models
{
    public class Transaction : BaseEntity
    {
        public int id { get; set; }
        public DateTime date { get; set; }
        public decimal payment_amount { get; set; }
        public decimal total_price { get; set; }
        public PaymentState Status { get; set; } = PaymentState.WaitingForPayment;
        public OrderState OrderStatus { get; set; } = OrderState.OrderComeIn;


        public int tenant_id { get; set; }

        [ForeignKey("tenant_id")]
        public virtual Tenant? tenant { get; set; }

        public string customer_name { get; set; } = string.Empty;
        public string customer_phone_number { get; set; } = string.Empty;

        public virtual ICollection<TransactionDetail> details { get; set; } = new List<TransactionDetail>();

        public Transaction() { }

        public Transaction(int id, decimal payment_amount, decimal total_price, int tenant_id, string customer_name, string customer_phone_number)
        {
            this.id = id;
            this.date = DateTime.Now;
            this.payment_amount = payment_amount;
            this.total_price = total_price;
            this.tenant_id = tenant_id;
            this.customer_name = customer_name;
            this.customer_phone_number = customer_phone_number;
        }
    }
}
