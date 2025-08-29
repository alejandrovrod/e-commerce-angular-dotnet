using MediatR;
using ECommerce.BuildingBlocks.Common.Models;

namespace ECommerce.Product.Application.Queries.Search;

public class GetSearchSuggestionsQuery : IRequest<ApiResponse<List<string>>>
{
    public string Query { get; set; } = string.Empty;
}
