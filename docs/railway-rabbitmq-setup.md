# 🐰 RabbitMQ Setup en Railway

## Opción 1: CloudAMQP (Recomendado)

### 1. Agregar CloudAMQP a tu proyecto Railway

```bash
# Desde Railway CLI
railway add cloudamqp

# O desde Railway Dashboard:
# 1. Ir a tu proyecto
# 2. Click en "Add Service"
# 3. Buscar "CloudAMQP"
# 4. Seleccionar plan (Little Lemur - Free)
```

### 2. Configurar variables de entorno

Railway automáticamente creará estas variables:
- `CLOUDAMQP_URL` - URL completa de conexión

### 3. Actualizar tu aplicación

```csharp
// Program.cs
var rabbitmqUrl = builder.Configuration.GetConnectionString("EventBus") 
    ?? Environment.GetEnvironmentVariable("CLOUDAMQP_URL") 
    ?? "amqp://admin:password123@localhost:5672";

builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(rabbitmqUrl);
        cfg.ConfigureEndpoints(context);
    });
});
```

## Opción 2: RabbitMQ Template

### 1. Usar template de RabbitMQ

```bash
# Desde Railway CLI
railway add --template rabbitmq

# Esto creará:
# - Servicio RabbitMQ
# - Variables de entorno automáticas
```

### 2. Variables de entorno creadas:

- `RABBITMQ_URL` - URL de conexión
- `RABBITMQ_USERNAME` - Usuario
- `RABBITMQ_PASSWORD` - Contraseña

## Opción 3: Manual con Docker

### 1. Crear servicio personalizado

```dockerfile
# railway.dockerfile
FROM rabbitmq:3-management
ENV RABBITMQ_DEFAULT_USER=admin
ENV RABBITMQ_DEFAULT_PASS=railway123
EXPOSE 5672 15672
```

### 2. Configurar en Railway

```toml
# railway.toml
[build]
dockerfile = "railway.dockerfile"

[deploy]
healthcheckPath = "/api/health"
healthcheckTimeout = 300
```

## 🔗 URLs de Conexión

### Formato CloudAMQP:
```
amqps://username:password@host.cloudamqp.com/vhost
```

### Formato Railway Template:
```
amqp://username:password@hostname:5672
```

## 🧪 Testing

### 1. Verificar conexión local

```bash
# Instalar RabbitMQ CLI (opcional)
docker run --rm -it rabbitmq:3-management rabbitmqctl status
```

### 2. Verificar en Railway

```csharp
// Health check endpoint
[HttpGet("health/rabbitmq")]
public async Task<IActionResult> CheckRabbitMQ()
{
    try
    {
        await _eventBus.PublishAsync(new TestEvent());
        return Ok("RabbitMQ connected");
    }
    catch (Exception ex)
    {
        return StatusCode(500, $"RabbitMQ error: {ex.Message}");
    }
}
```

## 💰 Costos Estimados

- **CloudAMQP Little Lemur**: Gratis (20 conexiones, 1GB/mes)
- **CloudAMQP Tiger**: $19/mes (100 conexiones, 5GB/mes)
- **RabbitMQ Template**: ~$5-10/mes (depende del uso)

## 🚀 Deploy Commands

```bash
# 1. Configurar Railway
railway login
railway link

# 2. Agregar RabbitMQ
railway add cloudamqp

# 3. Configurar variables
railway variables set RABBITMQ_URL=$CLOUDAMQP_URL

# 4. Deploy
railway up
```

## 📋 Variables de entorno necesarias

```bash
# En Railway Dashboard > Variables
JWT_SECRET_KEY=your-super-secure-jwt-key-here
DATABASE_URL=postgresql://user:pass@host:port/db
RABBITMQ_URL=amqps://user:pass@host.cloudamqp.com/vhost
REDIS_URL=redis://user:pass@host:port
```




