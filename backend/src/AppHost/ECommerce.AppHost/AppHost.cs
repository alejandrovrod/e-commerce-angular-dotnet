using Projects;

var builder = DistributedApplication.CreateBuilder(args);

// Para Railway: Solo ejecutar el API Gateway
// Los microservicios se ejecutarán como servicios separados en Railway

// Add API Gateway only (no microservices)
builder.AddProject<Projects.ECommerce_ApiGateway>("apigateway")
    .WithHttpHealthCheck("/health");

builder.Build().Run();
