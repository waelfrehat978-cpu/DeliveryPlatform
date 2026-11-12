using DeliveryPlatform.Data.Context;
using DeliveryPlatform.Service.DTOs;
using DeliveryPlatform.Service.DTOs.Products;
using DeliveryPlatform.Service.Interfaces;
using Microsoft.EntityFrameworkCore;
using DeliveryPlatform.Service.Exceptions;

namespace DeliveryPlatform.Service.Services
{
    public class ProductService : IProductService
    {
        private readonly DeliveryPlatformDbContext _context;

        public ProductService(DeliveryPlatformDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResultDto<ProductDto>> SearchProductsAsync(
            ProductSearchDto searchDto)
        {
            var query = _context.Products.AsNoTracking();

            // Search by product name
            if (!string.IsNullOrWhiteSpace(searchDto.Search))
            {
                query = query.Where(p =>
                    p.Name.Contains(searchDto.Search));
            }

            // Filter by store
            if (searchDto.StoreId.HasValue)
            {
                query = query.Where(p =>
                    p.StoreId == searchDto.StoreId.Value);
            }

            // Filter by category
            if (searchDto.CategoryId.HasValue)
            {
                query = query.Where(p =>
                    p.CategoryId == searchDto.CategoryId.Value);
            }

            // Filter by minimum price
            if (searchDto.MinPrice.HasValue)
            {
                query = query.Where(p =>
                    p.Price >= searchDto.MinPrice.Value);
            }

            // Filter by maximum price
            if (searchDto.MaxPrice.HasValue)
            {
                query = query.Where(p =>
                    p.Price <= searchDto.MaxPrice.Value);
            }

            // Filter by stock availability
            if (searchDto.InStock.HasValue)
            {
                query = searchDto.InStock.Value
                    ? query.Where(p => p.StockQuantity > 0)
                    : query.Where(p => p.StockQuantity == 0);
            }

            var totalCount = await query.CountAsync();

            
            // Sorting
            query = searchDto.SortBy?.ToLower() switch
            {
                "price" when searchDto.SortDescending =>
                    query.OrderByDescending(p => p.Price)
                         .ThenBy(p => p.Id),

                "price" =>
                    query.OrderBy(p => p.Price)
                         .ThenBy(p => p.Id),

                "name" when searchDto.SortDescending =>
                    query.OrderByDescending(p => p.Name)
                         .ThenBy(p => p.Id),

                _ =>
                    query.OrderBy(p => p.Name)
                         .ThenBy(p => p.Id)
            };

            var products = await query
                .Skip((searchDto.Page - 1) * searchDto.PageSize)
                .Take(searchDto.PageSize)
                .Select(p => new ProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    StockQuantity = p.StockQuantity,
                    StoreId = p.StoreId,
                    CategoryId = p.CategoryId
                })
                .ToListAsync();

            return new PagedResultDto<ProductDto>
            {
                Items = products,
                TotalCount = totalCount,
                Page = searchDto.Page,
                PageSize = searchDto.PageSize
            };
        }

        public async Task<ProductDetailsDto> GetProductByIdAsync(int id)
        {
            if (id <= 0)
                throw new ValidationException("id", "Product Id Must be greater than 0.");

            var product= await _context.Products
                .AsNoTracking()
                .Where(p => p.Id == id)
                .Select(p => new ProductDetailsDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price,
                    StockQuantity = p.StockQuantity,
                    StoreId = p.StoreId,
                    CategoryId = p.CategoryId
                })
                .FirstOrDefaultAsync();
            if (product is null)
                throw new NotFoundException("Product not found.");

            return product;
        }
    }
}