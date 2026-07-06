namespace sipetok_api.dto.Request
{
    public class TransactionRequestDto
    {
        public DateTime Date { get; set; }

        public double PaymentAmount { get; set; }

        public double TotalPrice { get; set; }

        public int TenantId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerPhoneNumber { get; set; } = string.Empty;

        public virtual ICollection<TransactionDetailDto> Details { get; set; } = new List<TransactionDetailDto>();

        public TransactionRequestDto() { }

        public TransactionRequestDto(double PaymentAmount, int TenantId, string CustomerName, string CustomerPhoneNumber)
        {
            this.Date = DateTime.Now;
            this.PaymentAmount = PaymentAmount;
            this.TenantId = TenantId;
            this.CustomerName = CustomerName;
            this.CustomerPhoneNumber = CustomerPhoneNumber;
        }
    }
}