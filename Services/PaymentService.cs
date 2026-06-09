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

        // 1. ALUR PEMBUATAN TRANSAKSI AWAL (Belum Ada Pembayaran)
        public virtual async Task<Transaction> ProcessTransaction(TransactionDto dto)
        {
            using var transactionScope = await dbContext.Database.BeginTransactionAsync();
            try
            {
                decimal totalPrice = 0;

                var transaction = new Transaction
                {
                    Date = dto.Date,
                    PaymentAmount = 0, // Set awal 0 karena belum dibayar
                    TotalPrice = 0,    // Akan dihitung otomatis dari detail di bawah
                    TenantId = dto.TenantId,
                    PaymentStatus = PaymentState.WaitingForPayment,
                    OrderStatus = OrderState.OrderComeIn,
                    CustomerName = dto.CustomerName,
                    CustomerPhoneNumber = dto.CustomerPhoneNumber,
                };

                dbContext.Transactions.Add(transaction);
                await dbContext.SaveChangesAsync();

                if (dto.Details != null)
                {
                    foreach (var d in dto.Details)
                    {
                        var eggCategory = await dbContext.EggCategories
                            .FindAsync(d.CategoryId);

                        if (eggCategory == null)
                        {
                            throw new Exception($"Kategori telur dengan ID {d.CategoryId} tidak ditemukan.");
                        }

                        decimal priceAtPurchase = eggCategory.Price;
                        decimal subtotal = (decimal)d.Quantity * priceAtPurchase;
                        totalPrice += subtotal;

                        dbContext.TransactionDetails.Add(new TransactionDetail
                        {
                            TransactionId = transaction.Id,
                            CategoryId = d.CategoryId,
                            Quantity = d.Quantity,
                            Subtotal = subtotal,
                            PriceAtPurchase = priceAtPurchase
                        });
                    }

                    transaction.TotalPrice = totalPrice;
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

        // 2. ALUR UPDATE STATUS & PROSES PEMBAYARAN NYATA
        // Sekarang menggunakan PaymentDto untuk menangkap payload uang yang dibayarkan
        public virtual async Task<bool> UpdateStatus(
            int id,
            PaymentTrigger paymentTrigger,
            OrderService orderService,
            OrderTrigger orderTrigger,
            PaymentDto? paymentDto = null)
        {
            using var dbTransaction = await dbContext.Database.BeginTransactionAsync();

            try
            {
                var transaksi = await dbContext.Transactions
                    .Include(t => t.Details)
                    .FirstOrDefaultAsync(t => t.Id == id);

                if (transaksi == null) return false;

                // Validasi & Transisi Status Pembayaran
                if (_transitions.TryGetValue((transaksi.PaymentStatus, paymentTrigger), out PaymentState nextPaymentState))
                {
                    // Jika aksi dipicu oleh tombol 'Pay', validasi isi nominal dari PaymentDto
                    if (paymentTrigger == PaymentTrigger.Pay)
                    {
                        if (paymentDto == null || paymentDto.PaymentAmount <= 0)
                        {
                            throw new Exception("Jumlah pembayaran (Payment Amount) tidak valid atau tidak dikirim.");
                        }

                        if (paymentDto.PaymentAmount < transaksi.TotalPrice)
                        {
                            throw new Exception($"Uang yang dibayarkan (Rp {paymentDto.PaymentAmount}) kurang dari total tagihan (Rp {transaksi.TotalPrice}).");
                        }

                        // Isi PaymentAmount transaksi dengan nominal asli dari dto baru
                        transaksi.PaymentAmount = paymentDto.PaymentAmount;
                    }

                    // LOGIKA PENGURANGAN STOK (FIFO - First In First Out)
                    if (nextPaymentState == PaymentState.Success)
                    {
                        foreach (var detail in transaksi.Details)
                        {
                            var availableEggs = await dbContext.Eggs
                                .Where(e => e.CategoryId == detail.CategoryId && e.Stock > 0)
                                .OrderBy(e => e.ProductionDate)
                                .ToListAsync();

                            double totalAvailableStock = availableEggs.Sum(e => e.Stock);

                            if (totalAvailableStock < detail.Quantity)
                            {
                                throw new Exception($"Stok telur tidak mencukupi! Total stok tersedia: {totalAvailableStock}, jumlah dibeli: {detail.Quantity}");
                            }

                            double quantityToDeduct = detail.Quantity;
                            foreach (var eggData in availableEggs)
                            {
                                if (quantityToDeduct <= 0) break;

                                if (eggData.Stock >= quantityToDeduct)
                                {
                                    eggData.Stock -= quantityToDeduct;
                                    quantityToDeduct = 0;
                                }
                                else
                                {
                                    quantityToDeduct -= eggData.Stock;
                                    eggData.Stock = 0;
                                }
                            }
                        }
                    }

                    // Set status pembayaran ke state berikutnya (Success / Cancelled)
                    transaksi.PaymentStatus = nextPaymentState;

                    // SINKRONISASI STATUS ORDER (OrderState)
                    var isOrderUpdated = orderService.UpdateOrderStatus(transaksi, orderTrigger);
                    if (!isOrderUpdated)
                    {
                        throw new Exception($"Transisi status pesanan tidak valid dari '{transaksi.OrderStatus}' dengan trigger '{orderTrigger}'.");
                    }

                    await dbContext.SaveChangesAsync();
                    await dbTransaction.CommitAsync();
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                await dbTransaction.RollbackAsync();
                throw new Exception(ex.Message);
            }
        }
    }
}