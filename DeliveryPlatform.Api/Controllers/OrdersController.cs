using DeliveryPlatform.Service.DTOs.Orders;
using DeliveryPlatform.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DeliveryPlatform.Api.Controllers
{
    [ApiController]
    [Route("api/v1/orders")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

     
        [HttpPost]
        public async Task<IActionResult> CreateOrder(
            [FromBody] CreateOrderDto request)
        {
            var result = await _orderService.CreateOrderAsync(request);

            return CreatedAtAction(
                nameof(GetOrderById),
                new { orderId = result.Id },
                result);
        }

     
        [HttpGet("{orderId:int}")]
        public async Task<IActionResult> GetOrderById(int orderId)
        {
            var result = await _orderService.GetOrderByIdAsync(orderId);

            return Ok(result);
        }

    
        [HttpGet("customer/{customerId:int}")]
        public async Task<IActionResult> GetCustomerOrders(
            int customerId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var result = await _orderService.GetCustomerOrdersAsync(
                customerId,
                page,
                pageSize);

            return Ok(result);
        }

        [HttpPut("{orderId:int}/status")]
        public async Task<IActionResult> UpdateOrderStatus(
            int orderId,
            [FromBody] UpdateOrderStatusDto request)
        {
            var result = await _orderService.UpdateOrderStatusAsync(
                orderId,
                request);

            return Ok(result);
        }
    }
}



 