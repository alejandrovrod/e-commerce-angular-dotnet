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
EXPOSE 18888

# Set environment variables
ENV ASPNETCORE_ENVIRONMENT=Production

# Create a startup script and use it
RUN echo '#!/bin/bash' > /start.sh && \
    echo 'echo "Starting ECommerce API Gateway..."' >> /start.sh && \
    echo 'cd /src' >> /start.sh && \
    echo 'dotnet run --project src/ApiGateway/ECommerce.ApiGateway.csproj --configuration Release' >> /start.sh && \
    chmod +x /start.sh

# Use the startup script
CMD ["/start.sh"]
