# ── Build ───────────────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copia la solución completa
COPY . .

# Restaura dependencias
RUN dotnet restore backend/ECommerce.sln

# Compila y publica el Product API
RUN dotnet publish backend/src/Services/Product/ECommerce.Product.API/ECommerce.Product.API.csproj \
    -c Release -o /app/publish --no-restore

# ── Runtime ─────────────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Install ICU libraries and locales for globalization support
RUN apt-get update && \
    apt-get install -y libicu-dev locales && \
    locale-gen es_MX.UTF-8 && \
    rm -rf /var/lib/apt/lists/*

# Set environment variables
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:8080
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
ENV LANG=es_MX.UTF-8
ENV LC_ALL=es_MX.UTF-8

# Copia la app publicada
COPY --from=build /app/publish .

# Expone el puerto
EXPOSE 8080

# Arranque
ENTRYPOINT ["dotnet", "ECommerce.Product.API.dll"]