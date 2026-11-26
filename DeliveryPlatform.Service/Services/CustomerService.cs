
using DeliveryPlatform.Data.Auditing;
using DeliveryPlatform.Data.Context;
using DeliveryPlatform.Service.DTOs.Customers;
using DeliveryPlatform.Service.Exceptions;
using DeliveryPlatform.Service.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DeliveryPlatform.Service.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly IAuditScope _auditScope;
        private readonly DeliveryPlatformDbContext _context;
        private readonly IResourceAuthorizationService _resourceAuthorization;

        public CustomerService(
            DeliveryPlatformDbContext context,
            IResourceAuthorizationService resourceAuthorization,IAuditScope auditScope)
        {
            _context = context;
            _resourceAuthorization = resourceAuthorization;
            _auditScope= auditScope;
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
                    PhoneNumber = _context.Users
                        .Where(u => u.CustomerId == c.Id)
                        .Select(u => u.PhoneNumber)
                        .FirstOrDefault() ?? string.Empty,
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

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.CustomerId == id);

            if (user is null)
                throw new NotFoundException("Customer account not found.");

            if (!string.IsNullOrWhiteSpace(customerDto.Email))
            {
                bool emailExists = await _context.Customers
                    .AnyAsync(c => c.Email == customerDto.Email
                                && c.Id != id);

                if (emailExists)
                    throw new ConflictException(
                        "Email is already in use.");
            }
            using (_auditScope.Enable())
            {
                customer.FirstName = customerDto.FirstName;
                customer.LastName = customerDto.LastName;
                customer.Email = customerDto.Email;
                await _context.SaveChangesAsync();
            }
            return new CustomerDto
            {
                Id = customer.Id,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                PhoneNumber = user.PhoneNumber ?? string.Empty,
                Email = customer.Email
            };
        }
    }
}
