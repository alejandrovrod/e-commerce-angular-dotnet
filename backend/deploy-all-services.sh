#!/bin/bash

# 🚀 Script para Desplegar Todos los Microservicios en Railway
# Este script te guía a través del proceso de despliegue

echo "🚀 Iniciando despliegue de microservicios en Railway..."
echo ""

# Colores para output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Función para imprimir mensajes con color
print_message() {
    echo -e "${GREEN}✅ $1${NC}"
}

print_warning() {
    echo -e "${YELLOW}⚠️  $1${NC}"
}

print_error() {
    echo -e "${RED}❌ $1${NC}"
}

print_info() {
    echo -e "${BLUE}ℹ️  $1${NC}"
}

# Verificar que estamos en el directorio correcto
if [ ! -f "ECommerce.sln" ]; then
    print_error "No se encontró ECommerce.sln. Asegúrate de estar en el directorio backend/"
    exit 1
fi

print_message "Directorio correcto detectado"

# Verificar que los Dockerfiles existen
services=("User" "Product" "Order" "Payment")
directories=("src/Services/User/ECommerce.User.API" "src/Services/Product/ECommerce.Product.API" "src/Services/Order/ECommerce.Order.API" "src/Services/Payment/ECommerce.Payment.API")

for i in "${!services[@]}"; do
    service="${services[$i]}"
    directory="${directories[$i]}"
    
    if [ -f "$directory/Dockerfile" ]; then
        print_message "Dockerfile encontrado para $service Service"
    else
        print_error "Dockerfile no encontrado para $service Service en $directory"
        exit 1
    fi
    
    if [ -f "$directory/railway.json" ]; then
        print_message "railway.json encontrado para $service Service"
    else
        print_error "railway.json no encontrado para $service Service en $directory"
        exit 1
    fi
done

print_message "Todos los archivos de configuración están presentes"

echo ""
print_info "📋 Pasos para desplegar en Railway:"
echo ""
echo "1. Ve a https://railway.app"
echo "2. Inicia sesión con tu cuenta"
echo "3. Para cada servicio, crea un nuevo proyecto:"
echo ""

for i in "${!services[@]}"; do
    service="${services[$i]}"
    directory="${directories[$i]}"
    port=$((7001 + i))
    
    echo "   🚀 $service Service:"
    echo "      - Directorio raíz: $directory"
    echo "      - Puerto: $port"
    echo "      - Health check: /health"
    echo ""
done

echo "4. Configura las variables de entorno para cada servicio:"
echo ""
echo "   📝 User Service:"
echo "      - DATABASE_URL"
echo "      - JWT_SECRET_KEY"
echo "      - REDIS_URL"
echo "      - RABBITMQ_URL"
echo ""
echo "   📝 Product Service:"
echo "      - DATABASE_URL"
echo "      - REDIS_URL"
echo "      - RABBITMQ_URL"
echo ""
echo "   📝 Order Service:"
echo "      - DATABASE_URL"
echo "      - RABBITMQ_URL"
echo ""
echo "   📝 Payment Service:"
echo "      - DATABASE_URL"
echo "      - STRIPE_SECRET_KEY"
echo "      - RABBITMQ_URL"
echo ""

print_warning "Después de desplegar todos los servicios, configura las URLs en el API Gateway:"
echo ""
echo "   🔧 API Gateway Variables:"
echo "      - USER_SERVICE_URL=https://tu-user-service.railway.app"
echo "      - PRODUCT_SERVICE_URL=https://tu-product-service.railway.app"
echo "      - ORDER_SERVICE_URL=https://tu-order-service.railway.app"
echo "      - PAYMENT_SERVICE_URL=https://tu-payment-service.railway.app"
echo ""

print_info "📚 Guías detalladas disponibles:"
echo "   - deploy-user-service.md"
echo "   - deploy-product-service.md"
echo "   - deploy-order-service.md"
echo "   - deploy-payment-service.md"
echo "   - configure-api-gateway.md"
echo ""

print_message "¡Preparado para desplegar! Sigue las guías detalladas para cada servicio."
