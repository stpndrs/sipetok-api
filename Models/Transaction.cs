using sipetok_api.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sipetok_api.Models
{
    public class Transaction
    {
        public int id { get; set; }
        public DateTime date { get; set; }
        public decimal payment_amount { get; set; } // Ubah ke decimal
        public decimal total_price { get; set; }    // Ubah ke decimal

        [ForeignKey("tenant_id")]
        public int tenant_id { get; set; }

        [ForeignKey("customer_id")]
        public int customer_id { get; set; }
        public PaymentState Status { get; set; } = PaymentState.Pending;

        public virtual ICollection<TransactionDetail> details { get; set; } = new List<TransactionDetail>();

        public Transaction() { }

        public Transaction(int id, decimal payment_amount, decimal total_price, int tenant_id, int customer_id)
        {
            this.id = id;
            this.date = DateTime.Now;
            this.payment_amount = payment_amount;
            this.total_price = total_price;
            this.tenant_id = tenant_id;
            this.customer_id = customer_id;


        }
    }
}
