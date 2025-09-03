# 🚀 Guía Completa de Despliegue - E-Commerce Microservicios

## 📋 Resumen

Esta guía te llevará paso a paso para desplegar todos los microservicios de tu aplicación e-commerce en Railway.

## 🏗️ Arquitectura del Sistema

```
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│   Frontend      │    │   API Gateway   │    │  Microservicios │
│   (Angular)     │◄──►│   (Railway)     │◄──►│   (Railway)     │
└─────────────────┘    └─────────────────┘    └─────────────────┘
                              │
                              ▼
                    ┌─────────────────┐
                    │   Servicios     │
                    │   Externos      │
                    │ (DB, Redis,     │
                    │  RabbitMQ)      │
                    └─────────────────┘
```

## 🎯 Orden de Despliegue

### **1. API Gateway (Ya desplegado ✅)**
- ✅ Configurado y funcionando
- ✅ Health check en `/health`
- ✅ Configurado para usar variables de entorno

### **2. Microservicios (Por desplegar)**

#### **A. User Service**
- **Directorio**: `backend/src/Services/User/ECommerce.User.API`
- **Puerto**: 7001
- **Guía**: `deploy-user-service.md`

#### **B. Product Service**
- **Directorio**: `backend/src/Services/Product/ECommerce.Product.API`
- **Puerto**: 7002
- **Guía**: `deploy-product-service.md`

#### **C. Order Service**
- **Directorio**: `backend/src/Services/Order/ECommerce.Order.API`
- **Puerto**: 7003
- **Guía**: `deploy-order-service.md`

#### **D. Payment Service**
- **Directorio**: `backend/src/Services/Payment/ECommerce.Payment.API`
- **Puerto**: 7004
- **Guía**: `deploy-payment-service.md`

## 🚀 Pasos Rápidos

### **Paso 1: Desplegar Microservicios**

Para cada microservicio:

1. **Ve a Railway** → https://railway.app
2. **Crea nuevo proyecto** → "Deploy from GitHub repo"
3. **Conecta tu repositorio** e-commerce
4. **Configura directorio raíz** según la tabla anterior
5. **Configura variables de entorno** (ver guías individuales)
6. **Despliega** y obtén la URL

### **Paso 2: Configurar API Gateway**

1. **Ve al proyecto del API Gateway** en Railway
2. **Agrega estas variables de entorno**:

```bash
USER_SERVICE_URL=https://tu-user-service.railway.app
PRODUCT_SERVICE_URL=https://tu-product-service.railway.app
ORDER_SERVICE_URL=https://tu-order-service.railway.app
PAYMENT_SERVICE_URL=https://tu-payment-service.railway.app
```

3. **Reinicia el API Gateway**

## 📚 Guías Detalladas

- **`deploy-user-service.md`** - Despliegue del User Service
- **`deploy-product-service.md`** - Despliegue del Product Service
- **`deploy-order-service.md`** - Despliegue del Order Service
- **`deploy-payment-service.md`** - Despliegue del Payment Service
- **`configure-api-gateway.md`** - Configuración del API Gateway

## 🔧 Variables de Entorno por Servicio

### **User Service**
```bash
DATABASE_URL=postgresql://...
JWT_SECRET_KEY=tu-clave-secreta-de-32-caracteres
REDIS_URL=redis://...
RABBITMQ_URL=amqp://...
```

### **Product Service**
```bash
DATABASE_URL=postgresql://...
REDIS_URL=redis://...
RABBITMQ_URL=amqp://...
```

### **Order Service**
```bash
DATABASE_URL=postgresql://...
RABBITMQ_URL=amqp://...
```

### **Payment Service**
```bash
DATABASE_URL=postgresql://...
STRIPE_SECRET_KEY=sk_test_...
RABBITMQ_URL=amqp://...
```

## 🧪 Verificación

### **Health Checks**
```bash
# API Gateway
curl https://tu-api-gateway.railway.app/health

# User Service
curl https://tu-user-service.railway.app/health

# Product Service
curl https://tu-product-service.railway.app/health

# Order Service
curl https://tu-order-service.railway.app/health

# Payment Service
curl https://tu-payment-service.railway.app/health
```

### **Endpoints del API Gateway**
```bash
# Autenticación
curl https://tu-api-gateway.railway.app/api/auth/login

# Productos
curl https://tu-api-gateway.railway.app/api/product

# Órdenes
curl https://tu-api-gateway.railway.app/api/orders

# Pagos
curl https://tu-api-gateway.railway.app/api/payments
```

## 🚨 Solución de Problemas

### **Build Falla**
- Verifica que el Dockerfile esté en la ubicación correcta
- Revisa los logs de build para errores específicos

### **Health Check Falla**
- Verifica que las variables de entorno estén configuradas
- Revisa los logs del servicio para errores de conexión

### **API Gateway no puede conectar**
- Verifica que las URLs estén configuradas correctamente
- Asegúrate de que las URLs terminen con `/`

## 📊 Monitoreo

Una vez desplegado, puedes monitorear:
- **Logs** en tiempo real en Railway
- **Métricas** de CPU y memoria
- **Health checks** automáticos
- **Despliegues** y rollbacks

## 🎉 ¡Listo!

Después de seguir esta guía, tendrás:
- ✅ API Gateway funcionando
- ✅ Todos los microservicios desplegados
- ✅ Sistema completo funcionando
- ✅ Endpoints accesibles desde internet

¡Tu aplicación e-commerce estará completamente desplegada en Railway! 🚀
