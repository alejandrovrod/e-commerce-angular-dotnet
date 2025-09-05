# Multi-stage build for .NET 9
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy the User API project files
COPY . ./

# Restore dependencies for the specific project and its references
RUN dotnet restore ECommerce.User.API.csproj

# Build and publish the User API project
RUN dotnet publish ECommerce.User.API.csproj -c Release -o /app/publish --no-restore

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final

# Install ICU libraries and locales for globalization support
RUN apt-get update && \
    apt-get install -y libicu-dev locales && \
    locale-gen es_MX.UTF-8 && \
    rm -rf /var/lib/apt/lists/*

WORKDIR /app

# Copy the published app
COPY --from=build /app/publish .

# Expose port
EXPOSE 8080

# Set environment variables
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:8080
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
ENV LANG=es_MX.UTF-8
ENV LC_ALL=es_MX.UTF-8

# Run the application
ENTRYPOINT ["dotnet", "ECommerce.User.API.dll"]
