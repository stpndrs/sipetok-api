using sipetok_api.Data;
using sipetok_api.Models;
using System;
using System.Threading.Tasks;

namespace sipetok_api.Observers
{
    public class StockObserver : IPaymentObserver
    {
        private readonly AppDbContext _dbContext;

        public StockObserver(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task OnPaymentSuccess(Transaction transaction)
        {
            foreach (var detail in transaction.Details)
            {
                var eggCategory = await _dbContext.EggCategories.FindAsync(detail.CategoryId);

                if (eggCategory == null)
                {
                    throw new Exception($"Kategori telur dengan ID {detail.CategoryId} tidak ditemukan.");
                }

                if (eggCategory.TotalEgg < detail.Quantity)
                {
                    throw new Exception($"Stok telur {eggCategory.Name} tidak mencukupi! Total stok tersedia: {eggCategory.TotalEgg}, jumlah dibeli: {detail.Quantity}");
                }

                eggCategory.TotalEgg -= detail.Quantity;

                _dbContext.EggCategories.Update(eggCategory);
            }

            await _dbContext.SaveChangesAsync();
        }

        public Task OnPaymentCancelled(Transaction transaction)
        {
            return Task.CompletedTask;
        }
    }
}