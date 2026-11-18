using DeliveryPlatform.Service.DTOs.Products;
using DeliveryPlatform.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
namespace DeliveryPlatform.Api.Controllers
{
    [ApiController]
    [Route("api/v1/products")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
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
    }
}