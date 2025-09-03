# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy everything
COPY . .

# Restore dependencies
RUN dotnet restore ECommerce.sln

# Build the application
RUN dotnet build ECommerce.sln -c Release --no-restore

# Publish the AppHost project with verbose output
RUN dotnet publish src/AppHost/ECommerce.AppHost/ECommerce.AppHost.csproj -c Release -o ./publish --verbosity normal
RUN echo "Checking if publish directory exists:"
RUN ls -la
RUN ls -la ./publish || echo "Publish directory does not exist"

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Copy published app
COPY --from=build /src/publish .

# Expose ports
EXPOSE 18888 18889 18890 18891 18892 18893 18894 18895

# Set environment variables
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:18888

# Start the published app
ENTRYPOINT ["dotnet", "ECommerce.AppHost.dll"]
