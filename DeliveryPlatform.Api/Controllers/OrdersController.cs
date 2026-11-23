using DeliveryPlatform.Service.DTOs.Orders;
using DeliveryPlatform.Service.Interfaces;
using DeliveryPlatform.Service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
namespace DeliveryPlatform.Api.Controllers
{
    [ApiController]
    [Route("api/v1/orders")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IAuditHistoryService _auditHistoryService;
        public OrdersController(
         IOrderService orderService,
         IAuditHistoryService auditHistoryService)
        {
            _orderService = orderService;
            _auditHistoryService = auditHistoryService;
        }

        [Authorize(Roles = "Customer")]
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

        [Authorize(Roles = "Customer,Partner,Admin")]
        [HttpGet("{orderId:int}")]
        public async Task<IActionResult> GetOrderById(int orderId)
        {
            var result = await _orderService.GetOrderByIdAsync(orderId);

            return Ok(result);
        }


        [Authorize(Roles = "Customer,Admin")]
        [HttpGet]
        public async Task<IActionResult> GetCustomerOrders(
     [FromQuery, Range(1, int.MaxValue)] int customerId,
     [FromQuery, Range(1, int.MaxValue)] int page = 1,
     [FromQuery, Range(1, 100)] int pageSize = 10)
        {
            var result = await _orderService.GetCustomerOrdersAsync(
                customerId,
                page,
                pageSize);

            return Ok(result);
        }

        [Authorize(Roles = "Partner,Admin")]
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
        [Authorize(Roles = "Admin")]
        [HttpGet("{orderId:int}/history")]
        public async Task<IActionResult> GetOrderHistory(
    int orderId,
    [FromQuery, Range(1, int.MaxValue)] int page = 1,
    [FromQuery, Range(1, 100)] int pageSize = 10)
        {
            var history = await _auditHistoryService
                .GetOrderHistoryAsync(orderId, page, pageSize);

            return Ok(history);
        }
    }
}



 