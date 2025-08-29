using MediatR;
using ECommerce.BuildingBlocks.Common.Models;
using ECommerce.Payment.Application.DTOs;

namespace ECommerce.Payment.Application.Queries.GetPayment;

public class GetPaymentsByUserQuery : IRequest<ApiResponse<List<PaymentDto>>>
{
    public Guid UserId { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
