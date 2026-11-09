using DeliveryPlatform.Service.Interfaces;
using DeliveryPlatform.Service.Services;
using Microsoft.AspNetCore.Mvc;

namespace DeliveryPlatform.Api.Controllers
{

    [ApiController]
    [Route("api/v1/stores")]
    public class StoresController : ControllerBase
    {
        private readonly IStoreService _storeService;

        public StoresController(IStoreService storeService)
        {
            _storeService = storeService;
        }

        [HttpGet]
        public async Task<IActionResult> GetStores(
            [FromQuery] string? name,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            if (page < 1)
                return BadRequest("Page must be greater than 0.");

            if (pageSize < 1 || pageSize > 100)
                return BadRequest("PageSize must be between 1 and 100.");

            var stores = await _storeService.GetStoresAsync(
                name, page, pageSize);

            return Ok(stores);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetStoreById(int id)
        {
            var store = await _storeService.GetStoreByIdAsync(id);

            if (store is null)
                return NotFound();

            return Ok(store);
        }
    }
}