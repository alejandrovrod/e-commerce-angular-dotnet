using MediatR;
using ECommerce.BuildingBlocks.Common.Models;
using ECommerce.Payment.Application.DTOs;
using ECommerce.Payment.Domain.Enums;

namespace ECommerce.Payment.Application.Commands.UpdatePaymentStatus;

public class UpdatePaymentStatusCommand : IRequest<ApiResponse<PaymentDto>>
{
    public Guid Id { get; set; }
    public PaymentStatus Status { get; set; }
    public string? Reason { get; set; }
    public string? GatewayTransactionId { get; set; }
    public string? GatewayPaymentId { get; set; }
    public Dictionary<string, object>? Metadata { get; set; }
}
