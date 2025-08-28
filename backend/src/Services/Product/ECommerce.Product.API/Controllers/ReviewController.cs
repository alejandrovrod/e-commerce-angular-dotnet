using Microsoft.AspNetCore.Mvc;
using ECommerce.BuildingBlocks.Common.Models;
using MediatR;
using ECommerce.Product.Application.DTOs;
using ECommerce.Product.Application.Commands.Review;
using ECommerce.Product.Application.Queries.Review;

namespace ECommerce.Product.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReviewController : ControllerBase
{
    private readonly IMediator _mediator;

    public ReviewController(IMediator mediator)
    {
        _mediator = mediator;
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
    public async Task<ActionResult<ApiResponse<List<ReviewDto>>>> GetByProductId(Guid productId, [FromQuery] GetReviewsByProductIdQuery query)
    {
        query.ProductId = productId;
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("user/{userId}")]
    public async Task<ActionResult<ApiResponse<List<ReviewDto>>>> GetByUserId(Guid userId, [FromQuery] GetReviewsByUserIdQuery query)
    {
        query.UserId = userId;
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

    [HttpPost("{id}/approve")]
    public async Task<ActionResult<ApiResponse<bool>>> Approve(Guid id)
    {
        var command = new ApproveReviewCommand { Id = id };
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("{id}/reject")]
    public async Task<ActionResult<ApiResponse<bool>>> Reject(Guid id, [FromBody] RejectReviewCommand command)
    {
        command.Id = id;
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpGet("product/{productId}/average-rating")]
    public async Task<ActionResult<ApiResponse<decimal>>> GetAverageRating(Guid productId)
    {
        var query = new GetAverageRatingQuery { ProductId = productId };
        var result = await _mediator.Send(query);
        return Ok(result);
    }
}
