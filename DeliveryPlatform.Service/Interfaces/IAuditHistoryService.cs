using DeliveryPlatform.Service.DTOs;
using DeliveryPlatform.Service.DTOs.AuditHistory;

namespace DeliveryPlatform.Service.Interfaces
{
    public interface IAuditHistoryService
    {
        Task<PagedResultDto<AuditHistoryDto>> GetProductHistoryAsync(
            int productId, int page, int pageSize);

        Task<PagedResultDto<AuditHistoryDto>> GetOrderHistoryAsync(
            int orderId, int page, int pageSize);

        Task<PagedResultDto<AuditHistoryDto>> GetCustomerHistoryAsync(
            int customerId, int page, int pageSize);

        Task<PagedResultDto<AuditHistoryDto>> GetStoreHistoryAsync(
            int storeId, int page, int pageSize);
    }
}/*
  
  الدالة	جدول التدقيق
GetProductHistoryAsync	ProductAuditTrails
GetOrderHistoryAsync	OrderAuditTrails
GetCustomerHistoryAsync	CustomerAuditTrails
GetStoreHistoryAsync	StoreAuditTrails
  
  */