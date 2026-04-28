using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sipetok_api.Models
{
    public class TransactionDetail
    {
        public int id { get; set; }

        [ForeignKey("transaction_id")]
        public int transaction_id { get; set; }

        [MaxLength(50)]
        public string category_name { get; set; } = string.Empty;
        public double quantity { get; set; }
        public decimal subtotal { get; set; } // Ubah ke decimal

        public TransactionDetail() { }

        public TransactionDetail(int id, int transaction_id, string category_name, double quantity, decimal subtotal)
        {
            this.id = id;
            this.transaction_id = transaction_id;
            this.category_name = category_name;
            this.quantity = quantity;
            this.subtotal = subtotal;
        }
    }
}
