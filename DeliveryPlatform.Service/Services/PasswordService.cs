using DeliveryPlatform.Data.Entities;
using Microsoft.AspNetCore.Identity;

namespace DeliveryPlatform.Service.Services;

public class PasswordService
{
    private readonly IPasswordHasher<User> _passwordHasher;

    public PasswordService(IPasswordHasher<User> passwordHasher)
    {
        _passwordHasher = passwordHasher;
    }

    public string HashPassword(User user, string password)
    {
        return _passwordHasher.HashPassword(user, password);
    }

    public bool VerifyPassword(User user, string password)
    {
        var result = _passwordHasher.VerifyHashedPassword(
            user,
            user.PasswordHash!,
            password);

        return result != PasswordVerificationResult.Failed;
    }
}
//الوظيف من  الكلاس تشفير كلمه المرور