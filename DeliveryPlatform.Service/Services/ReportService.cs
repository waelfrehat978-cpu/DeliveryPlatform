
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
        private readonly IResourceAuthorizationService _resourceAuthorization;
        private readonly DeliveryPlatformDbContext _context;

        public ReportService(
         DeliveryPlatformDbContext context,
         IResourceAuthorizationService resourceAuthorization)
        {
            _context = context;
            _resourceAuthorization = resourceAuthorization;
        }

        public async Task<StoreDashboardDto> GetStoreDashboardAsync(
            int storeId)
        {
            if (storeId <= 0)
                throw new ValidationException(
                    "storeId",
                    "Store ID must be greater than zero.");
            await _resourceAuthorization.EnsureStoreAccessAsync(storeId);
            // Calculate today's start at midnight in Amman timezone, then convert it to UTC.
            var amman = TimeZoneInfo.FindSystemTimeZoneById(
                "Asia/Amman");

            var nowInAmman = TimeZoneInfo.ConvertTimeFromUtc(
                DateTime.UtcNow, amman);

            var todayStartUtc = TimeZoneInfo.ConvertTimeToUtc(
                nowInAmman.Date, amman);

            var tomorrowStartUtc = todayStartUtc.AddDays(1);

            var last30DaysStartUtc = DateTime.UtcNow.AddDays(-30);

            // Base query for store orders
            var storeOrdersQuery = _context.Orders
                .Where(o =>
                    o.StoreId == storeId &&
                    o.Status != OrderStatus.Declined &&
                    o.Status != OrderStatus.Cancelled);

            // Today's orders
            var todayOrdersQuery = storeOrdersQuery
                .Where(o =>
                    o.CreatedAtUtc >= todayStartUtc &&
                    o.CreatedAtUtc < tomorrowStartUtc);

            var todayOrderCount = await todayOrdersQuery.CountAsync();

            var todayRevenue = await todayOrdersQuery
                .SumAsync(o => (decimal?)o.Total) ?? 0;

            // Average order value in the last 30 days
            var averageOrderValue = await storeOrdersQuery
                .Where(o => o.CreatedAtUtc >= last30DaysStartUtc)
                .AverageAsync(o => (decimal?)o.Total) ?? 0;

            // Top 5 selling products in the last 30 days
            var topProducts = await _context.OrderLines
                .Where(line =>
                    line.Order.StoreId == storeId &&
                    line.Order.CreatedAtUtc >= last30DaysStartUtc &&
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

            // Active order statuses only
            var activeStatuses = new[]
            {
                OrderStatus.Pending,
                OrderStatus.Accepted,
                OrderStatus.Preparing,
                OrderStatus.OutForDelivery
            };

            var activeStatusCounts = await _context.Orders
                .Where(o =>
                    o.StoreId == storeId &&
                    activeStatuses.Contains(o.Status))
                .GroupBy(o => o.Status)
                .Select(group => new OrderStatusCountDto
                {
                    Status = group.Key,
                    Count = group.Count()
                })
                .ToListAsync();

            return new StoreDashboardDto
            {
                TodayOrderCount = todayOrderCount,
                TodayRevenue = todayRevenue,
                TopProducts = topProducts,
                AverageOrderValue = averageOrderValue,
                ActiveStatusCounts = activeStatusCounts
            };
        }
    }
}
