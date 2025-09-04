# Use the official .NET 9 SDK image for building and running
FROM mcr.microsoft.com/dotnet/sdk:9.0

# Set working directory
WORKDIR /src

# Copy the entire backend structure
COPY . .

# Set working directory to Product Service
WORKDIR /src/src/Services/Product/ECommerce.Product.API

# Restore dependencies
RUN dotnet restore ECommerce.Product.API.csproj

# Build the application
RUN dotnet build ECommerce.Product.API.csproj -c Release --no-restore

# Expose port
EXPOSE 7002

# Set environment variables
ENV ASPNETCORE_ENVIRONMENT=Production

# Create a startup script
RUN echo '#!/bin/bash' > /start.sh && \
    echo 'echo "Starting Product Service..."' >> /start.sh && \
    echo 'cd /src/src/Services/Product/ECommerce.Product.API' >> /start.sh && \
    echo 'dotnet run --project ECommerce.Product.API.csproj --configuration Release' >> /start.sh && \
    chmod +x /start.sh

# Use the startup script
CMD ["/start.sh"]