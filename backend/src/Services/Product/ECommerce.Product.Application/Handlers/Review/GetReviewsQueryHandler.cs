using MediatR;
using ECommerce.BuildingBlocks.Common.Models;
using ECommerce.Product.Application.DTOs;
using ECommerce.Product.Application.Queries.Review;
using ECommerce.Product.Domain.Repositories;
using ECommerce.Product.Application.Interfaces;

namespace ECommerce.Product.Application.Handlers.Review;

public class GetReviewsQueryHandler : IRequestHandler<GetReviewsQuery, ApiResponse<List<ReviewDto>>>
{
    private readonly IReviewRepository _reviewRepository;
    private readonly IUnitOfWork _unitOfWork;

    public GetReviewsQueryHandler(IReviewRepository reviewRepository, IUnitOfWork unitOfWork)
    {
        _reviewRepository = reviewRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<List<ReviewDto>>> Handle(GetReviewsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var reviews = await _reviewRepository.GetAllAsync();
            
            var reviewDtos = reviews.Select(r => new ReviewDto
            {
                Id = r.Id,
                ProductId = r.ProductId,
                UserId = r.UserId,
                Title = r.Title,
                Content = r.Content,
                Rating = r.Rating,
                Status = r.Status,
                CreatedAt = r.CreatedAt,
                UpdatedAt = r.UpdatedAt
            }).ToList();

            return ApiResponse<List<ReviewDto>>.SuccessResult(reviewDtos);
        }
        catch (Exception ex)
        {
            return ApiResponse<List<ReviewDto>>.ErrorResult($"Error retrieving reviews: {ex.Message}");
        }
    }
}
