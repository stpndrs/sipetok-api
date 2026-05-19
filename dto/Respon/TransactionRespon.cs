using sipetok_api.Utils;

namespace sipetok_api.dto.Respon
{
    public class TransactionRespon
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public decimal PaymentAmount { get; set; }
        public decimal TotalPrice { get; set; }
        public int TenantId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerPhoneNumber { get; set; } = string.Empty;
        public PaymentState Status { get; set; } = PaymentState.WaitingForPayment;
        public OrderState OrderStatus { get; set; } = OrderState.OrderComeIn;

        public virtual ICollection<TransactionDetailRespon> Details { get; set; } = new List<TransactionDetailRespon>();

        public TransactionRespon() { }

        public TransactionRespon(int Id, decimal PaymentAmount, decimal TotalPrice, int TenantId, int customer_id, string CustomerName, string CustomerPhoneNumber)
        {
            this.Id = Id;
            this.Date = DateTime.Now;
            this.PaymentAmount = PaymentAmount;
            this.TotalPrice = TotalPrice;
            this.TenantId = TenantId;
            this.CustomerName = CustomerName;
            this.CustomerPhoneNumber = CustomerPhoneNumber;
        }
    }
}