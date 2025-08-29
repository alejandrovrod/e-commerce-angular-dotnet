using MediatR;
using ECommerce.BuildingBlocks.Common.Models;
using ECommerce.Product.Application.DTOs;
using ECommerce.Product.Domain.Enums;

namespace ECommerce.Product.Application.Queries.Search;

public class AdvancedSearchQuery : IRequest<ApiResponse<List<ProductDto>>>
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public string? Category { get; set; }
    public string? Brand { get; set; }
    public bool? IsFeatured { get; set; }
    public bool? IsDigital { get; set; }
    public ProductStatus? Status { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
