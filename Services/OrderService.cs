using sipetok_api.Models;
using sipetok_api.Utils;

namespace sipetok_api.Services
{
    public class OrderService
    {
        private readonly Dictionary<(OrderState, OrderTrigger), OrderState> transitions =
            new Dictionary<(OrderState, OrderTrigger), OrderState>
        {
            { (OrderState.OrderComeIn, OrderTrigger.PaymentSucceeded), OrderState.ReadyForPickup },
            { (OrderState.ReadyForPickup, OrderTrigger.PickedUp), OrderState.Completed },
            { (OrderState.OrderComeIn, OrderTrigger.CancelledByCustomer), OrderState.Cancelled }
        };

        public bool UpdateOrderStatus(Transaction transaction, OrderTrigger trigger)
        {
            try
            {
                if (transaction == null)
                {
                    return false;
                }

                if (transaction.OrderStatus == OrderState.OrderComeIn &&
                    trigger == OrderTrigger.PaymentSucceeded &&
                    transaction.PaymentStatus != PaymentState.Success)
                {
                    return false;
                }

                if (transitions.TryGetValue((transaction.OrderStatus, trigger), out OrderState nextOrderStatus))
                {
                    transaction.OrderStatus = nextOrderStatus;
                    return true;
                }

                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
