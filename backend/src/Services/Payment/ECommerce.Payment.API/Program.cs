using Serilog;
using Microsoft.EntityFrameworkCore;
using ECommerce.Payment.Infrastructure.Data;
using ECommerce.Payment.Application.Interfaces;
using ECommerce.Payment.Infrastructure.Repositories;
using ECommerce.Payment.Infrastructure.UnitOfWork;
using System.Text.Json.Serialization;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Configure port only for Railway deployment
if (Environment.GetEnvironmentVariable("RAILWAY_ENVIRONMENT") != null)
{
    var port = Environment.GetEnvironmentVariable("PORT") ?? "7004";
    var host = Environment.GetEnvironmentVariable("HOST") ?? "0.0.0.0";
    builder.WebHost.UseUrls($"http://{host}:{port}");
    Console.WriteLine($"Payment Service will start on port: {port} (Railway)");
}
else
{
    Console.WriteLine("Payment Service will use Aspire port configuration (Local)");
}

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Configure enums to be serialized as strings
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add Entity Framework with database connection
builder.Services.AddDbContext<PaymentDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("PaymentsDb")));

// Add MediatR
builder.Services.AddMediatR(cfg => {
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(ECommerce.Payment.Application.Commands.CreatePayment.CreatePaymentCommand).Assembly);
});

// Add AutoMapper
builder.Services.AddAutoMapper(typeof(Program).Assembly);

// Add repositories and services
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
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

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();

// Health check endpoint
app.MapGet("/health", () => Results.Ok(new { Status = "Healthy", Service = "Payment Service", Timestamp = DateTime.UtcNow }));

app.MapControllers();

try
{
    Log.Information("Starting Payment Service");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Payment Service terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
