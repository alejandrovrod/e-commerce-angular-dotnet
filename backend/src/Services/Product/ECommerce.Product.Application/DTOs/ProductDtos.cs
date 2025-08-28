using ECommerce.Product.Domain.Enums;

namespace ECommerce.Product.Application.DTOs;

public class ProductDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string SKU { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Brand { get; set; } = string.Empty;
    public Guid CategoryId { get; set; }
    public ProductStatus Status { get; set; }
    public bool IsFeatured { get; set; }
    public bool IsDigital { get; set; }
    public bool RequiresShipping { get; set; }
    public bool IsTaxable { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

