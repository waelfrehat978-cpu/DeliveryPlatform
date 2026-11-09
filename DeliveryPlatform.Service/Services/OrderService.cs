using System.Data;
using DeliveryPlatform.Data.Context;
using DeliveryPlatform.Data.Entities;
using DeliveryPlatform.Data.Enums;
using DeliveryPlatform.Service.DTOs.Orders;
using DeliveryPlatform.Service.Interfaces;
using Microsoft.EntityFrameworkCore;
using DeliveryPlatform.Service.Exceptions;

namespace DeliveryPlatform.Service.Services
{
    public class OrderService : IOrderService
    {
        private readonly DeliveryPlatformDbContext _context;

        public OrderService(DeliveryPlatformDbContext context)
        {
            _context = context;
        }

        private static readonly Dictionary<OrderStatus, OrderStatus[]>
            AllowedTransitions = new()
            {
                {
                    OrderStatus.Pending,
                    new[]
                    {
                        OrderStatus.Accepted,
                        OrderStatus.Declined,
                        OrderStatus.Cancelled
                    }
                },
                {
                    OrderStatus.Accepted,
                    new[]
                    {
                        OrderStatus.Preparing,
                        OrderStatus.Cancelled
                    }
                },
                {
                    OrderStatus.Preparing,
                    new[]
                    {
                        OrderStatus.OutForDelivery
                    }
                },
                {
                    OrderStatus.OutForDelivery,
                    new[]
                    {
                        OrderStatus.Delivered
                    }
                }
            };

        // Create Order
        public async Task<OrderResponseDto> CreateOrderAsync(
            CreateOrderDto request)
        {
            if (request.Items == null || !request.Items.Any())
                throw new ValidationException(
                    "Order must contain at least one item.");

            if (!Enum.IsDefined(typeof(PaymentMethod), request.PaymentMethod))
                throw new ValidationException("Invalid payment method.");

            if (request.Items.Any(i => i.ProductId <= 0))
                throw new ValidationException(
                    "Product ID must be greater than zero.");

            var productIds = request.Items
                .Select(i => i.ProductId)
                .ToList();

            if (productIds.Distinct().Count() != productIds.Count)
                throw new ValidationException(
                    "Duplicate products are not allowed.");

            await using var transaction =
                await _context.Database.BeginTransactionAsync(
                    IsolationLevel.Serializable);

            try
            {
                var customerExists = await _context.Customers
                    .AnyAsync(c => c.Id == request.CustomerId);

                if (!customerExists)
                    throw new NotFoundException(
                        "Customer not found.");

                var storeExists = await _context.Stores
                    .AnyAsync(s => s.Id == request.StoreId);

                if (!storeExists)
                    throw new NotFoundException(
                        "Store not found.");

                if (request.AddressId.HasValue)
                {
                    var addressExists = await _context.Addresses
                        .AnyAsync(a =>
                            a.Id == request.AddressId.Value &&
                            a.CustomerId == request.CustomerId);

                    if (!addressExists)
                        throw new NotFoundException(
                            "Address not found.");
                }

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

                decimal deliveryFee = 0;

                var order = new Order
                {
                    OrderCode = Guid.NewGuid()
                        .ToString("N")[..12].ToUpper(),

                    CustomerId = request.CustomerId,
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
                        ChangedAtUtc = DateTime.UtcNow,
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
        public async Task<OrderResponseDto> GetOrderByIdAsync(
            int orderId)
        {
            if (orderId <= 0)
                throw new ValidationException(
                    "Order ID must be greater than 0.");
            var order = await _context.Orders
                .Include(o => o.OrderLines)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
                throw new NotFoundException(
                    "Order not found.");

            return MapOrderToResponse(order);
        }

        // Get Customer Orders
        public async Task<CustomerOrderHistoryDto> GetCustomerOrdersAsync(
            int customerId,
            int page,
            int pageSize)
        {
            if (customerId <= 0)
                throw new ValidationException(
                    "Customer ID must be greater than 0.");
            if (page < 1 || pageSize < 1 || pageSize > 100)
                throw new ValidationException(
                    "Invalid pagination values.");
            var customerExists = await _context.Customers
                .AnyAsync(c => c.Id == customerId);

            if (!customerExists)
                throw new NotFoundException(
                    "Customer not found.");

            var query = _context.Orders
                .Where(o => o.CustomerId == customerId);

            var totalCount = await query.CountAsync();

            var orders = await query
                .OrderByDescending(o => o.CreatedAtUtc)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Include(o => o.OrderLines)
                .ToListAsync();

            return new CustomerOrderHistoryDto
            {
                Items = orders
                    .Select(MapOrderToResponse)
                    .ToList(),

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
                    "Order ID must be greater than 0.");
            if (!Enum.IsDefined(typeof(OrderStatus), request.Status))
                throw new ValidationException(
                    "Invalid order status.");

            await using var transaction =
                await _context.Database.BeginTransactionAsync(
                    IsolationLevel.Serializable);

            try
            {
                var order = await _context.Orders
                    .Include(o => o.OrderLines)
                    .FirstOrDefaultAsync(o => o.Id == orderId);

                if (order == null)
                    throw new NotFoundException(
                        "Order not found.");

                if (!AllowedTransitions.TryGetValue(
                    order.Status, out var allowedStatuses) ||
                    !allowedStatuses.Contains(request.Status))
                {
                    throw new BusinessRuleException(
                        $"Cannot change order status from {order.Status} " +
                        $"to {request.Status}.");
                }

                // Return stock only when Declined
                if (request.Status == OrderStatus.Declined ||
                    request.Status == OrderStatus.Cancelled)
                {
                    foreach (var line in order.OrderLines)
                    {
                        var product = await _context.Products
                            .FirstOrDefaultAsync(
                                p => p.Id == line.ProductId);

                        if (product != null)
                            product.StockQuantity += line.Quantity;
                    }
                }

                order.Status = request.Status;

                _context.OrderStatusHistories.Add(
                    new OrderStatusHistory
                    {
                        OrderId = order.Id,
                        Status = request.Status,
                        ChangedAtUtc = DateTime.UtcNow,
                        Note = request.Note
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