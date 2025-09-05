# Use the official .NET 9 SDK image for building and running
FROM mcr.microsoft.com/dotnet/sdk:9.0

# Install ICU libraries for globalization support
RUN apt-get update && apt-get install -y icu-devtools libicu-dev && rm -rf /var/lib/apt/lists/*

# Set working directory
WORKDIR /src

# Copy the Payment API project files
COPY . .

# Restore dependencies
RUN dotnet restore ECommerce.Payment.API.csproj

# Build the application
RUN dotnet build ECommerce.Payment.API.csproj -c Release --no-restore

# Expose port
EXPOSE 7004

# Set environment variables
ENV ASPNETCORE_ENVIRONMENT=Production
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

# Create a startup script
RUN echo '#!/bin/bash' > /start.sh && \
    echo 'echo "Starting Payment Service..."' >> /start.sh && \
    echo 'cd /src' >> /start.sh && \
    echo 'dotnet run --project ECommerce.Payment.API.csproj --configuration Release' >> /start.sh && \
    chmod +x /start.sh

# Use the startup script
CMD ["/start.sh"]
