using DeliveryPlatform.Data.Enums;

namespace DeliveryPlatform.Service.DTOs.Orders
{
    public class OrderStatusHistoryDto
    {
        public OrderStatus? OldStatus { get; set; }

        public OrderStatus Status { get; set; }

        public DateTime ChangedAtUtc { get; set; }

        public string? Note { get; set; }
    }
}