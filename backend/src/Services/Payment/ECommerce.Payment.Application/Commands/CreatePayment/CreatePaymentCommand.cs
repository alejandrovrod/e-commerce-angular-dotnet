using MediatR;
using ECommerce.BuildingBlocks.Common.Models;
using ECommerce.Payment.Application.DTOs;
using ECommerce.Payment.Domain.Enums;

namespace ECommerce.Payment.Application.Commands.CreatePayment;

public class CreatePaymentCommand : IRequest<ApiResponse<PaymentDto>>
{
    public Guid OrderId { get; set; }
    public Guid UserId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
    public PaymentMethodDto PaymentMethod { get; set; } = new();
    public PaymentGateway Gateway { get; set; }
}
