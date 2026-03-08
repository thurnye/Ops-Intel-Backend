using System.Text;
using OperationIntelligence.Api.Middlewares;
using OperationIntelligence.Core;
using OperationIntelligence.Core.Helpers;
using OperationIntelligence.Core.Interfaces;
using OperationIntelligence.Core.Services;
using OperationIntelligence.DB;
using OperationIntelligence.DB.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using FluentValidation;
using FluentValidation.AspNetCore;
using OperationIntelligence.Core.Validators;
using OperationIntelligence.Core.Security;
using OperationIntelligence.Api.Middlewares.Security;
using StackExchange.Redis;
using OperationIntelligence.Core.Cache;
using Microsoft.OpenApi;
using Microsoft.AspNetCore.Mvc.Controllers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();

// Swagger
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "OperationIntelligence API",
        Version = "v1"
    });

    options.TagActionsBy(api =>
    {
        if (api.ActionDescriptor is not ControllerActionDescriptor cad)
            return new[] { "default" };

        var ns = cad.ControllerTypeInfo.Namespace ?? string.Empty;
        var marker = ns.Contains(".Controllers.", StringComparison.OrdinalIgnoreCase)
            ? ".Controllers."
            : ".Controller.";
        var index = ns.IndexOf(marker, StringComparison.OrdinalIgnoreCase);

        if (index < 0)
            return new[] { cad.ControllerName.ToLowerInvariant() };

        var group = ns[(index + marker.Length)..]
            .Split('.', StringSplitOptions.RemoveEmptyEntries)
            .FirstOrDefault();

        var top = string.IsNullOrWhiteSpace(group) ? "default" : group.ToLowerInvariant();
        var controller = cad.ControllerName.ToLowerInvariant();

        return new[] { $"{top}/{controller}" };
    });
});

// JWT
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ClockSkew = TimeSpan.Zero
    };
});

// Validation
builder.Services.AddControllers(options =>
{
    options.Filters.Add<ValidationResponseFilter>();
});
// Add FluentValidation 
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();


// =========== Repositories =================================

// Auth
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

// Inventory
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IBrandRepository, BrandRepository>();
builder.Services.AddScoped<IWarehouseRepository, WarehouseRepository>();
builder.Services.AddScoped<ISupplierRepository, SupplierRepository>();
builder.Services.AddScoped<IUnitOfMeasureRepository, UnitOfMeasureRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductImageRepository, ProductImageRepository>();
builder.Services.AddScoped<IProductSupplierRepository, ProductSupplierRepository>();
builder.Services.AddScoped<IInventoryStockRepository, InventoryStockRepository>();
builder.Services.AddScoped<IStockMovementRepository, StockMovementRepository>();


// ORDER
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderItemRepository, OrderItemRepository>();
builder.Services.AddScoped<IOrderStatusHistoryRepository, OrderStatusHistoryRepository>();
builder.Services.AddScoped<IOrderPaymentRepository, OrderPaymentRepository>();
builder.Services.AddScoped<IOrderImageRepository, OrderImageRepository>();
builder.Services.AddScoped<IOrderNoteRepository, OrderNoteRepository>();
builder.Services.AddScoped<IOrderAddressRepository, OrderAddressRepository>();


// =========== End Of  Repositories =================================

// =========== Services =================================
builder.Services.AddScoped<IAuthService, AuthService>();


// Inventory
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IInventoryStockService, InventoryStockService>();
builder.Services.AddScoped<IStockMovementService, StockMovementService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IBrandService, BrandService>();
builder.Services.AddScoped<IUnitOfMeasureService, UnitOfMeasureService>();
builder.Services.AddScoped<IWarehouseService, WarehouseService>();
builder.Services.AddScoped<ISupplierService, SupplierService>();
builder.Services.AddScoped<IProductImageService, ProductImageService>();
builder.Services.AddScoped<IProductSupplierService, ProductSupplierService>();
builder.Services.AddScoped<BotDetectionService>();

// Order
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IOrderItemService, OrderItemService>();
builder.Services.AddScoped<IOrderStatusHistoryService, OrderStatusHistoryService>();
builder.Services.AddScoped<IOrderPaymentService, OrderPaymentService>();
builder.Services.AddScoped<IOrderImageService, OrderImageService>();
builder.Services.AddScoped<IOrderNoteService, OrderNoteService>();
builder.Services.AddScoped<IOrderAddressService, OrderAddressService>();


builder.Services.AddValidatorsFromAssemblyContaining<RegisterRequestValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateProductRequestValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateOrderRequestValidator>();
builder.Services.AddHttpContextAccessor();


//JWT Service
builder.Services.AddScoped<JwtService>();

// =========== End Of  Repositories =================================


// DbContext
builder.Services.AddDbContext<OperationIntelligenceDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


//CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:5173",
                "https://localhost:3000"
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
            .WithExposedHeaders("X-Access-Token");
    });
});

// Redis
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration["Redis:Configuration"];
    options.InstanceName = builder.Configuration["Redis:InstanceName"];
});
builder.Services.AddSingleton<IConnectionMultiplexer>(
    ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("Redis") ?? "localhost:6379"));
builder.Services.AddScoped<CacheInvalidationService>();


var app = builder.Build();

// Dev-time schema bootstrap when migrations are not used.
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<OperationIntelligenceDbContext>();
    db.Database.EnsureCreated();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "OperationIntelligence API v1");
    });
}
app.UseCors("AllowFrontend");
app.UseHttpsRedirection();

app.UseMiddleware<SanitizationMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<ErrorHandlingMiddleware>();

app.MapControllers();

app.Run();

public partial class Program
{

}
