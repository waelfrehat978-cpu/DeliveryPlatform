
using DeliveryPlatform.Service.DTOs.Customers;
using DeliveryPlatform.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DeliveryPlatform.Api.Controllers
{
    [ApiController]
    [Route("api/v1/customers")]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomersController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateCustomer(
            CreateCustomerDto customerDto)
        {
            var customer = await _customerService
                .CreateCustomerAsync(customerDto);

            return CreatedAtAction(
                nameof(GetCustomerById),
                new { id = customer.Id },
                customer);
        }

        [Authorize(Roles = "Customer,Admin")]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetCustomerById(int id)
        {
            var customer = await _customerService
                .GetCustomerByIdAsync(id);

            return Ok(customer);
        }

        [Authorize(Roles = "Customer,Admin")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateCustomer(
            int id,
            UpdateCustomerDto customerDto)
        {
            var customer = await _customerService
                .UpdateCustomerAsync(id, customerDto);

            return Ok(customer);
        }
    }
}
