using ECommerce.BuildingBlocks.Common.Models;

namespace ECommerce.Product.Domain.Entities;

public class Category : BaseAuditableEntity
{
    public string Name { get; private set; } = default!;
    public string Slug { get; private set; } = default!;
    public string? Description { get; private set; }
    public string? Image { get; private set; }
    public Guid? ParentId { get; private set; }
    public int Level { get; private set; }
    public int SortOrder { get; private set; }
    public bool IsActive { get; private set; } = true;
    public int ProductCount { get; private set; } = 0;

    // Navigation properties
    public virtual Category? Parent { get; private set; }
    public virtual ICollection<Category> Children { get; private set; } = new List<Category>();
    public virtual ICollection<Product> Products { get; private set; } = new List<Product>();

    private Category() { } // For MongoDB

    public static Category Create(string name, string? description = null, Guid? parentId = null)
    {
        var category = new Category
        {
            Name = name,
            Slug = GenerateSlug(name),
            Description = description,
            ParentId = parentId,
            Level = 0, // Will be calculated when parent is set
            IsActive = true
        };

        return category;
    }

    public void Update(string name, string? description = null, string? image = null)
    {
        Name = name;
        Slug = GenerateSlug(name);
        Description = description;
        Image = image;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetParent(Category? parent)
    {
        ParentId = parent?.Id;
        Level = parent?.Level + 1 ?? 0;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetImage(string imageUrl)
    {
        Image = imageUrl;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetSortOrder(int sortOrder)
    {
        SortOrder = sortOrder;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void IncrementProductCount()
    {
        ProductCount++;
        UpdatedAt = DateTime.UtcNow;
    }

    public void DecrementProductCount()
    {
        if (ProductCount > 0)
        {
            ProductCount--;
            UpdatedAt = DateTime.UtcNow;
        }
    }

    public void UpdateProductCount(int count)
    {
        ProductCount = Math.Max(0, count);
        UpdatedAt = DateTime.UtcNow;
    }

    public bool IsRoot() => ParentId == null;
    
    public bool HasChildren() => Children.Any();
    
    public bool HasProducts() => ProductCount > 0;

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



