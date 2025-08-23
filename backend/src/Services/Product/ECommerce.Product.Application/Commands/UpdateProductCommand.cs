using MediatR;
using ECommerce.Product.Application.DTOs;

namespace ECommerce.Product.Application.Commands;

public class UpdateProductCommand : IRequest<ProductDto?>
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? ShortDescription { get; set; }
    public decimal Price { get; set; }
    public decimal? CompareAtPrice { get; set; }
    public decimal? CostPrice { get; set; }
    public string Brand { get; set; } = string.Empty;
    public Guid CategoryId { get; set; }
    public List<string> Tags { get; set; } = new();
    public bool IsDigital { get; set; } = false;
    public bool RequiresShipping { get; set; } = true;
    public bool IsTaxable { get; set; } = true;
}


