using DeliveryPlatform.Service.DTOs.Reports;

namespace DeliveryPlatform.Service.Interfaces
{
    public interface IReportService
    {
        // Today's orders and revenue
        Task<StoreDailyReportDto> GetDailyReportAsync(int storeId);

        // Top 5 selling products in the last 30 days
        Task<List<TopSellingProductDto>> GetTopSellingProductsAsync(
            int storeId);

        // Average order value in the last 30 days
        Task<AverageOrderValueDto> GetAverageOrderValueAsync(
            int storeId);

        // Orders count by status
        Task<List<OrderStatusCountDto>> GetOrderStatusCountsAsync(
            int storeId);
    }
}