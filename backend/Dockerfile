# Use the official .NET 9 SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

# Set working directory
WORKDIR /src

# Copy solution file
COPY ECommerce.sln ./

# Copy all project files
COPY src/AppHost/ECommerce.AppHost/ECommerce.AppHost.csproj src/AppHost/ECommerce.AppHost/
COPY src/AppHost/ECommerce.ServiceDefaults/ECommerce.ServiceDefaults.csproj src/AppHost/ECommerce.ServiceDefaults/
COPY src/ApiGateway/ECommerce.ApiGateway.csproj src/ApiGateway/
COPY src/Services/Product/ECommerce.Product.API/ECommerce.Product.API.csproj src/Services/Product/ECommerce.Product.API/
COPY src/Services/Product/ECommerce.Product.Application/ECommerce.Product.Application.csproj src/Services/Product/ECommerce.Product.Application/
COPY src/Services/Product/ECommerce.Product.Domain/ECommerce.Product.Domain.csproj src/Services/Product/ECommerce.Product.Domain/
COPY src/Services/Product/ECommerce.Product.Infrastructure/ECommerce.Product.Infrastructure.csproj src/Services/Product/ECommerce.Product.Infrastructure/
COPY src/Services/User/ECommerce.User.API/ECommerce.User.API.csproj src/Services/User/ECommerce.User.API/
COPY src/Services/User/ECommerce.User.Application/ECommerce.User.Application.csproj src/Services/User/ECommerce.User.Application/
COPY src/Services/User/ECommerce.User.Domain/ECommerce.User.Domain.csproj src/Services/User/ECommerce.User.Domain/
COPY src/Services/User/ECommerce.User.Infrastructure/ECommerce.User.Infrastructure.csproj src/Services/User/ECommerce.User.Infrastructure/
COPY src/Services/Order/ECommerce.Order.API/ECommerce.Order.API.csproj src/Services/Order/ECommerce.Order.API/
COPY src/Services/Order/ECommerce.Order.Application/ECommerce.Order.Application.csproj src/Services/Order/ECommerce.Order.Application/
COPY src/Services/Order/ECommerce.Order.Domain/ECommerce.Order.Domain.csproj src/Services/Order/ECommerce.Order.Domain/
COPY src/Services/Order/ECommerce.Order.Infrastructure/ECommerce.Order.Infrastructure.csproj src/Services/Order/ECommerce.Order.Infrastructure/
COPY src/Services/Payment/ECommerce.Payment.API/ECommerce.Payment.API.csproj src/Services/Payment/ECommerce.Payment.API/
COPY src/Services/Payment/ECommerce.Payment.Application/ECommerce.Payment.Application.csproj src/Services/Payment/ECommerce.Payment.Application/
COPY src/Services/Payment/ECommerce.Payment.Domain/ECommerce.Payment.Domain.csproj src/Services/Payment/ECommerce.Payment.Domain/
COPY src/Services/Payment/ECommerce.Payment.Infrastructure/ECommerce.Payment.Infrastructure.csproj src/Services/Payment/ECommerce.Payment.Infrastructure/
COPY src/Services/File/ECommerce.File.API/ECommerce.File.API.csproj src/Services/File/ECommerce.File.API/
COPY src/Services/File/ECommerce.File.Application/ECommerce.File.Application.csproj src/Services/File/ECommerce.File.Application/
COPY src/Services/File/ECommerce.File.Domain/ECommerce.File.Domain.csproj src/Services/File/ECommerce.File.Domain/
COPY src/Services/File/ECommerce.File.Infrastructure/ECommerce.File.Infrastructure.csproj src/Services/File/ECommerce.File.Infrastructure/
COPY src/Services/Notification/ECommerce.Notification.API/ECommerce.Notification.API.csproj src/Services/Notification/ECommerce.Notification.API/
COPY src/BuildingBlocks/Common/ECommerce.BuildingBlocks.Common.csproj src/BuildingBlocks/Common/
COPY src/BuildingBlocks/EventBus/ECommerce.BuildingBlocks.EventBus.csproj src/BuildingBlocks/EventBus/
COPY src/BuildingBlocks/Security/ECommerce.BuildingBlocks.Security.csproj src/BuildingBlocks/Security/

# Restore dependencies
RUN dotnet restore

# Copy all source code
COPY . .

# Build the application
RUN dotnet build -c Release --no-restore

# Publish the AppHost
RUN dotnet publish src/AppHost/ECommerce.AppHost/ECommerce.AppHost.csproj -c Release -o /app/publish --no-restore

# Use the official .NET 9 runtime image for running
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime

# Set working directory
WORKDIR /app

# Copy published application
COPY --from=build /app/publish .

# Expose ports
EXPOSE 18888 18889 18890 18891 18892 18893 18894 18895

# Set environment variables
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:18888

# Start the AppHost
ENTRYPOINT ["dotnet", "ECommerce.AppHost.dll"]
