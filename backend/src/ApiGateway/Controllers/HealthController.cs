using Microsoft.AspNetCore.Mvc;

namespace ECommerce.ApiGateway.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    private readonly ILogger<HealthController> _logger;
    private readonly HttpClient _httpClient;

    public HealthController(ILogger<HealthController> logger, HttpClient httpClient)
    {
        _logger = logger;
        _httpClient = httpClient;
    }

    [HttpGet]
    public IActionResult Get()
    {
        var response = new
        {
            Service = "API Gateway",
            Status = "Healthy",
            Timestamp = DateTime.UtcNow,
            Version = "1.0.0",
            Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"
        };

        _logger.LogInformation("Health check requested for API Gateway");
        return Ok(response);
    }

    [HttpGet("services")]
    public async Task<IActionResult> GetServicesHealth()
    {
        var services = new Dictionary<string, object>();

        // Test User Service
        try
        {
            var userResponse = await _httpClient.GetAsync("http://localhost:7001/api/health");
            services["UserService"] = new
            {
                Status = userResponse.IsSuccessStatusCode ? "Healthy" : "Unhealthy",
                Url = "http://localhost:7001",
                ResponseTime = "N/A"
            };
        }
        catch (Exception ex)
        {
            services["UserService"] = new
            {
                Status = "Unhealthy",
                Url = "http://localhost:7001",
                Error = ex.Message
            };
        }

        // Test Product Service
        try
        {
            var productResponse = await _httpClient.GetAsync("http://localhost:7002/api/health");
            services["ProductService"] = new
            {
                Status = productResponse.IsSuccessStatusCode ? "Healthy" : "Unhealthy",
                Url = "http://localhost:7002",
                ResponseTime = "N/A"
            };
        }
        catch (Exception ex)
        {
            services["ProductService"] = new
            {
                Status = "Unhealthy",
                Url = "http://localhost:7002",
                Error = ex.Message
            };
        }

        // Test other services (placeholder)
        services["OrderService"] = new { Status = "Not Implemented", Url = "http://localhost:7003" };
        services["PaymentService"] = new { Status = "Not Implemented", Url = "http://localhost:7004" };

        var response = new
        {
            Gateway = "API Gateway",
            Status = "Healthy",
            Timestamp = DateTime.UtcNow,
            Services = services
        };

        _logger.LogInformation("Services health check requested");
        return Ok(response);
    }

    [HttpGet("test-user-service")]
    public async Task<IActionResult> TestUserService()
    {
        try
        {
            _logger.LogInformation("Testing communication with User Service...");
            
            var response = await _httpClient.GetAsync("http://localhost:7001/api/health/detailed");
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                
                var result = new
                {
                    Gateway = "API Gateway",
                    TestResult = "Success",
                    UserServiceResponse = content,
                    Timestamp = DateTime.UtcNow
                };

                _logger.LogInformation("Successfully communicated with User Service");
                return Ok(result);
            }
            else
            {
                _logger.LogWarning("User Service returned status code: {StatusCode}", response.StatusCode);
                return StatusCode((int)response.StatusCode, new { Error = "User Service returned error" });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error communicating with User Service");
            return StatusCode(500, new { Error = ex.Message });
        }
    }

    [HttpGet("test-product-service")]
    public async Task<IActionResult> TestProductService()
    {
        try
        {
            _logger.LogInformation("Testing communication with Product Service...");
            
            var response = await _httpClient.GetAsync("http://localhost:7002/api/health");
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                
                var result = new
                {
                    Gateway = "API Gateway",
                    TestResult = "Success",
                    ProductServiceResponse = content,
                    Timestamp = DateTime.UtcNow
                };

                _logger.LogInformation("Successfully communicated with Product Service");
                return Ok(result);
            }
            else
            {
                _logger.LogWarning("Product Service returned status code: {StatusCode}", response.StatusCode);
                return StatusCode((int)response.StatusCode, new { Error = "Product Service returned error" });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error communicating with Product Service");
            return StatusCode(500, new { Error = ex.Message });
        }
    }
}
