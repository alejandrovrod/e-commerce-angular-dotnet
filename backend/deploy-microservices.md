# 🚀 Guía para Desplegar Microservicios en Railway

## 📋 Pasos para Desplegar cada Microservicio

### 1. **Servicio de Usuario (User Service)**
```bash
# En Railway, crear un nuevo proyecto y conectar el repositorio
# Configurar el directorio raíz como: backend/src/Services/User/ECommerce.User.API
# Railway detectará automáticamente el Dockerfile
```

**Variables de Entorno necesarias:**
- `DATABASE_URL` - URL de la base de datos PostgreSQL
- `JWT_SECRET_KEY` - Clave secreta para JWT
- `REDIS_URL` - URL de Redis
- `RABBITMQ_URL` - URL de RabbitMQ

### 2. **Servicio de Producto (Product Service)**
```bash
# En Railway, crear un nuevo proyecto y conectar el repositorio
# Configurar el directorio raíz como: backend/src/Services/Product/ECommerce.Product.API
# Railway detectará automáticamente el Dockerfile
```

**Variables de Entorno necesarias:**
- `DATABASE_URL` - URL de la base de datos PostgreSQL
- `REDIS_URL` - URL de Redis
- `RABBITMQ_URL` - URL de RabbitMQ

### 3. **Servicio de Orden (Order Service)**
```bash
# En Railway, crear un nuevo proyecto y conectar el repositorio
# Configurar el directorio raíz como: backend/src/Services/Order/ECommerce.Order.API
# Railway detectará automáticamente el Dockerfile
```

**Variables de Entorno necesarias:**
- `DATABASE_URL` - URL de la base de datos PostgreSQL
- `RABBITMQ_URL` - URL de RabbitMQ

### 4. **Servicio de Pago (Payment Service)**
```bash
# En Railway, crear un nuevo proyecto y conectar el repositorio
# Configurar el directorio raíz como: backend/src/Services/Payment/ECommerce.Payment.API
# Railway detectará automáticamente el Dockerfile
```

**Variables de Entorno necesarias:**
- `DATABASE_URL` - URL de la base de datos PostgreSQL
- `STRIPE_SECRET_KEY` - Clave secreta de Stripe
- `RABBITMQ_URL` - URL de RabbitMQ

## 🔧 Configuración del API Gateway

Una vez que todos los microservicios estén desplegados, necesitas actualizar las URLs en el API Gateway:

### Variables de Entorno del API Gateway:
- `USER_SERVICE_URL` - URL del servicio de Usuario
- `PRODUCT_SERVICE_URL` - URL del servicio de Producto
- `ORDER_SERVICE_URL` - URL del servicio de Orden
- `PAYMENT_SERVICE_URL` - URL del servicio de Pago
- `FILE_SERVICE_URL` - URL del servicio de Archivos
- `NOTIFICATION_SERVICE_URL` - URL del servicio de Notificaciones

## 📝 Orden de Despliegue Recomendado

1. **Primero**: Desplegar servicios de infraestructura (Redis, RabbitMQ, PostgreSQL)
2. **Segundo**: Desplegar microservicios en este orden:
   - User Service
   - Product Service
   - Order Service
   - Payment Service
3. **Tercero**: Actualizar variables de entorno del API Gateway
4. **Cuarto**: Reiniciar el API Gateway

## 🔍 Verificación

Después del despliegue, verifica que cada servicio responda:
- `https://tu-servicio.railway.app/health`

## 🚨 Notas Importantes

- Cada microservicio necesita su propia base de datos
- Los servicios están configurados para usar el puerto asignado por Railway
- Los health checks están configurados en `/health`
- Los servicios están configurados para producción
