# Use the official .NET 9 SDK image
FROM mcr.microsoft.com/dotnet/sdk:9.0

# Set working directory
WORKDIR /app

# Copy the API Gateway project files
COPY . ./

# Restore dependencies for the specific project and its references
RUN dotnet restore ECommerce.ApiGateway.csproj

# Build the specific API Gateway project
RUN dotnet build ECommerce.ApiGateway.csproj -c Release --no-restore

# Expose port
EXPOSE 8080

# Set environment variables
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:8080

# Note: Environment variables are now handled dynamically in the application code

# Run the application
CMD ["dotnet", "run", "--configuration", "Release"]
