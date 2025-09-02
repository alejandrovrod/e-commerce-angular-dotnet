using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ECommerce.Product.Domain.Entities;

namespace ECommerce.Product.Infrastructure.Data.Configurations;

public class InventoryMovementConfiguration : IEntityTypeConfiguration<InventoryMovement>
{
    public void Configure(EntityTypeBuilder<InventoryMovement> builder)
    {
        builder.ToTable("InventoryMovements");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.ProductId)
            .IsRequired();

        builder.Property(m => m.MovementType)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(m => m.Quantity)
            .IsRequired();

        builder.Property(m => m.PreviousQuantity)
            .IsRequired();

        builder.Property(m => m.NewQuantity)
            .IsRequired();

        builder.Property(m => m.Reason)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(m => m.Notes)
            .HasMaxLength(1000);

        builder.Property(m => m.UserId);

        builder.Property(m => m.UserName)
            .HasMaxLength(100);

        builder.Property(m => m.OrderId);

        // Configurar la relación con Product
        builder.HasOne(m => m.Product)
            .WithMany()
            .HasForeignKey(m => m.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configurar índices para mejorar el rendimiento
        builder.HasIndex(m => m.ProductId);
        builder.HasIndex(m => m.MovementType);
        builder.HasIndex(m => m.CreatedAt);
        builder.HasIndex(m => m.UserId);
        builder.HasIndex(m => m.OrderId);

        // Configurar propiedades de auditoría
        builder.Property(m => m.CreatedAt)
            .IsRequired();

        builder.Property(m => m.UpdatedAt);

        builder.Property(m => m.CreatedBy)
            .HasMaxLength(100);

        builder.Property(m => m.UpdatedBy)
            .HasMaxLength(100);

        builder.Property(m => m.IsDeleted)
            .HasDefaultValue(false);

        builder.Property(m => m.DeletedAt);

        builder.Property(m => m.DeletedBy)
            .HasMaxLength(100);

        builder.Property(m => m.Version)
            .HasMaxLength(10)
            .HasDefaultValue("1.0");

        // Configurar Metadata como JSON o ignorar
        builder.Property(m => m.Metadata)
            .HasConversion(
                v => v == null ? null : System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                v => v == null ? null : System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>?>(v, (System.Text.Json.JsonSerializerOptions?)null)
            )
            .HasColumnType("nvarchar(max)");
    }
}
