using sipetok_api.Utils;
using System.ComponentModel.DataAnnotations;

namespace sipetok_api.dto.Request
{
    public class TransactionDto
    {
        public DateTime Date { get; set; }

        public double PaymentAmount { get; set; }

        public double TotalPrice { get; set; }

        public int TenantId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerPhoneNumber { get; set; } = string.Empty;

        public virtual ICollection<TransactionDetailDto> Details { get; set; } = new List<TransactionDetailDto>();

        public TransactionDto() { }

        public TransactionDto(double PaymentAmount, int TenantId, string CustomerName, string CustomerPhoneNumber)
        {
            this.Date = DateTime.Now;
            this.PaymentAmount = PaymentAmount;
            // this.TotalPrice = TotalPrice;
            this.TenantId = TenantId;
            this.CustomerName = CustomerName;
            this.CustomerPhoneNumber = CustomerPhoneNumber;
        }
    }
}