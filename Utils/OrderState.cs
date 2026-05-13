namespace sipetok_api.Utils
{
    public enum OrderState
    {
        WaitingForPayment,
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
