using DeliveryPlatform.Data.Auditing;
using DeliveryPlatform.Data.Context;
using DeliveryPlatform.Data.Entities;
using DeliveryPlatform.Service.DTOs;
using DeliveryPlatform.Service.DTOs.Stores;
using DeliveryPlatform.Service.Exceptions;
using DeliveryPlatform.Service.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DeliveryPlatform.Service.Services
{
    public class StoreService : IStoreService
    {
        private readonly IAuditScope _auditScope;
        private readonly DeliveryPlatformDbContext _context;

        public StoreService(DeliveryPlatformDbContext context, IAuditScope auditScope)
        {
            _context = context;
            _auditScope = auditScope;
        }

        public async Task<PagedResultDto<StoreDto>> GetStoresAsync(
            string? name,
            int page,
            int pageSize)
        {
            if (page < 1)
                throw new ValidationException("page", "Page must be greater than 0.");

            if (pageSize < 1 || pageSize > 100)
                throw new ValidationException(
                    "pageSize",
                    "PageSize must be between 1 and 100.");
            var query = _context.Stores.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(name))
            {
                query = query.Where(s => s.Name.Contains(name));
            }

            var totalCount = await query.CountAsync();

            var stores = await query
               .OrderBy(s => s.Name)
            .ThenBy(s => s.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(s => new StoreDto
                {
                    Id = s.Id,
                    Name = s.Name
                })
                .ToListAsync();

            return new PagedResultDto<StoreDto>
            {
                Items = stores,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<StoreDetailsDto> GetStoreByIdAsync(int id)
        {
            var store= await _context.Stores
                .AsNoTracking()
                .Where(s => s.Id == id)
                .Select(s => new StoreDetailsDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    Latitude = s.Latitude,
                    Longitude = s.Longitude,
                    ProductCount = _context.Products.Count(p => p.StoreId == s.Id)
                })
                .FirstOrDefaultAsync();
            if (store is null)
                throw new NotFoundException("Store not found.");

            return store;
        }
        public async Task<StoreDetailsDto> CreateStoreAsync(
    CreateStoreDto request)
        {
            var store = new Store
            {
                Name = request.Name,
                Latitude = request.Latitude,
                Longitude = request.Longitude
            };

            _context.Stores.Add(store);

            await _context.SaveChangesAsync();

            return new StoreDetailsDto
            {
                Id = store.Id,
                Name = store.Name,
                Latitude = store.Latitude,
                Longitude = store.Longitude,
                ProductCount = 0
            };
        }
        public async Task UpdateStoreNameAsync(int id, string name)
        {
            if (id <= 0)
                throw new ValidationException(
                    "id",
                    "Store ID must be greater than 0.");

            var store = await _context.Stores
                .FirstOrDefaultAsync(s => s.Id == id);

            if (store is null)
                throw new NotFoundException("Store not found.");

            using (_auditScope.Enable())
            {
                store.Name = name;

                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteStoreAsync(int id)
        {
            if (id <= 0)
                throw new ValidationException(
                    "id",
                    "Store ID must be greater than 0.");

            var store = await _context.Stores
                .FirstOrDefaultAsync(s => s.Id == id);

            if (store is null)
                throw new NotFoundException("Store not found.");

            using (_auditScope.Enable())
            {
                var products = await _context.Products
                    .Where(p => p.StoreId == id)
                    .ToListAsync();

                foreach (var product in products)
                {
                    _context.Products.Remove(product);
                }

                _context.Stores.Remove(store);

                await _context.SaveChangesAsync();

                /*
                 جلبنا جميع منتجات المتجر.



    استخدمنا Remove لكل منتج، وهذا عندك يتحول إلى Soft Delete بواسطة الـ Interceptor.



    حذفنا المتجر منطقيًا.



    نفّذنا SaveChangesAsync مرة واحدة.
                 */
            }
        }

    }
}