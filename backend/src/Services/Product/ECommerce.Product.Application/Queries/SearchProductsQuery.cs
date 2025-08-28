using MediatR;
using ECommerce.BuildingBlocks.Common.Models;
using ECommerce.Product.Application.DTOs;

namespace ECommerce.Product.Application.Queries.SearchProducts;

public class SearchProductsQuery : IRequest<ApiResponse<PagedResult<ProductDto>>>
{
    public string SearchTerm { get; set; } = string.Empty;
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public Guid? CategoryId { get; set; }
    public Guid? BrandId { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public bool? InStock { get; set; }
    public string? SortBy { get; set; } = "name";
    public string? SortOrder { get; set; } = "asc";
}




