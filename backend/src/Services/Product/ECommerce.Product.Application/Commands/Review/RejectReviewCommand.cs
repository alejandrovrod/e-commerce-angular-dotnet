using MediatR;
using ECommerce.BuildingBlocks.Common.Models;

namespace ECommerce.Product.Application.Commands.Review;

public class RejectReviewCommand : IRequest<ApiResponse<bool>>
{
    public Guid Id { get; set; }
    public string Reason { get; set; } = string.Empty;
}
