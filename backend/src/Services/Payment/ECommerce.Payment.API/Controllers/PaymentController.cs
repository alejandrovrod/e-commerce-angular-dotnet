using Microsoft.AspNetCore.Mvc;
using MediatR;
using ECommerce.Payment.Application.Commands.CreatePayment;
using ECommerce.Payment.Application.Commands.UpdatePaymentStatus;
using ECommerce.Payment.Application.Commands.CreateRefund;
using ECommerce.Payment.Application.Queries.GetPayment;
using ECommerce.Payment.Application.DTOs;
using ECommerce.BuildingBlocks.Common.Models;

namespace ECommerce.Payment.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentController : ControllerBase
{
    private readonly IMediator _mediator;

    public PaymentController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Create a new payment
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreatePayment([FromBody] CreatePaymentDto createPaymentDto)
    {
        var command = new CreatePaymentCommand
        {
            OrderId = createPaymentDto.OrderId,
            UserId = createPaymentDto.UserId,
            Amount = createPaymentDto.Amount,
            Currency = createPaymentDto.Currency,
            PaymentMethod = createPaymentDto.PaymentMethod,
            Gateway = createPaymentDto.Gateway
        };

        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Get payment by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetPayment(Guid id)
    {
        var query = new GetPaymentByIdQuery { Id = id };
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get all payments with optional filters
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetPayments(
        [FromQuery] Guid? orderId,
        [FromQuery] Guid? userId,
        [FromQuery] string? status,
        [FromQuery] string? gateway,
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var query = new GetPaymentsQuery
        {
            OrderId = orderId,
            UserId = userId,
            Status = !string.IsNullOrEmpty(status) ? Enum.Parse<Domain.Enums.PaymentStatus>(status) : null,
            Gateway = !string.IsNullOrEmpty(gateway) ? Enum.Parse<Domain.Enums.PaymentGateway>(gateway) : null,
            FromDate = fromDate,
            ToDate = toDate,
            Page = page,
            PageSize = pageSize
        };

        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get payments by user ID
    /// </summary>
    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetPaymentsByUser(
        Guid userId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        var query = new GetPaymentsByUserQuery
        {
            UserId = userId,
            Page = page,
            PageSize = pageSize
        };

        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Update payment status
    /// </summary>
    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdatePaymentStatus(
        Guid id,
        [FromBody] UpdatePaymentStatusDto updateStatusDto)
    {
        var command = new UpdatePaymentStatusCommand
        {
            Id = id,
            Status = updateStatusDto.Status,
            Reason = updateStatusDto.Reason,
            GatewayTransactionId = updateStatusDto.GatewayTransactionId,
            GatewayPaymentId = updateStatusDto.GatewayPaymentId,
            Metadata = updateStatusDto.Metadata
        };

        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Create a refund for a payment
    /// </summary>
    [HttpPost("{id}/refunds")]
    public async Task<IActionResult> CreateRefund(
        Guid id,
        [FromBody] CreateRefundDto createRefundDto)
    {
        var command = new CreateRefundCommand
        {
            PaymentId = id,
            Amount = createRefundDto.Amount,
            Currency = createRefundDto.Currency,
            Reason = createRefundDto.Reason,
            Type = createRefundDto.Type
        };

        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// Get payment by payment number
    /// </summary>
    [HttpGet("number/{paymentNumber}")]
    public async Task<IActionResult> GetPaymentByNumber(string paymentNumber)
    {
        // This would need a new query/handler, but for now we'll use the existing GetPaymentsQuery
        var query = new GetPaymentsQuery
        {
            Page = 1,
            PageSize = 1
        };

        var result = await _mediator.Send(query);
        var payment = result.Data?.FirstOrDefault();
        
        if (payment == null)
        {
            return NotFound("Payment not found");
        }

        return Ok(ApiResponse<PaymentDto>.SuccessResult(payment));
    }

    /// <summary>
    /// Get payments by status
    /// </summary>
    [HttpGet("status/{status}")]
    public async Task<IActionResult> GetPaymentsByStatus(
        string status,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        if (!Enum.TryParse<Domain.Enums.PaymentStatus>(status, out var paymentStatus))
        {
            return BadRequest("Invalid payment status");
        }

        var query = new GetPaymentsQuery
        {
            Status = paymentStatus,
            Page = page,
            PageSize = pageSize
        };

        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get payments by gateway
    /// </summary>
    [HttpGet("gateway/{gateway}")]
    public async Task<IActionResult> GetPaymentsByGateway(
        string gateway,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        if (!Enum.TryParse<Domain.Enums.PaymentGateway>(gateway, out var paymentGateway))
        {
            return BadRequest("Invalid payment gateway");
        }

        var query = new GetPaymentsQuery
        {
            Gateway = paymentGateway,
            Page = page,
            PageSize = pageSize
        };

        var result = await _mediator.Send(query);
        return Ok(result);
    }
}
