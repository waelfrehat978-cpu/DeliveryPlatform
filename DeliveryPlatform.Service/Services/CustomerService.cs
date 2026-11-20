
using DeliveryPlatform.Data.Context;
using DeliveryPlatform.Data.Entities;
using DeliveryPlatform.Service.DTOs.Customers;
using DeliveryPlatform.Service.Interfaces;
using DeliveryPlatform.Service.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace DeliveryPlatform.Service.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly DeliveryPlatformDbContext _context;
        private readonly IResourceAuthorizationService _resourceAuthorization;

        public CustomerService(
            DeliveryPlatformDbContext context,
            IResourceAuthorizationService resourceAuthorization)
        {
            _context = context;
            _resourceAuthorization = resourceAuthorization;
        }

        // Create Customer
        public async Task<CustomerDto> CreateCustomerAsync(
            CreateCustomerDto customerDto)
        {
            if (!string.IsNullOrWhiteSpace(customerDto.Email))
            {
                bool emailExists = await _context.Customers
                    .AnyAsync(c => c.Email == customerDto.Email);

                if (emailExists)
                    throw new ConflictException(
                        "Email is already in use.");
            }

            var customer = new Customer
            {
                FirstName = customerDto.FirstName,
                LastName = customerDto.LastName,
                PhoneNumber = customerDto.PhoneNumber,
                Email = customerDto.Email
            };

            _context.Customers.Add(customer);

            await _context.SaveChangesAsync();

            return new CustomerDto
            {
                Id = customer.Id,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                PhoneNumber = customer.PhoneNumber,
                Email = customer.Email
            };
        }

        // Get Customer By Id
        public async Task<CustomerDto> GetCustomerByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new ValidationException(
                    "id",
                    "Customer ID must be greater than 0.");
            }

            await _resourceAuthorization.EnsureCustomerAccessAsync(id);

            var customer = await _context.Customers
                .AsNoTracking()
                .Where(c => c.Id == id)
                .Select(c => new CustomerDto
                {
                    Id = c.Id,
                    FirstName = c.FirstName,
                    LastName = c.LastName,
                    PhoneNumber = c.PhoneNumber,
                    Email = c.Email
                })
                .FirstOrDefaultAsync();

            if (customer is null)
                throw new NotFoundException("Customer not found.");

            return customer;
        }

        // Update Customer
        public async Task<CustomerDto> UpdateCustomerAsync(
            int id,
            UpdateCustomerDto customerDto)
        {
            if (id <= 0)
            {
                throw new ValidationException(
                    "id",
                    "Customer ID must be greater than 0.");
            }

            await _resourceAuthorization.EnsureCustomerAccessAsync(id);

            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.Id == id);

            if (customer is null)
                throw new NotFoundException("Customer not found.");

            if (!string.IsNullOrWhiteSpace(customerDto.Email))
            {
                bool emailExists = await _context.Customers
                    .AnyAsync(c => c.Email == customerDto.Email
                                && c.Id != id);

                if (emailExists)
                    throw new ConflictException(
                        "Email is already in use.");
            }

            customer.FirstName = customerDto.FirstName;
            customer.LastName = customerDto.LastName;
            customer.PhoneNumber = customerDto.PhoneNumber;
            customer.Email = customerDto.Email;

            await _context.SaveChangesAsync();

            return new CustomerDto
            {
                Id = customer.Id,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                PhoneNumber = customer.PhoneNumber,
                Email = customer.Email
            };
        }
    }
}
