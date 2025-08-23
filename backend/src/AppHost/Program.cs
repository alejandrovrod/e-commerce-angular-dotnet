// Versión simplificada pero funcional del E-Commerce AppHost
// Este archivo ejecuta todos los servicios de forma coordinada

using System.Diagnostics;

Console.WriteLine("🚀 Iniciando E-Commerce AppHost Simplificado");
Console.WriteLine("⚠️  Nota: Las herramientas completas de Aspire requieren configuración adicional");
Console.WriteLine("📋 Iniciando servicios manualmente con orquestación básica...\n");

var services = new List<(string name, string path, int port, string description)>
{
    ("User Service", @"C:\Workspace\Cursor\e-commerce\backend\src\Services\User\ECommerce.User.API\ECommerce.User.API.csproj", 7001, "👤 Gestión de usuarios y autenticación"),
    ("Product Service", @"C:\Workspace\Cursor\e-commerce\backend\src\Services\Product\ECommerce.Product.API\ECommerce.Product.API.csproj", 7002, "📦 Catálogo de productos y gestión de inventario"),
    ("API Gateway", @"C:\Workspace\Cursor\e-commerce\backend\src\ApiGateway\ECommerce.ApiGateway.csproj", 7000, "🌐 Punto de entrada principal")
};

var processes = new List<Process>();
var cancellationTokenSource = new CancellationTokenSource();

// Manejar Ctrl+C para shutdown limpio
Console.CancelKeyPress += (_, e) =>
{
    e.Cancel = true;
    cancellationTokenSource.Cancel();
};

try
{
    // Verificar dependencias externas
    Console.WriteLine("🔍 Verificando dependencias externas...");
    
    // Verificar Docker y RabbitMQ
    await CheckDependency("Docker", async () =>
    {
        using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(3) };
        await client.GetAsync("http://localhost:15672");
        return true;
    }, "RabbitMQ Management en http://localhost:15672");

    // Verificar Redis
    await CheckDependency("Redis", async () =>
    {
        using var tcpClient = new System.Net.Sockets.TcpClient();
        await tcpClient.ConnectAsync("localhost", 6379);
        return true;
    }, "Redis en localhost:6379");

    Console.WriteLine();

    // Iniciar servicios
    foreach (var (name, path, port, description) in services)
    {
        Console.WriteLine($"🚀 Iniciando {name}...");
        Console.WriteLine($"   📍 Puerto: {port}");
        Console.WriteLine($"   📝 {description}");
        
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"run --project \"{path}\" --urls \"http://localhost:{port}\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            }
        };

        process.OutputDataReceived += (sender, args) =>
        {
            if (!string.IsNullOrEmpty(args.Data))
                Console.WriteLine($"[{name}] {args.Data}");
        };

        process.ErrorDataReceived += (sender, args) =>
        {
            if (!string.IsNullOrEmpty(args.Data))
                Console.WriteLine($"[{name}] ⚠️  {args.Data}");
        };

        if (process.Start())
        {
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            processes.Add(process);
            Console.WriteLine($"   ✅ {name} iniciado correctamente\n");
            
            // Esperar entre servicios para evitar conflictos de puertos
            await Task.Delay(3000, cancellationTokenSource.Token);
        }
        else
        {
            Console.WriteLine($"   ❌ Error iniciando {name}\n");
        }
    }

    Console.WriteLine("🎯 ¡Todos los servicios están ejecutándose!");
    Console.WriteLine("\n📊 URLs disponibles:");
    Console.WriteLine("   🌐 API Gateway:           http://localhost:7000");
    Console.WriteLine("   👤 User Service:          http://localhost:7001");
    Console.WriteLine("   📦 Product Service:       http://localhost:7002");
    Console.WriteLine("   🔍 API Gateway Health:    http://localhost:7000/api/health");
    Console.WriteLine("   💓 User Service Health:   http://localhost:7001/api/health");
    Console.WriteLine("   📦 Product Service Health: http://localhost:7002/api/health");
    Console.WriteLine("   🐰 RabbitMQ Management:   http://localhost:15672 (admin/password123)");
    Console.WriteLine("   📦 Redis:                 localhost:6379");
    
    Console.WriteLine("\n🧪 Pruebas rápidas:");
    Console.WriteLine("   curl http://localhost:7000/api/health");
    Console.WriteLine("   curl http://localhost:7001/api/health");
    Console.WriteLine("   curl http://localhost:7002/api/health");
    Console.WriteLine("   curl http://localhost:7000/api/health/test-user-service");

    Console.WriteLine("\n⚠️  Presiona Ctrl+C para detener todos los servicios");
    Console.WriteLine("🔧 Para el dashboard completo de Aspire, se necesita configuración adicional de herramientas");

    // Mantener vivo hasta Ctrl+C
    await Task.Delay(Timeout.Infinite, cancellationTokenSource.Token);
}
catch (OperationCanceledException)
{
    Console.WriteLine("\n🛑 Deteniendo servicios...");
}
finally
{
    // Limpiar procesos
    foreach (var process in processes)
    {
        try
        {
            if (!process.HasExited)
            {
                process.Kill();
                process.WaitForExit(5000);
            }
            process.Dispose();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠️  Error deteniendo proceso: {ex.Message}");
        }
    }
    
    Console.WriteLine("✅ Todos los servicios detenidos correctamente");
}

static async Task CheckDependency(string name, Func<Task<bool>> check, string description)
{
    try
    {
        if (await check())
        {
            Console.WriteLine($"   ✅ {name}: {description} - Disponible");
        }
    }
    catch
    {
        Console.WriteLine($"   ⚠️  {name}: {description} - No disponible");
        Console.WriteLine($"      💡 Para iniciar: docker-compose up -d");
    }
}
