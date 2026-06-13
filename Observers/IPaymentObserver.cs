using System.Threading.Tasks;
using sipetok_api.Models;

namespace sipetok_api.Observers
{
    public interface IPaymentObserver
    {
        Task OnPaymentSuccess(Transaction transaction);
        Task OnPaymentCancelled(Transaction transaction);
    }
}