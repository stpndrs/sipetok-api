namespace sipetok_api.Utils
{
    public enum OrderState
    {
        OrderComeIn,
        ReadyForPickup,
        Completed,
        Cancelled
    }

    public enum OrderTrigger
    {
        PaymentSucceeded,
        PickedUp,
        CancelledByCustomer
    }
}
