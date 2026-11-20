using DeliveryPlatform.Data.Context;
using DeliveryPlatform.Data.Interfaces;
using DeliveryPlatform.Data.Enums;
using DeliveryPlatform.Service.Exceptions;
using DeliveryPlatform.Service.Interfaces;
using Microsoft.EntityFrameworkCore;
using DeliveryPlatform.Data.Entities;
namespace DeliveryPlatform.Service.Services;

public class ResourceAuthorizationService : IResourceAuthorizationService
{
    private readonly DeliveryPlatformDbContext _context;
    private readonly ICurrentUser _currentUser;

    public ResourceAuthorizationService(
        DeliveryPlatformDbContext context,
        ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }
    public async Task EnsureStoreAccessAsync(int storeId)
    {
        var storeExists = await _context.Stores
            .AnyAsync(s => s.Id == storeId);

        if (!storeExists)
            throw new NotFoundException("Store not found.");

        if (_currentUser.Role == UserRole.Admin)
            return;

        if (_currentUser.Role != UserRole.Partner ||
            !_currentUser.UserId.HasValue ||
            !_currentUser.StoreId.HasValue)
        {
            throw new ForbiddenException(
                "You do not have access to this store.");
        }

        var currentStoreId = await _context.Users
            .Where(u =>
                u.Id == _currentUser.UserId.Value &&
                u.Role == UserRole.Partner)
            .Select(u => u.StoreId)
            .FirstOrDefaultAsync();

        if (currentStoreId != _currentUser.StoreId ||
            currentStoreId != storeId)
        {
            throw new ForbiddenException(
                "You do not have access to this store.");
        }
    }

    public async Task EnsureProductAccessAsync(int productId)
    {
        var productStoreId = await _context.Products
            .Where(p => p.Id == productId)
            .Select(p => (int?)p.StoreId)
            .FirstOrDefaultAsync();

        if (productStoreId is null)
            throw new NotFoundException("Product not found.");

        await EnsureStoreAccessAsync(productStoreId.Value);
    }

    public async Task EnsureCustomerAccessAsync(int customerId)
    {
        var customerExists = await _context.Customers
            .AnyAsync(c => c.Id == customerId);

        if (!customerExists)
            throw new NotFoundException("Customer not found.");

        if (_currentUser.Role == UserRole.Admin)
            return;

        if (_currentUser.Role != UserRole.Customer ||
            _currentUser.CustomerId != customerId)
        {
            throw new ForbiddenException(
                "You do not have access to this customer.");
        }
    }

    public async Task EnsureOrderAccessAsync(int orderId)
    {
        var order = await _context.Orders
            .Where(o => o.Id == orderId)
            .Select(o => new
            {
                o.CustomerId,
                o.StoreId
            })
            .FirstOrDefaultAsync();

        if (order is null)
            throw new NotFoundException("Order not found.");

        if (_currentUser.Role == UserRole.Admin)
            return;

        if (_currentUser.Role == UserRole.Customer)
        {
            if (_currentUser.CustomerId != order.CustomerId)
            {
                throw new ForbiddenException(
                    "You do not have access to this order.");
            }

            return;
        }

        if (_currentUser.Role == UserRole.Partner)
        {
            await EnsureStoreAccessAsync(order.StoreId);
            return;
        }

        throw new ForbiddenException(
            "You do not have access to this order.");
    }

}