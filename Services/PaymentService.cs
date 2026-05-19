using Microsoft.EntityFrameworkCore;
using sipetok_api.Data;
using sipetok_api.dto.Request;
using sipetok_api.Models;
using sipetok_api.Utils;

namespace sipetok_api.Services
{
    public class PaymentService
    {
        private readonly AppDbContext dbContext;

        private static readonly Dictionary<(PaymentState, PaymentTrigger), PaymentState> _transitions =
            new Dictionary<(PaymentState, PaymentTrigger), PaymentState>
        {
            { (PaymentState.WaitingForPayment, PaymentTrigger.Pay),    PaymentState.Success },
            { (PaymentState.WaitingForPayment, PaymentTrigger.Cancel), PaymentState.Cancelled },
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
                    Status = PaymentState.WaitingForPayment,
                    OrderStatus = OrderState.OrderComeIn, // Awal: Orderan Masuk
                    customer_name = dto.customer_name,
                    customer_phone_number = dto.customer_phone_number,
                };

                dbContext.Transactions.Add(transaction);
                await dbContext.SaveChangesAsync();

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

        // Ditambahkan parameter OrderService dan OrderTrigger agar eksekusi se-grup (Atomic)
        public virtual async Task<bool> UpdateStatus(int id, PaymentTrigger paymentTrigger, OrderService orderService, OrderTrigger orderTrigger)
        {
            using var dbTransaction = await dbContext.Database.BeginTransactionAsync();

            try
            {
                var transaksi = await dbContext.Transactions
                    .Include(t => t.details)
                    .FirstOrDefaultAsync(t => t.id == id);

                if (transaksi == null) return false;

                // 1. Validasi & Transisi Status Pembayaran
                if (_transitions.TryGetValue((transaksi.Status, paymentTrigger), out PaymentState nextPaymentState))
                {
                    // 2. LOGIKA PENGURANGAN STOK (Hanya jika Pembayaran Sukses)
                    if (nextPaymentState == PaymentState.Success)
                    {
                        foreach (var detail in transaksi.details)
                        {
                            // Mengambil data stok telur berdasarkan tenant
                            var eggData = await dbContext.Eggs
                                .FirstOrDefaultAsync(e => e.tenant_id == transaksi.tenant_id);

                            if (eggData == null)
                                throw new Exception($"Data stok telur tidak ditemukan untuk Tenant ini.");

                            if (eggData.stock < detail.quantity)
                                throw new Exception($"Stok telur tidak mencukupi! Sisa stok saat ini: {eggData.stock}, jumlah dibeli: {detail.quantity}");

                            // IMPLEMENTASI NYATA: Kurangi stok telur
                            eggData.stock -= detail.quantity;
                        }
                    }

                    // Set status pembayaran baru
                    transaksi.Status = nextPaymentState;

                    // 3. SINKRONISASI STATUS ORDER (OrderState)
                    // Panggil OrderService untuk merubah state order (misal: OrderComeIn -> ReadyForPickup)
                    var isOrderUpdated = orderService.UpdateOrderStatus(transaksi, orderTrigger);
                    if (!isOrderUpdated)
                    {
                        throw new Exception($"Transisi status pesanan tidak valid dari '{transaksi.OrderStatus}' dengan trigger '{orderTrigger}'.");
                    }

                    // Simpan semua perubahan (Payment, Stok Egg, dan Order Status)
                    await dbContext.SaveChangesAsync();
                    await dbTransaction.CommitAsync();
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                // Jika stok kurang atau ada error lain, rollback total!
                await dbTransaction.RollbackAsync();
                throw new Exception(ex.Message);
            }
        }
    }
}