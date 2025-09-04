# Use the official .NET 9 SDK image for building and running
FROM mcr.microsoft.com/dotnet/sdk:9.0

# Set working directory
WORKDIR /src

# Copy the Order API project files
COPY . .

# Restore dependencies
RUN dotnet restore ECommerce.Order.API.csproj

# Build the application
RUN dotnet build ECommerce.Order.API.csproj -c Release --no-restore

# Expose port
EXPOSE 7003

# Set environment variables
ENV ASPNETCORE_ENVIRONMENT=Production

# Create a startup script
RUN echo '#!/bin/bash' > /start.sh && \
    echo 'echo "Starting Order Service..."' >> /start.sh && \
    echo 'cd /src' >> /start.sh && \
    echo 'dotnet run --project ECommerce.Order.API.csproj --configuration Release' >> /start.sh && \
    chmod +x /start.sh

# Use the startup script
CMD ["/start.sh"]
