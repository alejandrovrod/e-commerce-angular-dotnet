using MediatR;
using ECommerce.BuildingBlocks.Common.Models;
using ECommerce.Product.Application.DTOs;

namespace ECommerce.Product.Application.Queries.Search;

public class SearchProductsQuery : IRequest<ApiResponse<List<ProductDto>>>
{
    public string Query { get; set; } = string.Empty;
    public string? Category { get; set; }
    public string? Brand { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
