using sipetok_api.Utilis;

namespace sipetok_api.dto.Respon
{
    class TransactionRespon
    {
        public int id {get; set;}
        public DateTime date { get; set; }
        public decimal payment_amount { get; set; }
        public decimal total_price { get; set; }
        public int tenant_id { get; set; }
        public int customer_id { get; set; }
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
        public virtual ICollection<TransactionDetailRespon> details { get; set; } = new List<TransactionDetailRespon>();

        public TransactionRespon() { }

        public TransactionRespon(int id, decimal payment_amount, decimal total_price, int tenant_id, int customer_id)
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