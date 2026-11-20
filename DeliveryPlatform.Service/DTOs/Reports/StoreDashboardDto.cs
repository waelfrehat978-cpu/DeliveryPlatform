
namespace DeliveryPlatform.Service.DTOs.Reports
{
    public class StoreDashboardDto
    {
        public int TodayOrderCount { get; set; }

        public decimal TodayRevenue { get; set; }

        public List<TopSellingProductDto> TopProducts { get; set; } = new();

        public decimal AverageOrderValue { get; set; }

        public List<OrderStatusCountDto> ActiveStatusCounts { get; set; } = new();
    }
}
