using Projects;

var builder = DistributedApplication.CreateBuilder(args);

// Para Railway: Solo ejecutar el API Gateway
// Los microservicios se ejecutarán como servicios separados en Railway

// Disable Docker for Railway
builder.Services.Configure<Microsoft.Extensions.Hosting.HostOptions>(options =>
{
    options.ServicesStartConcurrently = false;
});

// Add API Gateway only (no microservices)
builder.AddProject<Projects.ECommerce_ApiGateway>("apigateway")
    .WithHttpHealthCheck("/health")
    .WithEnvironment("ASPNETCORE_URLS", "http://+:18888")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Production");

builder.Build().Run();
