using MediatR;
using ECommerce.BuildingBlocks.Common.Models;
using ECommerce.Product.Application.DTOs;

namespace ECommerce.Product.Application.Commands.CreateProduct;

public class CreateProductCommand : IRequest<ApiResponse<ProductDto>>
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string SKU { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Brand { get; set; } = string.Empty;
    public Guid CategoryId { get; set; }
    public bool IsFeatured { get; set; } = false;
    public bool IsDigital { get; set; } = false;
    public bool RequiresShipping { get; set; } = true;
    public bool IsTaxable { get; set; } = true;
}





