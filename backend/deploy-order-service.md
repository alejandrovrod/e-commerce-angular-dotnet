# 🚀 Desplegando el Order Service en Railway

## 📋 Pasos Detallados

### **Paso 1: Crear el Proyecto en Railway**

1. **Ve a Railway** → https://railway.app
2. **Inicia sesión** con tu cuenta
3. **Crea un nuevo proyecto** → "New Project"
4. **Selecciona** → "Deploy from GitHub repo"
5. **Conecta tu repositorio** e-commerce
6. **Configura el directorio raíz** como: `backend/src/Services/Order/ECommerce.Order.API`

### **Paso 2: Configurar Variables de Entorno**

En Railway, ve a **Variables** y agrega estas variables:

```bash
# Base de datos PostgreSQL
DATABASE_URL=postgresql://usuario:password@host:puerto/database

# RabbitMQ para mensajería
RABBITMQ_URL=amqp://usuario:password@host:puerto

# Configuración de la aplicación
ASPNETCORE_ENVIRONMENT=Production

# Puerto (Railway lo asigna automáticamente)
PORT=7003
```

### **Paso 3: Verificar la Configuración**

Railway debería detectar automáticamente:
- ✅ **Dockerfile** en `backend/src/Services/Order/ECommerce.Order.API/Dockerfile`
- ✅ **railway.json** con la configuración de despliegue
- ✅ **Health check** en `/health`

### **Paso 4: Desplegar**

1. **Haz clic en "Deploy"**
2. **Espera** a que se complete el build
3. **Verifica** que el health check pase: `https://tu-order-service.railway.app/health`

### **Paso 5: Obtener la URL del Servicio**

Una vez desplegado, Railway te dará una URL como:
```
https://order-service-production-xxxx.up.railway.app
```

**¡Guarda esta URL!** La necesitarás para configurar el API Gateway.

## 🔧 Configurar el API Gateway

### **Paso 6: Actualizar Variables de Entorno del API Gateway**

En Railway, ve al proyecto del **API Gateway** y agrega:

```bash
ORDER_SERVICE_URL=https://tu-order-service.railway.app
```

### **Paso 7: Reiniciar el API Gateway**

1. **Ve al proyecto del API Gateway**
2. **Haz clic en "Restart"**
3. **Verifica** que los logs muestren la URL del Order Service

## 🧪 Verificar el Despliegue

### **Endpoints Disponibles:**

- **Health Check**: `https://tu-order-service.railway.app/health`
- **Órdenes**: `https://tu-api-gateway.railway.app/api/orders/*`

### **Funcionalidades del Order Service:**

- ✅ **Crear órdenes** - Procesar nuevas órdenes de compra
- ✅ **Gestionar estado** - Actualizar estado de órdenes
- ✅ **Historial** - Ver historial de órdenes del usuario
- ✅ **Cancelar órdenes** - Cancelar órdenes pendientes
- ✅ **Integración con pagos** - Comunicación con Payment Service

## 🔄 Integración con Otros Servicios

### **Dependencias:**
- **User Service** - Para validar usuarios y direcciones
- **Product Service** - Para validar productos y precios
- **Payment Service** - Para procesar pagos
- **RabbitMQ** - Para comunicación asíncrona

### **Eventos que Publica:**
- `OrderCreated` - Cuando se crea una nueva orden
- `OrderStatusChanged` - Cuando cambia el estado de una orden
- `OrderCancelled` - Cuando se cancela una orden

## 🚨 Solución de Problemas

### **Si el build falla:**
- Verifica que el Dockerfile esté en la ubicación correcta
- Revisa los logs de build para errores específicos

### **Si el health check falla:**
- Verifica que las variables de entorno estén configuradas
- Revisa los logs del servicio para errores de conexión
- Asegúrate de que la base de datos esté accesible

### **Si el API Gateway no puede conectar:**
- Verifica que `ORDER_SERVICE_URL` esté configurada correctamente
- Asegúrate de que la URL termine con `/` (ej: `https://service.railway.app/`)

## 📊 Monitoreo

Una vez desplegado, puedes monitorear:
- **Logs** en tiempo real en Railway
- **Métricas** de CPU y memoria
- **Health checks** automáticos
- **Despliegues** y rollbacks

¡El Order Service debería estar funcionando correctamente! 🎉
