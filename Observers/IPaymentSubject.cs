using System.Threading.Tasks;
using sipetok_api.Models;

namespace sipetok_api.Observers
{
    public interface IPaymentSubject
    {
        void Attach(IPaymentObserver observer);
        void Detach(IPaymentObserver observer);
        Task NotifyPaymentSuccess(Transaction transaction);
        Task NotifyPaymentCancelled(Transaction transaction);
    }
}