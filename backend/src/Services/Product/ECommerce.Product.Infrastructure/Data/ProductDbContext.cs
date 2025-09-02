using Microsoft.EntityFrameworkCore;
using ECommerce.Product.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace ECommerce.Product.Infrastructure.Data;

public class ProductDbContext : DbContext
{
    public ProductDbContext(DbContextOptions<ProductDbContext> options) : base(options)
    {
        // Log de debugging para verificar la conexión
        try
        {
            var connection = Database.GetDbConnection();
            Console.WriteLine($"ProductDbContext: Conectado a base de datos: {connection.Database}");
            Console.WriteLine($"ProductDbContext: Servidor: {connection.DataSource}");
            Console.WriteLine($"ProductDbContext: Estado de conexión: {connection.State}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ProductDbContext: Error obteniendo información de conexión: {ex.Message}");
        }
    }

    public DbSet<ECommerce.Product.Domain.Entities.Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Brand> Brands { get; set; }
    public DbSet<Inventory> Inventories { get; set; }
    public DbSet<InventoryMovement> InventoryMovements { get; set; }
    public DbSet<Review> Reviews { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all configurations from current assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ProductDbContext).Assembly);
    }

    // Remove hardcoded connection string - use dependency injection instead
    // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    // {
    //     if (!optionsBuilder.IsConfigured)
    //     {
    //         optionsBuilder.UseSqlServer("Server=localhost;Database=ECommerceProductDb;Trusted_Connection=true;MultipleActiveResultSets=true");
    //     }
    // }
}
