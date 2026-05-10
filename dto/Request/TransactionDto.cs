using sipetok_api.Utils;
using System.ComponentModel.DataAnnotations;

namespace sipetok_api.dto.Request
{
    public class TransactionDto
    {
        public DateTime date { get; set; }

        public decimal payment_amount { get; set; }

        public decimal total_price { get; set; }

        public int customer_id { get; set; }

        public PaymentState Status { get; set; } = PaymentState.Pending;

        public virtual CustomerDto? customer { get; set; }

        public virtual ICollection<TransactionDetailDto> details { get; set; } = new List<TransactionDetailDto>();
        
        public TransactionDto() { }

        public TransactionDto(decimal payment_amount, decimal total_price, int customer_id)
        {
            this.date = DateTime.Now;
            this.payment_amount = payment_amount;
            this.total_price = total_price;
            this.customer_id = customer_id;
        }
    }
}