# 🔧 Configurando el API Gateway con Todos los Microservicios

## 📋 Resumen de URLs Necesarias

Una vez que hayas desplegado todos los microservicios, necesitas configurar estas variables de entorno en el **API Gateway**:

```bash
# URLs de los microservicios
USER_SERVICE_URL=https://tu-user-service.railway.app
PRODUCT_SERVICE_URL=https://tu-product-service.railway.app
ORDER_SERVICE_URL=https://tu-order-service.railway.app
PAYMENT_SERVICE_URL=https://tu-payment-service.railway.app
FILE_SERVICE_URL=https://tu-file-service.railway.app
NOTIFICATION_SERVICE_URL=https://tu-notification-service.railway.app
```

## 🚀 Pasos para Configurar

### **Paso 1: Obtener las URLs de los Servicios**

Para cada microservicio desplegado, obtén la URL de Railway:

1. **Ve al proyecto del microservicio** en Railway
2. **Copia la URL** del dominio (ej: `https://service-production-xxxx.up.railway.app`)
3. **Asegúrate** de que termine con `/`

### **Paso 2: Configurar Variables de Entorno en el API Gateway**

1. **Ve al proyecto del API Gateway** en Railway
2. **Haz clic en "Variables"**
3. **Agrega cada variable** con su URL correspondiente

### **Paso 3: Reiniciar el API Gateway**

1. **Haz clic en "Restart"** en el proyecto del API Gateway
2. **Espera** a que se reinicie
3. **Verifica** que los logs muestren las URLs configuradas

## 🧪 Verificar la Configuración

### **Paso 4: Probar los Endpoints**

Una vez configurado, puedes probar estos endpoints:

#### **User Service:**
```bash
# Health check
curl https://tu-api-gateway.railway.app/api/auth/health

# Login
curl -X POST https://tu-api-gateway.railway.app/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"test@example.com","password":"password"}'
```

#### **Product Service:**
```bash
# Health check
curl https://tu-api-gateway.railway.app/api/product/health

# Obtener productos
curl https://tu-api-gateway.railway.app/api/product
```

#### **Order Service:**
```bash
# Health check
curl https://tu-api-gateway.railway.app/api/orders/health

# Obtener órdenes
curl https://tu-api-gateway.railway.app/api/orders
```

#### **Payment Service:**
```bash
# Health check
curl https://tu-api-gateway.railway.app/api/payments/health

# Obtener métodos de pago
curl https://tu-api-gateway.railway.app/api/payments/methods
```

## 🔍 Verificar Logs del API Gateway

En los logs del API Gateway, deberías ver algo como:

```
User Service URL: https://user-service.railway.app
Product Service URL: https://product-service.railway.app
Order Service URL: https://order-service.railway.app
Payment Service URL: https://payment-service.railway.app
```

## 🚨 Solución de Problemas

### **Si un servicio no responde:**

1. **Verifica** que el microservicio esté desplegado y funcionando
2. **Revisa** que la URL esté configurada correctamente
3. **Asegúrate** de que la URL termine con `/`
4. **Revisa** los logs del microservicio para errores

### **Si el API Gateway no puede conectar:**

1. **Verifica** que todas las variables de entorno estén configuradas
2. **Revisa** que las URLs sean accesibles desde internet
3. **Asegúrate** de que los microservicios estén en estado "Healthy"

### **Si hay errores 502 Bad Gateway:**

1. **Verifica** que el microservicio esté funcionando
2. **Revisa** que el health check del microservicio pase
3. **Asegúrate** de que la URL esté configurada correctamente

## 📊 Monitoreo Completo

Una vez configurado, puedes monitorear:

- **API Gateway** - Punto de entrada principal
- **User Service** - Autenticación y usuarios
- **Product Service** - Catálogo de productos
- **Order Service** - Gestión de órdenes
- **Payment Service** - Procesamiento de pagos

## 🎯 Próximos Pasos

Después de configurar el API Gateway:

1. **Probar** todos los endpoints
2. **Configurar** el frontend para usar el API Gateway
3. **Configurar** bases de datos para cada microservicio
4. **Configurar** Redis y RabbitMQ
5. **Configurar** Stripe para pagos

¡Tu sistema de microservicios debería estar funcionando completamente! 🎉
