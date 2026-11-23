
using DeliveryPlatform.Service.DTOs.Customers;
using DeliveryPlatform.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace DeliveryPlatform.Api.Controllers
{
    [ApiController]
    [Route("api/v1/customers")]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        private readonly IAuditHistoryService _auditHistoryService;

        public CustomersController(
            ICustomerService customerService,
            IAuditHistoryService auditHistoryService)
        {
            _customerService = customerService;
            _auditHistoryService = auditHistoryService;
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

        [Authorize(Roles = "Admin")]
        [HttpGet("{id:int}/history")]
        public async Task<IActionResult> GetCustomerHistory(
            int id,
            [FromQuery, Range(1, int.MaxValue)] int page = 1,
            [FromQuery, Range(1, 100)] int pageSize = 10)
        {
            var history = await _auditHistoryService
                .GetCustomerHistoryAsync(id, page, pageSize);

            return Ok(history);
        }
    }
}
