
using DeliveryPlatform.Service.DTOs.Reports;
using DeliveryPlatform.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DeliveryPlatform.Api.Controllers
{
    [ApiController]
    [Route("api/v1/stores/{storeId:int}/dashboard")]
    public class ReportsController : ControllerBase
    {
        private readonly IReportService _reportService;

        public ReportsController(IReportService reportService)
        {
            _reportService = reportService;
        }

        [HttpGet]
        public async Task<ActionResult<StoreDashboardDto>>
            GetStoreDashboard(int storeId)
        {
            var result =
                await _reportService.GetStoreDashboardAsync(storeId);

            return Ok(result);
        }
    }
}
