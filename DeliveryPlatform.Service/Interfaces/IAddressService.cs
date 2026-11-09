using DeliveryPlatform.Service.DTOs.Addresses;

namespace DeliveryPlatform.Service.Interfaces
{
    public interface IAddressService
    {
        Task<AddressResponseDto> AddAddressAsync(
            CreateAddressRequestDto request);

        Task<List<AddressResponseDto>> GetCustomerAddressesAsync(
            int customerId);
    }
}