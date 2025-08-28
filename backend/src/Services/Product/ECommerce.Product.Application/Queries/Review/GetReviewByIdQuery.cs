using MediatR;
using ECommerce.BuildingBlocks.Common.Models;
using ECommerce.Product.Application.DTOs;

namespace ECommerce.Product.Application.Queries.Review;

public class GetReviewByIdQuery : IRequest<ApiResponse<ReviewDto>>
{
    public Guid Id { get; set; }
}
