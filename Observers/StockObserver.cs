using Microsoft.EntityFrameworkCore;
using sipetok_api.Data;
using sipetok_api.Models;
using System;
using System.Linq;
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
                var availableEggs = await _dbContext.EggInventories
                    .Where(e => e.CategoryId == detail.CategoryId && e.Stock > 0)
                    .OrderBy(e => e.ProductionDate)
                    .ToListAsync();

                double totalAvailableStock = availableEggs.Sum(e => e.Stock);

                if (totalAvailableStock < detail.Quantity)
                {
                    throw new Exception($"Stok telur tidak mencukupi! Total stok tersedia: {totalAvailableStock}, jumlah dibeli: {detail.Quantity}");
                }

                var eggCategory = await _dbContext.EggCategories.FindAsync(detail.CategoryId);
                if (eggCategory != null)
                {
                    eggCategory.TotalEgg -= detail.Quantity;

                    if (eggCategory.TotalEgg < 0) eggCategory.TotalEgg = 0;

                    _dbContext.EggCategories.Update(eggCategory);
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

                    _dbContext.EggInventories.Update(eggData);
                }
            }

            await _dbContext.SaveChangesAsync();
        }

        public Task OnPaymentCancelled(Transaction transaction)
        {
            return Task.CompletedTask;
        }
    }
}