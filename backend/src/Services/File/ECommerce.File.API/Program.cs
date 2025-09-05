using Serilog;
using Microsoft.EntityFrameworkCore;
using ECommerce.File.Infrastructure.Data;
using ECommerce.File.Application.Interfaces;
using ECommerce.File.Infrastructure.Repositories;
using ECommerce.File.Infrastructure.UnitOfWork;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Globalization;

// Configure culture
CultureInfo.DefaultThreadCurrentCulture = new("es-MX");
CultureInfo.DefaultThreadCurrentUICulture = new("es-MX");

var builder = WebApplication.CreateBuilder(args);

// Configure port only for Railway deployment
if (Environment.GetEnvironmentVariable("RAILWAY_ENVIRONMENT") != null)
{
    var port = Environment.GetEnvironmentVariable("PORT") ?? "7005";
    var host = Environment.GetEnvironmentVariable("HOST") ?? "0.0.0.0";
    builder.WebHost.UseUrls($"http://{host}:{port}");
    Console.WriteLine($"File Service will start on port: {port} (Railway)");
}
else
{
    Console.WriteLine("File Service will use Aspire port configuration (Local)");
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
builder.Services.AddDbContext<FileDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("FilesDb")));

// Add MediatR
builder.Services.AddMediatR(cfg => {
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(ECommerce.File.Application.Commands.CreateFile.CreateFileCommand).Assembly);
});

// Add AutoMapper
builder.Services.AddAutoMapper(typeof(Program).Assembly);

// Add repositories and services
builder.Services.AddScoped<IFileRepository, FileRepository>();
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
app.MapGet("/health", () => Results.Ok(new { Status = "Healthy", Service = "File Service", Timestamp = DateTime.UtcNow }));

app.MapControllers();

try
{
    Log.Information("Starting File Service");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "File Service terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
