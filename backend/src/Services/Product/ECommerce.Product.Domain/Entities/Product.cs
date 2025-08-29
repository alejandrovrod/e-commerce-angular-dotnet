using ECommerce.BuildingBlocks.Common.Models;
using ECommerce.Product.Domain.ValueObjects;
using ECommerce.Product.Domain.Enums;

namespace ECommerce.Product.Domain.Entities;

public class Product : BaseAuditableEntity
{
    public string Name { get; private set; } = default!;
    public string Slug { get; private set; } = default!;
    public string Description { get; private set; } = default!;
    public string? ShortDescription { get; private set; }
    public string SKU { get; private set; } = default!;
    public Money Price { get; private set; } = default!;
    public Money? CompareAtPrice { get; private set; }
    public Money? CostPrice { get; private set; }
    public string Brand { get; private set; } = default!;
    public Guid CategoryId { get; private set; }
    public List<ProductImage> Images { get; private set; } = new();
    public List<ProductVariant> Variants { get; private set; } = new();
    public List<ProductSpecification> Specifications { get; private set; } = new();
    public List<string> Tags { get; private set; } = new();
    public Weight? Weight { get; private set; }
    public Dimensions? Dimensions { get; private set; }
    public Inventory Inventory { get; private set; } = default!;
    public ProductStatus Status { get; private set; } = ProductStatus.Draft;
    public bool IsFeatured { get; private set; } = false;
    public bool IsDigital { get; private set; } = false;
    public bool RequiresShipping { get; private set; } = true;
    public bool IsTaxable { get; private set; } = true;
    public SEO? SEO { get; private set; }
    public ProductRating Rating { get; private set; } = new();
    public ProductAnalytics Analytics { get; private set; } = new();

    // Navigation properties
    public virtual Category Category { get; private set; } = default!;
    public virtual ICollection<Review> Reviews { get; private set; } = new List<Review>();

    private Product() { } // For MongoDB

    public static Product Create(
        string name,
        string description,
        string sku,
        Money price,
        string brand,
        Guid categoryId,
        Inventory inventory)
    {
        var product = new Product
        {
            Name = name,
            Slug = GenerateSlug(name),
            Description = description,
            SKU = sku,
            Price = price,
            Brand = brand,
            CategoryId = categoryId,
            Inventory = inventory,
            Status = ProductStatus.Draft
        };

        return product;
    }

    public void UpdateBasicInfo(string name, string description, string? shortDescription = null)
    {
        Name = name;
        Slug = GenerateSlug(name);
        Description = description;
        ShortDescription = shortDescription;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdatePricing(Money price, Money? compareAtPrice = null, Money? costPrice = null)
    {
        Price = price;
        CompareAtPrice = compareAtPrice;
        CostPrice = costPrice;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateInventory(int stock, int lowStockThreshold = 0, bool trackQuantity = true)
    {
        Inventory = new Inventory(Id, stock, "Default");
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddImage(string url, string alt, bool isPrimary = false)
    {
        if (isPrimary)
        {
            // Set all other images to non-primary
            Images.ForEach(img => img.SetAsNonPrimary());
        }

        Images.Add(ProductImage.Create(url, alt, isPrimary, Images.Count));
        UpdatedAt = DateTime.UtcNow;
    }

    public void RemoveImage(string url)
    {
        Images.RemoveAll(img => img.Url == url);
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddVariant(string name, string value, Money? additionalPrice = null)
    {
        Variants.Add(ProductVariant.Create(name, value, additionalPrice));
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddSpecification(string name, string value, string? group = null)
    {
        Specifications.Add(new ProductSpecification(name, value, group));
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddTags(params string[] tags)
    {
        foreach (var tag in tags)
        {
            if (!Tags.Contains(tag, StringComparer.OrdinalIgnoreCase))
            {
                Tags.Add(tag.ToLowerInvariant());
            }
        }
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetFeatured(bool isFeatured)
    {
        IsFeatured = isFeatured;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetDigitalAndShipping(bool isDigital, bool requiresShipping)
    {
        IsDigital = isDigital;
        RequiresShipping = requiresShipping;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetTaxable(bool isTaxable)
    {
        IsTaxable = isTaxable;
        UpdatedAt = DateTime.UtcNow;
    }
    
    public void UpdateSKU(string sku)
    {
        SKU = sku;
        UpdatedAt = DateTime.UtcNow;
    }
    
    public void UpdateBrand(string brand)
    {
        Brand = brand;
        UpdatedAt = DateTime.UtcNow;
    }
    
    public void UpdateCategory(Guid categoryId)
    {
        CategoryId = categoryId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetStatus(ProductStatus status)
    {
        Status = status;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Publish()
    {
        if (Status == ProductStatus.Draft)
        {
            Status = ProductStatus.Active;
            UpdatedAt = DateTime.UtcNow;
        }
    }

    public void Unpublish()
    {
        if (Status == ProductStatus.Active)
        {
            Status = ProductStatus.Inactive;
            UpdatedAt = DateTime.UtcNow;
        }
    }

    public void UpdateSEO(string? metaTitle, string? metaDescription)
    {
        SEO = new SEO(metaTitle, metaDescription);
        UpdatedAt = DateTime.UtcNow;
    }

    public void RecordView()
    {
        Analytics.IncrementViews();
        UpdatedAt = DateTime.UtcNow;
    }

    public void RecordSale(int quantity = 1)
    {
        Analytics.RecordSale(quantity);
        // Note: Stock consumption should be handled by the inventory service
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateRating(decimal newRating, int reviewCount)
    {
        Rating.UpdateRating(newRating, reviewCount);
        UpdatedAt = DateTime.UtcNow;
    }

    public bool IsInStock() => Inventory.AvailableQuantity > 0;
    
    public bool IsLowStock() => Inventory.IsLowStock(10);
    
    public bool HasDiscount() => CompareAtPrice != null && CompareAtPrice.Amount > Price.Amount;
    
    public decimal GetDiscountPercentage()
    {
        if (!HasDiscount()) return 0;
        return ((CompareAtPrice!.Amount - Price.Amount) / CompareAtPrice.Amount) * 100;
    }

    private static string GenerateSlug(string name)
    {
        return name.ToLowerInvariant()
            .Replace(" ", "-")
            .Replace("&", "and")
            .ToCharArray()
            .Where(c => char.IsLetterOrDigit(c) || c == '-')
            .Aggregate(string.Empty, (current, c) => current + c);
    }
}
