using Serilog;
using System.Globalization;

// Configure culture
CultureInfo.DefaultThreadCurrentCulture = new("es-MX");
CultureInfo.DefaultThreadCurrentUICulture = new("es-MX");

var builder = WebApplication.CreateBuilder(args);

// Configure port for Railway
var port = Environment.GetEnvironmentVariable("PORT") ?? "7006";
var host = Environment.GetEnvironmentVariable("HOST") ?? "localhost";
builder.WebHost.UseUrls($"http://{host}:{port}");

// Log the port configuration
Console.WriteLine($"Notification Service will start on port: {port}");

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.WithProperty("ServiceName", "NotificationService")
    .CreateLogger();

builder.Host.UseSerilog();

// Add Aspire service defaults - will add later
// builder.AddServiceDefaults();

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
    Service = "Notification Service",
    Version = "1.0.0",
    Environment = app.Environment.EnvironmentName,
    Timestamp = DateTime.UtcNow
});

// Health check
app.MapGet("/health", () => Results.Ok(new { Status = "Healthy", Service = "NotificationService" }));

try
{
    Log.Information("Starting Notification Service");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Notification Service terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
