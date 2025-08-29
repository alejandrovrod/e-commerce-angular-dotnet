using Microsoft.AspNetCore.Mvc;
using ECommerce.BuildingBlocks.Common.Models;
using MediatR;
using ECommerce.Product.Application.DTOs;
using ECommerce.Product.Application.Commands.Review;
using ECommerce.Product.Application.Queries.Review;
using ECommerce.Product.Domain.Repositories;

namespace ECommerce.Product.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReviewController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IReviewRepository _reviewRepository;

    public ReviewController(IMediator mediator, IReviewRepository reviewRepository)
    {
        _mediator = mediator;
        _reviewRepository = reviewRepository;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<ReviewDto>>>> GetAll([FromQuery] GetReviewsQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<ReviewDto>>> GetById(Guid id)
    {
        var query = new GetReviewByIdQuery { Id = id };
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("product/{productId}")]
    public async Task<ActionResult<ApiResponse<List<ReviewDto>>>> GetByProductId(Guid productId)
    {
        var query = new GetReviewsByProductIdQuery { ProductId = productId };
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("user/{userId}")]
    public async Task<ActionResult<ApiResponse<List<ReviewDto>>>> GetByUserId(Guid userId)
    {
        var query = new GetReviewsByUserIdQuery { UserId = userId };
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("average-rating/{productId}")]
    public async Task<ActionResult<ApiResponse<double>>> GetAverageRating(Guid productId)
    {
        var query = new GetAverageRatingQuery { ProductId = productId };
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<ReviewDto>>> Create([FromBody] CreateReviewCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<ReviewDto>>> Update(Guid id, [FromBody] UpdateReviewCommand command)
    {
        command.Id = id;
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(Guid id)
    {
        var command = new DeleteReviewCommand { Id = id };
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpPatch("{id}/approve")]
    public async Task<ActionResult<ApiResponse<bool>>> Approve(Guid id)
    {
        var command = new ApproveReviewCommand { Id = id };
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpPatch("{id}/reject")]
    public async Task<ActionResult<ApiResponse<bool>>> Reject(Guid id)
    {
        var command = new RejectReviewCommand { Id = id };
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpPatch("{id}/helpful")]
    public async Task<ActionResult<ApiResponse<bool>>> MarkHelpful(Guid id, [FromBody] int count)
    {
        var review = await _reviewRepository.GetByIdAsync(id);
        if (review == null)
            return NotFound(ApiResponse<bool>.ErrorResult("Review not found"));

        // TODO: Implement UpdateHelpfulCount method in Review entity
        // For now, just return success
        return Ok(ApiResponse<bool>.SuccessResult(true));
    }

    [HttpGet("debug")]
    public async Task<ActionResult<object>> DebugReviews()
    {
        try
        {
            var reviewsFromContext = await _reviewRepository.GetAllAsync();
            
            var debugInfo = new
            {
                ContextReviewsCount = reviewsFromContext.Count,
                ContextReviews = reviewsFromContext.Select(r => new { r.Id, r.ProductId, r.UserId, r.Rating, r.Status, r.CreatedAt }),
                Message = "Debug info from ReviewController"
            };
            
            return Ok(debugInfo);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message, stackTrace = ex.StackTrace });
        }
    }
}
