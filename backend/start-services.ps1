# Script para ejecutar todos los microservicios
Write-Host "🚀 Iniciando microservicios..." -ForegroundColor Green

# Compilar todos los proyectos primero
Write-Host "📦 Compilando proyectos..." -ForegroundColor Yellow
dotnet build --no-restore

# Ejecutar servicios en background
Write-Host "🔐 Iniciando User Service en puerto 5001..." -ForegroundColor Cyan
Start-Process powershell -ArgumentList "-Command", "cd '$PWD'; dotnet run --project 'src\Services\User\ECommerce.User.API\ECommerce.User.API.csproj' --urls 'http://localhost:5001'"

Start-Sleep -Seconds 3

Write-Host "📦 Iniciando Product Service en puerto 5002..." -ForegroundColor Cyan  
Start-Process powershell -ArgumentList "-Command", "cd '$PWD'; dotnet run --project 'src\Services\Product\ECommerce.Product.API\ECommerce.Product.API.csproj' --urls 'http://localhost:5002'"

Start-Sleep -Seconds 3

Write-Host "🛍️ Iniciando Order Service en puerto 5003..." -ForegroundColor Cyan
Start-Process powershell -ArgumentList "-Command", "cd '$PWD'; dotnet run --project 'src\Services\Order\ECommerce.Order.API\ECommerce.Order.API.csproj' --urls 'http://localhost:5003'"

Start-Sleep -Seconds 3

Write-Host "💳 Iniciando Payment Service en puerto 5004..." -ForegroundColor Cyan
Start-Process powershell -ArgumentList "-Command", "cd '$PWD'; dotnet run --project 'src\Services\Payment\ECommerce.Payment.API\ECommerce.Payment.API.csproj' --urls 'http://localhost:5004'"

Start-Sleep -Seconds 3

Write-Host "🌐 Iniciando API Gateway en puerto 5000..." -ForegroundColor Green
Start-Process powershell -ArgumentList "-Command", "cd '$PWD'; dotnet run --project 'src\ApiGateway\ECommerce.ApiGateway.csproj' --urls 'http://localhost:5000'"

Write-Host "⏳ Esperando 10 segundos para que todos los servicios se inicien..." -ForegroundColor Yellow
Start-Sleep -Seconds 10

Write-Host "✅ Todos los servicios deberían estar ejecutándose!" -ForegroundColor Green
Write-Host ""
Write-Host "📋 URLs de los servicios:" -ForegroundColor White
Write-Host "   🌐 API Gateway:    http://localhost:5000" -ForegroundColor Cyan
Write-Host "   🔐 User Service:   http://localhost:5001" -ForegroundColor Cyan  
Write-Host "   📦 Product Service: http://localhost:5002" -ForegroundColor Cyan
Write-Host "   🛍️ Order Service:   http://localhost:5003" -ForegroundColor Cyan
Write-Host "   💳 Payment Service: http://localhost:5004" -ForegroundColor Cyan
Write-Host ""
Write-Host "🧪 Probando conectividad..." -ForegroundColor Yellow

# Probar conectividad
$services = @(
    @{Name="API Gateway"; Url="http://localhost:5000"},
    @{Name="User Service"; Url="http://localhost:5001"},
    @{Name="Product Service"; Url="http://localhost:5002"},
    @{Name="Order Service"; Url="http://localhost:5003"},
    @{Name="Payment Service"; Url="http://localhost:5004"}
)

foreach ($service in $services) {
    try {
        $response = Invoke-WebRequest -Uri $service.Url -TimeoutSec 5 -UseBasicParsing
        Write-Host "✅ $($service.Name): OK (Status: $($response.StatusCode))" -ForegroundColor Green
    }
    catch {
        Write-Host "❌ $($service.Name): Error - $($_.Exception.Message)" -ForegroundColor Red
    }
}

Write-Host ""
Write-Host "🎯 Para probar el API Gateway, usa estos endpoints:" -ForegroundColor White
Write-Host "   GET  http://localhost:5000/api/users (User Service via Gateway)" -ForegroundColor Cyan
Write-Host "   GET  http://localhost:5000/api/products (Product Service via Gateway)" -ForegroundColor Cyan
Write-Host "   GET  http://localhost:5000/api/orders (Order Service via Gateway)" -ForegroundColor Cyan
Write-Host "   GET  http://localhost:5000/api/payments (Payment Service via Gateway)" -ForegroundColor Cyan







