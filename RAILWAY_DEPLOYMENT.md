# 🚀 Guía de Deployment en Railway

## 📋 Prerrequisitos

1. **Cuenta en Railway**: [railway.app](https://railway.app)
2. **GitHub Repository**: Tu código debe estar en GitHub
3. **Railway CLI** (opcional): `npm install -g @railway/cli`

## 🗄️ 1. Configurar Base de Datos

### PostgreSQL en Railway
1. Ve a [Railway Dashboard](https://railway.app/dashboard)
2. Clic en "New Project"
3. Selecciona "Database" → "PostgreSQL"
4. Copia las variables de conexión:
   - `DATABASE_URL`
   - `PGHOST`
   - `PGPORT`
   - `PGUSER`
   - `PGPASSWORD`
   - `PGDATABASE`

## 🔧 2. Deployar API Gateway

### Opción A: Desde Railway Dashboard
1. **Nuevo Servicio**: Clic en "New Service" → "GitHub Repo"
2. **Seleccionar Repo**: Elige tu repositorio
3. **Configurar Build**:
   - **Root Directory**: `backend/src/ApiGateway`
   - **Build Command**: `dotnet restore && dotnet publish -c Release -o out`
   - **Start Command**: `dotnet out/ECommerce.ApiGateway.dll`
4. **Variables de Entorno**:
   ```
   ASPNETCORE_ENVIRONMENT=Production
   DATABASE_URL=${{Postgres.DATABASE_URL}}
   PRODUCT_SERVICE_URL=${{ProductService.RAILWAY_PUBLIC_DOMAIN}}
   FRONTEND_URL=${{Frontend.RAILWAY_PUBLIC_DOMAIN}}
   ```

### Opción B: Railway CLI
```bash
# Instalar Railway CLI
npm install -g @railway/cli

# Login
railway login

# Inicializar proyecto
railway init

# Deploy
railway up --service api-gateway
```

## 🛍️ 3. Deployar Product Service

### Configuración
1. **Nuevo Servicio**: Clic en "New Service" → "GitHub Repo"
2. **Configurar Build**:
   - **Root Directory**: `backend/src/Services/Product/ECommerce.Product.API`
   - **Build Command**: `dotnet restore && dotnet publish -c Release -o out`
   - **Start Command**: `dotnet out/ECommerce.Product.API.dll`
3. **Variables de Entorno**:
   ```
   ASPNETCORE_ENVIRONMENT=Production
   DATABASE_URL=${{Postgres.DATABASE_URL}}
   FRONTEND_URL=${{Frontend.RAILWAY_PUBLIC_DOMAIN}}
   API_GATEWAY_URL=${{ApiGateway.RAILWAY_PUBLIC_DOMAIN}}
   ```

## 🌐 4. Deployar Frontend

### Configuración
1. **Nuevo Servicio**: Clic en "New Service" → "GitHub Repo"
2. **Configurar Build**:
   - **Root Directory**: `frontend`
   - **Build Command**: `npm ci && npm run build:prod`
   - **Start Command**: `npm run start:prod`
3. **Variables de Entorno**:
   ```
   NODE_ENVIRONMENT=production
   API_GATEWAY_URL=${{ApiGateway.RAILWAY_PUBLIC_DOMAIN}}
   ```

## 🔗 5. Configurar Variables de Entorno Globales

En cada servicio, configura estas variables:

### API Gateway
```
ASPNETCORE_ENVIRONMENT=Production
DATABASE_URL=${{Postgres.DATABASE_URL}}
PRODUCT_SERVICE_URL=${{ProductService.RAILWAY_PUBLIC_DOMAIN}}
FRONTEND_URL=${{Frontend.RAILWAY_PUBLIC_DOMAIN}}
```

### Product Service
```
ASPNETCORE_ENVIRONMENT=Production
DATABASE_URL=${{Postgres.DATABASE_URL}}
FRONTEND_URL=${{Frontend.RAILWAY_PUBLIC_DOMAIN}}
API_GATEWAY_URL=${{ApiGateway.RAILWAY_PUBLIC_DOMAIN}}
```

### Frontend
```
NODE_ENVIRONMENT=production
API_GATEWAY_URL=${{ApiGateway.RAILWAY_PUBLIC_DOMAIN}}
```

## 🗃️ 6. Ejecutar Migraciones

### Opción A: Desde Railway Dashboard
1. Ve al servicio **Product Service**
2. Clic en "Deployments" → "View Logs"
3. Ejecuta en la consola:
```bash
dotnet ef database update --project ECommerce.Product.Infrastructure
```

### Opción B: Railway CLI
```bash
# Conectar al servicio
railway connect ProductService

# Ejecutar migraciones
dotnet ef database update --project ECommerce.Product.Infrastructure
```

## 🔍 7. Verificar Deployment

### URLs de los Servicios
- **Frontend**: `https://frontend-production-xxxx.up.railway.app`
- **API Gateway**: `https://api-gateway-production-xxxx.up.railway.app`
- **Product Service**: `https://product-service-production-xxxx.up.railway.app`

### Health Checks
- **API Gateway**: `https://api-gateway-production-xxxx.up.railway.app/health`
- **Product Service**: `https://product-service-production-xxxx.up.railway.app/health`

## 🚨 8. Troubleshooting

### Problemas Comunes

#### Error de CORS
```json
// En appsettings.Production.json
"Cors": {
  "AllowedOrigins": [
    "https://frontend-production-xxxx.up.railway.app",
    "https://*.railway.app"
  ]
}
```

#### Error de Base de Datos
```bash
# Verificar conexión
railway connect ProductService
dotnet ef database update
```

#### Error de Build
```bash
# Limpiar cache
railway run --service ProductService dotnet clean
railway run --service ProductService dotnet restore
```

## 📊 9. Monitoreo

### Railway Dashboard
- **Metrics**: CPU, Memory, Network
- **Logs**: Logs en tiempo real
- **Deployments**: Historial de deployments

### Health Checks
```bash
# Verificar estado de servicios
curl https://api-gateway-production-xxxx.up.railway.app/health
curl https://product-service-production-xxxx.up.railway.app/health
```

## 🔄 10. CI/CD Automático

### GitHub Actions (Opcional)
```yaml
# .github/workflows/deploy.yml
name: Deploy to Railway
on:
  push:
    branches: [main]
jobs:
  deploy:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v2
      - uses: actions/setup-dotnet@v2
        with:
          dotnet-version: '8.0'
      - run: dotnet restore
      - run: dotnet build
      - run: dotnet test
```

## 💰 11. Costos

### Railway Pricing
- **Hobby Plan**: $5/mes por servicio
- **Pro Plan**: $20/mes por servicio
- **Database**: Incluido en el plan

### Estimación de Costos
- **PostgreSQL**: $5/mes
- **API Gateway**: $5/mes
- **Product Service**: $5/mes
- **Frontend**: $5/mes
- **Total**: ~$20/mes

## 🎯 12. Próximos Pasos

1. **Configurar Dominio Personalizado**
2. **Implementar SSL/HTTPS**
3. **Configurar Backup de Base de Datos**
4. **Implementar Logging Centralizado**
5. **Configurar Alertas de Monitoreo**

---

## 📞 Soporte

- **Railway Docs**: [docs.railway.app](https://docs.railway.app)
- **Railway Discord**: [discord.gg/railway](https://discord.gg/railway)
- **Railway Status**: [status.railway.app](https://status.railway.app)