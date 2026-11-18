
using DeliveryPlatform.Data.Entities;
using DeliveryPlatform.Service.DTOs.Users;
namespace DeliveryPlatform.Service.Interfaces;

public interface IUserService
{
    Task<User> CreateStaffUserAsync(
        string email,
        string password,
        int? storeId = null);

    Task<User> LoginStaffAsync(
        string email,
        string password);

    Task<User> CreatePartnerAsync(CreatePartnerDto request);
}