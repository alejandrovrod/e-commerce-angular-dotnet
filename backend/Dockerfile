# Use the official .NET 9 SDK image for building and running
FROM mcr.microsoft.com/dotnet/sdk:9.0

# Set working directory
WORKDIR /src

# Copy the entire backend structure
COPY . .

# Set environment variables
ENV ASPNETCORE_ENVIRONMENT=Production

# Create a startup script that detects the service
RUN echo '#!/bin/bash' > /start.sh && \
    echo 'echo "Starting E-Commerce Service..."' >> /start.sh && \
    echo 'cd /src' >> /start.sh && \
    echo 'if [ -d "src/Services/Product/ECommerce.Product.API" ]; then' >> /start.sh && \
    echo '  echo "Starting Product Service..."' >> /start.sh && \
    echo '  cd src/Services/Product/ECommerce.Product.API' >> /start.sh && \
    echo '  dotnet run --project ECommerce.Product.API.csproj --configuration Release' >> /start.sh && \
    echo 'elif [ -d "src/Services/User/ECommerce.User.API" ]; then' >> /start.sh && \
    echo '  echo "Starting User Service..."' >> /start.sh && \
    echo '  cd src/Services/User/ECommerce.User.API' >> /start.sh && \
    echo '  dotnet run --project ECommerce.User.API.csproj --configuration Release' >> /start.sh && \
    echo 'elif [ -d "src/Services/Order/ECommerce.Order.API" ]; then' >> /start.sh && \
    echo '  echo "Starting Order Service..."' >> /start.sh && \
    echo '  cd src/Services/Order/ECommerce.Order.API' >> /start.sh && \
    echo '  dotnet run --project ECommerce.Order.API.csproj --configuration Release' >> /start.sh && \
    echo 'elif [ -d "src/Services/Payment/ECommerce.Payment.API" ]; then' >> /start.sh && \
    echo '  echo "Starting Payment Service..."' >> /start.sh && \
    echo '  cd src/Services/Payment/ECommerce.Payment.API' >> /start.sh && \
    echo '  dotnet run --project ECommerce.Payment.API.csproj --configuration Release' >> /start.sh && \
    echo 'elif [ -d "src/Services/File/ECommerce.File.API" ]; then' >> /start.sh && \
    echo '  echo "Starting File Service..."' >> /start.sh && \
    echo '  cd src/Services/File/ECommerce.File.API' >> /start.sh && \
    echo '  dotnet run --project ECommerce.File.API.csproj --configuration Release' >> /start.sh && \
    echo 'elif [ -d "src/ApiGateway" ]; then' >> /start.sh && \
    echo '  echo "Starting API Gateway..."' >> /start.sh && \
    echo '  cd src/ApiGateway' >> /start.sh && \
    echo '  dotnet run --project ECommerce.ApiGateway.csproj --configuration Release' >> /start.sh && \
    echo 'else' >> /start.sh && \
    echo '  echo "No service found! Available services:"' >> /start.sh && \
    echo '  find src -name "*.csproj" -type f' >> /start.sh && \
    echo '  exit 1' >> /start.sh && \
    echo 'fi' >> /start.sh && \
    chmod +x /start.sh

# Expose port (Railway will override this)
EXPOSE 8080

# Use the startup script
CMD ["/start.sh"]