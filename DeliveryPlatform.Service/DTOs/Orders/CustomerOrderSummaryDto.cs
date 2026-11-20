
using DeliveryPlatform.Data.Enums;

namespace DeliveryPlatform.Service.DTOs.Orders
{
    public class CustomerOrderSummaryDto
    {
        public int Id { get; set; }

        public string OrderCode { get; set; } = string.Empty;

        public DateTime CreatedAtUtc { get; set; }

        public OrderStatus Status { get; set; }

        public string StoreName { get; set; } = string.Empty;

        public int LineCount { get; set; }

        public decimal Total { get; set; }
    }
}
