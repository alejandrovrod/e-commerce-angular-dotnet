namespace ECommerce.Product.Application.DTOs;

public class CategoryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public Guid? ParentCategoryId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    // Navigation properties
    public CategoryDto? ParentCategory { get; set; }
    public List<CategoryDto> Subcategories { get; set; } = new();
    public List<ProductDto> Products { get; set; } = new();
}

public class CategoryHierarchyDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Image { get; set; }
    public int Level { get; set; }
    public int SortOrder { get; set; }
    public bool IsActive { get; set; }
    public int ProductCount { get; set; }
    public List<CategoryHierarchyDto> Children { get; set; } = new();
}

public class CategoryStatisticsDto
{
    public int TotalCategories { get; set; }
    public int ActiveCategories { get; set; }
    public int RootCategories { get; set; }
    public int LeafCategories { get; set; }
    public int TotalProducts { get; set; }
    public Dictionary<string, int> ProductsByCategory { get; set; } = new();
    public List<CategoryDto> TopCategories { get; set; } = new();
}

