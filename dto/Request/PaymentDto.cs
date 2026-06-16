using sipetok_api.Utils;
using System.ComponentModel.DataAnnotations;

namespace sipetok_api.dto.Request
{
    public class PaymentDto
    {
        public double PaymentAmount { get; set; }
        public PaymentDto() { }

        public PaymentDto(double PaymentAmount)
        {
            this.PaymentAmount = PaymentAmount;
        }
    }
}