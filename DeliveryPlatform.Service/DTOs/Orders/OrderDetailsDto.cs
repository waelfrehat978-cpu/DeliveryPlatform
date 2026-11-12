using DeliveryPlatform.Data.Enums;
using DeliveryPlatform.Service.DTOs.Addresses;

namespace DeliveryPlatform.Service.DTOs.Orders
{
    public class OrderDetailsDto
    {
        public int Id { get; set; }

        public string OrderCode { get; set; } = string.Empty;

        public DateTime CreatedAtUtc { get; set; }

        public OrderStatus Status { get; set; }

        public PaymentMethod PaymentMethod { get; set; }

        public decimal Subtotal { get; set; }

        public decimal DeliveryFee { get; set; }

        public decimal Total { get; set; }

        public int CustomerId { get; set; }

        public string CustomerName { get; set; } = string.Empty;

        public int StoreId { get; set; }

        public string StoreName { get; set; } = string.Empty;

        public AddressResponseDto Address { get; set; } = new();

        public List<OrderLineResponseDto> Lines { get; set; } = new();

        public List<OrderStatusHistoryDto> StatusHistory { get; set; } = new();
    }
}