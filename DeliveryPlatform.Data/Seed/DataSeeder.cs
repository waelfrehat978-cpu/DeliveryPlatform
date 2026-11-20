
using Bogus;
using DeliveryPlatform.Data.Context;
using DeliveryPlatform.Data.Entities;
using DeliveryPlatform.Data.Enums;
using EFCore.BulkExtensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace DeliveryPlatform.Data.Seed;

public class DataSeeder : IDataSeeder
{
    private readonly DeliveryPlatformDbContext _context;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly SeedOptions _seedOptions;

    private const int BatchSize = 10_000;
    private const int StoreCount = 50;
    private const int ProductCount = 40_000;
    private const int CustomerCount = 20_000;
    private const int ExtraAddressCount = 15_000;
    private const int OrderTarget = 300_000;

    private decimal _deliveryFee;

    // Per-table totals: rows inserted and time spent inside the bulk insert.
    private readonly Dictionary<string, (int Rows, TimeSpan Time)> _timings = new();

    public DataSeeder(
        DeliveryPlatformDbContext context,
        IPasswordHasher<User> passwordHasher,
        IOptions<SeedOptions> seedOptions)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _seedOptions = seedOptions.Value;
    }

    public async Task ReseedAsync(
        decimal deliveryFee,
        CancellationToken cancellationToken = default)
    {
        _deliveryFee = deliveryFee;
        var totalTimer = Stopwatch.StartNew();
        _timings.Clear();

        Console.WriteLine("Starting database reseed...");

        await ClearExistingDataAsync(cancellationToken);

        var stores = await SeedStoresAsync(cancellationToken);
        var categories = await SeedCategoriesAsync(cancellationToken);
        var customers = await SeedCustomersAsync(cancellationToken);

        await SeedUsersAsync(customers, stores, cancellationToken);

        var addresses = await SeedAddressesAsync(customers, cancellationToken);
        var products = await SeedProductsAsync(stores, categories, cancellationToken);

        await SeedOrdersAsync(customers, addresses, stores, products, cancellationToken);

        totalTimer.Stop();

        Console.WriteLine();
        Console.WriteLine("================ Seed timings ================");
        Console.WriteLine($"{"Table",-22}{"Rows",12}{"Insert time",16}");

        foreach (var (table, (rows, time)) in _timings)
            Console.WriteLine($"{table,-22}{rows,12:N0}{time,16:mm\\:ss\\.fff}");

        Console.WriteLine("----------------------------------------------");
        Console.WriteLine($"Total duration (incl. generation): {totalTimer.Elapsed:mm\\:ss\\.fff}");
        Console.WriteLine("==============================================");
    }

    private async Task ClearExistingDataAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("Clearing old data...");

        _context.Database.SetCommandTimeout(TimeSpan.FromMinutes(10));

        // Children before parents. ExecuteDelete runs a real DELETE (it bypasses the interceptor).
        await _context.CartLineOptions.IgnoreQueryFilters().ExecuteDeleteAsync(cancellationToken);
        await _context.OrderLineOptions.IgnoreQueryFilters().ExecuteDeleteAsync(cancellationToken);
        await _context.RefreshTokens.IgnoreQueryFilters().ExecuteDeleteAsync(cancellationToken);
        await _context.OrderStatusHistories.IgnoreQueryFilters().ExecuteDeleteAsync(cancellationToken);
        await _context.CartLines.IgnoreQueryFilters().ExecuteDeleteAsync(cancellationToken);
        await _context.OrderLines.IgnoreQueryFilters().ExecuteDeleteAsync(cancellationToken);
        await _context.Carts.IgnoreQueryFilters().ExecuteDeleteAsync(cancellationToken);
        await _context.Orders.IgnoreQueryFilters().ExecuteDeleteAsync(cancellationToken);
        await _context.Addresses.IgnoreQueryFilters().ExecuteDeleteAsync(cancellationToken);
        await _context.ProductOptions.IgnoreQueryFilters().ExecuteDeleteAsync(cancellationToken);
        await _context.ProductOptionGroups.IgnoreQueryFilters().ExecuteDeleteAsync(cancellationToken);
        await _context.Products.IgnoreQueryFilters().ExecuteDeleteAsync(cancellationToken);
        await _context.Categories.IgnoreQueryFilters().ExecuteDeleteAsync(cancellationToken);

        // Delete users before their related stores and customers.
        await _context.Users.IgnoreQueryFilters().ExecuteDeleteAsync(cancellationToken);

        await _context.Stores.IgnoreQueryFilters().ExecuteDeleteAsync(cancellationToken);
        await _context.Customers.IgnoreQueryFilters().ExecuteDeleteAsync(cancellationToken);

        Console.WriteLine("Old data cleared.");
    }

    private async Task<List<Store>> SeedStoresAsync(CancellationToken cancellationToken)
    {
        var stores = new Faker<Store>()
            .RuleFor(x => x.Name, f => f.Company.CompanyName())
            .RuleFor(x => x.Latitude, f => Math.Round(f.Random.Decimal(31.5m, 32.1m), 6))
            .RuleFor(x => x.Longitude, f => Math.Round(f.Random.Decimal(35.7m, 36.1m), 6))
            .Generate(StoreCount);

        await BulkInsertAsync("Stores", stores, setOutputIdentity: true, cancellationToken);

        return stores;
    }

    private async Task<List<Category>> SeedCategoriesAsync(CancellationToken cancellationToken)
    {
        var faker = new Faker();

        var categories = PriceRanges.Keys.Select(name => new Category
        {
            Name = name,
            Description = faker.Lorem.Sentence()
        }).ToList();

        await BulkInsertAsync("Categories", categories, setOutputIdentity: true, cancellationToken);

        return categories;
    }

    private async Task<List<Customer>> SeedCustomersAsync(CancellationToken cancellationToken)
    {
        var faker = new Faker();
        var customers = new List<Customer>(CustomerCount);

        for (int i = 0; i < CustomerCount; i++)
        {
            customers.Add(new Customer
            {
                FirstName = faker.Name.FirstName(),
                LastName = faker.Name.LastName(),
                Email = $"customer{i + 1}@example.com"
            });
        }

        await BulkInsertAsync("Customers", customers, setOutputIdentity: true, cancellationToken);

        return customers;
    }

    private async Task SeedUsersAsync(
        List<Customer> customers,
        List<Store> stores,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(_seedOptions.AdminEmail) ||
            string.IsNullOrWhiteSpace(_seedOptions.AdminPassword) ||
            string.IsNullOrWhiteSpace(_seedOptions.PartnerPassword))
        {
            throw new InvalidOperationException(
                "Seed credentials are missing from configuration.");
        }

        var users = new List<User>(customers.Count + stores.Count + 1);

        // One Customer user for every customer.
        foreach (var customer in customers)
        {
            users.Add(new User
            {
                PhoneNumber = $"079{customer.Id:D7}",
                Role = UserRole.Customer,
                CustomerId = customer.Id
            });
        }

        // One Partner user for every store.
        foreach (var store in stores)
        {
            var partner = new User
            {
                Email = $"partner{store.Id}@example.com",
                Role = UserRole.Partner,
                StoreId = store.Id
            };

            partner.PasswordHash = _passwordHasher.HashPassword(
                partner,
                _seedOptions.PartnerPassword);

            users.Add(partner);
        }

        // Exactly one Admin user.
        var admin = new User
        {
            Email = _seedOptions.AdminEmail,
            Role = UserRole.Admin
        };

        admin.PasswordHash = _passwordHasher.HashPassword(
            admin,
            _seedOptions.AdminPassword);

        users.Add(admin);

        await BulkInsertAsync(
            "Users",
            users,
            setOutputIdentity: false,
            cancellationToken);
    }

    private async Task<List<Address>> SeedAddressesAsync(
        List<Customer> customers,
        CancellationToken cancellationToken)
    {
        var faker = new Faker();

        string[] cities =
        {
            "Amman", "Irbid", "Zarqa", "Aqaba",
            "Madaba", "Salt", "Jerash", "Karak"
        };

        var addresses = new List<Address>(CustomerCount + ExtraAddressCount);

        Address Build(Customer customer, string label, bool isDefault) => new()
        {
            CustomerId = customer.Id,
            Label = label,
            Street = faker.Address.StreetAddress(),
            City = faker.PickRandom(cities),
            Latitude = Math.Round(faker.Random.Decimal(29.5m, 33.3m), 6),
            Longitude = Math.Round(faker.Random.Decimal(34.9m, 39.3m), 6),
            IsDefault = isDefault
        };

        // Every customer gets exactly ONE default address ("Home").
        foreach (var customer in customers)
            addresses.Add(Build(customer, "Home", isDefault: true));

        // Some customers also get a second, non-default address.
        foreach (var customer in customers.Take(ExtraAddressCount))
            addresses.Add(Build(customer, faker.PickRandom("Work", "Other"), isDefault: false));

        await BulkInsertAsync("Addresses", addresses, setOutputIdentity: true, cancellationToken);

        return addresses;
    }

    private static readonly Dictionary<string, (decimal Min, decimal Max)> PriceRanges = new()
    {
        ["Electronics"] = (50, 1500),
        ["Mobile Phones"] = (100, 1200),
        ["Computers"] = (250, 2000),
        ["Clothing"] = (5, 100),
        ["Shoes"] = (15, 180),
        ["Home Appliances"] = (30, 900),
        ["Furniture"] = (40, 1500),
        ["Groceries"] = (0.5m, 30),
        ["Snacks"] = (0.25m, 10),
        ["Beverages"] = (0.5m, 8),
        ["Bakery"] = (0.5m, 15),
        ["Dairy"] = (0.5m, 12),
        ["Beauty"] = (3, 100),
        ["Skincare"] = (5, 150),
        ["Sports"] = (5, 300),
        ["Toys"] = (3, 150),
        ["Books"] = (2, 60),
        ["Stationery"] = (0.25m, 20),
        ["Automotive"] = (5, 500),
        ["Pet Supplies"] = (2, 150),
        ["Jewelry"] = (10, 1000),
        ["Watches"] = (10, 800),
        ["Accessories"] = (2, 100),
        ["Kitchenware"] = (2, 200),
        ["Gaming"] = (10, 1000),
        ["Baby Products"] = (2, 150),
        ["Cleaning Supplies"] = (0.5m, 30),
        ["Frozen Food"] = (1, 30),
        ["Health Products"] = (2, 100),
        ["Outdoor Equipment"] = (5, 500)
    };

    private async Task<List<Product>> SeedProductsAsync(
        List<Store> stores,
        List<Category> categories,
        CancellationToken cancellationToken)
    {
        var faker = new Faker();
        var products = new List<Product>(ProductCount);

        for (int i = 0; i < ProductCount; i++)
        {
            var category = faker.PickRandom(categories);
            var range = PriceRanges[category.Name];

            // Squared random => most prices sit in the lower part of the range, like real shelves.
            var price = range.Min + (range.Max - range.Min) * (decimal)Math.Pow(faker.Random.Double(), 2);

            products.Add(new Product
            {
                // The running number keeps (StoreId, Name) unique.
                Name = $"{faker.Commerce.ProductName()} {i + 1}",
                Description = faker.Commerce.ProductDescription(),
                Price = Math.Round(price, 2),
                // ~4% out of stock.
                StockQuantity = faker.Random.Double() < 0.04 ? 0 : faker.Random.Int(1, 500),
                StoreId = stores[i % stores.Count].Id,
                CategoryId = category.Id
            });
        }

        await BulkInsertAsync("Products", products, setOutputIdentity: true, cancellationToken);

        return products;
    }

    private async Task SeedOrdersAsync(
        List<Customer> customers,
        List<Address> addresses,
        List<Store> stores,
        List<Product> products,
        CancellationToken cancellationToken)
    {
        var faker = new Faker();
        var rng = faker.Random;

        var addressesByCustomer = addresses
            .GroupBy(x => x.CustomerId)
            .ToDictionary(x => x.Key, x => x.ToList());

        var productsByStore = products
            .GroupBy(x => x.StoreId)
            .ToDictionary(x => x.Key, x => x.ToList());

        var now = DateTime.UtcNow;
        var oldest = now.AddMonths(-12);
        var finishedSpanSeconds = (now.AddHours(-3) - oldest).TotalSeconds;

        var usedCodes = new HashSet<string>(OrderTarget);
        var codeCounter = 0;
        var doneOrders = 0;

        for (int start = 0; start < OrderTarget; start += BatchSize)
        {
            cancellationToken.ThrowIfCancellationRequested();

            int batch = Math.Min(BatchSize, OrderTarget - start);

            var orders = new List<Order>(batch);
            var lines = new List<OrderLine>(batch * 4);
            var histories = new List<OrderStatusHistory>(batch * 5);

            for (int i = 0; i < batch; i++)
            {
                // Skewed: low indexes are picked far more often => a few heavy customers, most rare.
                var customer = customers[(int)(customers.Count * Math.Pow(rng.Double(), 3))];

                var store = stores[rng.Int(0, stores.Count - 1)];
                var storeProducts = productsByStore[store.Id];
                var address = faker.PickRandom(addressesByCustomer[customer.Id]);

                var status = GenerateOrderStatus(rng);

                var inProgress = status is OrderStatus.Pending or OrderStatus.Accepted
                    or OrderStatus.Preparing or OrderStatus.OutForDelivery;

                // In-progress orders are fresh (last 36h); finished ones are spread over 12 months.
                var orderDate = inProgress
                    ? now.AddHours(-rng.Double() * 36)
                    : oldest.AddSeconds(rng.Double() * finishedSpanSeconds);

                var order = new Order
                {
                    OrderCode = GenerateOrderCode(orderDate, rng, usedCodes, ref codeCounter),
                    CustomerId = customer.Id,
                    StoreId = store.Id,
                    AddressId = address.Id,
                    Status = status,
                    PaymentMethod = faker.PickRandom<PaymentMethod>(),
                    DeliveryFee = _deliveryFee,
                    CreatedAtUtc = orderDate
                };

                // 1..8 distinct products, most orders small.
                int lineTarget = 1 + (int)(Math.Pow(rng.Double(), 2) * 8);

                var chosen = new HashSet<int>();

                while (chosen.Count < lineTarget)
                    chosen.Add(rng.Int(0, storeProducts.Count - 1));

                decimal subtotal = 0;

                foreach (var index in chosen)
                {
                    var product = storeProducts[index];
                    int quantity = rng.Int(1, 4);

                    lines.Add(new OrderLine
                    {
                        Order = order,
                        ProductId = product.Id,
                        ProductName = product.Name,
                        UnitPrice = product.Price,
                        Quantity = quantity,
                        CreatedAtUtc = orderDate
                    });

                    subtotal += product.Price * quantity;
                }

                order.Subtotal = subtotal;
                order.Total = subtotal + order.DeliveryFee;

                orders.Add(order);
                histories.AddRange(GenerateStatusHistory(order, status, orderDate, now, rng));
            }

            // Orders first: we need their generated Ids for the children.
            await BulkInsertAsync("Orders", orders, setOutputIdentity: true, cancellationToken);

            foreach (var line in lines)
            {
                line.OrderId = line.Order.Id;
                line.Order = null!;
            }

            await BulkInsertAsync("OrderLines", lines, setOutputIdentity: false, cancellationToken);

            foreach (var history in histories)
            {
                history.OrderId = history.Order.Id;
                history.Order = null!;
            }

            await BulkInsertAsync("OrderStatusHistory", histories, setOutputIdentity: false, cancellationToken);

            doneOrders += orders.Count;

            Console.WriteLine($"Orders: {doneOrders:N0}/{OrderTarget:N0}");
        }
    }

    // ~82% Delivered, 7% Declined, 3% Cancelled, 8% still in progress.
    private static OrderStatus GenerateOrderStatus(Randomizer rng)
    {
        int value = rng.Int(1, 100);

        return value switch
        {
            <= 82 => OrderStatus.Delivered,
            <= 89 => OrderStatus.Declined,
            <= 92 => OrderStatus.Cancelled,
            <= 95 => OrderStatus.Pending,
            <= 97 => OrderStatus.Accepted,
            <= 99 => OrderStatus.Preparing,
            _ => OrderStatus.OutForDelivery
        };
    }

    // yyMM + 5 random digits + 3-digit counter (wraps after 999).
    // The HashSet only avoids useless retries; the unique index is the real guarantee.
    private static string GenerateOrderCode(
        DateTime date,
        Randomizer rng,
        HashSet<string> used,
        ref int counter)
    {
        string code;

        do
        {
            code = $"{date:yyMM}{rng.Int(0, 99_999):D5}{counter % 1000:D3}";
        }
        while (!used.Add(code));

        counter++;

        return code;
    }

    private static List<OrderStatusHistory> GenerateStatusHistory(
        Order order,
        OrderStatus finalStatus,
        DateTime orderDate,
        DateTime now,
        Randomizer rng)
    {
        OrderStatus[] road =
        {
            OrderStatus.Pending,
            OrderStatus.Accepted,
            OrderStatus.Preparing,
            OrderStatus.OutForDelivery
        };

        var statuses = finalStatus switch
        {
            OrderStatus.Delivered => road.Append(OrderStatus.Delivered).ToArray(),

            // Declined: stopped before leaving the store (from Pending, Accepted or Preparing).
            OrderStatus.Declined => road.Take(rng.Int(1, 3)).Append(OrderStatus.Declined).ToArray(),

            // Cancelled: only from OutForDelivery.
            OrderStatus.Cancelled => road.Append(OrderStatus.Cancelled).ToArray(),

            // In progress: the road up to the current status.
            _ => road.Take(Array.IndexOf(road, finalStatus) + 1).ToArray()
        };

        var histories = new List<OrderStatusHistory>(statuses.Length);

        for (int i = 0; i < statuses.Length; i++)
        {
            var at = orderDate.AddMinutes(i * 20);

            if (at > now)
                at = now;

            histories.Add(new OrderStatusHistory
            {
                Order = order,
                OldStatus = i == 0 ? null : statuses[i - 1],
                Status = statuses[i],
                CreatedAtUtc = at
            });
        }

        return histories;
    }

    private async Task BulkInsertAsync<TEntity>(
        string table,
        IList<TEntity> entities,
        bool setOutputIdentity,
        CancellationToken cancellationToken)
        where TEntity : class
    {
        if (entities.Count == 0)
            return;

        // Bulk operations skip SaveChangesInterceptor, so fill the audit values here.
        // This seeder is the only place allowed to do that.
        if (typeof(BaseEntity).IsAssignableFrom(typeof(TEntity)))
        {
            var now = DateTime.UtcNow;

            foreach (var entity in entities.Cast<BaseEntity>())
            {
                if (entity.CreatedAtUtc == default)
                    entity.CreatedAtUtc = now;

                entity.IsDeleted = false;
                entity.DeletedAtUtc = null;
            }
        }

        var timer = Stopwatch.StartNew();

        await _context.BulkInsertAsync(
            entities,
            new BulkConfig
            {
                BatchSize = BatchSize,
                SetOutputIdentity = setOutputIdentity
            },
            cancellationToken: cancellationToken);

        timer.Stop();

        _timings[table] = _timings.TryGetValue(table, out var old)
            ? (old.Rows + entities.Count, old.Time + timer.Elapsed)
            : (entities.Count, timer.Elapsed);

        if (table is "Stores" or "Categories" or "Customers"
            or "Users" or "Addresses" or "Products")
        {
            Console.WriteLine($"{table}: {entities.Count:N0} rows | {timer.Elapsed:mm\\:ss\\.fff}");
        }
    }
}
