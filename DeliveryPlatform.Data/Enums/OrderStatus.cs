namespace DeliveryPlatform.Data.Enums
{
    public enum OrderStatus
    {
        Pending = 0,
        Accepted = 1,
        Preparing = 2,
        OutForDelivery = 3,
        Delivered = 4,
        Declined = 5,
        Cancelled = 6
    }
}