using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ECommerce.Product.Domain.Entities;

namespace ECommerce.Product.Infrastructure.Data.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("Categories");
        
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).ValueGeneratedOnAdd();
        
        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(100);
            
        builder.Property(c => c.Description)
            .IsRequired()
            .HasMaxLength(500);
            
        builder.Property(c => c.IsActive)
            .IsRequired()
            .HasDefaultValue(true);
            
        builder.Property(c => c.ParentCategoryId)
            .IsRequired(false);
            
        builder.Property(c => c.CreatedAt)
            .IsRequired();
            
        builder.Property(c => c.UpdatedAt)
            .IsRequired();
            
        // Configure Metadata property as JSON
        builder.Property(c => c.Metadata)
            .HasColumnType("nvarchar(max)")
            .HasConversion(
                v => v == null ? null : System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                v => v == null ? null : System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(v, (System.Text.Json.JsonSerializerOptions?)null)
            );
        
        // Relationships
        builder.HasOne(c => c.ParentCategory)
            .WithMany(c => c.Subcategories)
            .HasForeignKey(c => c.ParentCategoryId)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder.HasMany(c => c.Products)
            .WithOne()
            .HasForeignKey("CategoryId")
            .OnDelete(DeleteBehavior.Restrict);
            
        // Indexes
        builder.HasIndex(c => c.Name).IsUnique();
        builder.HasIndex(c => c.ParentCategoryId);
        builder.HasIndex(c => c.IsActive);
    }
}
