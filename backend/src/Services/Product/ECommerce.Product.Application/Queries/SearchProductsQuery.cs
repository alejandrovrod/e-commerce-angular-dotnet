using MediatR;
using ECommerce.Product.Application.DTOs;

namespace ECommerce.Product.Application.Queries;

public class SearchProductsQuery : IRequest<PaginatedResult<ProductDto>>
{
    public string SearchTerm { get; set; } = string.Empty;
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}




