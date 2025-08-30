using Microsoft.EntityFrameworkCore;
using ECommerce.File.Domain.Entities;
using ECommerce.BuildingBlocks.Common.Models;
using System.Text.Json;

namespace ECommerce.File.Infrastructure.Data;

public class FileDbContext : DbContext
{
    public FileDbContext(DbContextOptions<FileDbContext> options) : base(options)
    {
    }

    public DbSet<ECommerce.File.Domain.Entities.File> Files { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure File entity
        modelBuilder.Entity<ECommerce.File.Domain.Entities.File>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FileName).IsRequired().HasMaxLength(255);
            entity.Property(e => e.OriginalFileName).IsRequired().HasMaxLength(500);
            entity.Property(e => e.FilePath).IsRequired().HasMaxLength(1000);
            entity.Property(e => e.ContentType).IsRequired().HasMaxLength(100);
            entity.Property(e => e.FileSize).IsRequired();
            entity.Property(e => e.FileExtension).IsRequired().HasMaxLength(20);
            entity.Property(e => e.Type).IsRequired();
            entity.Property(e => e.Status).IsRequired();
            entity.Property(e => e.UserId);
            entity.Property(e => e.OrderId);
            entity.Property(e => e.ProductId);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.ThumbnailPath).HasMaxLength(1000);
            entity.Property(e => e.IsPublic).IsRequired();
            entity.Property(e => e.ExpiresAt);
            entity.Property(e => e.DownloadCount).IsRequired();
            entity.Property(e => e.Checksum).HasMaxLength(64);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();

            // Configure Metadata property with JSON conversion
            entity.Property(e => e.Metadata)
                .HasColumnType("nvarchar(max)")
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => v == null ? null : JsonSerializer.Deserialize<Dictionary<string, object>>(v, (JsonSerializerOptions?)null)
                );

            // Indexes
            entity.HasIndex(e => e.FileName).IsUnique();
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.OrderId);
            entity.HasIndex(e => e.ProductId);
            entity.HasIndex(e => e.Type);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.IsPublic);
            entity.HasIndex(e => e.CreatedAt);
            entity.HasIndex(e => e.ExpiresAt);
        });
    }
}
