using DeliveryPlatform.Service.DTOs;
using DeliveryPlatform.Service.DTOs.Orders;

namespace DeliveryPlatform.Service.Interfaces
{
    public interface IOrderService
    {
        Task<OrderResponseDto> CreateOrderAsync(
            CreateOrderDto request);

        Task<OrderDetailsDto> GetOrderByIdAsync(
            int orderId);

        Task<PagedResultDto<CustomerOrderSummaryDto>> GetCustomerOrdersAsync(
     int customerId,
     int page,
     int pageSize);
        Task<OrderResponseDto> UpdateOrderStatusAsync(
            int orderId,
            UpdateOrderStatusDto request);
    }
}