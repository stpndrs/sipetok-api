using sipetok_api.Utils;

namespace sipetok_api.dto.Respon
{
    public class TransactionRespon
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public double PaymentAmount { get; set; }
        public double TotalPrice { get; set; }
        public int TenantId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerPhoneNumber { get; set; } = string.Empty;
        public PaymentState PaymentStatus { get; set; } = PaymentState.WaitingForPayment;
        public OrderState OrderStatus { get; set; } = OrderState.OrderComeIn;

        public virtual ICollection<TransactionDetailRespon> Details { get; set; } = new List<TransactionDetailRespon>();

        public TransactionRespon() { }

        public TransactionRespon(int Id, double PaymentAmount, double TotalPrice, int TenantId, string CustomerName, string CustomerPhoneNumber, int PaymentStatus, int OrderStatus)
        {
            this.Id = Id;
            this.Date = DateTime.Now;
            this.PaymentAmount = PaymentAmount;
            this.TotalPrice = TotalPrice;
            this.TenantId = TenantId;
            this.CustomerName = CustomerName;
            this.CustomerPhoneNumber = CustomerPhoneNumber;
            this.PaymentStatus = (PaymentState)PaymentStatus;
            this.OrderStatus = (OrderState)OrderStatus;
        }
    }
}