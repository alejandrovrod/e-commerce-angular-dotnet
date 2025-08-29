using MediatR;
using ECommerce.BuildingBlocks.Common.Models;
using ECommerce.Payment.Application.DTOs;

namespace ECommerce.Payment.Application.Queries.GetPayment;

public class GetPaymentByIdQuery : IRequest<ApiResponse<PaymentDto>>
{
    public Guid Id { get; set; }
}
