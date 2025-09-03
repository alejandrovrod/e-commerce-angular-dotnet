# 🚀 Railway - Todos los Servicios del Backend

## 💰 **Costo: $0/mes** (plan gratuito de Railway)

### **Servicios a Deployar:**

1. **✅ Frontend (Angular)** - Ya tienes
2. **🔄 API Gateway** - Punto de entrada único
3. **🔄 Product Service** - Gestión de productos
4. **🔄 User Service** - Gestión de usuarios
5. **🔄 Order Service** - Gestión de órdenes
6. **🔄 Payment Service** - Gestión de pagos
7. **🔄 File Service** - Gestión de archivos
8. **🔄 Notification Service** - Gestión de notificaciones

## 🔧 **Variables de Entorno para TODOS los Servicios**

### **Para API Gateway:**
```bash
ASPNETCORE_ENVIRONMENT=Production
DATABASE_URL=tu_connection_string_aqui
PRODUCT_SERVICE_URL=${{ProductService.RAILWAY_PUBLIC_DOMAIN}}
USER_SERVICE_URL=${{UserService.RAILWAY_PUBLIC_DOMAIN}}
ORDER_SERVICE_URL=${{OrderService.RAILWAY_PUBLIC_DOMAIN}}
PAYMENT_SERVICE_URL=${{PaymentService.RAILWAY_PUBLIC_DOMAIN}}
FILE_SERVICE_URL=${{FileService.RAILWAY_PUBLIC_DOMAIN}}
NOTIFICATION_SERVICE_URL=${{NotificationService.RAILWAY_PUBLIC_DOMAIN}}
FRONTEND_URL=${{Frontend.RAILWAY_PUBLIC_DOMAIN}}
```

### **Para cada servicio individual:**
```bash
ASPNETCORE_ENVIRONMENT=Production
DATABASE_URL=tu_connection_string_aqui
FRONTEND_URL=${{Frontend.RAILWAY_PUBLIC_DOMAIN}}
API_GATEWAY_URL=${{ApiGateway.RAILWAY_PUBLIC_DOMAIN}}
```

### **Para Frontend:**
```bash
NODE_ENVIRONMENT=production
API_GATEWAY_URL=${{ApiGateway.RAILWAY_PUBLIC_DOMAIN}}
```

## 📋 **Orden de Deployment (TODOS los Servicios)**

### **Fase 1: Servicios Base**
1. **Product Service**
2. **User Service**
3. **Order Service**
4. **Payment Service**
5. **File Service**
6. **Notification Service**

### **Fase 2: Gateway y Frontend**
7. **API Gateway** (último, para que tenga todas las URLs)
8. **Frontend** (actualizar variables)

## 🚀 **Pasos para Deployar TODOS los Servicios**

### **1. Crear Servicios en Railway:**
Para cada servicio:
- **New Service** → **GitHub Repo**
- **Root Directory** según el servicio:
  - `backend/src/Services/Product/ECommerce.Product.API`
  - `backend/src/Services/User/ECommerce.User.API`
  - `backend/src/Services/Order/ECommerce.Order.API`
  - `backend/src/Services/Payment/ECommerce.Payment.API`
  - `backend/src/Services/File/ECommerce.File.API`
  - `backend/src/Services/Notification/ECommerce.Notification.API`
  - `backend/src/ApiGateway`

### **2. Configurar Build Commands:**
```bash
Build Command: dotnet restore && dotnet publish -c Release -o out
Start Command: dotnet out/[NombreDelServicio].dll
```

### **3. Configurar Variables de Entorno:**
- Copia las variables de arriba
- Reemplaza `tu_connection_string_aqui` con tu connection string real
- Reemplaza `${{Servicio.RAILWAY_PUBLIC_DOMAIN}}` con las URLs reales

### **4. Verificar Conexión a Base de Datos:**
```bash
# Verificar que todos los servicios se conecten correctamente
# Los servicios se conectarán automáticamente a tu base de datos existente
```

## 🎯 **URLs Finales (TODOS los Servicios)**

- **Frontend**: `https://frontend-production-xxxx.up.railway.app`
- **API Gateway**: `https://api-gateway-production-xxxx.up.railway.app`
- **Product Service**: `https://product-service-production-xxxx.up.railway.app`
- **User Service**: `https://user-service-production-xxxx.up.railway.app`
- **Order Service**: `https://order-service-production-xxxx.up.railway.app`
- **Payment Service**: `https://payment-service-production-xxxx.up.railway.app`
- **File Service**: `https://file-service-production-xxxx.up.railway.app`
- **Notification Service**: `https://notification-service-production-xxxx.up.railway.app`

## 🔍 **Health Checks (TODOS los Servicios)**

- **API Gateway**: `/health`
- **Product Service**: `/health`
- **User Service**: `/health`
- **Order Service**: `/health`
- **Payment Service**: `/health`
- **File Service**: `/health`
- **Notification Service**: `/health`

## 🚨 **Troubleshooting para TODOS los Servicios**

### **Error de CORS:**
- Verificar que todas las URLs estén en `AllowedOrigins`
- Verificar que todos los servicios tengan CORS configurado

### **Error de Base de Datos:**
- Verificar `DATABASE_URL` en todos los servicios
- Verificar que todos los servicios puedan conectarse a la base de datos
- Verificar que las tablas existan en tu base de datos

### **Error de API Gateway:**
- Verificar que todos los servicios estén desplegados
- Verificar que las URLs de servicios sean correctas
- Verificar que todas las rutas estén configuradas

### **Error de Variables de Entorno:**
- Verificar que todas las variables estén configuradas
- Verificar que las URLs de servicios sean correctas
- Verificar que el connection string sea válido

## 💡 **Ventajas de Deployar TODOS los Servicios:**

1. **✅ Arquitectura completa** desde el inicio
2. **✅ Todas las funcionalidades** disponibles
3. **✅ Escalabilidad** individual por servicio
4. **✅ Mantenimiento** independiente por servicio
5. **✅ Desarrollo** paralelo por equipos

---

## 🌙 **¡Listo para Deployar TODOS los Servicios!**

Todos los archivos están configurados para deployar la arquitectura completa. Solo necesitas:
1. Crear los 7 servicios en Railway
2. Configurar las variables de entorno
3. Deployar en el orden indicado

¡Que descanses bien! 😴
