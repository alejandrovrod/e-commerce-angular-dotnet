using Microsoft.AspNetCore.Mvc;
using ECommerce.BuildingBlocks.Common.Models;
using MediatR;
using ECommerce.Product.Application.Queries.Search;
using ECommerce.Product.Application.DTOs;
using ECommerce.Product.Domain.Enums;

namespace ECommerce.Product.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SearchController : ControllerBase
{
    private readonly IMediator _mediator;

    public SearchController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<ProductDto>>>> Search([FromQuery] string q, [FromQuery] string? category, [FromQuery] string? brand)
    {
        var query = new SearchProductsQuery
        {
            Query = q,
            Category = category,
            Brand = brand
        };
        
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("advanced")]
    public async Task<ActionResult<ApiResponse<List<ProductDto>>>> AdvancedSearch(
        [FromQuery] string? name,
        [FromQuery] string? description,
        [FromQuery] decimal? minPrice,
        [FromQuery] decimal? maxPrice,
        [FromQuery] string? category,
        [FromQuery] string? brand,
        [FromQuery] bool? isFeatured,
        [FromQuery] bool? isDigital,
        [FromQuery] ProductStatus? status)
    {
        var query = new AdvancedSearchQuery
        {
            Name = name,
            Description = description,
            MinPrice = minPrice,
            MaxPrice = maxPrice,
            Category = category,
            Brand = brand,
            IsFeatured = isFeatured,
            IsDigital = isDigital,
            Status = status
        };
        
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("suggestions")]
    public async Task<ActionResult<ApiResponse<List<string>>>> GetSuggestions([FromQuery] string q)
    {
        var query = new GetSearchSuggestionsQuery { Query = q };
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("popular")]
    public async Task<ActionResult<ApiResponse<List<ProductDto>>>> GetPopularProducts([FromQuery] int limit = 10)
    {
        var query = new GetPopularProductsQuery { Limit = limit };
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("recent")]
    public async Task<ActionResult<ApiResponse<List<ProductDto>>>> GetRecentProducts([FromQuery] int limit = 10)
    {
        var query = new GetRecentProductsQuery { Limit = limit };
        var result = await _mediator.Send(query);
        return Ok(result);
    }
}
