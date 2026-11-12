using DeliveryPlatform.Data.Enums;

namespace DeliveryPlatform.Data.Entities
{
    public class OrderStatusHistory : BaseEntity
    {
        public int OrderId { get; set; }

        public Order Order { get; set; } = null!;

        public OrderStatus? OldStatus { get; set; }

        public OrderStatus Status { get; set; }

        public string? Note { get; set; }
    }
}