using Microsoft.EntityFrameworkCore;
using sipetok_api.Data;
using sipetok_api.dto.Request;
using sipetok_api.Models;
using sipetok_api.Utils;
using sipetok_api.Observers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace sipetok_api.Services
{
    public class PaymentService : IPaymentSubject
    {
        private readonly AppDbContext dbContext;
        private readonly List<IPaymentObserver> _observers = new List<IPaymentObserver>();

        private static readonly Dictionary<(PaymentState, PaymentTrigger), PaymentState> _transitions =
            new Dictionary<(PaymentState, PaymentTrigger), PaymentState>
        {
            { (PaymentState.WaitingForPayment, PaymentTrigger.Pay),    PaymentState.Success },
            { (PaymentState.WaitingForPayment, PaymentTrigger.Cancel), PaymentState.Cancelled },
        };

        public PaymentService(AppDbContext context, OrderService orderService)
        {
            dbContext = context;

            var stockObserver = new StockObserver(dbContext);
            this.Attach(stockObserver);
            this.Attach(orderService);
        }

        public void Attach(IPaymentObserver observer)
        {
            this._observers.Add(observer);
        }

        public void Detach(IPaymentObserver observer)
        {
            this._observers.Remove(observer);
        }

        public async Task NotifyPaymentSuccess(Transaction transaction)
        {
            foreach (var observer in _observers)
            {
                await observer.OnPaymentSuccess(transaction);
            }
        }

        public async Task NotifyPaymentCancelled(Transaction transaction)
        {
            foreach (var observer in _observers)
            {
                await observer.OnPaymentCancelled(transaction);
            }
        }

        public virtual async Task<Transaction> ProcessTransaction(TransactionDto dto)
        {
            using var transactionScope = await dbContext.Database.BeginTransactionAsync();
            try
            {
                double totalPrice = 0;
                var transaction = new Transaction
                {
                    Date = dto.Date,
                    PaymentAmount = 0,
                    TotalPrice = 0,
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
                        var eggCategory = await dbContext.EggCategories.FindAsync(d.CategoryId);
                        if (eggCategory == null) throw new Exception($"Kategori telur dengan ID {d.CategoryId} tidak ditemukan.");

                        double priceAtPurchase = eggCategory.Price;
                        double subtotal = (double)d.Quantity * priceAtPurchase;
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

        public virtual async Task<bool> UpdateStatus(
            int id,
            PaymentTrigger paymentTrigger,
            PaymentDto? paymentDto = null)
        {
            using var dbTransaction = await dbContext.Database.BeginTransactionAsync();

            try
            {
                var transaksi = await dbContext.Transactions
                    .Include(t => t.Details)
                    .FirstOrDefaultAsync(t => t.Id == id);

                if (transaksi == null) return false;

                if (_transitions.TryGetValue((transaksi.PaymentStatus, paymentTrigger), out PaymentState nextPaymentState))
                {
                    if (paymentTrigger == PaymentTrigger.Pay)
                    {
                        if (paymentDto == null || paymentDto.PaymentAmount <= 0)
                            throw new Exception("Jumlah pembayaran (Payment Amount) tidak valid atau tidak dikirim.");

                        if (paymentDto.PaymentAmount < transaksi.TotalPrice)
                            throw new Exception($"Uang yang dibayarkan (Rp {paymentDto.PaymentAmount}) kurang dari total tagihan (Rp {transaksi.TotalPrice}).");

                        transaksi.PaymentAmount = paymentDto.PaymentAmount;
                    }

                    transaksi.PaymentStatus = nextPaymentState;

                    if (nextPaymentState == PaymentState.Success)
                    {
                        await NotifyPaymentSuccess(transaksi);
                    }
                    else if (nextPaymentState == PaymentState.Cancelled)
                    {
                        await NotifyPaymentCancelled(transaksi);
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