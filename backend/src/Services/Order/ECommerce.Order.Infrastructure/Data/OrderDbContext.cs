using Microsoft.EntityFrameworkCore;
using ECommerce.Order.Domain.Entities;
using ECommerce.Order.Domain.ValueObjects;
using ECommerce.Order.Infrastructure.Data.Configurations;

namespace ECommerce.Order.Infrastructure.Data;

public class OrderDbContext : DbContext
{
    public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options)
    {
    }

    public DbSet<ECommerce.Order.Domain.Entities.Order> Orders { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply configurations
        modelBuilder.ApplyConfiguration(new OrderConfiguration());
        modelBuilder.ApplyConfiguration(new OrderItemConfiguration());
        modelBuilder.ApplyConfiguration(new AddressConfiguration());
        modelBuilder.ApplyConfiguration(new OrderPricingConfiguration());
        modelBuilder.ApplyConfiguration(new PaymentInfoConfiguration());
        modelBuilder.ApplyConfiguration(new OrderStatusHistoryConfiguration());

        // Configure value objects
        ConfigureValueObjects(modelBuilder);
    }

    private void ConfigureValueObjects(ModelBuilder modelBuilder)
    {
        // Configure Address as owned entity
        modelBuilder.Entity<ECommerce.Order.Domain.Entities.Order>()
            .OwnsOne(o => o.ShippingAddress, address =>
            {
                address.Property(a => a.Street).IsRequired().HasMaxLength(200);
                address.Property(a => a.City).IsRequired().HasMaxLength(100);
                address.Property(a => a.State).IsRequired().HasMaxLength(100);
                address.Property(a => a.PostalCode).IsRequired().HasMaxLength(20);
                address.Property(a => a.Country).IsRequired().HasMaxLength(100);
                address.Property(a => a.AddressLine2).HasMaxLength(200);
                address.Property(a => a.FullAddress).HasMaxLength(500);
            });

        modelBuilder.Entity<ECommerce.Order.Domain.Entities.Order>()
            .OwnsOne(o => o.BillingAddress, address =>
            {
                address.Property(a => a.Street).HasMaxLength(200);
                address.Property(a => a.City).HasMaxLength(100);
                address.Property(a => a.State).HasMaxLength(100);
                address.Property(a => a.PostalCode).HasMaxLength(20);
                address.Property(a => a.Country).HasMaxLength(100);
                address.Property(a => a.AddressLine2).HasMaxLength(200);
                address.Property(a => a.FullAddress).HasMaxLength(500);
            });

        // Configure OrderPricing as owned entity
        modelBuilder.Entity<ECommerce.Order.Domain.Entities.Order>()
            .OwnsOne(o => o.Pricing, pricing =>
            {
                pricing.Property(p => p.Subtotal).HasColumnType("decimal(18,2)");
                pricing.Property(p => p.Tax).HasColumnType("decimal(18,2)");
                pricing.Property(p => p.ShippingCost).HasColumnType("decimal(18,2)");
                pricing.Property(p => p.DiscountAmount).HasColumnType("decimal(18,2)");
                pricing.Property(p => p.Total).HasColumnType("decimal(18,2)");
            });

        // Configure PaymentInfo as owned entity
        modelBuilder.Entity<ECommerce.Order.Domain.Entities.Order>()
            .OwnsOne(o => o.PaymentInfo, payment =>
            {
                payment.Property(p => p.PaymentMethod).HasMaxLength(100);
                payment.Property(p => p.PaymentId).HasMaxLength(100);
                payment.Property(p => p.TransactionId).HasMaxLength(100);
            });

        // Configure OrderStatusHistory as owned entity
        modelBuilder.Entity<ECommerce.Order.Domain.Entities.Order>()
            .OwnsMany(o => o.StatusHistory, history =>
            {
                history.Property(h => h.Status).HasConversion<string>();
                history.Property(h => h.Reason).HasMaxLength(500);
                history.Property(h => h.Timestamp);
            });

        // Configure OrderItem as owned entity
        modelBuilder.Entity<ECommerce.Order.Domain.Entities.Order>()
            .OwnsMany(o => o.Items, item =>
            {
                item.Property(i => i.ProductId);
                item.Property(i => i.ProductName).HasMaxLength(200);
                item.Property(i => i.ProductSku).HasMaxLength(100);
                item.Property(i => i.UnitPrice).HasColumnType("decimal(18,2)");
                item.Property(i => i.Quantity);
                item.Property(i => i.Weight).HasColumnType("decimal(18,4)");
            });
    }
}
