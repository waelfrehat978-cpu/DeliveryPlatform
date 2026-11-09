using DeliveryPlatform.Service.DTOs.Addresses;
using DeliveryPlatform.Data.Context;
using DeliveryPlatform.Data.Entities;
using DeliveryPlatform.Service.Interfaces;
using Microsoft.EntityFrameworkCore;
using DeliveryPlatform.Service.Exceptions;
namespace DeliveryPlatform.Service.Services
{
    public class AddressService : IAddressService
    {
        private readonly DeliveryPlatformDbContext _context;

        public AddressService(DeliveryPlatformDbContext context)
        {
            _context = context;
        }

        public async Task<AddressResponseDto> AddAddressAsync(
            CreateAddressRequestDto request)
        {
            var customerExists = await _context.Customers
                .AnyAsync(c => c.Id == request.CustomerId);

            if (!customerExists)
            {
                throw new NotFoundException("Customer not found.");
            }

            var addresses = await _context.Addresses
                .Where(a => a.CustomerId == request.CustomerId)
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
                CustomerId = request.CustomerId,
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
                throw new ValidationException(
                    "Customer ID must be greater than 0.");
            var customerExists = await _context.Customers
                .AnyAsync(c => c.Id == customerId);

            if (!customerExists)
            {
                throw new NotFoundException("Customer not found.");
            }

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