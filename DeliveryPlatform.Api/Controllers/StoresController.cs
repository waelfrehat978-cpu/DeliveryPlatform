using DeliveryPlatform.Service.DTOs.Stores;
using DeliveryPlatform.Service.Interfaces;
using DeliveryPlatform.Service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
namespace DeliveryPlatform.Api.Controllers
{

    [ApiController]
    [Route("api/v1/stores")]
    public class StoresController : ControllerBase
    {
        private readonly IAuditHistoryService _auditHistoryService;
        private readonly IStoreService _storeService;

        public StoresController(
       IStoreService storeService,
       IAuditHistoryService auditHistoryService)
        {
            _storeService = storeService;
            _auditHistoryService = auditHistoryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetStores(
            [FromQuery] string? name,
            [FromQuery,Range(1,int.MaxValue)] int page = 1,
            [FromQuery,Range(1,100)] int pageSize = 10)
        {
          
            var stores = await _storeService.GetStoresAsync(
                name, page, pageSize);

            return Ok(stores);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetStoreById(int id)
        {
            var store = await _storeService.GetStoreByIdAsync(id);


            return Ok(store);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateStore(
    [FromBody] CreateStoreDto request)
        {
            var store = await _storeService.CreateStoreAsync(request);

            return CreatedAtAction(
                nameof(GetStoreById),
                new { id = store.Id },
                store);
        }
        [Authorize(Roles = "Admin")]
        [HttpPut("{id:int}/name")]
        public async Task<IActionResult> UpdateStoreName(
    int id,
    [FromBody] UpdateStoreNameDto request)
        {
            await _storeService.UpdateStoreNameAsync(id, request.Name);

            return NoContent();
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteStore(int id)
        {
            await _storeService.DeleteStoreAsync(id);

            return NoContent();
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("{id:int}/history")]
        public async Task<IActionResult> GetStoreHistory(
    int id,
    [FromQuery, Range(1, int.MaxValue)] int page = 1,
    [FromQuery, Range(1, 100)] int pageSize = 10)
        {
            var history = await _auditHistoryService
                .GetStoreHistoryAsync(id, page, pageSize);

            return Ok(history);
        }
    }
}