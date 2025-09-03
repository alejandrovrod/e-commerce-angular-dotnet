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

# Expose ports
EXPOSE 18888 18889 18890 18891 18892 18893 18894 18895

# Set environment variables
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:18888

# Make start script executable
RUN chmod +x start.sh

# Start the AppHost using the script
ENTRYPOINT ["./start.sh"]
