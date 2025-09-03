using Serilog;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;
using Yarp.ReverseProxy.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Load modular configuration files
builder.Configuration
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile("appsettings.Routes.json", optional: false, reloadOnChange: true)
    .AddJsonFile("appsettings.Clusters.json", optional: false, reloadOnChange: true)
    .AddJsonFile("appsettings.Security.json", optional: false, reloadOnChange: true)
    .AddJsonFile("appsettings.Logging.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.WithProperty("ServiceName", "ApiGateway")
    .CreateLogger();

builder.Host.UseSerilog();

// Add Aspire service defaults - will add later
//builder.AddServiceDefaults(); // Temporalmente comentado hasta resolver la referencia

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddHttpClient();

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

// Add CORS - Very permissive for development
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
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

// Add Rate Limiting
builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.User.Identity?.Name ?? httpContext.Request.Headers.Host.ToString(),
            factory: partition => new FixedWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = 100,
                Window = TimeSpan.FromMinutes(1)
            }));
    
    options.OnRejected = async (context, cancellationToken) =>
    {
        context.HttpContext.Response.StatusCode = 429;
        await context.HttpContext.Response.WriteAsync("Too many requests. Please try again later.", cancellationToken);
    };
});

// Add Health Checks
builder.Services.AddHealthChecks()
    .AddCheck("self", () => Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy("API Gateway is healthy"));

// Add OpenAPI/Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "E-Commerce API Gateway", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    
    // Add manual endpoints for User Service since Reverse Proxy endpoints are not auto-detected
    c.SwaggerDoc("user-service", new() { Title = "User Service", Version = "1" });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "E-Commerce API Gateway v1");
        c.RoutePrefix = string.Empty;
    });
}

// app.UseHttpsRedirection(); // Disabled for testing

// CORS must be the FIRST middleware
app.UseCors();

// Additional CORS middleware for debugging
app.Use(async (context, next) =>
{
    var origin = context.Request.Headers.Origin.FirstOrDefault();
    
    Log.Information("CORS Debug - Origin: {Origin}, Method: {Method}, Path: {Path}", 
        origin, context.Request.Method, context.Request.Path);
    
    // Add CORS headers to all responses
    if (!string.IsNullOrEmpty(origin))
    {
        context.Response.Headers.Add("Access-Control-Allow-Origin", origin);
        context.Response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS");
        context.Response.Headers.Add("Access-Control-Allow-Headers", "*");
        context.Response.Headers.Add("Access-Control-Allow-Credentials", "true");
    }
    
    if (context.Request.Method == "OPTIONS")
    {
        Log.Information("Handling OPTIONS request");
        context.Response.StatusCode = 200;
        await context.Response.WriteAsync("");
        return;
    }
    
    await next();
});

// Authentication & Authorization - Temporarily disabled for CORS testing
// app.UseAuthentication();
// app.UseAuthorization();

// Request Logging Middleware
app.Use(async (context, next) =>
{
    Log.Information("=== Gateway Request ===");
    Log.Information("Method: {Method}", context.Request.Method);
    Log.Information("Path: {Path}", context.Request.Path);
    Log.Information("QueryString: {QueryString}", context.Request.QueryString);
    Log.Information("Origin: {Origin}", context.Request.Headers.Origin.FirstOrDefault());
    Log.Information("RemoteIp: {RemoteIp}", context.Connection.RemoteIpAddress);
    
    // Log Reverse Proxy routing information
    var routeEndpoint = context.GetEndpoint();
    if (routeEndpoint != null)
    {
        Log.Information("Route matched: {RouteName}", routeEndpoint.DisplayName);
    }
    else
    {
        Log.Warning("No route matched for {Path}", context.Request.Path);
    }
    
    await next();
    
    Log.Information("=== Gateway Response ===");
    Log.Information("StatusCode: {StatusCode} for {Method} {Path}",
        context.Response.StatusCode,
        context.Request.Method,
        context.Request.Path);
    
    // Log response headers
    foreach (var header in context.Response.Headers)
    {
        Log.Information("Response Header: {Key} = {Value}", header.Key, string.Join(", ", header.Value.ToList()));
    }
});

// Map Controllers
app.MapControllers();

// Test endpoints for CORS
app.MapGet("/api/test", () => new { message = "CORS test successful", timestamp = DateTime.UtcNow });
app.MapGet("/api/product/test", () => new { message = "Product CORS test successful", timestamp = DateTime.UtcNow });
app.MapGet("/api/category/test", () => new { message = "Category CORS test successful", timestamp = DateTime.UtcNow });
app.MapGet("/api/brand/test", () => new { message = "Brand CORS test successful", timestamp = DateTime.UtcNow });
app.MapGet("/api/inventory/test", () => new { message = "Inventory CORS test successful", timestamp = DateTime.UtcNow });

// Add API Key middleware for Reverse Proxy
// app.Use(async (context, next) =>
// {
//     // Check if this is a Reverse Proxy request
//     if (context.Request.Path.StartsWithSegments("/api"))
//     {
//         var apiKey = context.RequestServices
//             .GetRequiredService<IConfiguration>()
//             .GetValue<string>("Security:ServiceApiKey") ?? "ecommerce-service-secret-key";
//         
//         // Add X-API-Key header to the request
//         context.Request.Headers["X-API-Key"] = apiKey;
//         
//         Log.Information("Added X-API-Key header for request: {Path}", context.Request.Path);
//     }
//     
//     await next();
// });

// Map Reverse Proxy
app.MapReverseProxy();

// Use Rate Limiting after Reverse Proxy - Temporarily disabled for CORS testing
// app.UseRateLimiter();

// Simple health check endpoint for Railway
app.MapGet("/health", () => new
{
    Status = "Healthy",
    Service = "E-Commerce API Gateway",
    Timestamp = DateTime.UtcNow,
    Environment = app.Environment.EnvironmentName
});

// Default endpoint
app.MapGet("/", () => new
{
    Service = "E-Commerce API Gateway",
    Version = "1.0.0",
    Environment = app.Environment.EnvironmentName,
    Timestamp = DateTime.UtcNow
});

try
{
    Log.Information("Starting API Gateway");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "API Gateway terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
