# Multi-stage build for .NET 9
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy the Payment API project files
COPY . .

# Restore dependencies
RUN dotnet restore ECommerce.Payment.API.csproj

# Build and publish the Payment API project
RUN dotnet publish ECommerce.Payment.API.csproj -c Release -o /app/publish --no-restore

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final

# Install ICU libraries for globalization support
RUN apt-get update && apt-get install -y libicu && rm -rf /var/lib/apt/lists/*

WORKDIR /app

# Copy the published app
COPY --from=build /app/publish .

# Expose port
EXPOSE 7004

# Set environment variables
ENV ASPNETCORE_ENVIRONMENT=Production
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

# Run the application
ENTRYPOINT ["dotnet", "ECommerce.Payment.API.dll"]
