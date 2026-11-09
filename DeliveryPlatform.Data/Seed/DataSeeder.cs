using Bogus;
using DeliveryPlatform.Data.Context;
using DeliveryPlatform.Data.Entities;
using DeliveryPlatform.Data.Enums;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace DeliveryPlatform.Data.Seed;

public class DataSeeder
{
    private readonly DeliveryPlatformDbContext _context;

    private const int BatchSize = 10_000;
    private const int StoreCount = 50;
    private const int ProductCount = 40_000;
    private const int CustomerCount = 20_000;
    private const int ExtraAddressCount = 15_000;   // 20,000 + 15,000 = 35,000 addresses
    private const int OrderTarget = 300_000;
    private const decimal DeliveryFee = 2.50m;      // flat, same as the README setting

    // Per-table totals: rows inserted and time spent inside the bulk insert.
    private readonly Dictionary<string, (int Rows, TimeSpan Time)> _timings = new();

    public DataSeeder(DeliveryPlatformDbContext context)
    {
        _context = context;
    }

    public async Task ReseedAsync(CancellationToken cancellationToken = default)
    {
        var totalTimer = Stopwatch.StartNew();
        _timings.Clear();

        Console.WriteLine("Starting database reseed...");// بدايه العمل 

        await ClearExistingDataAsync(cancellationToken);//يستدعي دالة مسؤولة عن تنظيف البيانات القديمة قبل إنشاء البيانات الجديدة.


        var stores = await SeedStoresAsync(cancellationToken);
        var categories = await SeedCategoriesAsync(cancellationToken);
        var customers = await SeedCustomersAsync(cancellationToken);
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
        _context.Database.SetCommandTimeout(TimeSpan.FromMinutes(10));//تحديد اقى وقت مسموح لتنفيذ امر sql 10 دقائق
        // Children before parents. ExecuteDelete runs a real DELETE (it bypasses the interceptor).
        await _context.CartLineOptions.IgnoreQueryFilters().ExecuteDeleteAsync(cancellationToken);
        await _context.OrderLineOptions.IgnoreQueryFilters().ExecuteDeleteAsync(cancellationToken);
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
        await _context.Stores.IgnoreQueryFilters().ExecuteDeleteAsync(cancellationToken);
        await _context.Customers.IgnoreQueryFilters().ExecuteDeleteAsync(cancellationToken);

        Console.WriteLine("Old data cleared.");
    }

    private async Task<List<Store>> SeedStoresAsync(CancellationToken cancellationToken)
    {
        var stores = new Faker<Store>()
            .RuleFor(x => x.Name, f => f.Company.CompanyName())//اسم عشوائي لشركه
            .RuleFor(x => x.Latitude, f => Math.Round(f.Random.Decimal(31.5m, 32.1m), 6))
            //ااحداثيات عرض عشوائيه بين 31.5و32.1

            .RuleFor(x => x.Longitude, f => Math.Round(f.Random.Decimal(35.7m, 36.1m), 6))
            //ااحداثيات طول عشوائيه بين 36.1و35.7
            .Generate(StoreCount);

        await BulkInsertAsync("Stores", stores, setOutputIdentity: true, cancellationToken);
        return stores;
        //ادخال المتاجر الى قاعده البيانات وارجاع القام بعد الادخال


    }

    private async Task<List<Category>> SeedCategoriesAsync(CancellationToken cancellationToken)
    {
        var faker = new Faker();//توليد البيانات الوهميه

        var categories = PriceRanges.Keys.Select(name => new Category
        {
            Name = name,
            Description = faker.Lorem.Sentence()
        }).ToList();
        /*
         هوPriceRanges.Keys: يأخذ أسماء التصنيفات الموجودة في قاموس PriceRanges.
Select: يحوّل كل اسم إلى كائن Category.
Name: يأخذ اسم التصنيف.
Description: ينشئ وصفاً عشوائياً.
ToList(): يحول النتائج إلى قائمة.
         
         */
        await BulkInsertAsync("Categories", categories, setOutputIdentity: true, cancellationToken);
        return categories;

        //ادخال البيانات على قاعد البيانات وارجاع القائمه
    }

    private async Task<List<Customer>> SeedCustomersAsync(CancellationToken cancellationToken)
        //انشاء العملاء وارجاع على شكل قائمه
    {
        var faker = new Faker();
       // نستخدمه لتوليد أسماء وبيانات عشوائية.
        var customers = new List<Customer>(CustomerCount);
        //انشاء قائمه تستوعب 20000 عميل 
        for (int i = 0; i < CustomerCount; i++)
        {
            customers.Add(new Customer
            {
                FirstName = faker.Name.FirstName(),
                LastName = faker.Name.LastName(),
                PhoneNumber = $"07{faker.Random.Replace("########")}",
                Email = $"customer{i + 1}@example.com"
            });
            /*
             * يولد اسماً أول عشوائياً.
يولد اسم عائلة عشوائياً.
ينشئ رقماً يبدأ بـ 07 ثم يضيف 8 أرقام عشوائية.
ينشئ إيميلاً فريداً لكل عميل.
             */
        }

        await BulkInsertAsync("Customers", customers, setOutputIdentity: true, cancellationToken);
        return customers;
       //ادخال البيانات على قاعد البيانات وارجاع القائمه
    }

    private async Task<List<Address>> SeedAddressesAsync(
        List<Customer> customers, CancellationToken cancellationToken)//cancellationToken: لإمكانية إلغاء العملية.

    {
        var faker = new Faker();
        string[] cities = { "Amman", "Irbid", "Zarqa", "Aqaba", "Madaba", "Salt", "Jerash", "Karak" };

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
        /*
         دالة محلية داخل SeedAddressesAsync، وظيفتها بناء عنوان جديد.
         */
        // Every customer gets exactly ONE default address ("Home").
        foreach (var customer in customers)
            addresses.Add(Build(customer, "Home", isDefault: true));
        //20000

        // Some customers also get a second, non-default address.
        foreach (var customer in customers.Take(ExtraAddressCount))//15000
            addresses.Add(Build(customer, faker.PickRandom("Work", "Other"), isDefault: false));
        //20000+15000=350000
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
        List<Store> stores, List<Category> categories, CancellationToken cancellationToken)
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
                // the running number keeps (StoreId, Name) unique
                Name = $"{faker.Commerce.ProductName()} {i + 1}",
                Description = faker.Commerce.ProductDescription(),
                Price = Math.Round(price, 2),
                // ~4% out of stock
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
            //يحدد حجم الدفعة الحالية، ولا يسمح لها بتجاوز العدد المتبقي من الطلبات.
        //ثم:

            var orders = new List<Order>(batch);
            var lines = new List<OrderLine>(batch * 4);
            var histories = new List<OrderStatusHistory>(batch * 5);
            

            for (int i = 0; i < batch; i++)
            {
                // Skewed: low indexes are picked far more often => a few heavy customers, most rare.
                var customer = customers[(int)(customers.Count * Math.Pow(rng.Double(), 3))];
                //يختار زبوناً عشوائياً،
                var store = stores[rng.Int(0, stores.Count - 1)];
                var storeProducts = productsByStore[store.Id];
                var address = faker.PickRandom(addressesByCustomer[customer.Id]);
                /*

                 يختار متجراً عشوائياً من قائمة المتاجر.
                يجلب المنتجات التابعة للمتجر الذي اخترناه فقط.
                يختار عنواناً عشوائياً من عناوين الزبون نفسه.
                 */
                var status = GenerateOrderStatus(rng);
                /*
                 
                 يستدعي دالة GenerateOrderStatus لتحديد حالة الطلب، مثل:
Pending
Accepted
Preparing
OutForDelivery
Delivered
Declined
Cancelled
                 */
                var inProgress = status is OrderStatus.Pending or OrderStatus.Accepted
                    or OrderStatus.Preparing or OrderStatus.OutForDelivery;
                //فحص حاله الطلب اذا كانت من الحالات ال 4 تكونtrue غير ذلك false
                // In-progress orders are fresh (last 36h); finished ones are spread over 12 months.
                var orderDate = inProgress
                    ? now.AddHours(-rng.Double() * 36)
                    : oldest.AddSeconds(rng.Double() * finishedSpanSeconds);
                /*
                 * تحديد تاريخ الطلب 
                 نستخدم العامل الثلاثي ? :، ومعناه:
إذا كان الطلب قيد التنفيذ: نختار تاريخاً خلال آخر 36 ساعة.
                 */
                var order = new Order
                {
                    OrderCode = GenerateOrderCode(orderDate, rng, usedCodes, ref codeCounter),
                    CustomerId = customer.Id,
                    StoreId = store.Id,
                    AddressId = address.Id,
                    Status = status,
                    PaymentMethod = faker.PickRandom<PaymentMethod>(),
                    DeliveryFee = DeliveryFee,
                    CreatedAtUtc = orderDate
                };
                //انشاء الطلب وتعبه البيانات

                // 1..8 distinct products, most orders small.
                int lineTarget = 1 + (int)(Math.Pow(rng.Double(), 2) * 8);
                // تحديد عدد المنتجات داخل الطلب الواحد 
                var chosen = new HashSet<int>();
                /*
                 منع تكرار المنتجات
                HashSet<int>();
                لا يسمح بتكرار العناصر
                 
                 */
                while (chosen.Count < lineTarget)
                    chosen.Add(rng.Int(0, storeProducts.Count - 1));

                decimal subtotal = 0;
                foreach (var index in chosen)
                {
                    var product = storeProducts[index];
                    int quantity = rng.Int(1, 4);

                    /*
                     - "subtotal": متغير لحساب مجموع أسعار المنتجات.
- "foreach": يمر على المنتجات المختارة.
- "product": المنتج الحالي.
- "quantity": كمية عشوائية من 1 إلى 4.

                     
                     */

                    lines.Add(new OrderLine
                    {
                        Order = order,
                        ProductId = product.Id,
                        ProductName = product.Name,   // snapshot
                        UnitPrice = product.Price,    // snapshot
                        Quantity = quantity,
                        CreatedAtUtc = orderDate
                    });
                    //حفظ الطلب 
                    subtotal += product.Price * quantity;// سعر الطلب * الكميه
                }

                order.Subtotal = subtotal;
                order.Total = subtotal + order.DeliveryFee;//مجموع المنتجات + التوصيل

                orders.Add(order);
                histories.AddRange(GenerateStatusHistory(order, status, orderDate, now, rng));
                /*
                 
                  "orders.Add(order)": إضافة الطلب إلى قائمة الطلبات.
- "GenerateStatusHistory": إنشاء سجل انتقالات حالات الطلب.
- "AddRange": إضافة جميع سجلات الحالات الناتجة إلى قائمة "histories".


                 
                 */


            }

            // Orders first: we need their generated Ids for the children.
            await BulkInsertAsync("Orders", orders, setOutputIdentity: true, cancellationToken);

            foreach (var line in lines)
            {
                line.OrderId = line.Order.Id;
                line.Order = null!;
            }//ربط order بي orderline
            await BulkInsertAsync("OrderLines", lines, setOutputIdentity: false, cancellationToken);

            foreach (var history in histories)
            {
                history.OrderId = history.Order.Id;
                history.Order = null!;
            }
            // ربط order بي orderstatushistory
            await BulkInsertAsync("OrderStatusHistory", histories, setOutputIdentity: false, cancellationToken);

            doneOrders += orders.Count;
            // زياده عدد الطلبات
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
        //تحديد حاله الطلب بنسبه محدده
    }

    // yyMM + 5 random digits + 3-digit counter (wraps after 999).
    // The HashSet only avoids useless retries; the unique index is the real guarantee.
    private static string GenerateOrderCode(
        DateTime date, Randomizer rng, HashSet<string> used, ref int counter)
    {

        /*
         Randomizer rng: توليد ارقام عشوائيه
          HashSet<string> used: حفظ الاكواد التي تم استخدامها مع منع تكرار نفس الكود
         */
        string code;
        do
        {

            //{date:yyMM} :السنه والشهر فقط
            //rng.Int(0, 99_999):D5 انشاء خمس ارقام عشريه عشوائيه
            //{counter % 1000:D3} 3 ارقام لعدد

            code = $"{date:yyMM}{rng.Int(0, 99_999):D5}{counter % 1000:D3}";
        }
        while (!used.Add(code));//مارح توقف غير لما يكون الكود جديد

        counter++;
        return code;
    }

    private static List<OrderStatusHistory> GenerateStatusHistory(//سجل تاريخي للحالات الطلب من البدايه حتى النهايه
        Order order, OrderStatus finalStatus, DateTime orderDate, DateTime now, Randomizer rng)
    {
        // انواع الحلات للطلب 
        OrderStatus[] road =
        {
            OrderStatus.Pending, OrderStatus.Accepted,
            OrderStatus.Preparing, OrderStatus.OutForDelivery
        };
        // اضافه حالات اخرى 
        OrderStatus[] statuses = finalStatus switch
        {
            OrderStatus.Delivered => road.Append(OrderStatus.Delivered).ToArray(),// اضافه حاله Delivered بعد الحالات الاوله 

            // Declined: stopped before leaving the store (from Pending, Accepted or Preparing).
            OrderStatus.Declined => road.Take(rng.Int(1, 3)).Append(OrderStatus.Declined).ToArray(),//حذف الحالات ال3 الاوله بعد تاكيد الطلب وخروج السائق فيصبح الطلب outfordriver

            // Cancelled: only from OutForDelivery.
            OrderStatus.Cancelled => road.Append(OrderStatus.Cancelled).ToArray(),//اضافه بعد الحالات الاساسيه حاله الالغاء

            // In progress: the road up to the current status.
            _ => road.Take(Array.IndexOf(road, finalStatus) + 1).ToArray() //اذا مفي حالات غير الاساسيه ناخذ من بدايه المستر للنهايه
        };

        var histories = new List<OrderStatusHistory>(statuses.Length); 

        for (int i = 0; i < statuses.Length; i++)
        {
            var at = orderDate.AddMinutes(i * 20);//انتقال الحاله بعد 20 دقيقه من السابقه
            if (at > now) at = now;   // منع اي تسجيل في المستقبل

            histories.Add(new OrderStatusHistory
            {
                Order = order,
                Status = statuses[i],
                ChangedAtUtc = at,
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
        {//typeof(BaseEntity).IsAssignableFrom فحص اذا  الmodel يرث من BaseEntity
            var now = DateTime.UtcNow;

            foreach (var entity in entities.Cast<BaseEntity>())
            {
                if (entity.CreatedAtUtc == default)
                    entity.CreatedAtUtc = now;

                entity.IsDeleted = false;
                entity.DeletedAtUtc = null;
            }
        }

        var timer = Stopwatch.StartNew();//حساب وقت عمليه الادخال 

        await _context.BulkInsertAsync(//ادخال البيانات دفعه واحده 
            entities,
            new BulkConfig
            {
                BatchSize = BatchSize,//حجم الدفعه 10000
                SetOutputIdentity = setOutputIdentity//يحدد هل نسترجع ال id 
            },
            cancellationToken: cancellationToken);

        timer.Stop();//يوقف حساب الوقت بعد انتهاء العمليه

        _timings[table] = _timings.TryGetValue(table, out var old)
            ? (old.Rows + entities.Count, old.Time + timer.Elapsed)
            : (entities.Count, timer.Elapsed);
        /*
         تسجيل احصائيات كل جدول باستخدام timings
         */
        if (table is "Stores" or "Categories" or "Customers" or "Addresses" or "Products")
            Console.WriteLine($"{table}: {entities.Count:N0} rows | {timer.Elapsed:mm\\:ss\\.fff}");

        //عرض عدد الصفوف ووقت الادخال
    }
}