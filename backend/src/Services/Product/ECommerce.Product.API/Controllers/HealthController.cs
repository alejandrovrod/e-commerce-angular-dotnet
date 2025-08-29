using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Product.API.Controllers;

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
    public IActionResult GetHealth()
    {
        return Ok(new 
        { 
            Status = "Product Service is healthy", 
            Timestamp = DateTime.UtcNow,
            Service = "ProductService",
            Version = "1.0.0"
        });
    }

    [HttpGet("detailed")]
    public IActionResult GetDetailedHealth()
    {
        return Ok(new
        {
            Status = "Healthy",
            Service = "ProductService",
            Version = "1.0.0",
            Timestamp = DateTime.UtcNow,
            Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development",
            MachineName = Environment.MachineName,
            ProcessId = Environment.ProcessId,
            Uptime = Environment.TickCount64
        });
    }
}







