using DeliveryPlatform.Service.DTOs.Reports;

namespace DeliveryPlatform.Service.Interfaces
{
    public interface IReportService
    {
        Task<StoreDashboardDto> GetStoreDashboardAsync(int storeId);
    }
}