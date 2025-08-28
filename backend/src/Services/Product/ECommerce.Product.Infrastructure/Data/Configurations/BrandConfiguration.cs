using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ECommerce.Product.Domain.Entities;

namespace ECommerce.Product.Infrastructure.Data.Configurations;

public class BrandConfiguration : IEntityTypeConfiguration<Brand>
{
    public void Configure(EntityTypeBuilder<Brand> builder)
    {
        builder.ToTable("Brands");
        
        builder.HasKey(b => b.Id);
        builder.Property(b => b.Id).ValueGeneratedOnAdd();
        
        builder.Property(b => b.Name)
            .IsRequired()
            .HasMaxLength(100);
            
        builder.Property(b => b.Description)
            .IsRequired()
            .HasMaxLength(500);
            
        builder.Property(b => b.LogoUrl)
            .IsRequired()
            .HasMaxLength(500);
            
        builder.Property(b => b.Website)
            .IsRequired()
            .HasMaxLength(200);
            
        builder.Property(b => b.IsActive)
            .IsRequired()
            .HasDefaultValue(true);
            
        builder.Property(b => b.CreatedAt)
            .IsRequired();
            
        builder.Property(b => b.UpdatedAt)
            .IsRequired();
            
        // Configure Metadata property as JSON
        builder.Property(b => b.Metadata)
            .HasColumnType("nvarchar(max)")
            .HasConversion(
                v => v == null ? null : System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                v => v == null ? null : System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(v, (System.Text.Json.JsonSerializerOptions?)null)
            );
        
        // Relationships
        builder.HasMany(b => b.Products)
            .WithOne()
            .HasForeignKey("BrandId")
            .OnDelete(DeleteBehavior.Restrict);
            
        // Indexes
        builder.HasIndex(b => b.Name).IsUnique();
        builder.HasIndex(b => b.IsActive);
    }
}
