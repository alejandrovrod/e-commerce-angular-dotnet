# 🚀 Desplegando el User Service en Railway

## 📋 Pasos Detallados

### **Paso 1: Crear el Proyecto en Railway**

1. **Ve a Railway** → https://railway.app
2. **Inicia sesión** con tu cuenta
3. **Crea un nuevo proyecto** → "New Project"
4. **Selecciona** → "Deploy from GitHub repo"
5. **Conecta tu repositorio** e-commerce
6. **Configura el directorio raíz** como: `backend/src/Services/User/ECommerce.User.API`

### **Paso 2: Configurar Variables de Entorno**

En Railway, ve a **Variables** y agrega estas variables:

```bash
# Base de datos PostgreSQL
DATABASE_URL=postgresql://usuario:password@host:puerto/database

# JWT Secret Key (¡MUY IMPORTANTE!)
JWT_SECRET_KEY=tu-clave-secreta-super-segura-de-al-menos-32-caracteres

# Redis para caché
REDIS_URL=redis://host:puerto

# RabbitMQ para mensajería
RABBITMQ_URL=amqp://usuario:password@host:puerto

# Configuración de la aplicación
ASPNETCORE_ENVIRONMENT=Production

# Puerto (Railway lo asigna automáticamente)
PORT=7001
```

### **Paso 3: Verificar la Configuración**

Railway debería detectar automáticamente:
- ✅ **Dockerfile** en `backend/src/Services/User/ECommerce.User.API/Dockerfile`
- ✅ **railway.json** con la configuración de despliegue
- ✅ **Health check** en `/health`

### **Paso 4: Desplegar**

1. **Haz clic en "Deploy"**
2. **Espera** a que se complete el build
3. **Verifica** que el health check pase: `https://tu-user-service.railway.app/health`

### **Paso 5: Obtener la URL del Servicio**

Una vez desplegado, Railway te dará una URL como:
```
https://user-service-production-xxxx.up.railway.app
```

**¡Guarda esta URL!** La necesitarás para configurar el API Gateway.

## 🔧 Configurar el API Gateway

### **Paso 6: Actualizar Variables de Entorno del API Gateway**

En Railway, ve al proyecto del **API Gateway** y agrega:

```bash
USER_SERVICE_URL=https://tu-user-service.railway.app
```

### **Paso 7: Reiniciar el API Gateway**

1. **Ve al proyecto del API Gateway**
2. **Haz clic en "Restart"**
3. **Verifica** que los logs muestren la URL del User Service

## 🧪 Verificar el Despliegue

### **Endpoints Disponibles:**

- **Health Check**: `https://tu-user-service.railway.app/health`
- **Autenticación**: `https://tu-api-gateway.railway.app/api/auth/*`
- **Direcciones**: `https://tu-api-gateway.railway.app/api/addresses/*`
- **Admin**: `https://tu-api-gateway.railway.app/api/admin/*`
- **Perfil de Usuario**: `https://tu-api-gateway.railway.app/api/users/profile`
- **Estadísticas**: `https://tu-api-gateway.railway.app/api/users/statistics`

## 🔐 Seguridad

### **Variables de Entorno Críticas:**

- **JWT_SECRET_KEY**: Debe ser una cadena segura de al menos 32 caracteres
- **DATABASE_URL**: Debe apuntar a una base de datos PostgreSQL segura
- **RABBITMQ_URL**: Para comunicación entre servicios

### **Ejemplo de JWT_SECRET_KEY:**
```bash
JWT_SECRET_KEY=mi-clave-super-secreta-para-jwt-de-32-caracteres-minimo
```

## 🚨 Solución de Problemas

### **Si el build falla:**
- Verifica que el Dockerfile esté en la ubicación correcta
- Revisa los logs de build para errores específicos

### **Si el health check falla:**
- Verifica que las variables de entorno estén configuradas
- Revisa los logs del servicio para errores de conexión
- Asegúrate de que la base de datos esté accesible

### **Si el API Gateway no puede conectar:**
- Verifica que `USER_SERVICE_URL` esté configurada correctamente
- Asegúrate de que la URL termine con `/` (ej: `https://service.railway.app/`)

## 📊 Monitoreo

Una vez desplegado, puedes monitorear:
- **Logs** en tiempo real en Railway
- **Métricas** de CPU y memoria
- **Health checks** automáticos
- **Despliegues** y rollbacks

¡El User Service debería estar funcionando correctamente! 🎉
