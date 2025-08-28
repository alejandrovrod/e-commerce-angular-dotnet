using MediatR;
using ECommerce.BuildingBlocks.Common.Models;

namespace ECommerce.Product.Application.Queries.Review;

public class GetAverageRatingQuery : IRequest<ApiResponse<decimal>>
{
    public Guid ProductId { get; set; }
}
