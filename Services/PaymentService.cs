using Microsoft.EntityFrameworkCore;
using sipetok_api.Data;
using sipetok_api.Utils;
using sipetok_api.Models;

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