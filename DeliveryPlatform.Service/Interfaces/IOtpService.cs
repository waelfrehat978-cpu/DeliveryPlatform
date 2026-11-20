using DeliveryPlatform.Data.Entities;

namespace DeliveryPlatform.Service.Interfaces;

public interface IOtpService
{
    Task RequestCodeAsync(string phoneNumber);

    Task<User?> VerifyCodeAsync(string phoneNumber, string code);

    Task<User> CompleteSignupAsync(
        string phoneNumber,
        string firstName,
        string lastName,
        string email);
}