using Projects;

var builder = DistributedApplication.CreateBuilder(args);

// Redis service - Configuración básica (health check implícito)
var redis = builder.AddRedis("redis")
	.WithEndpoint(port: 6379, name: "redis", targetPort: 6379)
	.WithEnvironment("REDIS_PASSWORD", "redis123");

// RabbitMQ service - Puertos estándar para acceso externo
var rabbitMQ = builder.AddRabbitMQ("rabbitmq")
	.WithEndpoint(port: 5672, name: "amqp", targetPort: 5672)      // Puerto estándar AMQP
	.WithEndpoint(port: 15672, name: "management", targetPort: 15672); // Puerto estándar Management

// Add microservices with Redis and RabbitMQ references
var userService = builder.AddProject<Projects.ECommerce_User_API>("userservice")
    .WithHttpHealthCheck("/health")
    .WithReference(redis)
    .WithReference(rabbitMQ);

var productService = builder.AddProject<Projects.ECommerce_Product_API>("productservice")
    .WithHttpHealthCheck("/health")
    .WithReference(redis)
    .WithReference(rabbitMQ);

var orderService = builder.AddProject<Projects.ECommerce_Order_API>("orderservice")
    .WithHttpHealthCheck("/health")
    .WithReference(redis)
    .WithReference(rabbitMQ);

var paymentService = builder.AddProject<Projects.ECommerce_Payment_API>("paymentservice")
    .WithHttpHealthCheck("/health")
    .WithReference(redis)
    .WithReference(rabbitMQ);

var notificationService = builder.AddProject<Projects.ECommerce_Notification_API>("notificationservice")
    .WithHttpHealthCheck("/health")
    .WithReference(redis)
    .WithReference(rabbitMQ);

var fileService = builder.AddProject<Projects.ECommerce_File_API>("fileservice")
    .WithHttpHealthCheck("/health")
    .WithReference(redis)
    .WithReference(rabbitMQ);

// Add API Gateway with all service references
builder.AddProject<Projects.ECommerce_ApiGateway>("apigateway")
    .WithHttpHealthCheck("/health")
    .WithReference(userService)
    .WithReference(productService)
    .WithReference(orderService)
    .WithReference(paymentService)
    .WithReference(notificationService)
    .WithReference(fileService)
    .WithReference(redis)
    .WithReference(rabbitMQ);

builder.Build().Run();
