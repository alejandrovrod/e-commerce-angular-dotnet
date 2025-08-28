using MediatR;
using ECommerce.BuildingBlocks.Common.Models;

namespace ECommerce.Product.Application.Commands.Review;

public class DeleteReviewCommand : IRequest<ApiResponse<bool>>
{
    public Guid Id { get; set; }
}
