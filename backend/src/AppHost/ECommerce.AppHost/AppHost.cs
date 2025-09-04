using Projects;

var builder = DistributedApplication.CreateBuilder(args);

// Los microservicios se conectarán directamente a Railway
// No necesitamos ejecutar Redis y RabbitMQ localmente

// Add microservices
var userService = builder.AddProject<Projects.ECommerce_User_API>("userservice")
	.WithHttpHealthCheck("/health");

var productService = builder.AddProject<Projects.ECommerce_Product_API>("productservice")
	.WithHttpHealthCheck("/health");

var orderService = builder.AddProject<Projects.ECommerce_Order_API>("orderservice")
	.WithHttpHealthCheck("/health");

var paymentService = builder.AddProject<Projects.ECommerce_Payment_API>("paymentservice")
	.WithHttpHealthCheck("/health");

//var notificationService = builder.AddProject<Projects.ECommerce_Notification_API>("notificationservice")
//    .WithHttpHealthCheck("/health");

var fileService = builder.AddProject<Projects.ECommerce_File_API>("fileservice")
	.WithHttpHealthCheck("/health");

// Add API Gateway with all service references
builder.AddProject<Projects.ECommerce_ApiGateway>("apigateway")
	.WithHttpHealthCheck("/health")
	.WithReference(userService)
	.WithReference(productService)
	.WithReference(orderService)
	.WithReference(paymentService)
	//.WithReference(notificationService)
	.WithReference(fileService);

builder.Build().Run();