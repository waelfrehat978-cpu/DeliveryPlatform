using DeliveryPlatform.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

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
    }
}