# Use the official .NET 9 SDK image
FROM mcr.microsoft.com/dotnet/sdk:9.0

# Set working directory
WORKDIR /app

# Copy the entire backend structure
COPY . ./

# Restore dependencies for the solution
RUN dotnet restore backend/ECommerce.sln

# Build the specific API Gateway project
RUN dotnet build backend/src/ApiGateway/ECommerce.ApiGateway.csproj -c Release --no-restore

# Set working directory to the API Gateway project
WORKDIR /app/backend/src/ApiGateway

# Expose port
EXPOSE 8080

# Set environment variables
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:8080

# Run the application
CMD ["dotnet", "run", "--configuration", "Release"]
