using DeliveryPlatform.Service.DTOs.Orders;

namespace DeliveryPlatform.Service.Interfaces
{
    public interface IOrderService
    {
        Task<OrderResponseDto> CreateOrderAsync(
            CreateOrderDto request);

        Task<OrderResponseDto> GetOrderByIdAsync(
            int orderId);

        Task<CustomerOrderHistoryDto> GetCustomerOrdersAsync(
            int customerId,
            int page,
            int pageSize);

        Task<OrderResponseDto> UpdateOrderStatusAsync(
            int orderId,
            UpdateOrderStatusDto request);
    }
}