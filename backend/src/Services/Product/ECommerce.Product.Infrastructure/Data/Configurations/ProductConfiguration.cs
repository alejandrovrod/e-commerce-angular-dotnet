using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ECommerce.Product.Domain.Entities;
using ECommerce.Product.Domain.Enums;

namespace ECommerce.Product.Infrastructure.Data.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<ECommerce.Product.Domain.Entities.Product>
{
    public void Configure(EntityTypeBuilder<ECommerce.Product.Domain.Entities.Product> builder)
    {
        builder.ToTable("Products");
        
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).ValueGeneratedOnAdd();
        
        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(200);
            
        builder.Property(p => p.Slug)
            .IsRequired()
            .HasMaxLength(200);
            
        builder.Property(p => p.Description)
            .IsRequired()
            .HasMaxLength(2000);
            
        builder.Property(p => p.ShortDescription)
            .HasMaxLength(500);
            
        builder.Property(p => p.SKU)
            .IsRequired()
            .HasMaxLength(100);
            
        builder.Property(p => p.Brand)
            .IsRequired()
            .HasMaxLength(100);
            
        builder.Property(p => p.CategoryId)
            .IsRequired();
            
        builder.Property(p => p.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasDefaultValue(ProductStatus.Draft);
            
        builder.Property(p => p.IsFeatured)
            .IsRequired()
            .HasDefaultValue(false);
            
        builder.Property(p => p.IsDigital)
            .IsRequired()
            .HasDefaultValue(false);
            
        builder.Property(p => p.RequiresShipping)
            .IsRequired()
            .HasDefaultValue(true);
            
        builder.Property(p => p.IsTaxable)
            .IsRequired()
            .HasDefaultValue(true);
            
        builder.Property(p => p.CreatedAt)
            .IsRequired();
            
        builder.Property(p => p.UpdatedAt)
            .IsRequired();
            
        // Configure Metadata property as JSON
        builder.Property(p => p.Metadata)
            .HasColumnType("nvarchar(max)")
            .HasConversion(
                v => v == null ? null : System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                v => v == null ? null : System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(v, (System.Text.Json.JsonSerializerOptions?)null)
            );
        
        // Configure complex types
        builder.OwnsOne(p => p.Price, price =>
        {
            price.Property(p => p.Amount).HasColumnType("decimal(18,2)");
            price.Property(p => p.Currency).HasMaxLength(3);
        });
        
        builder.OwnsOne(p => p.CompareAtPrice, price =>
        {
            price.Property(p => p.Amount).HasColumnType("decimal(18,2)");
            price.Property(p => p.Currency).HasMaxLength(3);
        });
        
        builder.OwnsOne(p => p.CostPrice, price =>
        {
            price.Property(p => p.Amount).HasColumnType("decimal(18,2)");
            price.Property(p => p.Currency).HasMaxLength(3);
        });
        
        // Configure Tags as JSON
        builder.Property(p => p.Tags)
            .HasColumnType("nvarchar(max)")
            .HasConversion(
                v => v == null ? null : System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                v => v == null ? new List<string>() : System.Text.Json.JsonSerializer.Deserialize<List<string>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new List<string>()
            );
            
        // Configure ProductVariants as owned entities
        builder.OwnsMany(p => p.Variants, variant =>
        {
            variant.Property(v => v.Name).HasMaxLength(100);
            variant.Property(v => v.Value).HasMaxLength(100);
            variant.Property(v => v.Stock);
            variant.Property(v => v.SKU).HasMaxLength(100);
            
            // Configure AdditionalPrice as owned entity
            variant.OwnsOne(v => v.AdditionalPrice, price =>
            {
                price.Property(p => p.Amount).HasColumnType("decimal(18,2)");
                price.Property(p => p.Currency).HasMaxLength(3);
            });
        });
        
        // Configure ProductImages as owned entities
        builder.OwnsMany(p => p.Images, image =>
        {
            image.Property(i => i.Url).HasMaxLength(500);
            image.Property(i => i.Alt).HasMaxLength(200);
            image.Property(i => i.IsPrimary);
            image.Property(i => i.SortOrder);
        });
        
        // Configure ProductSpecifications as owned entities
        builder.OwnsMany(p => p.Specifications, spec =>
        {
            spec.Property(s => s.Name).HasMaxLength(100);
            spec.Property(s => s.Value).HasMaxLength(500);
            spec.Property(s => s.Group).HasMaxLength(100);
        });
        
        // Configure Dimensions as owned entity
        builder.OwnsOne(p => p.Dimensions, dimensions =>
        {
            dimensions.Property(d => d.Length).HasColumnType("decimal(10,3)");
            dimensions.Property(d => d.Width).HasColumnType("decimal(10,3)");
            dimensions.Property(d => d.Height).HasColumnType("decimal(10,3)");
            dimensions.Property(d => d.Unit).HasMaxLength(10);
        });
        
        // Configure Weight as owned entity
        builder.OwnsOne(p => p.Weight, weight =>
        {
            weight.Property(w => w.Value).HasColumnType("decimal(10,3)");
            weight.Property(w => w.Unit).HasMaxLength(10);
        });
        
        // Configure SEO as owned entity
        builder.OwnsOne(p => p.SEO, seo =>
        {
            seo.Property(s => s.MetaTitle).HasMaxLength(200);
            seo.Property(s => s.MetaDescription).HasMaxLength(500);
        });
        
        // Configure ProductRating as owned entity
        builder.OwnsOne(p => p.Rating, rating =>
        {
            rating.Property(r => r.AverageRating).HasColumnType("decimal(3,2)");
            rating.Property(r => r.ReviewCount);
        });
        
        // Configure ProductAnalytics as owned entity
        builder.OwnsOne(p => p.Analytics, analytics =>
        {
            analytics.Property(a => a.Views);
            analytics.Property(a => a.Sales);
            analytics.Property(a => a.Revenue).HasColumnType("decimal(18,2)");
        });
        
        // Relationships
        builder.HasOne(p => p.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
            
        builder.HasOne(p => p.Inventory)
            .WithOne(i => i.Product)
            .HasForeignKey<Inventory>(i => i.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
            
        // Indexes
        builder.HasIndex(p => p.Name);
        builder.HasIndex(p => p.Slug).IsUnique();
        builder.HasIndex(p => p.SKU).IsUnique();
        builder.HasIndex(p => p.CategoryId);
        builder.HasIndex(p => p.Brand);
        builder.HasIndex(p => p.Status);
        builder.HasIndex(p => p.IsFeatured);
        builder.HasIndex(p => p.IsDigital);
        builder.HasIndex(p => p.CreatedAt);
    }
}
