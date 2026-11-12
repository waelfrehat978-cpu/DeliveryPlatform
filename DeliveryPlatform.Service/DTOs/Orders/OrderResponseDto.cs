using DeliveryPlatform.Data.Enums;

namespace DeliveryPlatform.Service.DTOs.Orders
{
    public class OrderResponseDto
    {
        public int Id { get; set; }

        public string OrderCode { get; set; } = string.Empty;

        public int CustomerId { get; set; }

        public int StoreId { get; set; }

        public int AddressId { get; set; }

        public OrderStatus Status { get; set; }

        public decimal Subtotal { get; set; }

        public decimal DeliveryFee { get; set; }

        public decimal Total { get; set; }

        public PaymentMethod PaymentMethod { get; set; }

        public List<OrderLineResponseDto> Items { get; set; } = new();
    }
}