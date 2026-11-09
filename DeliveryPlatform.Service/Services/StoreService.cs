using DeliveryPlatform.Data.Context;
using DeliveryPlatform.Service.DTOs;
using DeliveryPlatform.Service.DTOs.Stores;
using DeliveryPlatform.Service.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DeliveryPlatform.Service.Services
{
    public class StoreService : IStoreService
    {
        private readonly DeliveryPlatformDbContext _context;

        public StoreService(DeliveryPlatformDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResultDto<StoreDto>> GetStoresAsync(
            string? name,
            int page,
            int pageSize)
        {
            var query = _context.Stores.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(name))
            {
                query = query.Where(s => s.Name.Contains(name));
            }

            var totalCount = await query.CountAsync();

            var stores = await query
                .OrderBy(s => s.Name)
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

        public async Task<StoreDetailsDto?> GetStoreByIdAsync(int id)
        {
            return await _context.Stores
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
        }
    }
}