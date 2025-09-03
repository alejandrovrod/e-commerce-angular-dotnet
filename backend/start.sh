#!/bin/bash
set -e

echo "Starting ECommerce AppHost..."

# Change to the source directory
cd /src

# Run the AppHost project
dotnet run --project src/AppHost/ECommerce.AppHost/ECommerce.AppHost.csproj --configuration Release