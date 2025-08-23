using ECommerce.Product.Domain.Enums;

namespace ECommerce.Product.Application.DTOs;

public class ProductDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? ShortDescription { get; set; }
    public string SKU { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal? CompareAtPrice { get; set; }
    public decimal? CostPrice { get; set; }
    public string Brand { get; set; } = string.Empty;
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public List<ProductImageDto> Images { get; set; } = new();
    public List<ProductVariantDto> Variants { get; set; } = new();
    public List<ProductSpecificationDto> Specifications { get; set; } = new();
    public List<string> Tags { get; set; } = new();
    public WeightDto? Weight { get; set; }
    public DimensionsDto? Dimensions { get; set; }
    public InventoryDto Inventory { get; set; } = new();
    public ProductStatus Status { get; set; }
    public bool IsFeatured { get; set; }
    public bool IsDigital { get; set; }
    public bool RequiresShipping { get; set; }
    public bool IsTaxable { get; set; }
    public SEODto? SEO { get; set; }
    public ProductRatingDto Rating { get; set; } = new();
    public ProductAnalyticsDto Analytics { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class ProductImageDto
{
    public string Url { get; set; } = string.Empty;
    public string Alt { get; set; } = string.Empty;
    public bool IsPrimary { get; set; }
    public int Order { get; set; }
}

public class ProductVariantDto
{
    public string Name { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public decimal? AdditionalPrice { get; set; }
}

public class ProductSpecificationDto
{
    public string Name { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string? Group { get; set; }
}

public class WeightDto
{
    public decimal Value { get; set; }
    public string Unit { get; set; } = string.Empty;
}

public class DimensionsDto
{
    public decimal Length { get; set; }
    public decimal Width { get; set; }
    public decimal Height { get; set; }
    public string Unit { get; set; } = string.Empty;
}

public class InventoryDto
{
    public int Stock { get; set; }
    public int LowStockThreshold { get; set; }
    public bool TrackQuantity { get; set; }
    public bool AllowBackorder { get; set; }
    public bool IsInStock => Stock > 0;
    public bool IsLowStock => Stock <= LowStockThreshold;
}

public class SEODto
{
    public string? MetaTitle { get; set; }
    public string? MetaDescription { get; set; }
}

public class ProductRatingDto
{
    public decimal AverageRating { get; set; }
    public int ReviewCount { get; set; }
    public int FiveStarCount { get; set; }
    public int FourStarCount { get; set; }
    public int ThreeStarCount { get; set; }
    public int TwoStarCount { get; set; }
    public int OneStarCount { get; set; }
}

public class ProductAnalyticsDto
{
    public int Views { get; set; }
    public int Sales { get; set; }
    public decimal Revenue { get; set; }
}

public class PaginatedResult<T>
{
    public List<T> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public bool HasNextPage => Page < TotalPages;
    public bool HasPreviousPage => Page > 1;
}

public class MoneyDto
{
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
}

