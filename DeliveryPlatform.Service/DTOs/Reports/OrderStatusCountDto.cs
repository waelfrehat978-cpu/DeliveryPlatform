using DeliveryPlatform.Data.Enums;

namespace DeliveryPlatform.Service.DTOs.Reports
{
    public class OrderStatusCountDto
    {
        public OrderStatus Status { get; set; }

        public int Count { get; set; }
    }
}