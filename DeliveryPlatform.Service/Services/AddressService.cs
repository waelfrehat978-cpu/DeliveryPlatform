
using DeliveryPlatform.Service.DTOs.Addresses;
using DeliveryPlatform.Data.Context;
using DeliveryPlatform.Data.Entities;
using DeliveryPlatform.Data.Enums;
using DeliveryPlatform.Data.Interfaces;
using DeliveryPlatform.Service.Interfaces;
using Microsoft.EntityFrameworkCore;
using DeliveryPlatform.Service.Exceptions;

namespace DeliveryPlatform.Service.Services
{
    public class AddressService : IAddressService
    {
        private readonly DeliveryPlatformDbContext _context;
        private readonly IResourceAuthorizationService _resourceAuthorization;
        private readonly ICurrentUser _currentUser;

        public AddressService(
            DeliveryPlatformDbContext context,
            IResourceAuthorizationService resourceAuthorization,
            ICurrentUser currentUser)
        {
            _context = context;
            _resourceAuthorization = resourceAuthorization;
            _currentUser = currentUser;
        }

        public async Task<AddressResponseDto> AddAddressAsync(
            CreateAddressRequestDto request)
        {
            int customerId;

            if (_currentUser.Role == UserRole.Customer)
            {
                if (!_currentUser.CustomerId.HasValue)
                {
                    throw new ForbiddenException(
                        "You do not have access to this customer.");
                }

                customerId = _currentUser.CustomerId.Value;
            }
            else if (_currentUser.Role == UserRole.Admin)
            {
                customerId = request.CustomerId;
            }
            else
            {
                throw new ForbiddenException(
                    "You are not allowed to add addresses.");
            }

            await _resourceAuthorization.EnsureCustomerAccessAsync(
                customerId);

            var addresses = await _context.Addresses
                .Where(a => a.CustomerId == customerId)
                .ToListAsync();

            bool isFirstAddress = !addresses.Any();

            if (isFirstAddress || request.IsDefault)
            {
                foreach (var existingAddress in addresses)
                {
                    existingAddress.IsDefault = false;
                }

                request.IsDefault = true;
            }

            var address = new Address
            {
                CustomerId = customerId,
                Label = request.Label,
                Street = request.Street,
                City = request.City,
                Latitude = request.Latitude,
                Longitude = request.Longitude,
                IsDefault = request.IsDefault
            };

            _context.Addresses.Add(address);

            await _context.SaveChangesAsync();

            return new AddressResponseDto
            {
                Id = address.Id,
                CustomerId = address.CustomerId,
                Label = address.Label,
                Street = address.Street,
                City = address.City,
                Latitude = address.Latitude,
                Longitude = address.Longitude,
                IsDefault = address.IsDefault
            };
        }

        public async Task<List<AddressResponseDto>> GetCustomerAddressesAsync(
            int customerId)
        {
            if (customerId <= 0)
            {
                throw new ValidationException(
                    "customerId",
                    "Customer ID must be greater than 0.");
            }

            await _resourceAuthorization.EnsureCustomerAccessAsync(
                customerId);

            return await _context.Addresses
                .Where(a => a.CustomerId == customerId)
                .OrderByDescending(a => a.IsDefault)
                .Select(a => new AddressResponseDto
                {
                    Id = a.Id,
                    CustomerId = a.CustomerId,
                    Label = a.Label,
                    Street = a.Street,
                    City = a.City,
                    Latitude = a.Latitude,
                    Longitude = a.Longitude,
                    IsDefault = a.IsDefault
                })
                .ToListAsync();
        }
    }
}
