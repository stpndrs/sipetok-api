using sipetok_api.Utils;

namespace sipetok_api.dto.Response
{
    public class TransactionResponseDto
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public double PaymentAmount { get; set; }
        public double TotalPrice { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerPhoneNumber { get; set; } = string.Empty;
        public PaymentState PaymentStatus { get; set; } = PaymentState.WaitingForPayment;
        public OrderState OrderStatus { get; set; } = OrderState.OrderComeIn;

        public virtual ICollection<TransactionDetailResponseDto> Details { get; set; } = new List<TransactionDetailResponseDto>();

        public TransactionResponseDto() { }

        public TransactionResponseDto(int Id, double PaymentAmount, double TotalPrice, string CustomerName, string CustomerPhoneNumber, int PaymentStatus, int OrderStatus)
        {
            this.Id = Id;
            this.Date = DateTime.Now;
            this.PaymentAmount = PaymentAmount;
            this.TotalPrice = TotalPrice;
            this.CustomerName = CustomerName;
            this.CustomerPhoneNumber = CustomerPhoneNumber;
            this.PaymentStatus = (PaymentState)PaymentStatus;
            this.OrderStatus = (OrderState)OrderStatus;
        }
    }
}