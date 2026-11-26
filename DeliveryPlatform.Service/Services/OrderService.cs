using DeliveryPlatform.Data.Auditing;
using DeliveryPlatform.Data.Context;
using DeliveryPlatform.Data.Entities;
using DeliveryPlatform.Data.Enums;
using DeliveryPlatform.Data.Interfaces;
using DeliveryPlatform.Service.DTOs;
using DeliveryPlatform.Service.DTOs.Addresses;
using DeliveryPlatform.Service.DTOs.Orders;
using DeliveryPlatform.Service.Exceptions;
using DeliveryPlatform.Service.Interfaces;
using DeliveryPlatform.Service.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Data;
using Microsoft.EntityFrameworkCore.Storage;
using System.Security.Cryptography;

namespace DeliveryPlatform.Service.Services
{
    public class OrderService : IOrderService
    {
        private readonly IAuditScope _auditScope;
        private readonly IClockService _clock;
        private readonly DeliveryPlatformDbContext _context;
        private readonly OrderOptions _orderOptions;
        private readonly IResourceAuthorizationService _resourceAuthorization;
        private readonly ICurrentUser _currentUser;

        public OrderService(
       DeliveryPlatformDbContext context,
       IOptions<OrderOptions> orderOptions,
       IResourceAuthorizationService resourceAuthorization,
       ICurrentUser currentUser,
       IClockService clock, IAuditScope auditScope)
        {
            _context = context;
            _orderOptions = orderOptions.Value;
            _resourceAuthorization = resourceAuthorization;
            _currentUser = currentUser;
            _clock = clock;
            _auditScope=auditScope;

        }

        private static readonly Dictionary<OrderStatus, OrderStatus[]> AllowedTransitions = new()
        {
            [OrderStatus.Pending] = new[]
            {
                OrderStatus.Accepted,
                OrderStatus.Declined
            },

            [OrderStatus.Accepted] = new[]
            {
                OrderStatus.Preparing,
                OrderStatus.Declined
            },

            [OrderStatus.Preparing] = new[]
            {
                OrderStatus.OutForDelivery,
                OrderStatus.Declined
            },
            [OrderStatus.OutForDelivery] = new[]
{
         OrderStatus.Delivered
}

        };

        // Create Order

        public async Task<OrderResponseDto> CreateOrderAsync(
            CreateOrderDto request)
        {
            if (request.Items == null || !request.Items.Any())
                throw new ValidationException(
                    "Items",
                    "Order must contain at least one item.");

            if (!Enum.IsDefined(typeof(PaymentMethod), request.PaymentMethod))
                throw new ValidationException(
                    "PaymentMethod",
                    "Invalid payment method.");

            if (request.Items.Any(i => i.ProductId <= 0))
                throw new ValidationException(
                    "Items",
                    "Product ID must be greater than zero.");

            var productIds = request.Items
                .Select(i => i.ProductId)
                .ToList();

            if (productIds.Distinct().Count() != productIds.Count)
                throw new ValidationException(
                    "Items",
                    "Duplicate products are not allowed.");
            // Only Customers can create orders.
            if (_currentUser.Role != UserRole.Customer)
            {
                throw new ForbiddenException(
                    "You are not allowed to create orders.");
            }

            if (!_currentUser.CustomerId.HasValue)
            {
                throw new ForbiddenException(
                    "You do not have access to this customer.");
            }

            int customerId = _currentUser.CustomerId.Value;
            await _resourceAuthorization.EnsureCustomerAccessAsync(
                customerId);

            await using var transaction =
                await _context.Database.BeginTransactionAsync(
                    IsolationLevel.Serializable);

            try
            {
                var storeExists = await _context.Stores
                    .AnyAsync(s => s.Id == request.StoreId);

                if (!storeExists)
                    throw new NotFoundException("Store not found.");

                var addressExists = await _context.Addresses
                    .AnyAsync(a =>
                        a.Id == request.AddressId &&
                        a.CustomerId == customerId);

                if (!addressExists)
                    throw new NotFoundException(
                        $"Address {request.AddressId} was not found for customer {customerId}.");

                var products = await _context.Products
                    .Where(p =>
                        productIds.Contains(p.Id) &&
                        p.StoreId == request.StoreId)
                    .ToListAsync();

                if (products.Count != productIds.Count)
                    throw new NotFoundException(
                        "One or more products were not found in this store.");

                decimal subtotal = 0;

                foreach (var item in request.Items)
                {
                    var product = products.First(
                        p => p.Id == item.ProductId);

                    if (product.StockQuantity < item.Quantity)
                        throw new BusinessRuleException(
                            $"Insufficient stock for {product.Name}.");

                    subtotal += product.Price * item.Quantity;
                }

                decimal deliveryFee = _orderOptions.DeliveryFee;

                var connection = _context.Database.GetDbConnection();

                await using var command = connection.CreateCommand();

                command.CommandText =
                    "SELECT NEXT VALUE FOR dbo.OrderCodeSequence;";

                var currentTransaction = _context.Database.CurrentTransaction;

                if (currentTransaction != null)
                {
                    command.Transaction = currentTransaction.GetDbTransaction();
                }

                var sequenceResult = await command.ExecuteScalarAsync();

                var sequenceNumber = Convert.ToInt64(sequenceResult);
                var date = _clock.UtcNow;

                var randomPart = RandomNumberGenerator.GetInt32(
                    0, 100_000);

                var orderCode =
                    $"{date:yyMM}{randomPart:D5}{sequenceNumber % 1000:D3}";

                var order = new Order
                {
                    OrderCode = orderCode,
                    CustomerId = customerId,
                    StoreId = request.StoreId,
                    AddressId = request.AddressId,
                    Status = OrderStatus.Pending,
                    Subtotal = subtotal,
                    DeliveryFee = deliveryFee,
                    Total = subtotal + deliveryFee,
                    PaymentMethod = request.PaymentMethod
                };

                foreach (var item in request.Items)
                {
                    var product = products.First(
                        p => p.Id == item.ProductId);

                    order.OrderLines.Add(new OrderLine
                    {
                        Order = order,
                        ProductId = product.Id,
                        ProductName = product.Name,
                        Quantity = item.Quantity,
                        UnitPrice = product.Price
                    });

                    product.StockQuantity -= item.Quantity;
                }

                _context.Orders.Add(order);

                _context.OrderStatusHistories.Add(
                    new OrderStatusHistory
                    {
                        Order = order,
                        Status = OrderStatus.Pending,
                        OldStatus = null,
                        Note = "Order created."
                    });

                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return MapOrderToResponse(order);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        // Get Order By Id
        public async Task<OrderDetailsDto> GetOrderByIdAsync(int orderId)
        {
            if (orderId <= 0)
                throw new ValidationException(
                    "orderId",
                    "Order ID must be greater than 0.");

            await _resourceAuthorization.EnsureOrderAccessAsync(orderId);

            var order = await _context.Orders
                .AsNoTracking()
                .Include(o => o.Customer)
                .Include(o => o.Store)
                .Include(o => o.Address)
                .Include(o => o.OrderLines)
                .Include(o => o.StatusHistory)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
                throw new NotFoundException(
                    $"Order {orderId} was not found.");

            return new OrderDetailsDto
            {
                Id = order.Id,
                OrderCode = order.OrderCode,
                CreatedAtUtc = order.CreatedAtUtc,
                Status = order.Status,
                PaymentMethod = order.PaymentMethod,
                Subtotal = order.Subtotal,
                DeliveryFee = order.DeliveryFee,
                Total = order.Total,

                CustomerId = order.CustomerId,
                CustomerName = $"{order.Customer.FirstName} {order.Customer.LastName}",

                StoreId = order.StoreId,
                StoreName = order.Store.Name,

                Address = new AddressResponseDto
                {
                    Id = order.Address.Id,
                    CustomerId = order.Address.CustomerId,
                    Label = order.Address.Label,
                    Street = order.Address.Street,
                    City = order.Address.City,
                    Latitude = order.Address.Latitude,
                    Longitude = order.Address.Longitude,
                    IsDefault = order.Address.IsDefault
                },

                Lines = order.OrderLines
                    .Select(line => new OrderLineResponseDto
                    {
                        ProductId = line.ProductId,
                        ProductName = line.ProductName,
                        Quantity = line.Quantity,
                        UnitPrice = line.UnitPrice,
                        LineTotal = line.Quantity * line.UnitPrice
                    })
                    .ToList(),

                StatusHistory = order.StatusHistory
                    .OrderBy(h => h.Id)
                    .Select(h => new OrderStatusHistoryDto
                    {
                        OldStatus = h.OldStatus,
                        Status = h.Status,
                        ChangedAtUtc = h.CreatedAtUtc,
                        Note = h.Note
                    })
                    .ToList()
            };
        }

        // Get Customer Orders
        public async Task<PagedResultDto<CustomerOrderSummaryDto>> GetCustomerOrdersAsync(
            int customerId,
            int page,
            int pageSize)
        {
            if (customerId <= 0)
                throw new ValidationException(
                    "customerId",
                    "Customer ID must be greater than 0.");

            if (page < 1 || pageSize < 1 || pageSize > 100)
                throw new ValidationException(
                    "page",
                    "Invalid pagination values.");

            // Customers can only access their own orders.
            if (_currentUser.Role == UserRole.Customer)
            {
                if (!_currentUser.CustomerId.HasValue)
                {
                    throw new ForbiddenException(
                        "You do not have access to this customer.");
                }

                customerId = _currentUser.CustomerId.Value;
            }
            else if (_currentUser.Role != UserRole.Admin)
            {
                throw new ForbiddenException(
                    "You are not allowed to access customer orders.");
            }

            await _resourceAuthorization.EnsureCustomerAccessAsync(
                customerId);

            var query = _context.Orders
                .Where(o => o.CustomerId == customerId);

            var totalCount = await query.CountAsync();

            var orders = await query
                .OrderByDescending(o => o.CreatedAtUtc)
                .ThenByDescending(o => o.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(o => new CustomerOrderSummaryDto
                {
                    Id = o.Id,
                    OrderCode = o.OrderCode,
                    CreatedAtUtc = o.CreatedAtUtc,
                    Status = o.Status,
                    StoreName = o.Store.Name,
                    LineCount = o.OrderLines.Count,
                    Total = o.Total
                })
                .ToListAsync();

            return new PagedResultDto<CustomerOrderSummaryDto>
            {
                Items = orders,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        // Update Order Status
        public async Task<OrderResponseDto> UpdateOrderStatusAsync(
            int orderId,
            UpdateOrderStatusDto request)
        {
            if (orderId <= 0)
                throw new ValidationException(
                    "orderId",
                    "Order ID must be greater than 0.");

            if (!Enum.IsDefined(typeof(OrderStatus), request.Status))
                throw new ValidationException(
                    "Status",
                    "Invalid order status.");

            await using var transaction =
                await _context.Database.BeginTransactionAsync(
                    IsolationLevel.Serializable);

            try
            {
                await _resourceAuthorization.EnsureOrderAccessAsync(orderId);

                var order = await _context.Orders
                    .Include(o => o.OrderLines)
                    .FirstOrDefaultAsync(o => o.Id == orderId);

                if (order == null)
                    throw new NotFoundException("Order not found.");

                if (_currentUser.Role != UserRole.Admin)
                {
                    var allowedStatuses = AllowedTransitions.TryGetValue(
                        order.Status, out var nextStatuses)
                            ? nextStatuses
                            : Array.Empty<OrderStatus>();

                    if (!allowedStatuses.Contains(request.Status))
                    {
                        var allowedText = allowedStatuses.Length == 0
    ? "nothing, because this status is final"
    : string.Join(", ", allowedStatuses);

                        throw new BusinessRuleException(
                            $"Cannot change order status from {order.Status} to {request.Status}. Allowed: {allowedText}.");
                    }
                }
                using (_auditScope.Enable())
                {
                    // Return stock when the order is declined or cancelled.
                    var oldStatus = order.Status;

                if ((request.Status == OrderStatus.Declined ||
                     request.Status == OrderStatus.Cancelled) &&
                    (oldStatus == OrderStatus.Pending ||
                     oldStatus == OrderStatus.Accepted ||
                     oldStatus == OrderStatus.Preparing))
                {
                    foreach (var line in order.OrderLines)
                    {
                        var product = await _context.Products
                            .FirstOrDefaultAsync(p => p.Id == line.ProductId);

                        if (product != null)
                            product.StockQuantity += line.Quantity;
                    }
                }
              

                    order.Status = request.Status;

                    _context.OrderStatusHistories.Add(
                        new OrderStatusHistory
                        {
                            OrderId = order.Id,
                            OldStatus = oldStatus,
                            Status = request.Status,
                            Note = request.Note
                        });

                    await _context.SaveChangesAsync();
                }
                await transaction.CommitAsync();

                return MapOrderToResponse(order);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        // Mapping
        private static OrderResponseDto MapOrderToResponse(
            Order order)
        {
            return new OrderResponseDto
            {
                Id = order.Id,
                OrderCode = order.OrderCode,
                CustomerId = order.CustomerId,
                StoreId = order.StoreId,
                AddressId = order.AddressId,
                Status = order.Status,
                Subtotal = order.Subtotal,
                DeliveryFee = order.DeliveryFee,
                Total = order.Total,
                PaymentMethod = order.PaymentMethod,

                Items = order.OrderLines.Select(line =>
                    new OrderLineResponseDto
                    {
                        ProductId = line.ProductId,
                        ProductName = line.ProductName,
                        Quantity = line.Quantity,
                        UnitPrice = line.UnitPrice,
                        LineTotal = line.Quantity * line.UnitPrice
                    }).ToList()
            };
        }
    }
}
