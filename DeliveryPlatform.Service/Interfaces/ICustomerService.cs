
using DeliveryPlatform.Service.DTOs.Customers;

namespace DeliveryPlatform.Service.Interfaces
{
    public interface ICustomerService
    {
        Task<CustomerDto> GetCustomerByIdAsync(int id);

        Task<CustomerDto> UpdateCustomerAsync(
            int id,
            UpdateCustomerDto customerDto);
    }
}
