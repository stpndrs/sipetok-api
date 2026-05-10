using Microsoft.EntityFrameworkCore;
using sipetok_api.Data;
using sipetok_api.dto.Request;
using sipetok_api.Models;
using sipetok_api.Utils;

namespace sipetok_api.service
{
    public class PaymentService
    {
        private readonly AppDbContext dbContext;

        
        private static readonly Dictionary<(PaymentState, string), PaymentState> _transitions =
            new Dictionary<(PaymentState, string), PaymentState>
        {
            // Jalur sukses
            { (PaymentState.Pending, "NEXT"),    PaymentState.Processing },
            { (PaymentState.Processing, "NEXT"), PaymentState.Success },

            // Jalur gajadi (Cancel)
            { (PaymentState.Pending, "CANCEL"),    PaymentState.Cancelled },
            { (PaymentState.Processing, "CANCEL"), PaymentState.Cancelled }
        };

        public PaymentService(AppDbContext context)
        {
            dbContext = context;
        }

        public async Task<Transaction> ProcessTransaction(TransactionDto dto)
        {
            using var transactionScope = await dbContext.Database.BeginTransactionAsync();
            try
            {
                var transaction = new Transaction
                {
                    date = dto.date,
                    payment_amount = dto.payment_amount,
                    total_price = dto.total_price,
                    tenant_id = dto.tenant_id,
                    Status = dto.Status,
                    customer_name = dto.customer_name,
                    customer_phone_number = dto.customer_phone_number,
                };

                dbContext.Transactions.Add(transaction);
                await dbContext.SaveChangesAsync();

                // Tambahkan Details
                if (dto.details != null)
                {
                    foreach (var d in dto.details)
                    {
                        dbContext.TransactionDetails.Add(new TransactionDetail
                        {
                            transaction_id = transaction.id,
                            category_name = d.category_name,
                            quantity = d.quantity,
                            subtotal = d.subtotal
                        });
                    }
                    await dbContext.SaveChangesAsync();
                }

                await transactionScope.CommitAsync();
                return transaction;
            }
            catch (Exception)
            {
                await transactionScope.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> UpdateStatus(int id, string action = "NEXT")
        {
            var transaksi = await dbContext.Transactions.FirstOrDefaultAsync(t => t.id == id);
            if (transaksi == null) return false;

            // Cek tabel anomali atau tidak
            if (_transitions.TryGetValue((transaksi.Status, action.ToUpper()), out PaymentState nextState))
            {
                transaksi.Status = nextState;

                try
                {
                    await dbContext.SaveChangesAsync();
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }

            return false;
        }
    }
}