using DeliveryPlatform.Data.Context;
using DeliveryPlatform.Data.Interceptors;
using DeliveryPlatform.Data.Seed;
using DeliveryPlatform.Service.Services;
using DeliveryPlatform.Service.Interfaces;
using Microsoft.EntityFrameworkCore;
using DeliveryPlatform.Data.Entities;
using Microsoft.AspNetCore.Identity;
using DeliveryPlatform.Api.Middleware;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Microsoft.AspNetCore.Authorization;
using DeliveryPlatform.Service.Options;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi;
using DeliveryPlatform.Data.Interfaces;
using DeliveryPlatform.Api.Services;
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
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "Enter your JWT Access Token."
    });

    options.AddSecurityRequirement(document =>
        new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference("Bearer", document)] = []
        });
});
builder.Services.Configure<OrderOptions>(
    builder.Configuration.GetSection("Order"));
builder.Services.Configure<JwtOptions>(
    builder.Configuration.GetSection("Jwt"));
builder.Services.Configure<OtpOptions>(
    builder.Configuration.GetSection("Otp"));
builder.Services.Configure<SeedOptions>(
    builder.Configuration.GetSection("Seed"));
var jwtOptions = builder.Configuration
    .GetSection("Jwt")
    .Get<JwtOptions>()!;

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.MapInboundClaims = false;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtOptions.Issuer,

            ValidateAudience = true,
            ValidAudience = jwtOptions.Audience,

            ValidateLifetime = true,

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtOptions.Key)),

            ClockSkew = TimeSpan.Zero,

            NameClaimType = "sub",
            RoleClaimType = "role"
        };
    });
builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});
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
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<PasswordService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddSingleton<IClockService, ClockService>();
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddScoped<ISmsSender, LogSmsSender>();
builder.Services.AddScoped<IOtpService, OtpService>();
builder.Services.AddScoped<IResourceAuthorizationService,ResourceAuthorizationService>();
builder.Services.AddScoped<IRefreshTokenService,RefreshTokenService>();
// Register DataSeeder
builder.Services.AddScoped<IDataSeeder, DataSeeder>();
builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<ICurrentUser, CurrentUser>();
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
app.UseAuthentication();

app.UseAuthorization();
app.MapControllers();

app.Run();