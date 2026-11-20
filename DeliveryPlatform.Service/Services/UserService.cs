using DeliveryPlatform.Data.Context;
using DeliveryPlatform.Data.Entities;
using DeliveryPlatform.Data.Enums;
using DeliveryPlatform.Service.Exceptions;
using DeliveryPlatform.Service.Interfaces;
using Microsoft.EntityFrameworkCore;
using DeliveryPlatform.Service.DTOs.Users;
namespace DeliveryPlatform.Service.Services;

public class UserService : IUserService
{
    private readonly DeliveryPlatformDbContext _context;
    private readonly PasswordService _passwordService;

    public UserService(
        DeliveryPlatformDbContext context,
        PasswordService passwordService)
    {
        _context = context;
        _passwordService = passwordService;
    }

    public async Task<User> CreateStaffUserAsync(
        string email,
        string password,
        int? storeId = null)
    {
        var role = storeId.HasValue
            ? UserRole.Partner
            : UserRole.Admin;

        var user = new User
        {
            Email = email,
            Role = role,
            StoreId = storeId
        };

        user.PasswordHash = _passwordService.HashPassword(
            user,
            password);

        _context.Users.Add(user);

        await _context.SaveChangesAsync();

        return user;
    }

    public async Task<User> LoginStaffAsync(
        string email,
        string password)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email);

        if (user == null ||
            (user.Role != UserRole.Partner &&
             user.Role != UserRole.Admin))
        {
            throw new UnauthorizedException("Invalid credentials.");
        }

        if (!_passwordService.VerifyPassword(user, password))
        {
            throw new UnauthorizedException("Invalid credentials.");
        }

        return user;
    }
    public async Task<User> CreatePartnerAsync(CreatePartnerDto request)
    {
        // Check whether the store exists.
        var storeExists = await _context.Stores
            .AnyAsync(s => s.Id == request.StoreId);

        if (!storeExists)
            throw new KeyNotFoundException("Store not found.");
        var partnerExists = await _context.Users
    .AnyAsync(u => u.StoreId == request.StoreId
                && u.Role == UserRole.Partner);

        if (partnerExists)
            throw new ConflictException(
                "This store already has a Partner.");
        // Check whether the email is already registered.
        var emailExists = await _context.Users
            .AnyAsync(u => u.Email == request.Email);

        if (emailExists)
            throw new ConflictException("Email is already registered.");

        // Create the Partner account.
        var user = new User
        {
            Email = request.Email,
            Role = UserRole.Partner,
            StoreId = request.StoreId
        };

        user.PasswordHash = _passwordService.HashPassword(
            user,
            request.Password);

        _context.Users.Add(user);

        await _context.SaveChangesAsync();

        return user;
    }

}