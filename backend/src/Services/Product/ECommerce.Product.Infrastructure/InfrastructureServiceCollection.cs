using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using ECommerce.Product.Application.Interfaces;
using ECommerce.Product.Infrastructure.Data;
using ECommerce.Product.Infrastructure.Services;
using ECommerce.Product.Domain.Repositories;
using ECommerce.Product.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Product.Infrastructure;

public static class InfrastructureServiceCollection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Add DbContext
        var connectionString = configuration.GetConnectionString("ProductsDb");
        Console.WriteLine($"InfrastructureServiceCollection: Cadena de conexión configurada: {connectionString}");
        
        // Forzar conexión remota si no se encuentra la cadena de conexión
        if (string.IsNullOrEmpty(connectionString))
        {
            connectionString = "Server=db11545.public.databaseasp.net; Database=db11545; User Id=db11545; Password=Xd6?b!K9Y2_t;TrustServerCertificate=True;";
            Console.WriteLine($"InfrastructureServiceCollection: Usando cadena de conexión hardcodeada: {connectionString}");
        }
        
        services.AddDbContext<ProductDbContext>(options =>
            options.UseSqlServer(
                connectionString,
                b => b.MigrationsAssembly(typeof(ProductDbContext).Assembly.FullName)
                    .EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorNumbersToAdd: null)));

        // Add Repositories
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IBrandRepository, BrandRepository>();
        services.AddScoped<IInventoryRepository, InventoryRepository>();
        services.AddScoped<IReviewRepository, ReviewRepository>();

        // Add Services
        services.AddScoped<ICacheService, RedisCacheService>();
        services.AddScoped<IEventService, MassTransitEventService>();
        
        // Add Unit of Work
        services.AddScoped<IUnitOfWork, ECommerce.Product.Infrastructure.Data.UnitOfWork>();

        return services;
    }
}
