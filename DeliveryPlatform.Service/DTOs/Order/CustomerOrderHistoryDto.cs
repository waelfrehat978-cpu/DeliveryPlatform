namespace DeliveryPlatform.Service.DTOs.Orders
{
    public class CustomerOrderHistoryDto
    {
        public List<OrderResponseDto> Items { get; set; } = new();

        public int TotalCount { get; set; }

        public int Page { get; set; }

        public int PageSize { get; set; }
    }
}