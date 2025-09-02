# Script para probar servicios individuales
param(
    [string]$Service = "user"
)

Write-Host "🧪 Probando servicios del e-commerce..." -ForegroundColor Green

function Test-Service {
    param(
        [string]$Name,
        [string]$Url
    )
    
    Write-Host "🔍 Probando $Name..." -ForegroundColor Yellow
    
    try {
        $response = Invoke-WebRequest -Uri $Url -TimeoutSec 10 -ErrorAction Stop
        Write-Host "✅ $Name: OK (Status: $($response.StatusCode))" -ForegroundColor Green
        return $true
    }
    catch {
        Write-Host "❌ $Name: Error - $($_.Exception.Message)" -ForegroundColor Red
        return $false
    }
}

function Test-Port {
    param([int]$Port)
    try {
        $connection = New-Object System.Net.Sockets.TcpClient
        $connection.Connect("localhost", $Port)
        $connection.Close()
        return $true
    }
    catch {
        return $false
    }
}

# Verificar RabbitMQ primero
Write-Host "🐰 Verificando RabbitMQ..." -ForegroundColor Cyan
if (Test-Service -Name "RabbitMQ Management" -Url "http://localhost:15672") {
    Write-Host "✅ RabbitMQ está disponible" -ForegroundColor Green
} else {
    Write-Host "⚠️  RabbitMQ no está disponible" -ForegroundColor Orange
}

Write-Host ""

# Probar servicios según el parámetro
switch ($Service.ToLower()) {
    "user" {
        Write-Host "👤 Probando User Service..." -ForegroundColor Magenta
        
        # Verificar puerto
        if (Test-Port -Port 7001) {
            Test-Service -Name "User Health" -Url "http://localhost:7001/api/health"
            Test-Service -Name "User Detailed Health" -Url "http://localhost:7001/api/health/detailed"
        } else {
            Write-Host "❌ User Service no está escuchando en puerto 7001" -ForegroundColor Red
        }
    }
    
    "gateway" {
        Write-Host "🌐 Probando API Gateway..." -ForegroundColor Magenta
        
        # Verificar puerto
        if (Test-Port -Port 7000) {
            Test-Service -Name "Gateway Root" -Url "http://localhost:7000"
            Test-Service -Name "Gateway Health" -Url "http://localhost:7000/api/health"
            Test-Service -Name "Gateway Services Health" -Url "http://localhost:7000/api/health/services"
            Test-Service -Name "Gateway Test User Service" -Url "http://localhost:7000/api/health/test-user-service"
        } else {
            Write-Host "❌ API Gateway no está escuchando en puerto 7000" -ForegroundColor Red
        }
    }
    
    "all" {
        Write-Host "🔍 Probando todos los servicios..." -ForegroundColor Magenta
        
        $services = @(
            @{Name="API Gateway"; Port=7000; Url="http://localhost:7000"},
            @{Name="User Service"; Port=7001; Url="http://localhost:7001/api/health"},
            @{Name="Product Service"; Port=7002; Url="http://localhost:7002/api/health"},
            @{Name="Order Service"; Port=7003; Url="http://localhost:7003/api/health"},
            @{Name="Payment Service"; Port=7004; Url="http://localhost:7004/api/health"},
            @{Name="File Service"; Port=7005; Url="http://localhost:7005/api/health"},
            @{Name="Notification Service"; Port=7006; Url="http://localhost:7006/api/health"}
        )
        
        foreach ($svc in $services) {
            if (Test-Port -Port $svc.Port) {
                Test-Service -Name $svc.Name -Url $svc.Url
            } else {
                Write-Host "❌ $($svc.Name) no está ejecutándose (Puerto $($svc.Port))" -ForegroundColor Red
            }
        }
    }
    
    default {
        Write-Host "❌ Servicio no reconocido: $Service" -ForegroundColor Red
        Write-Host "Opciones válidas: user, gateway, all" -ForegroundColor Yellow
    }
}

Write-Host "`n🎯 Prueba completada!" -ForegroundColor Green









