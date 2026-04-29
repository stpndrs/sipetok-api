using sipetok_api.Utilis;

namespace sipetok_api.dto.Request
{
    class TransactionDto
    {
        public DateTime date { get; set; }
        public decimal payment_amount { get; set; }
        public decimal total_price { get; set; }
        public int tenant_id { get; set; }
        public int customer_id { get; set; }
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
        public virtual CustomerDto? customer { get; set; }

        public virtual ICollection<TransactionDetailDto> details { get; set; } = new List<TransactionDetailDto>();

        public TransactionDto() { }

        public TransactionDto(decimal payment_amount, decimal total_price, int tenant_id, int customer_id)
        {
            this.date = DateTime.Now;
            this.payment_amount = payment_amount;
            this.total_price = total_price;
            this.tenant_id = tenant_id;
            this.customer_id = customer_id;
        }
    }
}