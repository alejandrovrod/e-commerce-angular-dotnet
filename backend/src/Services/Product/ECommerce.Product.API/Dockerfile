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

# IMPORTANTE: asegurar ICU y desactivar Globalization Invariant
# (en imágenes Debian/Ubuntu basta con instalar libicu)
RUN apt-get update \
 && apt-get install -y --no-install-recommends libicu \
 && rm -rf /var/lib/apt/lists/*

# Variables de entorno recomendadas
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false \
    ASPNETCORE_ENVIRONMENT=Production \
    ASPNETCORE_URLS=http://+:8080 \
    # Opcional: fija locale si lo necesitas
    LANG=es_MX.UTF-8 \
    LC_ALL=es_MX.UTF-8

# Copia la app publicada
COPY --from=build /app/publish .

# Expone el puerto
EXPOSE 8080

# Arranque
ENTRYPOINT ["dotnet", "ECommerce.Product.API.dll"]