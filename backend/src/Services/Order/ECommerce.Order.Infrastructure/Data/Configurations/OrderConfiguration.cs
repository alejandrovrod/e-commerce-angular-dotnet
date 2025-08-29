using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ECommerce.Order.Domain.Entities;
using ECommerce.Order.Domain.Enums;

namespace ECommerce.Order.Infrastructure.Data.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<ECommerce.Order.Domain.Entities.Order>
{
    public void Configure(EntityTypeBuilder<ECommerce.Order.Domain.Entities.Order> builder)
    {
        builder.ToTable("Orders");

        builder.HasKey(o => o.Id);

        builder.Property(o => o.OrderNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(o => o.UserId)
            .IsRequired();

        builder.Property(o => o.Status)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(o => o.CouponCode)
            .HasMaxLength(50);

        builder.Property(o => o.ShippingMethodId);

        builder.Property(o => o.Notes)
            .HasMaxLength(1000);

        builder.Property(o => o.TrackingNumber)
            .HasMaxLength(100);

        builder.Property(o => o.ShippedAt);
        builder.Property(o => o.DeliveredAt);

        // Configure indexes
        builder.HasIndex(o => o.OrderNumber)
            .IsUnique();

        builder.HasIndex(o => o.UserId);
        builder.HasIndex(o => o.Status);
        builder.HasIndex(o => o.CreatedAt);

        // Configure metadata
        builder.Property(o => o.Metadata)
            .HasColumnType("nvarchar(max)")
            .HasConversion(
                v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                v => System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new Dictionary<string, object>()
            );
    }
}
