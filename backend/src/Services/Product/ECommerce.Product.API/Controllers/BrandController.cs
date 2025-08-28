using Microsoft.AspNetCore.Mvc;
using ECommerce.BuildingBlocks.Common.Models;
using MediatR;
using ECommerce.Product.Application.DTOs;
using ECommerce.Product.Application.Commands.Brand;
using ECommerce.Product.Application.Queries.Brand;

namespace ECommerce.Product.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BrandController : ControllerBase
{
    private readonly IMediator _mediator;

    public BrandController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<BrandDto>>>> GetAll([FromQuery] GetBrandsQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<BrandDto>>> GetById(Guid id)
    {
        var query = new GetBrandByIdQuery { Id = id };
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<BrandDto>>> Create([FromBody] CreateBrandCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<BrandDto>>> Update(Guid id, [FromBody] UpdateBrandCommand command)
    {
        command.Id = id;
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<bool>>> Delete(Guid id)
    {
        var command = new DeleteBrandCommand { Id = id };
        var result = await _mediator.Send(command);
        return Ok(result);
    }
}
