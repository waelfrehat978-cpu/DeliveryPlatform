using DeliveryPlatform.Data.Context;
using DeliveryPlatform.Data.Interceptors;
using DeliveryPlatform.Data.Seed;
using DeliveryPlatform.Service.Services;
using DeliveryPlatform.Service.Interfaces;
using Microsoft.EntityFrameworkCore;

using DeliveryPlatform.Api.Middleware;
using Microsoft.AspNetCore.Mvc;
using Serilog;

using DeliveryPlatform.Service.Options;
using Microsoft.Extensions.Options;
var builder = WebApplication.CreateBuilder(args);
// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
  .WriteTo.File(
    path: "Logs/log-.txt",
    rollingInterval: RollingInterval.Day,
    retainedFileCountLimit: 30
)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var errors = context.ModelState
                .Where(x => x.Value?.Errors.Count > 0)
                .ToDictionary(
                    x => x.Key,
                    x => x.Value!.Errors
                        .Select(e => e.ErrorMessage)
                        .ToArray()
                );

            return new BadRequestObjectResult(new
            {
                StatusCode = StatusCodes.Status400BadRequest,
                Message = "Validation failed.",
                Errors = errors,
                CorrelationId = context.HttpContext.TraceIdentifier
            });
        };
    });


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Configure<OrderOptions>(
    builder.Configuration.GetSection("Order"));

// Register AppSaveChangesInterceptor
builder.Services.AddScoped<AppSaveChangesInterceptor>();

// Register DbContext
builder.Services.AddDbContext<DeliveryPlatformDbContext>(
    (serviceProvider, options) =>
    {
        options.UseSqlServer(
       builder.Configuration.GetConnectionString("DefaultConnection"));

        if (builder.Environment.IsDevelopment())
        {
            options.EnableSensitiveDataLogging();
            options.LogTo(
                message => Log.Information("{EFCoreSQL}", message),
                LogLevel.Information);
        }

        options.AddInterceptors(
            serviceProvider.GetRequiredService<AppSaveChangesInterceptor>());
    });

// Register Services
builder.Services.AddScoped<IStoreService, StoreService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IAddressService, AddressService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IReportService, ReportService>();

// Register DataSeeder
builder.Services.AddScoped<IDataSeeder, DataSeeder>();

var app = builder.Build();

// Run the seeder only when started with: dotnet run -- --reseed
if (args.Contains("--reseed"))
{
    using var scope = app.Services.CreateScope();
    var seeder = scope.ServiceProvider.GetRequiredService<IDataSeeder>();

    var orderOptions = scope.ServiceProvider
        .GetRequiredService<IOptions<OrderOptions>>()
        .Value;

    await seeder.ReseedAsync(orderOptions.DeliveryFee);
    return;
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseSerilogRequestLogging();

app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run();