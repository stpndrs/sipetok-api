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

        private static readonly Dictionary<(PaymentState, PaymentTrigger), PaymentState> _transitions =
            new Dictionary<(PaymentState, PaymentTrigger), PaymentState>
        {
            // Jalur sukses
            { (PaymentState.Pending, PaymentTrigger.Process),    PaymentState.Processing },
            { (PaymentState.Processing, PaymentTrigger.Pay), PaymentState.Success },

            // Jalur gajadi (Cancel)
            { (PaymentState.Pending, PaymentTrigger.Cancel),    PaymentState.Cancelled },
            { (PaymentState.Processing, PaymentTrigger.Cancel), PaymentState.Cancelled }
        };

        public PaymentService(AppDbContext context)
        {
            dbContext = context;
        }

        public virtual async Task<Transaction> ProcessTransaction(TransactionDto dto)
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

        public virtual async Task<bool> UpdateStatus(int id, PaymentTrigger trigger = PaymentTrigger.Pay)
        {
            using var dbTransaction = await dbContext.Database.BeginTransactionAsync();

            try
            {
                // 1. Ambil data transaksi beserta detailnya
                var transaksi = await dbContext.Transactions
                    .Include(t => t.details)
                    .FirstOrDefaultAsync(t => t.id == id);

                if (transaksi == null) return false;

                // 2. Cek validasi transisi status
                if (_transitions.TryGetValue((transaksi.Status, PaymentTrigger.Pay), out PaymentState nextState))
                {
                    // LOGIKA PENGURANGAN STOK: Terjadi jika status berubah menjadi Success (Selesai/Dibayar)
                    if (nextState == PaymentState.Success)
                    {
                        foreach (var detail in transaksi.details)
                        {
                            // Cari data telur di tabel Eggs. 
                            // Kita asumsikan ada relasi atau pencocokan berdasarkan category_name
                            var eggData = await dbContext.Eggs
                                .FirstOrDefaultAsync(e => e.tenant_id == transaksi.tenant_id);
                            // Catatan: Jika ada banyak jenis telur, tambahkan filter kategori di sini, 
                            // misal: .FirstOrDefaultAsync(e => e.id == detail.egg_id)

                            if (eggData == null)
                                throw new Exception($"Data stok telur tidak ditemukan untuk Tenant ini.");

                            if (eggData.stock < detail.quantity)
                                throw new Exception($"Stok telur tidak mencukupi! Sisa stok: {eggData.stock}");

                            // Kurangi stok di tabel Eggs
                            //eggData.stock -= detail.quantity;
                        }
                    }

                    // 3. Update status transaksi
                    transaksi.Status = nextState;

                    await dbContext.SaveChangesAsync();

                    // Komit transaksi database
                    await dbTransaction.CommitAsync();
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                // Jika ada error (stok kurang/DB error), batalkan semua perubahan
                await dbTransaction.RollbackAsync();
                // Log pesan error agar bisa ditangkap di Controller (ex.Message)
                throw new Exception(ex.Message);
            }
        }
    }
}