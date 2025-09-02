using Microsoft.AspNetCore.Mvc;
using ECommerce.BuildingBlocks.Common.Models;
using MediatR;
using ECommerce.Product.Application.DTOs;
using ECommerce.Product.Application.Commands.Inventory;
using ECommerce.Product.Application.Queries.Inventory;
using ECommerce.Product.Domain.Repositories;

namespace ECommerce.Product.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InventoryController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IInventoryRepository _inventoryRepository;

    public InventoryController(IMediator mediator, IInventoryRepository inventoryRepository)
    {
        _mediator = mediator;
        _inventoryRepository = inventoryRepository;
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

    [HttpGet("low-stock")]
    public async Task<ActionResult<ApiResponse<List<InventoryDto>>>> GetLowStock([FromQuery] GetLowStockQuery query)
    {
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

    [HttpPatch("{id}/stock")]
    public async Task<ActionResult<ApiResponse<bool>>> AdjustStock(Guid id, [FromBody] ECommerce.Product.Application.Commands.Inventory.AdjustStockCommand command)
    {
        // Para este endpoint, el id es el ID del inventario, necesitamos obtener el ProductId
        var inventory = await _inventoryRepository.GetByIdAsync(id);
        if (inventory == null)
        {
            return NotFound(ApiResponse<bool>.ErrorResult("Inventory not found"));
        }
        
        command.ProductId = inventory.ProductId;
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpPatch("adjust")]
    public async Task<ActionResult<ApiResponse<bool>>> AdjustStockDirect([FromBody] ECommerce.Product.Application.Commands.Inventory.AdjustStockCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpPatch("{id}/reserve")]
    public async Task<ActionResult<ApiResponse<bool>>> ReserveStock(Guid id, [FromBody] ReserveStockCommand command)
    {
        command.Id = id;
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [HttpPatch("{id}/release")]
    public async Task<ActionResult<ApiResponse<bool>>> ReleaseStock(Guid id, [FromBody] int quantity)
    {
        var inventory = await _inventoryRepository.GetByIdAsync(id);
        if (inventory == null)
            return NotFound(ApiResponse<bool>.ErrorResult("Inventory not found"));

        // TODO: Implement ReleaseStock method in Inventory entity
        // For now, just return success
        return Ok(ApiResponse<bool>.SuccessResult(true));
    }

    [HttpGet("history")]
    public async Task<ActionResult<ApiResponse<List<InventoryMovementDto>>>> GetHistory([FromQuery] GetInventoryHistoryQuery query)
    {
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("history/{productId}")]
    public async Task<ActionResult<ApiResponse<List<InventoryMovementDto>>>> GetProductHistory(Guid productId)
    {
        var query = new GetInventoryHistoryQuery { ProductId = productId };
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    [HttpGet("debug")]
    public async Task<ActionResult<object>> DebugInventory()
    {
        try
        {
            var inventoryFromContext = await _inventoryRepository.GetAllAsync();
            
            var debugInfo = new
            {
                ContextInventoryCount = inventoryFromContext.Count,
                ContextInventory = inventoryFromContext.Select(i => new { i.Id, i.ProductId, i.Quantity, i.Location, i.CreatedAt }),
                Message = "Debug info from InventoryController"
            };
            
            return Ok(debugInfo);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message, stackTrace = ex.StackTrace });
        }
    }
}
