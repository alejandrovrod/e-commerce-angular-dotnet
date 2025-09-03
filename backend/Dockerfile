# Use the official .NET 9 SDK image for building and running
FROM mcr.microsoft.com/dotnet/sdk:9.0

# Set working directory
WORKDIR /src

# Copy everything
COPY . .

# Restore dependencies
RUN dotnet restore ECommerce.sln

# Build the application
RUN dotnet build ECommerce.sln -c Release --no-restore

# Publish the AppHost project
RUN dotnet publish src/AppHost/ECommerce.AppHost/ECommerce.AppHost.csproj -c Release -o /app/publish

# Set working directory to published app
WORKDIR /app/publish

# Expose ports
EXPOSE 18888 18889 18890 18891 18892 18893 18894 18895

# Set environment variables
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:18888

# Start the published AppHost
ENTRYPOINT ["dotnet", "ECommerce.AppHost.dll"]
