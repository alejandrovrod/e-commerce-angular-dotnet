using MediatR;
using ECommerce.BuildingBlocks.Common.Models;
using ECommerce.Product.Application.DTOs;

namespace ECommerce.Product.Application.Queries.Search;

public class GetRecentProductsQuery : IRequest<ApiResponse<List<ProductDto>>>
{
    public int Limit { get; set; } = 10;
}
