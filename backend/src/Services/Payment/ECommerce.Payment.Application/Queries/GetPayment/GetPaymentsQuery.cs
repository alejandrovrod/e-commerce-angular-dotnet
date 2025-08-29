using MediatR;
using ECommerce.BuildingBlocks.Common.Models;
using ECommerce.Payment.Application.DTOs;
using ECommerce.Payment.Domain.Enums;

namespace ECommerce.Payment.Application.Queries.GetPayment;

public class GetPaymentsQuery : IRequest<ApiResponse<List<PaymentDto>>>
{
    public Guid? OrderId { get; set; }
    public Guid? UserId { get; set; }
    public PaymentStatus? Status { get; set; }
    public PaymentGateway? Gateway { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
