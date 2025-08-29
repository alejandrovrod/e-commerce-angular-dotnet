using Microsoft.EntityFrameworkCore;
using ECommerce.Payment.Domain.Entities;
using ECommerce.BuildingBlocks.Common.Models;
using System.Text.Json;

namespace ECommerce.Payment.Infrastructure.Data;

public class PaymentDbContext : DbContext
{
    public PaymentDbContext(DbContextOptions<PaymentDbContext> options) : base(options)
    {
    }

    public DbSet<ECommerce.Payment.Domain.Entities.Payment> Payments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Payment entity
        modelBuilder.Entity<ECommerce.Payment.Domain.Entities.Payment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.PaymentNumber).IsRequired().HasMaxLength(50);
            entity.Property(e => e.OrderId).IsRequired();
            entity.Property(e => e.UserId).IsRequired();
            entity.Property(e => e.Status).IsRequired();
            entity.Property(e => e.Gateway).IsRequired();
            entity.Property(e => e.GatewayTransactionId).HasMaxLength(255);
            entity.Property(e => e.GatewayPaymentId).HasMaxLength(255);
            entity.Property(e => e.FailureReason).HasMaxLength(500);
            entity.Property(e => e.ProcessedAt);
            entity.Property(e => e.ExpiresAt);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();

            // Configure Metadata property with JSON conversion
            entity.Property(e => e.Metadata)
                .HasColumnType("nvarchar(max)")
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => v == null ? null : JsonSerializer.Deserialize<Dictionary<string, object>>(v, (JsonSerializerOptions?)null)
                );

            // Configure owned entities
            entity.OwnsOne(e => e.Amount, amount =>
            {
                amount.Property(a => a.Amount).HasColumnType("decimal(18,2)");
                amount.Property(a => a.Currency).HasMaxLength(3);
            });

            entity.OwnsOne(e => e.PaymentMethod, paymentMethod =>
            {
                paymentMethod.Property(pm => pm.Type).HasMaxLength(50);
                paymentMethod.Property(pm => pm.Last4).HasMaxLength(4);
                paymentMethod.Property(pm => pm.Brand).HasMaxLength(50);
                paymentMethod.Property(pm => pm.ExpiryMonth).HasMaxLength(2);
                paymentMethod.Property(pm => pm.ExpiryYear).HasMaxLength(4);
            });

            entity.OwnsOne(e => e.Details, details =>
            {
                details.Property(d => d.ProcessorName).HasMaxLength(100);
                details.Property(d => d.ProcessorData)
                    .HasColumnType("nvarchar(max)")
                    .HasConversion(
                        v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                        v => v == null ? new Dictionary<string, string>() : JsonSerializer.Deserialize<Dictionary<string, string>>(v, (JsonSerializerOptions?)null) ?? new Dictionary<string, string>()
                    );
            });

            // Configure collections
            entity.OwnsMany(e => e.Attempts, attempts =>
            {
                attempts.Property(a => a.Status).IsRequired();
                attempts.Property(a => a.Message).HasMaxLength(500);
                attempts.Property(a => a.ErrorCode).HasMaxLength(100);
                attempts.Property(a => a.AttemptedAt).IsRequired();
            });

            entity.OwnsMany(e => e.Refunds, refunds =>
            {
                refunds.Property(r => r.PaymentId).IsRequired();
                refunds.Property(r => r.Reason).HasMaxLength(500);
                refunds.Property(r => r.Type).IsRequired();
                refunds.Property(r => r.Status).IsRequired();
                refunds.Property(r => r.GatewayRefundId).HasMaxLength(255);
                refunds.Property(r => r.CreatedAt).IsRequired();
                refunds.Property(r => r.ProcessedAt);

                refunds.OwnsOne(r => r.Amount, refundAmount =>
                {
                    refundAmount.Property(ra => ra.Amount).HasColumnType("decimal(18,2)");
                    refundAmount.Property(ra => ra.Currency).HasMaxLength(3);
                });
            });

            // Indexes
            entity.HasIndex(e => e.PaymentNumber).IsUnique();
            entity.HasIndex(e => e.OrderId);
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.Gateway);
            entity.HasIndex(e => e.CreatedAt);
        });
    }
}
