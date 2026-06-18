using sipetok_api.Utils;
using System.ComponentModel.DataAnnotations;

namespace sipetok_api.dto.Request
{
    public class PaymentRequestDto
    {
        public double PaymentAmount { get; set; }
        public PaymentRequestDto() { }

        public PaymentRequestDto(double PaymentAmount)
        {
            this.PaymentAmount = PaymentAmount;
        }
    }
}