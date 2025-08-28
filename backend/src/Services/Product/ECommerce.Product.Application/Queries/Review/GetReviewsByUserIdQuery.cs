using MediatR;
using ECommerce.BuildingBlocks.Common.Models;
using ECommerce.Product.Application.DTOs;
using ECommerce.Product.Domain.Enums;

namespace ECommerce.Product.Application.Queries.Review;

public class GetReviewsByUserIdQuery : IRequest<ApiResponse<List<ReviewDto>>>
{
    public Guid UserId { get; set; }
    public ReviewStatus? Status { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
