using MediatR;
using ECommerce.BuildingBlocks.Common.Models;
using ECommerce.Product.Application.DTOs;
using ECommerce.Product.Domain.Entities;
using ECommerce.Product.Domain.Repositories;
using ECommerce.Product.Application.Commands.Review;

namespace ECommerce.Product.Application.Handlers.Review;

public class CreateReviewCommandHandler : IRequestHandler<CreateReviewCommand, ApiResponse<ReviewDto>>
{
    private readonly IReviewRepository _reviewRepository;

    public CreateReviewCommandHandler(IReviewRepository reviewRepository)
    {
        _reviewRepository = reviewRepository;
    }

    public async Task<ApiResponse<ReviewDto>> Handle(CreateReviewCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Check if user already reviewed this product
            if (await _reviewRepository.ExistsByUserAndProductAsync(request.UserId, request.ProductId))
            {
                return ApiResponse<ReviewDto>.ErrorResult("User has already reviewed this product");
            }

            // Validate rating
            if (request.Rating < 1 || request.Rating > 5)
            {
                return ApiResponse<ReviewDto>.ErrorResult("Rating must be between 1 and 5");
            }

            // Create new review
            var review = new ECommerce.Product.Domain.Entities.Review(request.ProductId, request.UserId, request.Title, request.Content, request.Rating);
            
            // Save to repository
            await _reviewRepository.AddAsync(review);
            // Note: SaveChangesAsync is handled by the Unit of Work pattern or called from the controller

            // Map to DTO
            var reviewDto = new ReviewDto
            {
                Id = review.Id,
                ProductId = review.ProductId,
                UserId = review.UserId,
                Title = review.Title,
                Content = review.Content,
                Rating = review.Rating,
                Status = review.Status,
                CreatedAt = review.CreatedAt,
                UpdatedAt = review.UpdatedAt
            };

            return ApiResponse<ReviewDto>.SuccessResult(reviewDto);
        }
        catch (Exception ex)
        {
            return ApiResponse<ReviewDto>.ErrorResult($"Error creating review: {ex.Message}");
        }
    }
}
