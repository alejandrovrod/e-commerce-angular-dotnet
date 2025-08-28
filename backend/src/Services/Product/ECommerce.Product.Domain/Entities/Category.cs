using ECommerce.BuildingBlocks.Common.Models;

namespace ECommerce.Product.Domain.Entities;

public class Category : BaseAuditableEntity
{
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public bool IsActive { get; private set; }
    public Guid? ParentCategoryId { get; private set; }
    
    // Navigation properties
    public Category? ParentCategory { get; private set; }
    public List<Category> Subcategories { get; private set; } = new();
    public List<Product> Products { get; private set; } = new();
    
    // Private constructor for EF Core
    private Category() { }
    
    public Category(string name, string description, Guid? parentCategoryId = null)
    {
        Name = name;
        Description = description;
        IsActive = true;
        ParentCategoryId = parentCategoryId;
    }
    
    public void Update(string name, string description)
    {
        Name = name;
        Description = description;
        UpdatedAt = DateTime.UtcNow;
    }
    
    public void SetActive(bool isActive)
    {
        IsActive = isActive;
        UpdatedAt = DateTime.UtcNow;
    }
    
    public void SetParentCategory(Guid? parentCategoryId)
    {
        ParentCategoryId = parentCategoryId;
        UpdatedAt = DateTime.UtcNow;
    }
}






