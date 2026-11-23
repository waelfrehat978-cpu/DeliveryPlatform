
using System.Text.Json;
using DeliveryPlatform.Data.Context;
using DeliveryPlatform.Data.Enums;
using DeliveryPlatform.Data.Interfaces;
using DeliveryPlatform.Service.DTOs;
using DeliveryPlatform.Service.DTOs.AuditHistory;
using DeliveryPlatform.Service.Exceptions;
using DeliveryPlatform.Service.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DeliveryPlatform.Service.Services
{
    public class AuditHistoryService : IAuditHistoryService
    {
        private readonly DeliveryPlatformDbContext _context;
        private readonly IResourceAuthorizationService _resourceAuthorization;
        private readonly ICurrentUser _currentUser;

        public AuditHistoryService(
            DeliveryPlatformDbContext context,
            IResourceAuthorizationService resourceAuthorization,
            ICurrentUser currentUser)
        {
            _context = context;
            _resourceAuthorization = resourceAuthorization;
            _currentUser = currentUser;
        }

        public async Task<PagedResultDto<AuditHistoryDto>> GetProductHistoryAsync(
            int productId, int page, int pageSize)
        {
            EnsureAdminOrPartner();

            if (_currentUser.Role == UserRole.Partner)
            {
                await _resourceAuthorization.EnsureProductAccessAsync(productId);
            }

            var query = _context.ProductAuditTrails
                .Where(a => a.EntityName == "Product"
                         && a.EntityId == productId.ToString())
                .OrderByDescending(a => a.TimestampUtc);

            return await BuildHistoryResultAsync(query, page, pageSize);
        }

        public async Task<PagedResultDto<AuditHistoryDto>> GetOrderHistoryAsync(
            int orderId, int page, int pageSize)
        {
            EnsureAdminOnly();

            var query = _context.OrderAuditTrails
                .Where(a => a.EntityName == "Order"
                         && a.EntityId == orderId.ToString())
                .OrderByDescending(a => a.TimestampUtc);

            return await BuildHistoryResultAsync(query, page, pageSize);
        }

        public async Task<PagedResultDto<AuditHistoryDto>> GetCustomerHistoryAsync(
            int customerId, int page, int pageSize)
        {
            EnsureAdminOnly();

            var query = _context.CustomerAuditTrails
                .Where(a => a.EntityName == "Customer"
                         && a.EntityId == customerId.ToString())
                .OrderByDescending(a => a.TimestampUtc);

            return await BuildHistoryResultAsync(query, page, pageSize);
        }

        public async Task<PagedResultDto<AuditHistoryDto>> GetStoreHistoryAsync(
            int storeId, int page, int pageSize)
        {
            EnsureAdminOnly();

            var query = _context.StoreAuditTrails
                .Where(a => a.EntityName == "Store"
                         && a.EntityId == storeId.ToString())
                .OrderByDescending(a => a.TimestampUtc);

            return await BuildHistoryResultAsync(query, page, pageSize);
        }

        private void EnsureAdminOrPartner()
        {
            if (_currentUser.Role != UserRole.Admin &&
                _currentUser.Role != UserRole.Partner)
            {
                throw new ForbiddenException(
                    "You do not have permission to read audit history.");
            }
        }

        private void EnsureAdminOnly()
        {
            if (_currentUser.Role != UserRole.Admin)
            {
                throw new ForbiddenException(
                    "Only Admin can read this audit history.");
            }
        }

        private async Task<PagedResultDto<AuditHistoryDto>> BuildHistoryResultAsync(
            IQueryable<Data.Auditing.AuditTrail> query,
            int page,
            int pageSize)
        {
            var totalCount = await query.CountAsync();

            var records = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var items = new List<AuditHistoryDto>();

            foreach (var record in records)
            {
                string displayName = "System";

                if (record.UserId.HasValue)
                {
                    var user = await _context.Users
                        .Include(u => u.Customer)
                        .FirstOrDefaultAsync(u => u.Id == record.UserId.Value);

                    if (user != null)
                    {
                        displayName = user.Customer != null
                            ? $"{user.Customer.FirstName} {user.Customer.LastName}"
                            : user.Email ?? "Unknown";
                    }
                }

                var dictionary =
                    JsonSerializer.Deserialize<Dictionary<string, ChangeValues>>(
                        record.ChangesJson);

                var changes = dictionary?
                    .Select(change => new AuditChangeDto
                    {
                        Property = change.Key,
                        OldValue = change.Value.Old,
                        NewValue = change.Value.New
                    })
                    .ToList() ?? new List<AuditChangeDto>();

                items.Add(new AuditHistoryDto
                {
                    TimestampUtc = record.TimestampUtc,
                    Action = record.Action,
                    UserId = record.UserId,
                    UserDisplayName = displayName,
                    Changes = changes
                });
            }

            return new PagedResultDto<AuditHistoryDto>
            {
                Items = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        private class ChangeValues
        {
            public JsonElement? Old { get; set; }
            public JsonElement? New { get; set; }
        }
    }
}
