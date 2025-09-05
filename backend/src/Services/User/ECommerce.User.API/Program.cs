using ECommerce.User.Infrastructure;
using ECommerce.User.Application;
using ECommerce.User.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Net;
using Serilog;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using FluentValidation;
using FluentValidation.AspNetCore;
using MassTransit;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using ECommerce.User.Application.Events;
using System.Globalization;

// Configure culture
CultureInfo.DefaultThreadCurrentCulture = new("es-MX");
CultureInfo.DefaultThreadCurrentUICulture = new("es-MX");

var builder = WebApplication.CreateBuilder(args);

// Configure port only for Railway deployment
if (Environment.GetEnvironmentVariable("RAILWAY_ENVIRONMENT") != null)
{
    var port = Environment.GetEnvironmentVariable("PORT") ?? "7001";
    var host = Environment.GetEnvironmentVariable("HOST") ?? "0.0.0.0";
    builder.WebHost.UseUrls($"http://{host}:{port}");
    Console.WriteLine($"User Service will start on port: {port} (Railway)");
}
else
{
    Console.WriteLine("User Service will use Aspire port configuration (Local)");
}

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.WithProperty("ServiceName", "UserService")
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "User Service API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Add Application and Infrastructure layers
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

// Entity Framework is configured in InfrastructureServiceCollection

// Add Redis
builder.Services.AddStackExchangeRedisCache(options =>
{
    var redisConfig = builder.Configuration.GetSection("Redis");
    options.Configuration = $"{redisConfig["Host"]}:{redisConfig["Port"]},password={redisConfig["Password"]}";
});

// Add JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey is required");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

// Add MassTransit for Event Bus with RabbitMQ
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
        
        // Configure message topology for events
        cfg.Message<UserRegisteredEvent>(e => e.SetEntityName("user-registered"));
        
        // Configure the endpoint for UserRegisteredEvent
        cfg.ReceiveEndpoint("user-registered", e =>
        {
            // Configure the endpoint for UserRegisteredEvent
        });
        
        cfg.ConfigureEndpoints(context);
        
        // Configure retry policy for connection failures
        cfg.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));
        cfg.UseCircuitBreaker(cb =>
        {
            cb.TrackingPeriod = TimeSpan.FromMinutes(1);
            cb.TripThreshold = 15;
            cb.ActiveThreshold = 10;
            cb.ResetInterval = TimeSpan.FromMinutes(5);
        });
    });
});

// Make MassTransit startup optional for development
builder.Services.Configure<MassTransitHostOptions>(options =>
{
    options.WaitUntilStarted = false;
    options.StartTimeout = TimeSpan.FromSeconds(10);
    options.StopTimeout = TimeSpan.FromSeconds(30);
});

// Add FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

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

// Add Health Checks
builder.Services.AddHealthChecks()
    .AddCheck("self", () => HealthCheckResult.Healthy("User Service is healthy"));

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection(); // Disabled for testing
app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

// Add network security middleware to block direct external access
app.Use(async (context, next) =>
{
    var remoteIp = context.Connection.RemoteIpAddress;
    var forwardedFor = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
    
    // Only allow connections from localhost (API Gateway) or forwarded requests
    if (remoteIp != null && 
        !IPAddress.IsLoopback(remoteIp) && 
        !remoteIp.Equals(IPAddress.Parse("127.0.0.1")) &&
        !remoteIp.Equals(IPAddress.Parse("::1")) &&
        string.IsNullOrEmpty(forwardedFor))
    {
        context.Response.StatusCode = 403;
        await context.Response.WriteAsync("Direct external access is not allowed. Use the API Gateway.", cancellationToken: context.RequestAborted);
        return;
    }
    
    await next();
});

// Add API Key authentication to block direct access
app.Use(async (context, next) =>
{
    try
    {
        // Skip authentication for health checks
        if (context.Request.Path.StartsWithSegments("/health"))
        {
            await next();
            return;
        }
        
        var apiKey = context.Request.Headers["X-API-Key"].FirstOrDefault();
        var expectedApiKey = builder.Configuration["Security:ServiceApiKey"] ?? "ecommerce-service-secret-key";
        
        if (string.IsNullOrEmpty(apiKey) || apiKey != expectedApiKey)
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("API Key required. Direct access is not allowed.", cancellationToken: context.RequestAborted);
            return;
        }
        
        await next();
    }
    catch (TaskCanceledException)
    {
        // Handle task cancellation gracefully (client disconnected, request aborted, etc.)
        // Don't log this as an error as it's expected behavior
        return;
    }
    catch (Exception ex)
    {
        // Log other unexpected errors
        Log.Error(ex, "Error in API Key middleware");
        context.Response.StatusCode = 500;
        await context.Response.WriteAsync("Internal server error", cancellationToken: context.RequestAborted);
    }
});

app.MapControllers();

// Add health check endpoint
app.MapHealthChecks("/health");

try
{
    Log.Information("Starting User Service");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "User Service terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
