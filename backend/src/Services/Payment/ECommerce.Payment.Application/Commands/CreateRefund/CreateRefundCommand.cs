using MediatR;
using ECommerce.BuildingBlocks.Common.Models;
using ECommerce.Payment.Application.DTOs;
using ECommerce.Payment.Domain.Enums;

namespace ECommerce.Payment.Application.Commands.CreateRefund;

public class CreateRefundCommand : IRequest<ApiResponse<RefundDto>>
{
    public Guid PaymentId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
    public string Reason { get; set; } = string.Empty;
    public RefundType Type { get; set; } = RefundType.Full;
}
