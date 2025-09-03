# 🚀 Desplegando el Payment Service en Railway

## 📋 Pasos Detallados

### **Paso 1: Crear el Proyecto en Railway**

1. **Ve a Railway** → https://railway.app
2. **Inicia sesión** con tu cuenta
3. **Crea un nuevo proyecto** → "New Project"
4. **Selecciona** → "Deploy from GitHub repo"
5. **Conecta tu repositorio** e-commerce
6. **Configura el directorio raíz** como: `backend/src/Services/Payment/ECommerce.Payment.API`

### **Paso 2: Configurar Variables de Entorno**

En Railway, ve a **Variables** y agrega estas variables:

```bash
# Base de datos PostgreSQL
DATABASE_URL=postgresql://usuario:password@host:puerto/database

# Stripe Secret Key (¡MUY IMPORTANTE!)
STRIPE_SECRET_KEY=sk_test_tu_clave_secreta_de_stripe

# RabbitMQ para mensajería
RABBITMQ_URL=amqp://usuario:password@host:puerto

# Configuración de la aplicación
ASPNETCORE_ENVIRONMENT=Production

# Puerto (Railway lo asigna automáticamente)
PORT=7004
```

### **Paso 3: Verificar la Configuración**

Railway debería detectar automáticamente:
- ✅ **Dockerfile** en `backend/src/Services/Payment/ECommerce.Payment.API/Dockerfile`
- ✅ **railway.json** con la configuración de despliegue
- ✅ **Health check** en `/health`

### **Paso 4: Desplegar**

1. **Haz clic en "Deploy"**
2. **Espera** a que se complete el build
3. **Verifica** que el health check pase: `https://tu-payment-service.railway.app/health`

### **Paso 5: Obtener la URL del Servicio**

Una vez desplegado, Railway te dará una URL como:
```
https://payment-service-production-xxxx.up.railway.app
```

**¡Guarda esta URL!** La necesitarás para configurar el API Gateway.

## 🔧 Configurar el API Gateway

### **Paso 6: Actualizar Variables de Entorno del API Gateway**

En Railway, ve al proyecto del **API Gateway** y agrega:

```bash
PAYMENT_SERVICE_URL=https://tu-payment-service.railway.app
```

### **Paso 7: Reiniciar el API Gateway**

1. **Ve al proyecto del API Gateway**
2. **Haz clic en "Restart"**
3. **Verifica** que los logs muestren la URL del Payment Service

## 🧪 Verificar el Despliegue

### **Endpoints Disponibles:**

- **Health Check**: `https://tu-payment-service.railway.app/health`
- **Pagos**: `https://tu-api-gateway.railway.app/api/payments/*`

### **Funcionalidades del Payment Service:**

- ✅ **Procesar pagos** - Integración con Stripe
- ✅ **Gestionar métodos de pago** - Tarjetas, PayPal, etc.
- ✅ **Reembolsos** - Procesar reembolsos
- ✅ **Historial de pagos** - Ver historial de transacciones
- ✅ **Webhooks** - Manejar notificaciones de Stripe

## 💳 Configuración de Stripe

### **Variables de Entorno Críticas:**

- **STRIPE_SECRET_KEY**: Clave secreta de Stripe (empieza con `sk_test_` o `sk_live_`)
- **STRIPE_PUBLISHABLE_KEY**: Clave pública (para el frontend)

### **Obtener las Claves de Stripe:**

1. **Ve a Stripe Dashboard** → https://dashboard.stripe.com
2. **Inicia sesión** o crea una cuenta
3. **Ve a Developers** → **API Keys**
4. **Copia la Secret Key** (empieza con `sk_test_`)

### **Ejemplo de STRIPE_SECRET_KEY:**
```bash
STRIPE_SECRET_KEY=sk_test_51ABC123...tu_clave_completa_de_stripe
```

## 🔄 Integración con Otros Servicios

### **Dependencias:**
- **User Service** - Para validar usuarios
- **Order Service** - Para procesar pagos de órdenes
- **RabbitMQ** - Para comunicación asíncrona

### **Eventos que Publica:**
- `PaymentProcessed` - Cuando se procesa un pago exitosamente
- `PaymentFailed` - Cuando falla un pago
- `RefundProcessed` - Cuando se procesa un reembolso

## 🚨 Solución de Problemas

### **Si el build falla:**
- Verifica que el Dockerfile esté en la ubicación correcta
- Revisa los logs de build para errores específicos

### **Si el health check falla:**
- Verifica que las variables de entorno estén configuradas
- Revisa los logs del servicio para errores de conexión
- Asegúrate de que la base de datos esté accesible

### **Si el API Gateway no puede conectar:**
- Verifica que `PAYMENT_SERVICE_URL` esté configurada correctamente
- Asegúrate de que la URL termine con `/` (ej: `https://service.railway.app/`)

### **Si Stripe no funciona:**
- Verifica que `STRIPE_SECRET_KEY` esté configurada correctamente
- Asegúrate de usar la clave de test (`sk_test_`) para desarrollo
- Revisa los logs para errores de autenticación con Stripe

## 📊 Monitoreo

Una vez desplegado, puedes monitorear:
- **Logs** en tiempo real en Railway
- **Métricas** de CPU y memoria
- **Health checks** automáticos
- **Despliegues** y rollbacks
- **Transacciones de Stripe** en el dashboard

¡El Payment Service debería estar funcionando correctamente! 🎉
