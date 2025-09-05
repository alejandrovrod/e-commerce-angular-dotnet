# Script para reemplazar variables de entorno en archivos JSON
Write-Host "=== Reemplazando variables de entorno ==="

# Leer el archivo appsettings.Clusters.json
$clustersPath = "appsettings.Clusters.json"
$content = Get-Content $clustersPath -Raw

# Obtener variables de entorno
$productServiceUrl = $env:PRODUCT_SERVICE_URL
$userServiceUrl = $env:USER_SERVICE_URL
$orderServiceUrl = $env:ORDER_SERVICE_URL
$paymentServiceUrl = $env:PAYMENT_SERVICE_URL
$fileServiceUrl = $env:FILE_SERVICE_URL

# Si no hay variables de entorno, usar localhost
if (-not $productServiceUrl) { $productServiceUrl = "http://localhost:7002" }
if (-not $userServiceUrl) { $userServiceUrl = "http://localhost:7001" }
if (-not $orderServiceUrl) { $orderServiceUrl = "http://localhost:7003" }
if (-not $paymentServiceUrl) { $paymentServiceUrl = "http://localhost:7004" }
if (-not $fileServiceUrl) { $fileServiceUrl = "http://localhost:7005" }

Write-Host "Product Service URL: $productServiceUrl"
Write-Host "User Service URL: $userServiceUrl"
Write-Host "Order Service URL: $orderServiceUrl"
Write-Host "Payment Service URL: $paymentServiceUrl"
Write-Host "File Service URL: $fileServiceUrl"

# Reemplazar variables
$content = $content -replace '\$\{PRODUCT_SERVICE_URL\}', $productServiceUrl
$content = $content -replace '\$\{USER_SERVICE_URL\}', $userServiceUrl
$content = $content -replace '\$\{ORDER_SERVICE_URL\}', $orderServiceUrl
$content = $content -replace '\$\{PAYMENT_SERVICE_URL\}', $paymentServiceUrl
$content = $content -replace '\$\{FILE_SERVICE_URL\}', $fileServiceUrl

# Escribir el archivo actualizado
Set-Content $clustersPath -Value $content

Write-Host "Variables reemplazadas exitosamente!"
Write-Host "=== Fin del reemplazo ==="
