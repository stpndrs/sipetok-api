using sipetok_api.Utils;

namespace sipetok_api.Services
{
    public class OrderService
    {
        private readonly Dictionary<(OrderState, OrderTrigger), OrderState> transitions =
            new Dictionary<(OrderState, OrderTrigger), OrderState>
        {
            { (OrderState.WaitingForPayment, OrderTrigger.PaymentSucceeded), OrderState.ReadyForPickup },
            { (OrderState.ReadyForPickup, OrderTrigger.PickedUp), OrderState.Completed },
            { (OrderState.WaitingForPayment, OrderTrigger.CancelledByCustomer), OrderState.Cancelled }
        };

        public bool UpdateOrderStatus(OrderState currentOrderState, OrderTrigger trigger, PaymentState currentPaymentState, out OrderState nextOrderState)
        {
            nextOrderState = currentOrderState;

            if (currentOrderState == OrderState.WaitingForPayment &&
                trigger == OrderTrigger.PaymentSucceeded &&
                currentPaymentState != PaymentState.Success)
            {
                return false;
            }

            if (transitions.TryGetValue((currentOrderState, trigger), out OrderState result))
            {
                nextOrderState = result;
                return true;
            }

            return false;
        }
    }
}
