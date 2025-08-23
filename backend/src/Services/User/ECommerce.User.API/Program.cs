using ECommerce.User.Infrastructure;
using ECommerce.User.Application;
using Serilog;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using FluentValidation;
using FluentValidation.AspNetCore;
using MassTransit;
using Microsoft.Extensions.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.WithProperty("ServiceName", "UserService")
    .CreateLogger();

builder.Host.UseSerilog();

// Add Aspire service defaults - will add later
// builder.AddServiceDefaults(); // Temporalmente comentado hasta resolver la referencia

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

// Add Application and Infrastructure layers - will implement later
// builder.Services.AddApplicationServices();
// builder.Services.AddInfrastructureServices(builder.Configuration);

// Add Entity Framework - will configure later
// builder.AddSqlServerDbContext<UserDbContext>("UsersDb");

// Add Redis - will configure later
// builder.AddRedis("Redis");

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

// Add MassTransit for Event Bus with RabbitMQ - Temporarily disabled for demo
// builder.Services.AddMassTransit(x =>
// {
//     x.UsingRabbitMq((context, cfg) =>
//     {
//         var connectionString = builder.Configuration.GetConnectionString("EventBus") ?? "amqp://admin:password123@localhost:5672";
//         cfg.Host(connectionString);
//         cfg.ConfigureEndpoints(context);
//         
//         // Configure retry policy for connection failures
//         cfg.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));
//         cfg.UseCircuitBreaker(cb =>
//         {
//             cb.TrackingPeriod = TimeSpan.FromMinutes(1);
//             cb.TripThreshold = 15;
//             cb.ActiveThreshold = 10;
//             cb.ResetInterval = TimeSpan.FromMinutes(5);
//         });
//     });
// });

// // Make MassTransit startup optional for development
// builder.Services.Configure<MassTransitHostOptions>(options =>
// {
//     options.WaitUntilStarted = false;
//     options.StartTimeout = TimeSpan.FromSeconds(10);
//     options.StopTimeout = TimeSpan.FromSeconds(30);
// });

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

app.MapControllers();

// Map Aspire default endpoints - will add later
// app.MapDefaultEndpoints();

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
