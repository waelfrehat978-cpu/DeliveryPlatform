using DeliveryPlatform.Service.DTOs.Products;
using DeliveryPlatform.Service.Interfaces;
using DeliveryPlatform.Service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
namespace DeliveryPlatform.Api.Controllers
{
    [ApiController]
    [Route("api/v1/products")]
    public class ProductsController : ControllerBase
    {
        private readonly IAuditHistoryService _auditHistoryService;
        private readonly IProductService _productService;

        public ProductsController(
        IProductService productService,
        IAuditHistoryService auditHistoryService)
        {
            _productService = productService;
            _auditHistoryService = auditHistoryService;
        }


        [HttpGet]
        public async Task<IActionResult> SearchProducts(
            [FromQuery] ProductSearchDto searchDto)
        {
           

            var products = await _productService.SearchProductsAsync(searchDto);

            return Ok(products);
        }

       
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetProductById(int id)
        {
           

            var product = await _productService.GetProductByIdAsync(id);


            return Ok(product);
        }
        [Authorize(Roles = "Partner,Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateProduct(
       [FromBody] CreateProductDto request)
        {
            var product = await _productService.CreateProductAsync(request);

            return CreatedAtAction(
                nameof(GetProductById),
                new { id = product.Id },
                product);
        }
        [Authorize(Roles = "Partner,Admin")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateProduct(
     int id,
     [FromBody] UpdateProductDto request)
        {
            var product = await _productService.UpdateProductAsync(
                id,
                request);

            return Ok(product);
        }
        [Authorize(Roles = "Partner,Admin")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            await _productService.DeleteProductAsync(id);

            return NoContent();
        }
        [Authorize(Roles = "Partner,Admin")]
        [HttpGet("{id:int}/history")]
        public async Task<IActionResult> GetProductHistory(
    int id,
    [FromQuery, Range(1, int.MaxValue)] int page = 1,
    [FromQuery, Range(1, 100)] int pageSize = 10)
        {
            var history = await _auditHistoryService
                .GetProductHistoryAsync(id, page, pageSize);

            return Ok(history);
        }
    }
}