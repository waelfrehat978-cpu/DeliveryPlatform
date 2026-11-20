
using DeliveryPlatform.Data.Entities;
using DeliveryPlatform.Service.DTOs.Users;
namespace DeliveryPlatform.Service.Interfaces;

public interface IUserService
{
   

    Task<User> LoginStaffAsync(
        string email,
        string password);

    Task<User> CreatePartnerAsync(CreatePartnerDto request);
}