
using DeliveryPlatform.Data.Context;
using DeliveryPlatform.Data.Entities;
using DeliveryPlatform.Data.Enums;
using DeliveryPlatform.Data.Interfaces;
using DeliveryPlatform.Service.DTOs;
using DeliveryPlatform.Service.DTOs.Products;
using DeliveryPlatform.Service.Exceptions;
using DeliveryPlatform.Service.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DeliveryPlatform.Service.Services
{
    public class ProductService : IProductService
    {
        private readonly DeliveryPlatformDbContext _context;
        private readonly IResourceAuthorizationService _resourceAuthorization;
        private readonly ICurrentUser _currentUser;

        public ProductService(
            DeliveryPlatformDbContext context,
            IResourceAuthorizationService resourceAuthorization,
            ICurrentUser currentUser)
        {
            _context = context;
            _resourceAuthorization = resourceAuthorization;
            _currentUser = currentUser;
        }

        public async Task<PagedResultDto<ProductDto>> SearchProductsAsync(
            ProductSearchDto searchDto)
        {
            var query = _context.Products.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(searchDto.Search))
            {
                query = query.Where(p =>
                    p.Name.Contains(searchDto.Search));
            }

            if (searchDto.StoreId.HasValue)
            {
                query = query.Where(p =>
                    p.StoreId == searchDto.StoreId.Value);
            }

            if (searchDto.CategoryId.HasValue)
            {
                query = query.Where(p =>
                    p.CategoryId == searchDto.CategoryId.Value);
            }

            if (searchDto.MinPrice.HasValue)
            {
                query = query.Where(p =>
                    p.Price >= searchDto.MinPrice.Value);
            }

            if (searchDto.MaxPrice.HasValue)
            {
                query = query.Where(p =>
                    p.Price <= searchDto.MaxPrice.Value);
            }

            if (searchDto.InStock.HasValue)
            {
                query = searchDto.InStock.Value
                    ? query.Where(p => p.StockQuantity > 0)
                    : query.Where(p => p.StockQuantity == 0);
            }

            var totalCount = await query.CountAsync();

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
                throw new ValidationException(
                    "id",
                    "Product Id Must be greater than 0.");

            var product = await _context.Products
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

        public async Task<ProductDetailsDto> CreateProductAsync(
            CreateProductDto request)
        {
            int storeId;

            if (_currentUser.Role == UserRole.Partner)
            {
                if (!_currentUser.StoreId.HasValue)
                {
                    throw new ForbiddenException(
                        "You do not have access to this store.");
                }

                storeId = _currentUser.StoreId.Value;

                await _resourceAuthorization.EnsureStoreAccessAsync(
                    storeId);
            }

            else if (_currentUser.Role == UserRole.Admin)
            {
                if (!request.StoreId.HasValue)
                {
                    throw new ValidationException(
                        "StoreId",
                        "StoreId is required for Admin.");
                }

                storeId = request.StoreId.Value;

                await _resourceAuthorization.EnsureStoreAccessAsync(
                    storeId);
            }
            else
            {
                throw new ForbiddenException(
                    "You are not allowed to create products.");
            }

            var categoryExists = await _context.Categories
      .AnyAsync(c => c.Id == request.CategoryId);

            if (!categoryExists)
                throw new NotFoundException("Category not found.");

            var nameExists = await _context.Products
                .AnyAsync(p =>
                    p.StoreId == storeId &&
                    p.Name == request.Name);

            if (nameExists)
            {
                throw new ConflictException(
                    "A product with this name already exists in this store.");
            }

            var product = new Product
            {
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                StockQuantity = request.StockQuantity,
                StoreId = storeId,
                CategoryId = request.CategoryId
            };

            _context.Products.Add(product);

            await _context.SaveChangesAsync();

            return new ProductDetailsDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                StockQuantity = product.StockQuantity,
                StoreId = product.StoreId,
                CategoryId = product.CategoryId
            };
        }

        public async Task<ProductDetailsDto> UpdateProductAsync(
            int id,
            UpdateProductDto request)
        {
            await _resourceAuthorization.EnsureProductAccessAsync(id);

            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product is null)
                throw new NotFoundException("Product not found.");

            var categoryExists = await _context.Categories
                .AnyAsync(c => c.Id == request.CategoryId);

            if (!categoryExists)
                throw new NotFoundException("Category not found.");

            var nameExists = await _context.Products
             .AnyAsync(p =>
            p.StoreId == product.StoreId &&
            p.Name == request.Name &&
            p.Id != id);

            if (nameExists)
            {
                throw new ConflictException(
                    "A product with this name already exists in this store.");
            }


            product.Name = request.Name;
            product.Description = request.Description;
            product.Price = request.Price;
            product.StockQuantity = request.StockQuantity;
            product.CategoryId = request.CategoryId;

            await _context.SaveChangesAsync();

            return new ProductDetailsDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                StockQuantity = product.StockQuantity,
                StoreId = product.StoreId,
                CategoryId = product.CategoryId
            };
        }
    }
}
