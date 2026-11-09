using DeliveryPlatform.Data.Context;
using DeliveryPlatform.Data.Enums;
using DeliveryPlatform.Service.DTOs.Reports;
using DeliveryPlatform.Service.Exceptions;
using DeliveryPlatform.Service.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DeliveryPlatform.Service.Services
{
    public class ReportService : IReportService
    {
        private readonly DeliveryPlatformDbContext _context;

        public ReportService(DeliveryPlatformDbContext context)
        {
            _context = context;
        }

        // Today's Orders and Revenue
        public async Task<StoreDailyReportDto> GetDailyReportAsync(
            int storeId)
        {
            if (storeId <= 0)
                throw new ValidationException("Store ID must be greater than zero.");

            var storeExists = await _context.Stores
                .AnyAsync(s => s.Id == storeId);

            if (!storeExists)
                throw new NotFoundException("Store not found.");
            var startOfDay = DateTime.UtcNow.Date;
            var endOfDay = startOfDay.AddDays(1);
         
            var orders = _context.Orders
                .Where(o =>
                    o.StoreId == storeId &&
                    o.CreatedAtUtc >= startOfDay &&
                    o.CreatedAtUtc < endOfDay &&
                    o.Status != OrderStatus.Declined &&
                    o.Status != OrderStatus.Cancelled);

            /*
               o.StoreId == storeId 
            طاب تابع للمتجر
             o.CreatedAtUtc >= startOfDay
            طلب انشئ بعد باديه اليوم
              o.CreatedAtUtc < endOfDay &&
            طال تنشئ قبل بدايه اليوم التالي 
             o.Status != OrderStatus.Declined 
            استبعاد الطلبات المرفوضه
                    o.Status != OrderStatus.Cancelled);
            استبعاد الطلبات الملغاة 
             */

            var totalOrders = await orders.CountAsync();
            // حساب عدد الطلبات 

            var totalRevenue = await orders
                .SumAsync(o => (decimal?)o.Total) ?? 0;
            // حساب الايرادات 

            return new StoreDailyReportDto
            {
                TotalOrders = totalOrders,
                TotalRevenue = totalRevenue
            };
        }

        // Top 5 Selling Products in Last 30 Days
        public async Task<List<TopSellingProductDto>>
            GetTopSellingProductsAsync(int storeId)
        {
            if (storeId <= 0)
                throw new ValidationException("Store ID must be greater than zero.");

            var storeExists = await _context.Stores
                .AnyAsync(s => s.Id == storeId);

            if (!storeExists)
                throw new NotFoundException("Store not found.");
            var startDate = DateTime.UtcNow.AddDays(-30);

            var products = await _context.OrderLines
                .Where(line =>
                    line.Order.StoreId == storeId &&
                    line.Order.CreatedAtUtc >= startDate &&
                    line.Order.Status != OrderStatus.Declined &&
                    line.Order.Status != OrderStatus.Cancelled)
                .GroupBy(line => new
                {
                    line.ProductId,
                    line.ProductName
                })
                .Select(group => new TopSellingProductDto
                {
                    ProductId = group.Key.ProductId,
                    ProductName = group.Key.ProductName,
                    TotalQuantitySold = group.Sum(
                        line => line.Quantity)
                })
                .OrderByDescending(p => p.TotalQuantitySold)
                .Take(5)
                .ToListAsync();

            return products;
        }

        // Average Order Value in Last 30 Days
        public async Task<AverageOrderValueDto>
            GetAverageOrderValueAsync(int storeId)
        {
            if (storeId <= 0)
                throw new ValidationException("Store ID must be greater than zero.");

            var storeExists = await _context.Stores
                .AnyAsync(s => s.Id == storeId);

            if (!storeExists)
                throw new NotFoundException("Store not found.");
            var startDate = DateTime.UtcNow.AddDays(-30);

            var orders = _context.Orders
                .Where(o =>
                    o.StoreId == storeId &&
                    o.CreatedAtUtc >= startDate &&
                    o.Status != OrderStatus.Declined &&
                    o.Status != OrderStatus.Cancelled);

            var totalOrders = await orders.CountAsync();

            var averageValue = totalOrders == 0
                ? 0
                : await orders.AverageAsync(o => o.Total);

            return new AverageOrderValueDto
            {
                AverageValue = averageValue
            };
        }

        // Order Counts by Status
        public async Task<List<OrderStatusCountDto>>
            GetOrderStatusCountsAsync(int storeId)
        {
            if (storeId <= 0)
                throw new ValidationException("Store ID must be greater than zero.");

            var storeExists = await _context.Stores
                .AnyAsync(s => s.Id == storeId);

            if (!storeExists)
                throw new NotFoundException("Store not found.");
            var counts = await _context.Orders
                .Where(o =>
                    o.StoreId == storeId &&
                    o.Status != OrderStatus.Declined &&
                    o.Status != OrderStatus.Cancelled)
                .GroupBy(o => o.Status)
                .Select(group => new OrderStatusCountDto
                {
                    Status = group.Key,
                    Count = group.Count()
                })
                .ToListAsync();

            return counts;
        }
    }
}