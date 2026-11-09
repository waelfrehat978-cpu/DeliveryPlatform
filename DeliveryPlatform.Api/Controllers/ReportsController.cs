using DeliveryPlatform.Service.DTOs.Reports;
using DeliveryPlatform.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DeliveryPlatform.Api.Controllers
{
    [ApiController]
    [Route("api/stores/{storeId:int}/reports")]
    public class ReportsController : ControllerBase
    {
        private readonly IReportService _reportService;

        public ReportsController(IReportService reportService)
        {
            _reportService = reportService;
        }

     
        [HttpGet("daily")]
        public async Task<ActionResult<StoreDailyReportDto>> GetDailyReport(
            int storeId)
        {
            var result = await _reportService.GetDailyReportAsync(storeId);

            return Ok(result);
        }

       
        [HttpGet("top-products")]
        public async Task<ActionResult<List<TopSellingProductDto>>>
            GetTopSellingProducts(int storeId)
        {
            var result =
                await _reportService.GetTopSellingProductsAsync(storeId);

            return Ok(result);
        }

       
        [HttpGet("average-order-value")]
        public async Task<ActionResult<AverageOrderValueDto>>
            GetAverageOrderValue(int storeId)
        {
            var result =
                await _reportService.GetAverageOrderValueAsync(storeId);

            return Ok(result);
        }

        // GET: api/stores/1/reports/order-status-counts
        [HttpGet("order-status-counts")]
        public async Task<ActionResult<List<OrderStatusCountDto>>>
            GetOrderStatusCounts(int storeId)
        {
            var result =
                await _reportService.GetOrderStatusCountsAsync(storeId);

            return Ok(result);
        }
    }
}