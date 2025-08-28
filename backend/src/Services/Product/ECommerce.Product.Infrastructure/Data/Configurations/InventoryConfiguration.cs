using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ECommerce.Product.Domain.Entities;

namespace ECommerce.Product.Infrastructure.Data.Configurations;

public class InventoryConfiguration : IEntityTypeConfiguration<Inventory>
{
    public void Configure(EntityTypeBuilder<Inventory> builder)
    {
        builder.ToTable("Inventories");
        
        builder.HasKey(i => i.Id);
        builder.Property(i => i.Id).ValueGeneratedOnAdd();
        
        builder.Property(i => i.ProductId)
            .IsRequired();
            
        builder.Property(i => i.Quantity)
            .IsRequired()
            .HasDefaultValue(0);
            
        builder.Property(i => i.ReservedQuantity)
            .IsRequired()
            .HasDefaultValue(0);
            
        builder.Property(i => i.AvailableQuantity)
            .IsRequired()
            .HasDefaultValue(0);
            
        builder.Property(i => i.Location)
            .IsRequired()
            .HasMaxLength(200);
            
        builder.Property(i => i.LastUpdated)
            .IsRequired();
            
        builder.Property(i => i.CreatedAt)
            .IsRequired();
            
        builder.Property(i => i.UpdatedAt)
            .IsRequired();
            
        // Configure Metadata property as JSON
        builder.Property(i => i.Metadata)
            .HasColumnType("nvarchar(max)")
            .HasConversion(
                v => v == null ? null : System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                v => v == null ? null : System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(v, (System.Text.Json.JsonSerializerOptions?)null)
            );
        
        // Relationships
        builder.HasOne(i => i.Product)
            .WithOne()
            .HasForeignKey<Inventory>(i => i.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
            
        // Indexes
        builder.HasIndex(i => i.ProductId).IsUnique();
        builder.HasIndex(i => i.Location);
        builder.HasIndex(i => i.AvailableQuantity);
    }
}
