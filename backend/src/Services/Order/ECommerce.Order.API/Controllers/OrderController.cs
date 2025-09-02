using Microsoft.AspNetCore.Mvc;
using MediatR;
using ECommerce.Order.Application.Commands.CreateOrder;
using ECommerce.Order.Application.Commands.UpdateOrder;
using ECommerce.Order.Application.Commands.OrderStatus;
using ECommerce.Order.Application.Commands.OrderItem;
using ECommerce.Order.Application.Queries.GetOrder;
using ECommerce.BuildingBlocks.Common.Models;

namespace ECommerce.Order.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrderController : ControllerBase
{
    private readonly IMediator _mediator;

    public OrderController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Create a new order
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Get order by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetOrderById(Guid id)
    {
        var query = new GetOrderByIdQuery { Id = id };
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get all orders with optional filters
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetOrders(
        [FromQuery] Guid? userId = null,
        [FromQuery] string? status = null,
        [FromQuery] DateTime? fromDate = null,
        [FromQuery] DateTime? toDate = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var query = new GetOrdersQuery
        {
            UserId = userId,
            Status = !string.IsNullOrEmpty(status) ? Enum.Parse<ECommerce.Order.Domain.Enums.OrderStatus>(status) : null,
            FromDate = fromDate,
            ToDate = toDate,
            Page = page,
            PageSize = pageSize
        };

        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get orders by user ID
    /// </summary>
    [HttpGet("user/{userId:guid}")]
    public async Task<IActionResult> GetOrdersByUser(
        Guid userId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var query = new GetOrdersByUserQuery
        {
            UserId = userId,
            Page = page,
            PageSize = pageSize
        };

        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Update order information
    /// </summary>
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateOrder(Guid id, [FromBody] UpdateOrderCommand command)
    {
        command.Id = id;
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Update order status
    /// </summary>
    [HttpPut("{id:guid}/status")]
    public async Task<IActionResult> UpdateOrderStatus(Guid id, [FromBody] UpdateOrderStatusCommand command)
    {
        command.Id = id;
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Add item to order
    /// </summary>
    [HttpPost("{id:guid}/items")]
    public async Task<IActionResult> AddOrderItem(Guid id, [FromBody] AddOrderItemCommand command)
    {
        command.OrderId = id;
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Remove item from order
    /// </summary>
    [HttpDelete("{id:guid}/items/{productId:guid}")]
    public async Task<IActionResult> RemoveOrderItem(Guid id, Guid productId)
    {
        var command = new RemoveOrderItemCommand
        {
            OrderId = id,
            ProductId = productId
        };

        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Update item quantity in order
    /// </summary>
    [HttpPut("{id:guid}/items/{productId:guid}/quantity")]
    public async Task<IActionResult> UpdateOrderItemQuantity(
        Guid id, 
        Guid productId, 
        [FromBody] UpdateOrderItemQuantityCommand command)
    {
        command.OrderId = id;
        command.ProductId = productId;
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Confirm order
    /// </summary>
    [HttpPost("{id:guid}/confirm")]
    public async Task<IActionResult> ConfirmOrder(Guid id)
    {
        var command = new UpdateOrderStatusCommand
        {
            Id = id,
            NewStatus = ECommerce.Order.Domain.Enums.OrderStatus.Confirmed,
            Reason = "Order confirmed"
        };

        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Process order
    /// </summary>
    [HttpPost("{id:guid}/process")]
    public async Task<IActionResult> ProcessOrder(Guid id)
    {
        var command = new UpdateOrderStatusCommand
        {
            Id = id,
            NewStatus = ECommerce.Order.Domain.Enums.OrderStatus.Processing,
            Reason = "Order is being processed"
        };

        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Ship order
    /// </summary>
    [HttpPost("{id:guid}/ship")]
    public async Task<IActionResult> ShipOrder(Guid id, [FromBody] ShipOrderRequest request)
    {
        var command = new UpdateOrderStatusCommand
        {
            Id = id,
            NewStatus = ECommerce.Order.Domain.Enums.OrderStatus.Shipped,
            Reason = "Order shipped",
            TrackingNumber = request.TrackingNumber
        };

        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Deliver order
    /// </summary>
    [HttpPost("{id:guid}/deliver")]
    public async Task<IActionResult> DeliverOrder(Guid id)
    {
        var command = new UpdateOrderStatusCommand
        {
            Id = id,
            NewStatus = ECommerce.Order.Domain.Enums.OrderStatus.Delivered,
            Reason = "Order delivered"
        };

        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Cancel order
    /// </summary>
    [HttpPost("{id:guid}/cancel")]
    public async Task<IActionResult> CancelOrder(Guid id, [FromBody] CancelOrderRequest request)
    {
        var command = new UpdateOrderStatusCommand
        {
            Id = id,
            NewStatus = ECommerce.Order.Domain.Enums.OrderStatus.Cancelled,
            Reason = $"Order cancelled: {request.Reason}"
        };

        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Refund order
    /// </summary>
    [HttpPost("{id:guid}/refund")]
    public async Task<IActionResult> RefundOrder(Guid id, [FromBody] RefundOrderRequest request)
    {
        var command = new UpdateOrderStatusCommand
        {
            Id = id,
            NewStatus = ECommerce.Order.Domain.Enums.OrderStatus.Refunded,
            Reason = $"Order refunded: {request.Reason}"
        };

        var result = await _mediator.Send(command);
        return Ok(result);
    }
}

// Request DTOs for specific operations
public class ShipOrderRequest
{
    public string? TrackingNumber { get; set; }
}

public class CancelOrderRequest
{
    public string Reason { get; set; } = string.Empty;
}

public class RefundOrderRequest
{
    public string Reason { get; set; } = string.Empty;
}




