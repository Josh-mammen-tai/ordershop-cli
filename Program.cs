using Microsoft.EntityFrameworkCore;
using OrderShop.Data;
using OrderShop.Data.Repositories;
using OrderShop.Services;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// EF Core context (SQLite for the demo).
builder.Services.AddDbContext<ShopDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("Shop") ?? "Data Source=shop.db"));

// Data layer — repositories.
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IReviewRepository, ReviewRepository>();

// Business-flow services.
builder.Services.AddScoped<PricingService>();
builder.Services.AddScoped<PaymentService>();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<InventoryService>();
builder.Services.AddScoped<CheckoutService>();
builder.Services.AddScoped<FulfillmentService>();
builder.Services.AddScoped<RefundService>();
builder.Services.AddScoped<OrderCancellationService>();
builder.Services.AddScoped<ReviewService>();
builder.Services.AddScoped<CustomerService>();

builder.Services.AddControllers();

WebApplication app = builder.Build();

app.MapControllers();
app.Run();
