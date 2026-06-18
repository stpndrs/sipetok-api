using Microsoft.EntityFrameworkCore;
using sipetok_api.Data;
using sipetok_api.Models;
using sipetok_api.Utils;
using sipetok_api.Observers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using sipetok_api.dto.Request;

namespace sipetok_api.Services
{
    public class PaymentService : IPaymentSubject
    {
        private readonly AppDbContext _dbContext;
        private readonly List<IPaymentObserver> _observers = new(); // C# Modern shorthand instantiation

        private static readonly Dictionary<(PaymentState, PaymentTrigger), PaymentState> _transitions =
            new Dictionary<(PaymentState, PaymentTrigger), PaymentState>
        {
            { (PaymentState.WaitingForPayment, PaymentTrigger.Pay),    PaymentState.Success },
            { (PaymentState.WaitingForPayment, PaymentTrigger.Cancel), PaymentState.Cancelled },
        };

        public PaymentService(AppDbContext context, OrderService orderService)
        {
            _dbContext = context;

            // Otomatis daftarkan stockObserver dan orderService sebagai pendengar perubahan status
            this.Attach(new StockObserver(_dbContext));
            this.Attach(orderService);
        }

        public void Attach(IPaymentObserver observer)
        {
            _observers.Add(observer);
        }

        public void Detach(IPaymentObserver observer)
        {
            _observers.Remove(observer);
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

        public virtual async Task<bool> UpdateStatus(int id, PaymentTrigger paymentTrigger, PaymentRequestDto? paymentDto = null)
        {
            using var dbTransaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                var transaksi = await _dbContext.Transactions
                    .Include(t => t.Details)
                    .FirstOrDefaultAsync(t => t.Id == id);

                if (transaksi == null) return false;

                // Cek apakah transisi state pembayaran terdaftar di State Machine
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

                    // Menggunakan Pola Observer untuk menyiarkan perubahan data ke modul stok & pemesanan secara terpusat (DRY)
                    if (nextPaymentState == PaymentState.Success)
                    {
                        await NotifyPaymentSuccess(transaksi);
                    }
                    else if (nextPaymentState == PaymentState.Cancelled)
                    {
                        await NotifyPaymentCancelled(transaksi);
                    }

                    await _dbContext.SaveChangesAsync();
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