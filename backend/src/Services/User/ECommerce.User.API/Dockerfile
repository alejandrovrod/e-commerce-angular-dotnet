# Use the official .NET 9 SDK image
FROM mcr.microsoft.com/dotnet/sdk:9.0

# Set working directory
WORKDIR /app

# Copy the entire backend structure
COPY . ./

# Restore dependencies for the specific project and its references
RUN dotnet restore backend/src/Services/User/ECommerce.User.API/ECommerce.User.API.csproj

# Build the specific API project
RUN dotnet build backend/src/Services/User/ECommerce.User.API/ECommerce.User.API.csproj -c Release --no-restore

# Set working directory to the API project
WORKDIR /app/backend/src/Services/User/ECommerce.User.API

# Expose port
EXPOSE 8080

# Set environment variables
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:8080

# Run the application
CMD ["dotnet", "run", "--configuration", "Release"]
