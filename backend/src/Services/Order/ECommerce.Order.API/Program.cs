using Serilog;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

// Configure culture
CultureInfo.DefaultThreadCurrentCulture = new("es-MX");
CultureInfo.DefaultThreadCurrentUICulture = new("es-MX");

var builder = WebApplication.CreateBuilder(args);

// Configure port only for Railway deployment
if (Environment.GetEnvironmentVariable("RAILWAY_ENVIRONMENT") != null)
{
    var port = Environment.GetEnvironmentVariable("PORT") ?? "7003";
    var host = Environment.GetEnvironmentVariable("HOST") ?? "0.0.0.0";
    builder.WebHost.UseUrls($"http://{host}:{port}");
    Console.WriteLine($"Order Service will start on port: {port} (Railway)");
}
else
{
    Console.WriteLine("Order Service will use Aspire port configuration (Local)");
}

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.WithProperty("ServiceName", "OrderService")
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add MediatR - register from both the API and Application assemblies
builder.Services.AddMediatR(cfg => {
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(ECommerce.Order.Application.Commands.CreateOrder.CreateOrderCommand).Assembly);
});

// Add Entity Framework with database connection
builder.Services.AddDbContext<ECommerce.Order.Infrastructure.Data.OrderDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("OrdersDb")));

// Add Repositories
builder.Services.AddScoped<ECommerce.Order.Application.Interfaces.IOrderRepository, ECommerce.Order.Infrastructure.Repositories.OrderRepository>();
builder.Services.AddScoped<ECommerce.Order.Application.Interfaces.IUnitOfWork, ECommerce.Order.Infrastructure.Repositories.UnitOfWork>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Default endpoint
app.MapGet("/", () => new
{
    Service = "Order Service",
    Version = "1.0.0",
    Environment = app.Environment.EnvironmentName,
    Timestamp = DateTime.UtcNow
});

// Health check
app.MapGet("/health", () => Results.Ok(new { Status = "Healthy", Service = "OrderService" }));

try
{
    Log.Information("Starting Order Service");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Order Service terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
