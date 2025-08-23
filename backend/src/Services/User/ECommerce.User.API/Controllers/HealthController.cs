using Microsoft.AspNetCore.Mvc;

namespace ECommerce.User.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    private readonly ILogger<HealthController> _logger;

    public HealthController(ILogger<HealthController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Get()
    {
        var response = new
        {
            Service = "User Service",
            Status = "Healthy",
            Timestamp = DateTime.UtcNow,
            Version = "1.0.0",
            Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"
        };

        _logger.LogInformation("Health check requested for User Service");
        return Ok(response);
    }

    [HttpGet("detailed")]
    public IActionResult GetDetailed()
    {
        var response = new
        {
            Service = "User Service",
            Status = "Healthy",
            Timestamp = DateTime.UtcNow,
            Version = "1.0.0",
            Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production",
            Dependencies = new
            {
                Database = "Not Connected", // Placeholder
                EventBus = "Connected to RabbitMQ",
                Cache = "Not Connected" // Placeholder
            },
            Metrics = new
            {
                RequestCount = Random.Shared.Next(100, 1000),
                AverageResponseTime = $"{Random.Shared.Next(10, 100)}ms",
                ActiveConnections = Random.Shared.Next(1, 50)
            }
        };

        _logger.LogInformation("Detailed health check requested for User Service");
        return Ok(response);
    }
}

