
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using DeliveryPlatform.Data.Context;
using DeliveryPlatform.Data.Entities;
using DeliveryPlatform.Data.Enums;
using DeliveryPlatform.Service.Exceptions;
using DeliveryPlatform.Service.Interfaces;
using DeliveryPlatform.Service.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ValidationException =
    DeliveryPlatform.Service.Exceptions.ValidationException;
namespace DeliveryPlatform.Service.Services;

public class OtpService : IOtpService
{
    private readonly DeliveryPlatformDbContext _context;
    private readonly ISmsSender _smsSender;
    private readonly OtpOptions _otpOptions;

    public OtpService(
        DeliveryPlatformDbContext context,
        ISmsSender smsSender,
        IOptions<OtpOptions> otpOptions)
    {
        _context = context;
        _smsSender = smsSender;
        _otpOptions = otpOptions.Value;
    }

    // 1. Request OTP
    public async Task RequestCodeAsync(string phoneNumber)
    {
        if (!IsValidPhoneNumber(phoneNumber))
        {
            throw new ValidationException(
                "PhoneNumber",
                "Phone number must contain 10 to 15 digits.");
        }

        var now = DateTime.UtcNow;
         
        // Check the waiting period.
        var hasRecentRequest = await _context.OtpCodes
            .AnyAsync(o =>
                o.PhoneNumber == phoneNumber &&
                o.CreatedAtUtc >= now.AddSeconds(
                    -_otpOptions.SecondsBetweenRequests));

        if (hasRecentRequest)
        {
            throw new TooManyRequestsException(
                "Please wait before requesting another code.");
        }

        // Check the hourly request limit.
        var requestsCount = await _context.OtpCodes
            .CountAsync(o =>
                o.PhoneNumber == phoneNumber &&
                o.CreatedAtUtc >= now.AddHours(-1));

        if (requestsCount >= _otpOptions.MaxRequestsPerHour)
        {
            throw new TooManyRequestsException(
                "Maximum OTP requests per hour exceeded.");
        }

        // Generate a secure 6-digit code.
        var code = RandomNumberGenerator
            .GetInt32(0, 1_000_000)
            .ToString("D6");

        var otp = new OtpCode
        {
            PhoneNumber = phoneNumber,
            Code = code,
            ExpiresAtUtc = now.AddMinutes(
                _otpOptions.ExpiryMinutes),
            WrongAttempts = 0,
            IsUsed = false
        };

        _context.OtpCodes.Add(otp);

        await _context.SaveChangesAsync();

        // Send OTP through SMS sender.
        await _smsSender.SendAsync(
            phoneNumber,
            $"Your verification code is: {code}");
    }

    // 2. Verify OTP
    public async Task<User?> VerifyCodeAsync(
        string phoneNumber,
        string code)
    {
        var otp = await _context.OtpCodes
            .Where(o => o.PhoneNumber == phoneNumber)
            .OrderByDescending(o => o.CreatedAtUtc)
            .FirstOrDefaultAsync();

        if (otp == null ||
            otp.IsUsed ||
            otp.ExpiresAtUtc <= DateTime.UtcNow ||
            otp.WrongAttempts >= _otpOptions.MaxWrongTries)
        {
            throw new UnauthorizedException(
                "Invalid or expired code.");
        }

        if (string.IsNullOrWhiteSpace(code) ||
            code.Length != 6 ||
            !code.All(char.IsAsciiDigit) ||
            otp.Code != code)
        {
            otp.WrongAttempts++;

            await _context.SaveChangesAsync();

            throw new UnauthorizedException(
                "Invalid or expired code.");
        }

        otp.IsUsed = true;
        otp.VerifiedAtUtc = DateTime.UtcNow;

        var user = await _context.Users
            .FirstOrDefaultAsync(u =>
                u.PhoneNumber == phoneNumber);

        await _context.SaveChangesAsync();

        return user;
    }

    // 3. Complete Customer Signup
    public async Task<User> CompleteSignupAsync(
        string phoneNumber,
        string firstName,
        string lastName,
        string email)
    {
        if (!IsValidPhoneNumber(phoneNumber))
        {
            throw new ValidationException(
                "PhoneNumber",
                "Phone number must contain 10 to 15 digits.");
        }

        firstName = firstName?.Trim() ?? string.Empty;
        lastName = lastName?.Trim() ?? string.Empty;
        email = email?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(firstName) ||
            firstName.Length > 100)
        {
            throw new ValidationException(
                "FirstName",
                "First name is required and must not exceed 100 characters.");
        }

        if (string.IsNullOrWhiteSpace(lastName) ||
            lastName.Length > 100)
        {
            throw new ValidationException(
                "LastName",
                "Last name is required and must not exceed 100 characters.");
        }
        if (string.IsNullOrWhiteSpace(email) ||
            email.Length > 254 ||
            !System.Net.Mail.MailAddress.TryCreate(email, out _))
        {
            throw new ValidationException(
                "Email",
                "A valid email address is required.");
        }

        await using var transaction =
            await _context.Database.BeginTransactionAsync();

        // Find the latest successfully verified OTP.
        var otp = await _context.OtpCodes
            .Where(o =>
                o.PhoneNumber == phoneNumber &&
                o.VerifiedAtUtc != null)
            .OrderByDescending(o => o.CreatedAtUtc)
            .FirstOrDefaultAsync();

        if (otp == null ||
            !otp.IsUsed ||
            otp.IsSignupCompleted ||
            otp.VerifiedAtUtc < DateTime.UtcNow.AddMinutes(
                -_otpOptions.ExpiryMinutes))
        {
            throw new UnauthorizedException(
                "Phone verification is required.");
        }

        // Prevent creating another account for the same phone.
        var phoneAlreadyRegistered = await _context.Users
            .AnyAsync(u => u.PhoneNumber == phoneNumber);

        if (phoneAlreadyRegistered)
        {
            throw new ConflictException(
                "This phone number is already registered.");
        }

        // Prevent duplicate customer email.
        var emailAlreadyRegistered = await _context.Customers
            .AnyAsync(c => c.Email == email);

        if (emailAlreadyRegistered)
        {
            throw new ConflictException(
                "This email address is already registered.");
        }

        // Create Customer.
        var customer = new Customer
        {
            FirstName = firstName,
            LastName = lastName,
            PhoneNumber = phoneNumber,
            Email = email
        };

        _context.Customers.Add(customer);

        await _context.SaveChangesAsync();

        // Create User linked to Customer.
        var user = new User
        {
            PhoneNumber = phoneNumber,
            Email = null,
            PasswordHash = null,
            Role = UserRole.Customer,
            CustomerId = customer.Id,
            StoreId = null
        };

        _context.Users.Add(user);

        // Mark signup as completed.
        otp.IsSignupCompleted = true;

        await _context.SaveChangesAsync();

        await transaction.CommitAsync();

        return user;
    }

    // Validate phone number.
    private static bool IsValidPhoneNumber(string phoneNumber)
    {
        return !string.IsNullOrWhiteSpace(phoneNumber) &&
               phoneNumber.Length >= 10 &&
               phoneNumber.Length <= 15 &&
               phoneNumber.All(char.IsAsciiDigit);
    }
}
