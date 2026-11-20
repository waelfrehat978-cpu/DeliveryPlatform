using DeliveryPlatform.Data.Entities;

namespace DeliveryPlatform.Service.Interfaces;

public interface IOtpService
{
    Task RequestCodeAsync(string phoneNumber);

    Task<(User? User, string? SignupToken)> VerifyCodeAsync(
     string phoneNumber,
     string code);

    Task<User> CompleteSignupAsync(
      string signupToken,
      string firstName,
      string lastName,
      string email);
}