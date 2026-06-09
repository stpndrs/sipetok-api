using sipetok_api.Utils;
using System.ComponentModel.DataAnnotations;

namespace sipetok_api.dto.Request
{
    public class PaymentDto
    {
        public decimal PaymentAmount { get; set; }
        public PaymentDto() { }

        public PaymentDto(decimal PaymentAmount)
        {
            this.PaymentAmount = PaymentAmount;
        }
    }
}