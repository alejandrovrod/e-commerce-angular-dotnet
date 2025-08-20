# Deployment en Railway - E-Commerce Platform

## 🚂 **Railway Deployment Guide**

Railway es una excelente plataforma para deployar nuestra arquitectura de microservicios. Aquí te muestro cómo hacerlo.

## 📋 **Estrategias de Deployment**

### **Estrategia 1: Deployment por Servicios (Recomendado)**

#### **Servicios a Deployar:**

1. **Frontend (Angular)**
2. **API Gateway**
3. **User Service**
4. **Product Service** 
5. **Order Service**
6. **Payment Service**
7. **Notification Service**
8. **File Service**
9. **PostgreSQL** (Railway Database)
10. **Redis** (Railway Database)

### **Estrategia 2: Deployment Monolítico Temporal**
Para comenzar rápido, podemos deployar algunos servicios juntos.

## 🔧 **Configuración por Servicio**

### **1. Frontend - Angular**

**railway.toml**
```toml
[build]
command = "npm ci && npm run build"
watchPatterns = ["src/**"]

[deploy]
startCommand = "npm start"
healthcheckPath = "/"
healthcheckTimeout = 300
restartPolicyType = "on_failure"

[env]
NODE_ENV = "production"
PORT = "3000"
```

**package.json** (actualizar scripts)
```json
{
  "scripts": {
    "start": "ng serve --host 0.0.0.0 --port $PORT --disable-host-check",
    "build:prod": "ng build --configuration production",
    "serve:railway": "npx http-server dist/ecommerce-frontend -p $PORT -a 0.0.0.0"
  }
}
```

### **2. API Gateway**

**railway.toml**
```toml
[build]
command = "dotnet publish --configuration Release --output out"

[deploy]
startCommand = "dotnet out/ECommerce.ApiGateway.dll"
healthcheckPath = "/health"
healthcheckTimeout = 300

[env]
ASPNETCORE_ENVIRONMENT = "Production"
ASPNETCORE_URLS = "http://0.0.0.0:$PORT"
```

### **3. User Service**

**railway.toml**
```toml
[build]
command = "dotnet publish --configuration Release --output out"

[deploy]
startCommand = "dotnet out/ECommerce.User.API.dll"
healthcheckPath = "/health"

[env]
ASPNETCORE_ENVIRONMENT = "Production"
ASPNETCORE_URLS = "http://0.0.0.0:$PORT"
ConnectionStrings__DefaultConnection = "$DATABASE_URL"
```

### **4. Product Service**

**railway.toml**
```toml
[build]
command = "dotnet publish --configuration Release --output out"

[deploy]
startCommand = "dotnet out/ECommerce.Product.API.dll"
healthcheckPath = "/health"

[env]
ASPNETCORE_ENVIRONMENT = "Production"
ASPNETCORE_URLS = "http://0.0.0.0:$PORT"
MongoDB__ConnectionString = "$MONGODB_URL"
Redis__ConnectionString = "$REDIS_URL"
```

## 🗄️ **Bases de Datos en Railway**

### **PostgreSQL**
```bash
# Railway provee automáticamente:
DATABASE_URL=postgresql://user:password@host:port/database
```

### **Redis**
```bash
# Agregar Redis como servicio:
REDIS_URL=redis://user:password@host:port
```

### **MongoDB**
```bash
# Usar MongoDB Atlas o agregar como servicio externo:
MONGODB_URL=mongodb+srv://user:password@cluster.mongodb.net/database
```

## 🌐 **Variables de Entorno por Servicio**

### **Frontend**
```bash
# railway.json o Panel de Railway
ANGULAR_ENV=production
API_BASE_URL=https://api-gateway.railway.app
STRIPE_PUBLIC_KEY=pk_live_...
```

### **API Gateway**
```bash
ASPNETCORE_ENVIRONMENT=Production
USER_SERVICE_URL=https://user-service.railway.app
PRODUCT_SERVICE_URL=https://product-service.railway.app
ORDER_SERVICE_URL=https://order-service.railway.app
PAYMENT_SERVICE_URL=https://payment-service.railway.app
JWT_SECRET_KEY=$JWT_SECRET
```

### **User Service**
```bash
DATABASE_URL=$DATABASE_URL
JWT_SECRET_KEY=$JWT_SECRET
REDIS_URL=$REDIS_URL
SMTP_HOST=smtp.sendgrid.net
SMTP_API_KEY=$SENDGRID_API_KEY
```

### **Product Service**
```bash
MONGODB_URL=$MONGODB_URL
REDIS_URL=$REDIS_URL
ELASTICSEARCH_URL=$ELASTICSEARCH_URL
CLOUDINARY_URL=$CLOUDINARY_URL
```

### **Payment Service**
```bash
STRIPE_SECRET_KEY=$STRIPE_SECRET_KEY
STRIPE_WEBHOOK_SECRET=$STRIPE_WEBHOOK_SECRET
PAYPAL_CLIENT_ID=$PAYPAL_CLIENT_ID
PAYPAL_CLIENT_SECRET=$PAYPAL_CLIENT_SECRET
```

## 📁 **Estructura de Archivos para Railway**

```
e-commerce/
├── frontend/
│   ├── railway.toml
│   ├── package.json
│   └── [Angular files...]
├── backend/
│   ├── src/
│   │   ├── ApiGateway/
│   │   │   ├── railway.toml
│   │   │   └── [API Gateway files...]
│   │   └── Services/
│   │       ├── User/
│   │       │   ├── railway.toml
│   │       │   └── [User service files...]
│   │       ├── Product/
│   │       │   ├── railway.toml
│   │       │   └── [Product service files...]
│   │       └── [Other services...]
└── railway-deploy.sh
```

## 🔄 **Script de Deployment Automatizado**

**railway-deploy.sh**
```bash
#!/bin/bash

echo "🚂 Deploying E-Commerce to Railway"

# Deploy databases first
echo "📊 Setting up databases..."
railway add --database postgresql
railway add --database redis

# Deploy backend services
echo "🔧 Deploying backend services..."
cd backend/src/ApiGateway && railway up --detach
cd ../Services/User && railway up --detach
cd ../Product && railway up --detach
cd ../Order && railway up --detach
cd ../Payment && railway up --detach
cd ../Notification && railway up --detach
cd ../File && railway up --detach

# Deploy frontend
echo "🎨 Deploying frontend..."
cd ../../../../frontend && railway up

echo "✅ Deployment complete!"
echo "🌐 Check your services at: https://railway.app/dashboard"
```

## 🔗 **Configuración de Networking**

### **Service Discovery en Railway**
```csharp
// appsettings.Production.json para cada microservicio
{
  "ServiceUrls": {
    "UserService": "https://user-service-production.railway.app",
    "ProductService": "https://product-service-production.railway.app",
    "OrderService": "https://order-service-production.railway.app",
    "PaymentService": "https://payment-service-production.railway.app"
  }
}
```

### **CORS Configuration**
```csharp
// API Gateway
services.AddCors(options =>
{
    options.AddPolicy("Production", builder =>
    {
        builder
            .WithOrigins("https://your-frontend.railway.app")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});
```

## 📊 **Monitoreo y Logging**

### **Health Checks**
```csharp
// Cada microservicio
public void Configure(IApplicationBuilder app)
{
    app.UseHealthChecks("/health", new HealthCheckOptions
    {
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    });
}
```

### **Logging**
```json
{
  "Serilog": {
    "WriteTo": [
      {
        "Name": "Console",
        "Args": {
          "outputTemplate": "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}"
        }
      }
    ]
  }
}
```

## 💰 **Estimación de Costos**

### **Tier Starter ($5/mes por servicio)**
- Frontend: $5/mes
- API Gateway: $5/mes
- 6 Microservicios: $30/mes
- PostgreSQL: $5/mes
- Redis: $5/mes
- **Total: ~$50/mes**

### **Optimizaciones de Costos**
1. **Combinar servicios pequeños** en un solo deployment
2. **Usar bases de datos compartidas** para desarrollo
3. **Implementar auto-scaling** para servicios bajo demanda

## 🚀 **Pasos para Deployment**

### **1. Preparación**
```bash
# Instalar Railway CLI
npm install -g @railway/cli

# Login
railway login

# Crear proyecto
railway init
```

### **2. Configurar Servicios**
```bash
# Para cada servicio
railway add
railway variables set KEY=VALUE
```

### **3. Deploy**
```bash
# Deploy automático con Git
git push origin main

# O deploy manual
railway up
```

## 🔧 **Configuraciones Específicas**

### **Frontend Build Configuration**
```json
{
  "build": {
    "builder": "@angular-devkit/build-angular:browser",
    "options": {
      "baseHref": "/",
      "deployUrl": "/",
      "optimization": true,
      "outputHashing": "all",
      "sourceMap": false,
      "namedChunks": false,
      "aot": true,
      "extractLicenses": true,
      "vendorChunk": false,
      "buildOptimizer": true
    }
  }
}
```

### **Backend Dockerfile (alternativo)**
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["Service/Service.csproj", "Service/"]
RUN dotnet restore "Service/Service.csproj"
COPY . .
WORKDIR "/src/Service"
RUN dotnet build "Service.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Service.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Service.dll"]
```

## ✅ **Checklist de Deployment**

- [ ] Configurar variables de entorno
- [ ] Configurar bases de datos
- [ ] Configurar CORS
- [ ] Configurar health checks
- [ ] Configurar logging
- [ ] Configurar SSL/HTTPS
- [ ] Configurar dominios custom
- [ ] Configurar monitoring
- [ ] Configurar backups
- [ ] Testear en staging

¿Te gustaría que proceda con la configuración específica de algún servicio para Railway?
