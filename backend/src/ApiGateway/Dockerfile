# Use the official .NET 9 SDK image
FROM mcr.microsoft.com/dotnet/sdk:9.0

# Set working directory
WORKDIR /app

# Copy csproj and restore dependencies
COPY *.csproj ./
RUN dotnet restore

# Copy everything else
COPY . ./

# Build the application
RUN dotnet build -c Release --no-restore

# Expose port
EXPOSE 8080

# Set environment variables
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:8080

# Run the application
CMD ["dotnet", "run", "--configuration", "Release"]
