using sipetok_api.Utils;

namespace sipetok_api.dto.Respon
{
    public class TransactionRespon
    {
        public int id {get; set;}
        public DateTime date { get; set; }
        public decimal payment_amount { get; set; }
        public decimal total_price { get; set; }
        public int tenant_id { get; set; }
        public string customer_name { get; set; } = string.Empty;
        public string customer_phone_number { get; set; } = string.Empty;
        public PaymentState Status { get; set; } = PaymentState.WaitingForPayment;
        public OrderState OrderStatus { get; set; } = OrderState.OrderComeIn;

        public virtual ICollection<TransactionDetailRespon> details { get; set; } = new List<TransactionDetailRespon>();

        public TransactionRespon() { }

        public TransactionRespon(int id, decimal payment_amount, decimal total_price, int tenant_id, int customer_id, string customer_name, string customer_phone_number)
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