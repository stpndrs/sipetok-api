using sipetok_api.Utils;

namespace sipetok_api.dto.Request
{
    public class TransactionDto
    {
        public DateTime date { get; set; }
        public decimal payment_amount { get; set; }
        public decimal total_price { get; set; }
        public int tenant_id { get; set; }
        public string customer_name { get; set; } = string.Empty;
        public string customer_phone_number { get; set; } = string.Empty;
        public PaymentState Status { get; set; } = PaymentState.Pending;
        public virtual ICollection<TransactionDetailDto> details { get; set; } = new List<TransactionDetailDto>();
        public TransactionDto() { }

        public TransactionDto(decimal payment_amount, decimal total_price, int tenant_id, int customer_id, string customer_name, string customer_phone_number)
        {
            this.date = DateTime.Now;
            this.payment_amount = payment_amount;
            this.total_price = total_price;
            this.tenant_id = tenant_id;
            this.customer_name = customer_name;
            this.customer_phone_number = customer_phone_number;
        }
    }
}