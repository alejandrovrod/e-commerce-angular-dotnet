# 🚀 Deployment Completo en Railway - Lista de Verificación

## ✅ **Servicios Configurados**

### **1. Frontend (Angular)**
- ✅ `frontend/railway.json`
- ✅ `frontend/src/environments/environment.prod.ts`
- ✅ `frontend/package.json` (script start:prod)

### **2. API Gateway**
- ✅ `backend/src/ApiGateway/railway.json`
- ✅ `backend/src/ApiGateway/appsettings.Production.json`
- ✅ Configurado para todos los servicios

### **3. Product Service**
- ✅ `backend/src/Services/Product/ECommerce.Product.API/railway.json`
- ✅ `backend/src/Services/Product/ECommerce.Product.API/appsettings.Production.json`
- ✅ `backend/src/Services/Product/ECommerce.Product.API/run-migrations.sh`

### **4. User Service**
- ✅ `backend/src/Services/User/ECommerce.User.API/railway.json`
- ✅ `backend/src/Services/User/ECommerce.User.API/appsettings.Production.json`

### **5. Order Service**
- ✅ `backend/src/Services/Order/ECommerce.Order.API/railway.json`
- ✅ `backend/src/Services/Order/ECommerce.Order.API/appsettings.Production.json`

### **6. Payment Service**
- ✅ `backend/src/Services/Payment/ECommerce.Payment.API/railway.json`
- ✅ `backend/src/Services/Payment/ECommerce.Payment.API/appsettings.Production.json`

### **7. File Service**
- ✅ `backend/src/Services/File/ECommerce.File.API/railway.json`
- ✅ `backend/src/Services/File/ECommerce.File.API/appsettings.Production.json`

### **8. Notification Service**
- ✅ `backend/src/Services/Notification/ECommerce.Notification.API/railway.json`
- ✅ `backend/src/Services/Notification/ECommerce.Notification.API/appsettings.Production.json`

## 🔧 **Variables de Entorno Necesarias**

### **Para API Gateway:**
```
ASPNETCORE_ENVIRONMENT=Production
DATABASE_URL=${{Postgres.DATABASE_URL}}
PRODUCT_SERVICE_URL=${{ProductService.RAILWAY_PUBLIC_DOMAIN}}
USER_SERVICE_URL=${{UserService.RAILWAY_PUBLIC_DOMAIN}}
ORDER_SERVICE_URL=${{OrderService.RAILWAY_PUBLIC_DOMAIN}}
PAYMENT_SERVICE_URL=${{PaymentService.RAILWAY_PUBLIC_DOMAIN}}
FILE_SERVICE_URL=${{FileService.RAILWAY_PUBLIC_DOMAIN}}
NOTIFICATION_SERVICE_URL=${{NotificationService.RAILWAY_PUBLIC_DOMAIN}}
FRONTEND_URL=${{Frontend.RAILWAY_PUBLIC_DOMAIN}}
```

### **Para cada servicio individual:**
```
ASPNETCORE_ENVIRONMENT=Production
DATABASE_URL=${{Postgres.DATABASE_URL}}
FRONTEND_URL=${{Frontend.RAILWAY_PUBLIC_DOMAIN}}
API_GATEWAY_URL=${{ApiGateway.RAILWAY_PUBLIC_DOMAIN}}
```

### **Para Frontend:**
```
NODE_ENVIRONMENT=production
API_GATEWAY_URL=${{ApiGateway.RAILWAY_PUBLIC_DOMAIN}}
```

## 📋 **Orden de Deployment**

1. **Base de Datos PostgreSQL** (ya tienes)
2. **Product Service** (primero, para migraciones)
3. **User Service**
4. **Order Service**
5. **Payment Service**
6. **File Service**
7. **Notification Service**
8. **API Gateway** (último, para que tenga todas las URLs)
9. **Frontend** (ya tienes, solo actualizar variables)

## 🚀 **Pasos Rápidos para Deployar**

### **1. Crear Servicios en Railway:**
- Ve a [Railway Dashboard](https://railway.app/dashboard)
- Para cada servicio: "New Service" → "GitHub Repo"
- Selecciona tu repositorio
- Configura el Root Directory según el servicio

### **2. Configurar Build Commands:**
```
Build Command: dotnet restore && dotnet publish -c Release -o out
Start Command: dotnet out/[NombreDelServicio].dll
```

### **3. Configurar Variables de Entorno:**
- Copia las variables de arriba
- Reemplaza `${{Servicio.RAILWAY_PUBLIC_DOMAIN}}` con las URLs reales

### **4. Ejecutar Migraciones:**
- Solo en Product Service (tiene las migraciones)
- En Railway CLI: `railway run --service ProductService ./run-migrations.sh`

## 💰 **Costo Estimado**
- **8 servicios** × $5/mes = **$40/mes**
- **1 base de datos** = **$5/mes**
- **Total**: **~$45/mes**

## 🎯 **URLs Finales**
- **Frontend**: `https://frontend-production-xxxx.up.railway.app`
- **API Gateway**: `https://api-gateway-production-xxxx.up.railway.app`
- **Product Service**: `https://product-service-production-xxxx.up.railway.app`
- **User Service**: `https://user-service-production-xxxx.up.railway.app`
- **Order Service**: `https://order-service-production-xxxx.up.railway.app`
- **Payment Service**: `https://payment-service-production-xxxx.up.railway.app`
- **File Service**: `https://file-service-production-xxxx.up.railway.app`
- **Notification Service**: `https://notification-service-production-xxxx.up.railway.app`

## 🔍 **Health Checks**
- **API Gateway**: `/health`
- **Todos los servicios**: `/health`

## 🚨 **Troubleshooting Rápido**

### **Error de CORS:**
- Verificar que todas las URLs estén en `AllowedOrigins`

### **Error de Base de Datos:**
- Verificar `DATABASE_URL` en todos los servicios
- Ejecutar migraciones en Product Service

### **Error de API Gateway:**
- Verificar que todos los servicios estén desplegados
- Verificar que las URLs de servicios sean correctas

---

## 🌙 **¡Listo para Dormir!**

Todos los archivos están configurados. Solo necesitas:
1. Crear los servicios en Railway
2. Configurar las variables de entorno
3. Deployar en el orden indicado

¡Que descanses bien! 😴
