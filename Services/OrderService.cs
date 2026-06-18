using sipetok_api.Models;
using sipetok_api.Utils;
using sipetok_api.Observers;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace sipetok_api.Services
{
    public class OrderService : IPaymentObserver
    {
        private readonly Dictionary<(OrderState, OrderTrigger), OrderState> _transitions =
            new Dictionary<(OrderState, OrderTrigger), OrderState>
        {
            { (OrderState.OrderComeIn, OrderTrigger.PaymentSucceeded), OrderState.ReadyForPickup },
            { (OrderState.ReadyForPickup, OrderTrigger.PickedUp), OrderState.Completed },
            { (OrderState.OrderComeIn, OrderTrigger.CancelledByCustomer), OrderState.Cancelled }
        };

        public Task OnPaymentSuccess(Transaction transaction)
        {
            UpdateOrderStatus(transaction, OrderTrigger.PaymentSucceeded);
            return Task.CompletedTask;
        }

        public Task OnPaymentCancelled(Transaction transaction)
        {
            UpdateOrderStatus(transaction, OrderTrigger.CancelledByCustomer);
            return Task.CompletedTask;
        }

        public bool UpdateOrderStatus(Transaction transaction, OrderTrigger trigger)
        {
            if (transaction == null) return false;

            if (transaction.OrderStatus == OrderState.OrderComeIn &&
                trigger == OrderTrigger.PaymentSucceeded &&
                transaction.PaymentStatus != PaymentState.Success)
            {
                return false;
            }

            if (_transitions.TryGetValue((transaction.OrderStatus, trigger), out OrderState nextOrderStatus))
            {
                transaction.OrderStatus = nextOrderStatus;
                return true;
            }

            return false;
        }
    }
}