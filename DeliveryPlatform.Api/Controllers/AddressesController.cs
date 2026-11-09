using DeliveryPlatform.Service.DTOs.Addresses;
using DeliveryPlatform.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DeliveryPlatform.Api.Controllers
{
    [ApiController]
    [Route("api/v1/addresses")]
    public class AddressesController : ControllerBase
    {
        private readonly IAddressService _addressService;

        public AddressesController(IAddressService addressService)
        {
            _addressService = addressService;
        }

        [HttpPost]
        public async Task<ActionResult<AddressResponseDto>> AddAddress(
            CreateAddressRequestDto request)
        {
            var result = await _addressService.AddAddressAsync(request);

            return CreatedAtAction(
                nameof(GetCustomerAddresses),
                new { customerId = result.CustomerId },
                result);
        }

        [HttpGet("customer/{customerId:int}")]
        public async Task<ActionResult<List<AddressResponseDto>>> GetCustomerAddresses(
            int customerId)
        {
            var result = await _addressService.GetCustomerAddressesAsync(customerId);

            return Ok(result);
        }
    }
}