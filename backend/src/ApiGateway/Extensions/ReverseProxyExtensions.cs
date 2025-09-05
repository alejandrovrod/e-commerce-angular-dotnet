using Yarp.ReverseProxy.Configuration;
using Microsoft.Extensions.Primitives;

namespace ECommerce.ApiGateway.Extensions;

public static class ReverseProxyExtensions
{
    public static IServiceCollection AddDynamicReverseProxy(this IServiceCollection services, IConfiguration configuration)
    {
		// Obtener variables de entorno para los servicios

		var endpointsServices = configuration.GetSection("Services");

		var productServiceUrl = Environment.GetEnvironmentVariable("PRODUCT_SERVICE_URL") ?? (!string.IsNullOrEmpty(endpointsServices["PRODUCT_SERVICE_URL"]) ? endpointsServices["PRODUCT_SERVICE_URL"] : "http://localhost:7002");
        var userServiceUrl = Environment.GetEnvironmentVariable("USER_SERVICE_URL") ?? (!string.IsNullOrEmpty(endpointsServices["USER_SERVICE_URL"]) ? endpointsServices["USER_SERVICE_URL"] : "http://localhost:7001");
        var orderServiceUrl = Environment.GetEnvironmentVariable("ORDER_SERVICE_URL") ?? (!string.IsNullOrEmpty(endpointsServices["ORDER_SERVICE_URL"]) ? endpointsServices["ORDER_SERVICE_URL"] : "http://localhost:7003");
        var paymentServiceUrl = Environment.GetEnvironmentVariable("PAYMENT_SERVICE_URL") ?? (!string.IsNullOrEmpty(endpointsServices["PAYMENT_SERVICE_URL"]) ? endpointsServices["PAYMENT_SERVICE_URL"] : "http://localhost:7004");
        var fileServiceUrl = Environment.GetEnvironmentVariable("FILE_SERVICE_URL") ?? (!string.IsNullOrEmpty(endpointsServices["FILE_SERVICE_URL"]) ? endpointsServices["FILE_SERVICE_URL"] : "http://localhost:7005");

        Console.WriteLine("=== Configurando ReverseProxy dinámicamente ===");
        Console.WriteLine($"Product Service URL: {productServiceUrl}");
        Console.WriteLine($"User Service URL: {userServiceUrl}");
        Console.WriteLine($"Order Service URL: {orderServiceUrl}");
        Console.WriteLine($"Payment Service URL: {paymentServiceUrl}");
        Console.WriteLine($"File Service URL: {fileServiceUrl}");

        // Configurar rutas dinámicamente
        var routes = new[]
        {
            // Product Service routes
            new RouteConfig()
            {
                RouteId = "product-route",
                ClusterId = "product-cluster",
                Match = new RouteMatch { Path = "/api/product/{**catch-all}" },
                Transforms = new[]
                {
                    new Dictionary<string, string> { ["PathPattern"] = "/api/product/{**catch-all}" },
                    new Dictionary<string, string> { ["RequestHeader"] = "X-API-Key", ["Append"] = "ecommerce-service-secret-key" }
                },
               
            },
            new RouteConfig()
            {
                RouteId = "category-route",
                ClusterId = "product-cluster",
                Match = new RouteMatch { Path = "/api/category/{**catch-all}" },
                Transforms = new[]
                {
                    new Dictionary<string, string> { ["PathPattern"] = "/api/category/{**catch-all}" },
                    new Dictionary<string, string> { ["RequestHeader"] = "X-API-Key", ["Append"] = "ecommerce-service-secret-key" }
                }
            },
            new RouteConfig()
            {
                RouteId = "brand-route",
                ClusterId = "product-cluster",
                Match = new RouteMatch { Path = "/api/brand/{**catch-all}" },
                Transforms = new[]
                {
                    new Dictionary<string, string> { ["PathPattern"] = "/api/brand/{**catch-all}" },
                    new Dictionary<string, string> { ["RequestHeader"] = "X-API-Key", ["Append"] = "ecommerce-service-secret-key" }
                }
            },
            new RouteConfig()
            {
                RouteId = "inventory-route",
                ClusterId = "product-cluster",
                Match = new RouteMatch { Path = "/api/inventory/{**catch-all}" },
                Transforms = new[]
                {
                    new Dictionary<string, string> { ["PathPattern"] = "/api/inventory/{**catch-all}" },
                    new Dictionary<string, string> { ["RequestHeader"] = "X-API-Key", ["Append"] = "ecommerce-service-secret-key" }
                }
            },
            // User Service routes
            new RouteConfig()
            {
                RouteId = "user-auth-route",
                ClusterId = "user-cluster",
                Match = new RouteMatch { Path = "/api/auth/{**catch-all}" },
                Transforms = new[]
                {
                    new Dictionary<string, string> { ["PathPattern"] = "/api/auth/{**catch-all}" },
                    new Dictionary<string, string> { ["RequestHeader"] = "X-API-Key", ["Append"] = "ecommerce-service-secret-key" }
                }
            },
            new RouteConfig()
            {
                RouteId = "user-addresses-route",
                ClusterId = "user-cluster",
                Match = new RouteMatch { Path = "/api/addresses/{**catch-all}" },
                Transforms = new[]
                {
                    new Dictionary<string, string> { ["PathPattern"] = "/api/addresses/{**catch-all}" },
                    new Dictionary<string, string> { ["RequestHeader"] = "X-API-Key", ["Append"] = "ecommerce-service-secret-key" }
                }
            },
            new RouteConfig()
            {
                RouteId = "user-admin-route",
                ClusterId = "user-cluster",
                Match = new RouteMatch { Path = "/api/admin/{**catch-all}" },
                Transforms = new[]
                {
                    new Dictionary<string, string> { ["PathPattern"] = "/api/admin/{**catch-all}" },
                    new Dictionary<string, string> { ["RequestHeader"] = "X-API-Key", ["Append"] = "ecommerce-service-secret-key" }
                }
            },
            // Order Service routes
            new RouteConfig()
            {
                RouteId = "order-route",
                ClusterId = "order-cluster",
                Match = new RouteMatch { Path = "/api/orders/{**catch-all}" },
                Transforms = new[]
                {
                    new Dictionary<string, string> { ["PathPattern"] = "/api/orders/{**catch-all}" }
                }
            },
            // Payment Service routes
            new RouteConfig()
            {
                RouteId = "payment-route",
                ClusterId = "payment-cluster",
                Match = new RouteMatch { Path = "/api/payments/{**catch-all}" },
                Transforms = new[]
                {
                    new Dictionary<string, string> { ["PathPattern"] = "/api/payments/{**catch-all}" }
                }
            },
            // File Service routes
            new RouteConfig()
            {
                RouteId = "file-route",
                ClusterId = "file-cluster",
                Match = new RouteMatch { Path = "/api/files/{**catch-all}" },
                Transforms = new[]
                {
                    new Dictionary<string, string> { ["PathPattern"] = "/api/files/{**catch-all}" }
                }
            }
        };

        // Configurar clusters dinámicamente
        var clusters = new[]
        {
            new ClusterConfig()
            {
                ClusterId = "product-cluster",
                Destinations = new Dictionary<string, DestinationConfig>
                {
                    ["destination1"] = new DestinationConfig() { Address = productServiceUrl }
                },
                LoadBalancingPolicy = "PowerOfTwoChoices"
            },
            new ClusterConfig()
            {
                ClusterId = "user-cluster",
                Destinations = new Dictionary<string, DestinationConfig>
                {
                    ["destination1"] = new DestinationConfig() { Address = userServiceUrl }
                },
                LoadBalancingPolicy = "PowerOfTwoChoices"
            },
            new ClusterConfig()
            {
                ClusterId = "order-cluster",
                Destinations = new Dictionary<string, DestinationConfig>
                {
                    ["destination1"] = new DestinationConfig() { Address = orderServiceUrl }
                },
                LoadBalancingPolicy = "PowerOfTwoChoices"
            },
            new ClusterConfig()
            {
                ClusterId = "payment-cluster",
                Destinations = new Dictionary<string, DestinationConfig>
                {
                    ["destination1"] = new DestinationConfig() { Address = paymentServiceUrl }
                },
                LoadBalancingPolicy = "PowerOfTwoChoices"
            },
            new ClusterConfig()
            {
                ClusterId = "file-cluster",
                Destinations = new Dictionary<string, DestinationConfig>
                {
                    ["destination1"] = new DestinationConfig() { Address = fileServiceUrl }
                },
                LoadBalancingPolicy = "PowerOfTwoChoices"
            }
        };

        // Registrar el proveedor de configuración
        services.AddSingleton<IProxyConfigProvider>(new StaticConfigProvider(routes, clusters));

        // Agregar ReverseProxy
        services.AddReverseProxy();

        Console.WriteLine("=== ReverseProxy configurado dinámicamente ===");
        return services;
    }
}

// Proveedor de configuración estática
public class StaticConfigProvider : IProxyConfigProvider
{
    private readonly IProxyConfig _config;

    public StaticConfigProvider(RouteConfig[] routes, ClusterConfig[] clusters)
    {
        _config = new StaticConfig(routes, clusters);
    }

    public IProxyConfig GetConfig() => _config;
}

// Configuración estática
public class StaticConfig : IProxyConfig
{
    public IReadOnlyList<RouteConfig> Routes { get; }
    public IReadOnlyList<ClusterConfig> Clusters { get; }
    public IChangeToken ChangeToken => new CancellationChangeToken(new CancellationTokenSource().Token);

    public StaticConfig(RouteConfig[] routes, ClusterConfig[] clusters)
    {
        Routes = routes;
        Clusters = clusters;
    }
}
