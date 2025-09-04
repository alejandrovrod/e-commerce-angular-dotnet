using ECommerce.Product.Application;
using ECommerce.Product.Infrastructure;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Serilog;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);

// Configure port only for Railway deployment
if (Environment.GetEnvironmentVariable("RAILWAY_ENVIRONMENT") != null)
{
    var port = Environment.GetEnvironmentVariable("PORT") ?? "7002";
    var host = Environment.GetEnvironmentVariable("HOST") ?? "0.0.0.0";
    builder.WebHost.UseUrls($"http://{host}:{port}");
    Console.WriteLine($"Product Service will start on port: {port} (Railway)");
}
else
{
    Console.WriteLine("Product Service will use Aspire port configuration (Local)");
}

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .Enrich.WithProperty("ServiceName", "ProductService")
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add Application and Infrastructure layers
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

// Add Redis Cache
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis") ?? "localhost:6379";
});

// Add MassTransit for Event Bus
builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        var rabbitMqConfig = builder.Configuration.GetSection("RabbitMQ");
        var host = rabbitMqConfig["Host"] ?? "localhost";
        var port = int.Parse(rabbitMqConfig["Port"] ?? "5672");
        var username = rabbitMqConfig["Username"] ?? "guest";
        var password = rabbitMqConfig["Password"] ?? "guest";
        
        var connectionString = $"amqp://{username}:{password}@{host}:{port}/";
        cfg.Host(new Uri(connectionString));
        cfg.ConfigureEndpoints(context);
    });
});

// Add Health Checks
builder.Services.AddHealthChecks()
    .AddCheck("self", () => HealthCheckResult.Healthy("Product Service is healthy"));

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();

// Add health check endpoint
app.MapHealthChecks("/health");

app.MapControllers();

try
{
    Log.Information("Starting Product Service");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Product Service terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
