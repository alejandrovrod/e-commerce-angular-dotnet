# Guía de Desarrollo - E-Commerce Platform

## 🚀 Configuración del Entorno de Desarrollo

### Prerrequisitos

#### Software Requerido
- **.NET 9 SDK** - [Descargar](https://dotnet.microsoft.com/download/dotnet/9.0)
- **Node.js 20+** - [Descargar](https://nodejs.org/)
- **Docker Desktop** - [Descargar](https://www.docker.com/products/docker-desktop)
- **Visual Studio 2022** o **VS Code** con extensiones C# y Angular
- **Git** - [Descargar](https://git-scm.com/)

#### Herramientas Opcionales
- **Azure Data Studio** - Para gestión de bases de datos
- **MongoDB Compass** - Para gestión de MongoDB
- **Postman** - Para testing de APIs
- **Angular CLI** - `npm install -g @angular/cli`

### 🏗️ Configuración Inicial

#### 1. Clonar el Repositorio
```bash
git clone https://github.com/tu-usuario/e-commerce.git
cd e-commerce
```

#### 2. Configurar Backend (.NET 9 Aspire)

```bash
cd backend

# Restaurar paquetes NuGet
dotnet restore

# Verificar que todo compile
dotnet build

# Ejecutar migraciones de base de datos (cuando estén disponibles)
dotnet ef database update --project src/Services/User/ECommerce.User.Infrastructure

# Ejecutar el AppHost de Aspire
dotnet run --project src/AppHost
```

#### 3. Configurar Frontend (Angular 20)

```bash
cd frontend

# Instalar dependencias
npm install

# Verificar la instalación
ng version

# Ejecutar en modo desarrollo
ng serve
```

#### 4. Configurar Infraestructura con Docker

```bash
# Desde la raíz del proyecto
docker-compose up -d

# Verificar que todos los servicios estén corriendo
docker-compose ps
```

### 🔧 Scripts de Desarrollo Disponibles

#### Backend
```bash
# Ejecutar todos los microservicios con Aspire
dotnet run --project src/AppHost

# Ejecutar un microservicio específico
dotnet run --project src/Services/User/ECommerce.User.API

# Ejecutar tests
dotnet test

# Generar migraciones
dotnet ef migrations add MigrationName --project src/Services/User/ECommerce.User.Infrastructure

# Linting y formato
dotnet format
```

#### Frontend
```bash
# Desarrollo
npm start
ng serve

# Build para producción
npm run build
ng build --configuration production

# Tests
npm test
ng test

# Tests con cobertura
npm run test:coverage

# Linting
npm run lint
ng lint

# Formato de código
npm run format
```

## 📁 Estructura de Desarrollo

### Convenciones de Nombres

#### Backend (.NET)
- **Namespaces**: `ECommerce.ServiceName.Layer`
- **Clases**: PascalCase (`UserService`, `ProductController`)
- **Métodos**: PascalCase (`GetUserById`, `CreateProduct`)
- **Variables**: camelCase (`userId`, `productName`)
- **Constantes**: PascalCase (`MaxRetryAttempts`)

#### Frontend (Angular)
- **Componentes**: kebab-case (`user-profile`, `product-list`)
- **Servicios**: PascalCase (`UserService`, `ProductService`)
- **Interfaces**: PascalCase con 'I' (`IUser`, `IProduct`)
- **Variables**: camelCase (`userName`, `productList`)
- **Constantes**: UPPER_SNAKE_CASE (`API_ENDPOINTS`)

### Patrones de Desarrollo

#### Backend - Clean Architecture

```
Service/
├── API/                    # Controllers, Middleware
├── Application/            # Use Cases, DTOs, Interfaces
│   ├── Commands/          # CQRS Commands
│   ├── Queries/           # CQRS Queries
│   ├── Services/          # Application Services
│   └── Validators/        # FluentValidation
├── Domain/                # Entities, Value Objects, Domain Services
│   ├── Entities/
│   ├── ValueObjects/
│   ├── Events/
│   └── Interfaces/
└── Infrastructure/        # Data Access, External Services
    ├── Data/
    ├── Services/
    └── Repositories/
```

#### Frontend - Modular Architecture

```
features/
├── feature-name/
│   ├── components/        # Feature-specific components
│   ├── pages/            # Route components
│   ├── services/         # Feature services
│   ├── models/           # TypeScript interfaces
│   ├── guards/           # Route guards
│   └── feature.routes.ts # Feature routing
```

### Flujo de Trabajo Git

#### Branches
- `main` - Código de producción
- `develop` - Código en desarrollo
- `feature/feature-name` - Nuevas características
- `bugfix/bug-description` - Corrección de bugs
- `hotfix/critical-fix` - Correcciones críticas

#### Commits
Usar [Conventional Commits](https://www.conventionalcommits.org/):

```bash
feat: add user authentication
fix: resolve cart calculation bug
docs: update API documentation
style: format code according to standards
refactor: restructure product service
test: add unit tests for user service
chore: update dependencies
```

## 🧪 Testing

### Backend Testing

#### Unit Tests
```bash
# Ejecutar todos los tests
dotnet test

# Ejecutar tests con cobertura
dotnet test --collect:"XPlat Code Coverage"

# Tests de un proyecto específico
dotnet test src/Services/User/ECommerce.User.Tests
```

#### Integration Tests
```bash
# Tests de integración usando TestContainers
dotnet test src/Services/User/ECommerce.User.IntegrationTests
```

### Frontend Testing

#### Unit Tests
```bash
# Tests unitarios con Jasmine/Karma
ng test

# Tests con cobertura
ng test --code-coverage

# Tests en modo watch
ng test --watch
```

#### E2E Tests
```bash
# Tests end-to-end con Cypress
ng e2e
```

### Test Data
```bash
# Seed data para desarrollo
dotnet run --project tools/DataSeeder
```

## 🔍 Debugging

### Backend Debugging
1. **Visual Studio**: F5 para debug con breakpoints
2. **VS Code**: Configurar launch.json para microservicios
3. **Docker**: `docker-compose -f docker-compose.debug.yml up`

### Frontend Debugging
1. **Chrome DevTools**: F12 en el navegador
2. **Angular DevTools**: Extensión de Chrome
3. **VS Code**: Configurar debugger for Chrome

## 📊 Monitoring y Logging

### Acceso a Herramientas de Desarrollo

Una vez que ejecutes `docker-compose up -d`, tendrás acceso a:

- **Seq (Logs)**: http://localhost:5341
- **Jaeger (Tracing)**: http://localhost:16686
- **Prometheus (Metrics)**: http://localhost:9090
- **Grafana (Dashboards)**: http://localhost:3000 (admin/admin123)
- **RabbitMQ Management**: http://localhost:15672 (admin/password123)
- **Kibana (Search Analytics)**: http://localhost:5601
- **MailHog (Email Testing)**: http://localhost:8025

### Logging en el Código

#### Backend
```csharp
// Structured logging with Serilog
_logger.LogInformation("User {UserId} created order {OrderId}", 
    userId, orderId);

_logger.LogError(ex, "Failed to process payment for order {OrderId}", 
    orderId);
```

#### Frontend
```typescript
// Console logging con categorías
console.log('[UserService] Fetching user profile');
console.error('[CartService] Failed to add item:', error);
```

## 🚀 Despliegue Local

### Desarrollo Completo
```bash
# 1. Iniciar infraestructura
docker-compose up -d

# 2. Ejecutar backend
cd backend
dotnet run --project src/AppHost

# 3. Ejecutar frontend (nueva terminal)
cd frontend
ng serve

# 4. Acceder a la aplicación
# Frontend: http://localhost:4200
# API Gateway: http://localhost:5000
# Aspire Dashboard: http://localhost:15000
```

### Solo Infraestructura
```bash
# Para ejecutar solo las dependencias (DB, Cache, etc.)
docker-compose up -d sqlserver mongodb redis rabbitmq

# Ejecutar servicios manualmente
dotnet run --project src/Services/User/ECommerce.User.API
```

## 🔧 Configuración de IDEs

### Visual Studio 2022
1. Instalar workload "ASP.NET and web development"
2. Instalar extensión "Aspire"
3. Configurar múltiples proyectos de inicio
4. Configurar debugging para microservicios

### VS Code
```json
// .vscode/launch.json para backend
{
  "version": "0.2.0",
  "configurations": [
    {
      "name": "AppHost",
      "type": "coreclr",
      "request": "launch",
      "program": "${workspaceFolder}/backend/src/AppHost/bin/Debug/net9.0/ECommerce.AppHost.dll",
      "args": [],
      "cwd": "${workspaceFolder}/backend/src/AppHost",
      "stopAtEntry": false
    }
  ]
}
```

```json
// .vscode/tasks.json para frontend
{
  "version": "2.0.0",
  "tasks": [
    {
      "type": "npm",
      "script": "start",
      "path": "frontend/",
      "group": "build",
      "label": "npm: start - frontend"
    }
  ]
}
```

## 📋 Checklist para Desarrollo

### Antes de Hacer Commit
- [ ] Código compila sin errores
- [ ] Tests unitarios pasan
- [ ] Linting sin errores
- [ ] Documentación actualizada
- [ ] Variables de entorno configuradas
- [ ] Migraciones aplicadas (si aplica)

### Antes de Pull Request
- [ ] Branch actualizado con develop
- [ ] Tests de integración pasan
- [ ] Código revisado por par
- [ ] Performance verificado
- [ ] Seguridad validada
- [ ] Documentación de API actualizada

### Comandos Útiles

```bash
# Limpiar todo y empezar de cero
docker-compose down -v
docker system prune -f
dotnet clean && dotnet restore
rm -rf frontend/node_modules && npm install

# Ver logs de un servicio específico
docker-compose logs -f sqlserver

# Acceder a contenedor
docker exec -it ecommerce-sqlserver bash

# Reset de base de datos
dotnet ef database drop --project src/Services/User/ECommerce.User.Infrastructure
dotnet ef database update --project src/Services/User/ECommerce.User.Infrastructure
```

## 🆘 Troubleshooting

### Problemas Comunes

#### "Puerto ya en uso"
```bash
# Encontrar proceso usando puerto
netstat -ano | findstr :5000
# Matar proceso
taskkill /PID [PID] /F
```

#### "Error de conexión a base de datos"
```bash
# Verificar contenedores
docker-compose ps
# Reiniciar servicio específico
docker-compose restart sqlserver
```

#### "Node modules corrupted"
```bash
# Limpiar e instalar
cd frontend
rm -rf node_modules package-lock.json
npm install
```

#### "Certificate issues"
```bash
# Trust development certificates
dotnet dev-certs https --trust
```

¿Necesitas ayuda con algún aspecto específico del desarrollo?
