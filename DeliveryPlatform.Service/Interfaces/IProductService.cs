using DeliveryPlatform.Service.DTOs;
using DeliveryPlatform.Service.DTOs.Products;

namespace DeliveryPlatform.Service.Interfaces
{
    public interface IProductService
    {
        Task<PagedResultDto<ProductDto>> SearchProductsAsync(
            ProductSearchDto searchDto);

        Task<ProductDetailsDto> GetProductByIdAsync(int id);
        Task<ProductDetailsDto> CreateProductAsync(
    CreateProductDto request);
        Task<ProductDetailsDto> UpdateProductAsync(
    int id,
    UpdateProductDto request);
        Task DeleteProductAsync(int id);
    }
}