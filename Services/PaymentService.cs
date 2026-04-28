using Microsoft.EntityFrameworkCore;
using sipetok_api.Data;
using sipetok_api.Utilis;
using sipetok_api.Models;
using System;
using System.Threading.Tasks;

namespace sipetok_api.service
{
    public class PaymentService
    {
        private readonly AppDbContext dbContext;

        public PaymentService(AppDbContext context)
        {
            dbContext = context;
        }

        public async Task<bool> UpdateStatus(int id)
        {
            var transaksi = await dbContext.Transactions.FirstOrDefaultAsync(t => t.id == id);

            if (transaksi == null) return false;

            // Update field Status 
            if (transaksi.Status == PaymentStatus.Pending)
            {
                transaksi.Status = PaymentStatus.Success;

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