# E-Commerce Platform - Angular 20 + .NET 9 Aspire

## 🏗️ Arquitectura del Sistema

Este proyecto implementa una plataforma de e-commerce moderna utilizando:

### Frontend
- **Angular 20** con Signals y Stores para state management
- **Tailwind CSS** para estilos modulares
- **Arquitectura modular** con componentes separados

### Backend
- **.NET 9 Aspire** con microservicios
- **Clean Architecture** en cada microservicio
- **API Gateway** para enrutamiento centralizado

## 📁 Estructura del Proyecto

```
e-commerce/
├── frontend/                    # Angular 20 Application
│   ├── src/
│   │   ├── app/
│   │   │   ├── core/           # Core services, guards, interceptors
│   │   │   ├── shared/         # Shared components, pipes, directives
│   │   │   ├── features/       # Feature modules
│   │   │   │   ├── auth/
│   │   │   │   ├── products/
│   │   │   │   ├── orders/
│   │   │   │   ├── admin/
│   │   │   │   └── payment/
│   │   │   └── stores/         # Signal stores for state management
│   │   ├── assets/
│   │   └── styles/             # Tailwind configuration
│   ├── tailwind.config.js
│   └── package.json
├── backend/                     # .NET 9 Aspire Solution
│   ├── src/
│   │   ├── ApiGateway/         # API Gateway (Ocelot/YARP)
│   │   ├── Services/           # Microservices
│   │   │   ├── User.Service/
│   │   │   ├── Product.Service/
│   │   │   ├── Order.Service/
│   │   │   ├── Payment.Service/
│   │   │   ├── Notification.Service/
│   │   │   └── File.Service/
│   │   ├── BuildingBlocks/     # Shared libraries
│   │   │   ├── Common/
│   │   │   ├── EventBus/
│   │   │   └── Security/
│   │   └── AppHost/            # Aspire App Host
│   └── ECommerce.sln
├── docker-compose.yml          # Development containers
└── docs/                       # Documentation
    ├── architecture.md
    ├── api-specs/
    └── deployment.md
```

## 🚀 Características Principales

### ✅ Gestión de Usuarios
- Autenticación JWT con refresh tokens
- Perfiles de usuario y gestión de roles
- Control de acceso basado en roles (RBAC)

### ✅ Gestión de Productos
- CRUD completo para productos
- Categorización y filtrado avanzado
- Búsqueda con ElasticSearch
- Gestión de inventario en tiempo real

### ✅ Experiencia de Compra
- Carrito de compras persistente
- Lista de deseos
- Recomendaciones personalizadas
- Checkout optimizado

### ✅ Procesamiento de Pedidos
- Flujo de pedidos con Saga pattern
- Seguimiento en tiempo real
- Historial completo de pedidos

### ✅ Integración de Pagos
- Múltiples pasarelas (Stripe, PayPal)
- Webhooks para confirmación
- Manejo seguro de transacciones

### ✅ Panel Administrativo
- Dashboard con analytics
- Gestión de productos e inventario
- Reportes de ventas
- Gestión de usuarios y pedidos

## 🛠️ Tecnologías Utilizadas

### Frontend
- **Angular 20** - Framework principal
- **Angular Signals** - Gestión reactiva de estado
- **RxJS** - Programación reactiva
- **Tailwind CSS** - Framework de estilos
- **Angular Material** - Componentes UI

### Backend
- **.NET 9** - Runtime y SDK
- **ASP.NET Core** - Framework web
- **Aspire** - Orquestación de microservicios
- **Entity Framework Core** - ORM
- **MediatR** - Patrón mediator
- **FluentValidation** - Validaciones
- **Mapster** - Object mapping

### Bases de Datos
- **SQL Server** - Datos transaccionales
- **MongoDB** - Catálogo de productos
- **Redis** - Cache distribuido
- **ElasticSearch** - Búsqueda avanzada

### DevOps
- **Docker** - Contenedores
- **GitHub Actions** - CI/CD
- **Azure** - Cloud hosting
- **Terraform** - Infrastructure as Code

## 🏃‍♂️ Inicio Rápido

### Prerrequisitos
- .NET 9 SDK
- Node.js 20+
- Docker Desktop
- SQL Server / MongoDB

### Instalación

1. **Clonar el repositorio**
   ```bash
   git clone https://github.com/tu-usuario/e-commerce.git
   cd e-commerce
   ```

2. **Setup Backend**
   ```bash
   cd backend
   dotnet restore
   dotnet run --project src/AppHost
   ```

3. **Setup Frontend**
   ```bash
   cd frontend
   npm install
   ng serve
   ```

4. **Levantar servicios de infraestructura**
   ```bash
   docker-compose up -d
   ```

## 📖 Documentación

- [Arquitectura del Sistema](docs/architecture.md)
- [Especificaciones de API](docs/api-specs/)
- [Guía de Despliegue](docs/deployment.md)

## 🤝 Contribución

1. Fork el proyecto
2. Crea una rama para tu feature (`git checkout -b feature/AmazingFeature`)
3. Commit tus cambios (`git commit -m 'Add some AmazingFeature'`)
4. Push a la rama (`git push origin feature/AmazingFeature`)
5. Abre un Pull Request

## 📄 Licencia

Este proyecto está bajo la Licencia MIT - ver el archivo [LICENSE](LICENSE) para detalles.

## 👥 Equipo

- **Lead Developer** - [Tu Nombre]
- **Frontend Architect** - [Nombre]
- **Backend Architect** - [Nombre]
- **DevOps Engineer** - [Nombre]
