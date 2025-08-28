using Microsoft.AspNetCore.Mvc;
using ECommerce.BuildingBlocks.Common.Models;
using MediatR;
using ECommerce.Product.Application.DTOs;
using ECommerce.Product.Application.Commands.Inventory;
using ECommerce.Product.Application.Queries.Inventory;

namespace ECommerce.Product.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InventoryController : ControllerBase
{
    private readonly IMediator _mediator;

    public InventoryController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<InventoryDto>>>> GetAll([FromQuery] GetInventoryQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<InventoryDto>>> GetById(Guid id)
    {
        var query = new GetInventoryByIdQuery { Id = id };
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("product/{productId}")]
    public async Task<ActionResult<ApiResponse<InventoryDto>>> GetByProductId(Guid productId)
    {
        var query = new GetInventoryByProductIdQuery { ProductId = productId };
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<InventoryDto>>> Create([FromBody] CreateInventoryCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<InventoryDto>>> Update(Guid id, [FromBody] UpdateInventoryCommand command)
    {
        command.Id = id;
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("{id}/adjust")]
    public async Task<ActionResult<ApiResponse<InventoryDto>>> AdjustStock(Guid id, [FromBody] AdjustStockCommand command)
    {
        command.Id = id;
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpPost("{id}/reserve")]
    public async Task<ActionResult<ApiResponse<bool>>> ReserveStock(Guid id, [FromBody] ReserveStockCommand command)
    {
        command.Id = id;
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpGet("low-stock")]
    public async Task<ActionResult<ApiResponse<List<InventoryDto>>>> GetLowStock([FromQuery] GetLowStockQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }
}
