using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ECommerce.Product.Domain.Entities;
using ECommerce.Product.Domain.Enums;

namespace ECommerce.Product.Infrastructure.Data.Configurations;

public class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.ToTable("Reviews");
        
        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).ValueGeneratedOnAdd();
        
        builder.Property(r => r.ProductId)
            .IsRequired();
            
        builder.Property(r => r.UserId)
            .IsRequired();
            
        builder.Property(r => r.Title)
            .IsRequired()
            .HasMaxLength(200);
            
        builder.Property(r => r.Content)
            .IsRequired()
            .HasMaxLength(2000);
            
        builder.Property(r => r.Rating)
            .IsRequired()
            .HasDefaultValue(5);
            
        builder.Property(r => r.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasDefaultValue(ReviewStatus.Pending);
            
        builder.Property(r => r.CreatedAt)
            .IsRequired();
            
        builder.Property(r => r.UpdatedAt)
            .IsRequired();
            
        // Configure Metadata property as JSON
        builder.Property(r => r.Metadata)
            .HasColumnType("nvarchar(max)")
            .HasConversion(
                v => v == null ? null : System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                v => v == null ? null : System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(v, (System.Text.Json.JsonSerializerOptions?)null)
            );
        
        // Relationships
        builder.HasOne(r => r.Product)
            .WithMany()
            .HasForeignKey(r => r.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
            
        // Indexes
        builder.HasIndex(r => r.ProductId);
        builder.HasIndex(r => r.UserId);
        builder.HasIndex(r => r.Status);
        builder.HasIndex(r => r.Rating);
        builder.HasIndex(r => r.CreatedAt);
    }
}
