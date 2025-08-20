# Arquitectura del Sistema E-Commerce

## 🏛️ Principios Arquitectónicos

### Clean Architecture
Cada microservicio sigue los principios de Clean Architecture:

1. **Independencia de frameworks**: El core business no depende de tecnologías específicas
2. **Testabilidad**: El sistema es fácilmente testeable
3. **Independencia de UI**: La UI puede cambiar sin afectar el core
4. **Independencia de base de datos**: Reglas de negocio no están atadas a la DB
5. **Independencia de agencias externas**: El core no sabe de servicios externos

### Patrón de Capas por Microservicio

```
┌─────────────────────────────────────────┐
│           API/Controllers               │  ← Presentation Layer
├─────────────────────────────────────────┤
│         Application Services            │  ← Application Layer
├─────────────────────────────────────────┤
│           Domain Models                 │  ← Domain Layer
├─────────────────────────────────────────┤
│       Infrastructure Services          │  ← Infrastructure Layer
└─────────────────────────────────────────┘
```

## 🔧 Microservicios Detallados

### 1. User Service - Gestión de Usuarios

**Responsabilidades:**
- Autenticación y autorización
- Gestión de perfiles de usuario
- Control de roles y permisos

**Estructura:**
```
User.Service/
├── src/
│   ├── User.API/                    # Presentation Layer
│   │   ├── Controllers/
│   │   ├── Middleware/
│   │   ├── Program.cs
│   │   └── appsettings.json
│   ├── User.Application/            # Application Layer
│   │   ├── Commands/
│   │   │   ├── CreateUser/
│   │   │   ├── UpdateUser/
│   │   │   └── DeleteUser/
│   │   ├── Queries/
│   │   │   ├── GetUser/
│   │   │   └── GetUsers/
│   │   ├── DTOs/
│   │   ├── Interfaces/
│   │   ├── Services/
│   │   └── Validators/
│   ├── User.Domain/                 # Domain Layer
│   │   ├── Entities/
│   │   │   ├── User.cs
│   │   │   ├── Role.cs
│   │   │   └── Permission.cs
│   │   ├── ValueObjects/
│   │   ├── Interfaces/
│   │   ├── Events/
│   │   └── Exceptions/
│   └── User.Infrastructure/         # Infrastructure Layer
│       ├── Data/
│       │   ├── Contexts/
│       │   ├── Configurations/
│       │   └── Repositories/
│       ├── Services/
│       │   ├── JwtService.cs
│       │   └── EmailService.cs
│       └── External/
├── tests/
└── User.Service.csproj
```

**Tecnologías:**
- ASP.NET Core Identity
- JWT Authentication
- Entity Framework Core
- SQL Server
- Redis (caching)

### 2. Product Service - Gestión de Productos

**Responsabilidades:**
- CRUD de productos
- Gestión de categorías
- Búsqueda y filtrado
- Gestión de inventario

**Estructura:**
```
Product.Service/
├── src/
│   ├── Product.API/
│   │   ├── Controllers/
│   │   │   ├── ProductsController.cs
│   │   │   ├── CategoriesController.cs
│   │   │   └── SearchController.cs
│   │   └── Program.cs
│   ├── Product.Application/
│   │   ├── Commands/
│   │   │   ├── CreateProduct/
│   │   │   ├── UpdateProduct/
│   │   │   ├── DeleteProduct/
│   │   │   └── UpdateInventory/
│   │   ├── Queries/
│   │   │   ├── GetProduct/
│   │   │   ├── SearchProducts/
│   │   │   └── GetCategories/
│   │   └── Services/
│   │       └── SearchService.cs
│   ├── Product.Domain/
│   │   ├── Entities/
│   │   │   ├── Product.cs
│   │   │   ├── Category.cs
│   │   │   └── Inventory.cs
│   │   └── ValueObjects/
│   │       ├── Price.cs
│   │       └── Dimension.cs
│   └── Product.Infrastructure/
│       ├── Data/
│       │   └── MongoContext.cs
│       ├── Repositories/
│       └── Services/
│           └── ElasticSearchService.cs
└── tests/
```

**Tecnologías:**
- MongoDB
- ElasticSearch
- AutoMapper
- FluentValidation

### 3. Order Service - Gestión de Pedidos

**Responsabilidades:**
- Procesamiento de pedidos
- Seguimiento de estados
- Historial de pedidos
- Coordinación con otros servicios

**Estructura:**
```
Order.Service/
├── src/
│   ├── Order.API/
│   ├── Order.Application/
│   │   ├── Commands/
│   │   │   ├── CreateOrder/
│   │   │   ├── UpdateOrderStatus/
│   │   │   └── CancelOrder/
│   │   ├── Queries/
│   │   │   ├── GetOrder/
│   │   │   └── GetOrderHistory/
│   │   ├── Sagas/
│   │   │   └── OrderProcessingSaga.cs
│   │   └── Services/
│   ├── Order.Domain/
│   │   ├── Entities/
│   │   │   ├── Order.cs
│   │   │   ├── OrderItem.cs
│   │   │   └── ShippingAddress.cs
│   │   ├── Enums/
│   │   │   └── OrderStatus.cs
│   │   └── Events/
│   │       ├── OrderCreatedEvent.cs
│   │       └── OrderCompletedEvent.cs
│   └── Order.Infrastructure/
│       ├── Data/
│       └── EventBus/
└── tests/
```

**Tecnologías:**
- SQL Server
- MassTransit (Event Bus)
- Saga Pattern
- Redis

### 4. Payment Service - Procesamiento de Pagos

**Responsabilidades:**
- Integración con pasarelas de pago
- Procesamiento de transacciones
- Manejo de webhooks
- Gestión de reembolsos

**Estructura:**
```
Payment.Service/
├── src/
│   ├── Payment.API/
│   │   ├── Controllers/
│   │   │   ├── PaymentsController.cs
│   │   │   └── WebhooksController.cs
│   │   └── Program.cs
│   ├── Payment.Application/
│   │   ├── Commands/
│   │   │   ├── ProcessPayment/
│   │   │   ├── RefundPayment/
│   │   │   └── HandleWebhook/
│   │   └── Services/
│   │       ├── IPaymentGateway.cs
│   │       └── PaymentService.cs
│   ├── Payment.Domain/
│   │   ├── Entities/
│   │   │   ├── Payment.cs
│   │   │   └── Transaction.cs
│   │   └── Enums/
│   │       └── PaymentStatus.cs
│   └── Payment.Infrastructure/
│       ├── Gateways/
│   │       ├── StripeGateway.cs
│   │       └── PayPalGateway.cs
│       └── Services/
└── tests/
```

### 5. Notification Service - Sistema de Notificaciones

**Responsabilidades:**
- Envío de emails
- Notificaciones push
- SMS
- Notificaciones en tiempo real

**Estructura:**
```
Notification.Service/
├── src/
│   ├── Notification.API/
│   ├── Notification.Application/
│   │   ├── Commands/
│   │   │   ├── SendEmail/
│   │   │   ├── SendPushNotification/
│   │   │   └── SendSMS/
│   │   └── Services/
│   ├── Notification.Domain/
│   │   ├── Entities/
│   │   │   └── NotificationTemplate.cs
│   │   └── Enums/
│   │       └── NotificationType.cs
│   └── Notification.Infrastructure/
│       ├── Services/
│       │   ├── EmailService.cs
│       │   ├── SMSService.cs
│       │   └── PushNotificationService.cs
│       └── External/
└── tests/
```

### 6. File Service - Gestión de Archivos

**Responsabilidades:**
- Subida de archivos
- Procesamiento de imágenes
- Almacenamiento en la nube
- Optimización de assets

**Estructura:**
```
File.Service/
├── src/
│   ├── File.API/
│   ├── File.Application/
│   │   ├── Commands/
│   │   │   ├── UploadFile/
│   │   │   ├── ProcessImage/
│   │   │   └── DeleteFile/
│   │   └── Services/
│   ├── File.Domain/
│   │   └── Entities/
│   │       └── FileMetadata.cs
│   └── File.Infrastructure/
│       ├── Services/
│       │   ├── CloudinaryService.cs
│       │   └── ImageProcessingService.cs
│       └── Storage/
└── tests/
```

## 🔄 Comunicación entre Microservicios

### Patrones de Comunicación

1. **Synchronous**: HTTP/REST para consultas inmediatas
2. **Asynchronous**: Event-driven para operaciones no críticas
3. **Saga Pattern**: Para transacciones distribuidas complejas

### Event Bus
```csharp
// Ejemplo de evento
public class OrderCreatedEvent : IIntegrationEvent
{
    public Guid OrderId { get; set; }
    public Guid UserId { get; set; }
    public decimal TotalAmount { get; set; }
    public List<OrderItem> Items { get; set; }
    public DateTime CreatedAt { get; set; }
}
```

### API Gateway
- **Entrada única** para todas las peticiones del frontend
- **Routing** automático a microservicios
- **Authentication** centralizada
- **Rate limiting** y **throttling**
- **Logging** y **monitoring** unificado

## 🔒 Seguridad

### Autenticación y Autorización
- **JWT tokens** con refresh tokens
- **OAuth 2.0** para integraciones externas
- **Role-based access control** (RBAC)
- **API Keys** para servicios externos

### Comunicación Segura
- **HTTPS** obligatorio en producción
- **mTLS** entre microservicios
- **API Gateway** como punto de entrada único
- **Secrets management** con Azure Key Vault

## 📊 Monitoring y Observabilidad

### Herramientas
- **Application Insights** - Telemetría
- **Serilog** - Logging estructurado
- **Health Checks** - Monitoreo de salud
- **Distributed Tracing** - Seguimiento de requests

### Métricas Clave
- Response time por endpoint
- Error rates
- Throughput
- Resource utilization
- Business metrics (orders, revenue, etc.)

## 🚀 Despliegue

### Containerización
- **Docker** para cada microservicio
- **Multi-stage builds** para optimización
- **Health checks** integrados

### Orquestación
- **Kubernetes** en Azure AKS
- **Helm charts** para deployment
- **GitOps** con ArgoCD

### Environments
- **Development**: Docker Compose local
- **Staging**: AKS con datos de prueba
- **Production**: AKS con alta disponibilidad
