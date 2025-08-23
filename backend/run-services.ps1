# Script para ejecutar servicios del e-commerce
param(
    [string]$Service = "all"
)

Write-Host "🚀 Iniciando servicios del e-commerce..." -ForegroundColor Green

# Función para verificar si un puerto está en uso
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

# Función para ejecutar un servicio
function Start-Service {
    param(
        [string]$Name,
        [string]$ProjectPath,
        [int]$Port
    )
    
    Write-Host "📦 Iniciando $Name en puerto $Port..." -ForegroundColor Yellow
    
    if (Test-Port -Port $Port) {
        Write-Host "⚠️  Puerto $Port ya está en uso. Saltando $Name." -ForegroundColor Orange
        return
    }
    
    $job = Start-Job -ScriptBlock {
        param($path, $port)
        Set-Location $using:PWD
        dotnet run --project $path --urls "http://localhost:$port"
    } -ArgumentList $ProjectPath, $Port
    
    Write-Host "✅ $Name iniciado (Job ID: $($job.Id))" -ForegroundColor Green
    return $job
}

# Verificar que estamos en el directorio correcto
if (-not (Test-Path "src")) {
    Write-Host "❌ Error: Ejecuta este script desde el directorio 'backend'" -ForegroundColor Red
    exit 1
}

# Verificar RabbitMQ
Write-Host "🐰 Verificando RabbitMQ..." -ForegroundColor Cyan
try {
    $response = Invoke-WebRequest -Uri "http://localhost:15672" -TimeoutSec 5 -ErrorAction Stop
    Write-Host "✅ RabbitMQ está funcionando" -ForegroundColor Green
}
catch {
    Write-Host "❌ RabbitMQ no está disponible. Inicia Docker: docker-compose up -d rabbitmq" -ForegroundColor Red
    exit 1
}

$jobs = @()

# Ejecutar servicios según el parámetro
switch ($Service.ToLower()) {
    "user" {
        $jobs += Start-Service -Name "User Service" -ProjectPath "src\Services\User\ECommerce.User.API\ECommerce.User.API.csproj" -Port 7001
    }
    "gateway" {
        $jobs += Start-Service -Name "API Gateway" -ProjectPath "src\ApiGateway\ECommerce.ApiGateway.csproj" -Port 7000
    }
    "all" {
        $jobs += Start-Service -Name "User Service" -ProjectPath "src\Services\User\ECommerce.User.API\ECommerce.User.API.csproj" -Port 7001
        Start-Sleep 3
        $jobs += Start-Service -Name "API Gateway" -ProjectPath "src\ApiGateway\ECommerce.ApiGateway.csproj" -Port 7000
    }
    default {
        Write-Host "❌ Servicio no reconocido: $Service" -ForegroundColor Red
        Write-Host "Opciones válidas: user, gateway, all" -ForegroundColor Yellow
        exit 1
    }
}

Write-Host "`n🎯 Servicios iniciados. Presiona Ctrl+C para detener todos." -ForegroundColor Green
Write-Host "🌐 URLs disponibles:" -ForegroundColor Cyan
Write-Host "   • API Gateway: http://localhost:7000" -ForegroundColor White
Write-Host "   • User Service: http://localhost:7001" -ForegroundColor White
Write-Host "   • RabbitMQ UI: http://localhost:15672 (admin/password123)" -ForegroundColor White

# Mantener el script corriendo
try {
    while ($true) {
        Start-Sleep 1
        
        # Verificar si algún job falló
        foreach ($job in $jobs) {
            if ($job.State -eq "Failed") {
                Write-Host "❌ Un servicio falló. Detalles:" -ForegroundColor Red
                Receive-Job $job
            }
        }
    }
}
finally {
    Write-Host "`n🛑 Deteniendo servicios..." -ForegroundColor Yellow
    $jobs | Stop-Job
    $jobs | Remove-Job
    Write-Host "✅ Servicios detenidos" -ForegroundColor Green
}

