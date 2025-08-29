using MediatR;
using ECommerce.BuildingBlocks.Common.Models;
using ECommerce.Product.Application.DTOs;

namespace ECommerce.Product.Application.Commands;

public class UpdateProductCommand : IRequest<ApiResponse<ProductDto>>
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? ShortDescription { get; set; }
    public decimal Price { get; set; }
    public decimal? CompareAtPrice { get; set; }
    public decimal? CostPrice { get; set; }
    public string Brand { get; set; } = string.Empty;
    public Guid? CategoryId { get; set; }
    public string SKU { get; set; } = string.Empty;
    public List<string> Tags { get; set; } = new();
    public bool IsDigital { get; set; } = false;
    public bool IsFeatured { get; set; } = false;
    public bool RequiresShipping { get; set; } = true;
    public bool IsTaxable { get; set; } = true;
    public Domain.Enums.ProductStatus Status { get; set; }
}






