using DeliveryPlatform.Service.DTOs.Stores;
using DeliveryPlatform.Service.DTOs;
namespace DeliveryPlatform.Service.Interfaces
{
    public interface IStoreService
    {
        Task<PagedResultDto<StoreDto>> GetStoresAsync(
            string? name,
            int page,
            int pageSize
            );
        Task<StoreDetailsDto>
            GetStoreByIdAsync(int id);
        Task<StoreDetailsDto> CreateStoreAsync(CreateStoreDto request);
        Task UpdateStoreNameAsync(int id, string name);

        Task DeleteStoreAsync(int id);
    }
}
