# Multi-stage build for .NET 9
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy the API Gateway project files
COPY . ./

# Restore dependencies for the specific project and its references
RUN dotnet restore ECommerce.ApiGateway.csproj

# Build and publish the API Gateway project
RUN dotnet publish ECommerce.ApiGateway.csproj -c Release -o /app/publish --no-restore

# Runtime stage with ICU support
FROM mcr.microsoft.com/dotnet/aspnet:9.0-jammy AS final

# Install ICU libraries for globalization support
RUN apt-get update && apt-get install -y libicu70 && rm -rf /var/lib/apt/lists/*

WORKDIR /app

# Copy the published app
COPY --from=build /app/publish .

# Expose port
EXPOSE 8080

# Set environment variables
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:8080
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

# Run the application
ENTRYPOINT ["dotnet", "ECommerce.ApiGateway.dll"]
