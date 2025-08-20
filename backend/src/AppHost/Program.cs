var builder = DistributedApplication.CreateBuilder(args);

// Infrastructure Services
var sqlServer = builder.AddSqlServer("sqlserver")
    .WithDataVolume()
    .AddDatabase("ecommerce-users")
    .AddDatabase("ecommerce-orders");

var mongodb = builder.AddMongoDB("mongodb")
    .WithDataVolume()
    .AddDatabase("ecommerce-products");

var redis = builder.AddRedis("redis")
    .WithDataVolume();

var rabbitmq = builder.AddRabbitMQ("rabbitmq")
    .WithDataVolume();

// Microservices
var userService = builder.AddProject<Projects.ECommerce_User_API>("user-service")
    .WithReference(sqlServer.GetDatabase("ecommerce-users"))
    .WithReference(redis)
    .WithReference(rabbitmq);

var productService = builder.AddProject<Projects.ECommerce_Product_API>("product-service")
    .WithReference(mongodb.GetDatabase("ecommerce-products"))
    .WithReference(redis)
    .WithReference(rabbitmq);

var orderService = builder.AddProject<Projects.ECommerce_Order_API>("order-service")
    .WithReference(sqlServer.GetDatabase("ecommerce-orders"))
    .WithReference(redis)
    .WithReference(rabbitmq);

var paymentService = builder.AddProject<Projects.ECommerce_Payment_API>("payment-service")
    .WithReference(redis)
    .WithReference(rabbitmq);

var notificationService = builder.AddProject<Projects.ECommerce_Notification_API>("notification-service")
    .WithReference(rabbitmq);

var fileService = builder.AddProject<Projects.ECommerce_File_API>("file-service")
    .WithReference(redis);

// API Gateway
var apiGateway = builder.AddProject<Projects.ECommerce_ApiGateway>("api-gateway")
    .WithReference(userService)
    .WithReference(productService)
    .WithReference(orderService)
    .WithReference(paymentService)
    .WithReference(notificationService)
    .WithReference(fileService);

builder.Build().Run();
