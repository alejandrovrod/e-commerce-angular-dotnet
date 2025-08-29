using MediatR;
using ECommerce.BuildingBlocks.Common.Models;
using ECommerce.Payment.Application.DTOs;
using ECommerce.Payment.Application.Queries.GetPayment;
using ECommerce.Payment.Application.Interfaces;

namespace ECommerce.Payment.Application.Handlers.GetPayment;

public class GetPaymentByIdQueryHandler : IRequestHandler<GetPaymentByIdQuery, ApiResponse<PaymentDto>>
{
    private readonly IPaymentRepository _paymentRepository;

    public GetPaymentByIdQueryHandler(IPaymentRepository paymentRepository)
    {
        _paymentRepository = paymentRepository;
    }

    public async Task<ApiResponse<PaymentDto>> Handle(GetPaymentByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var payment = await _paymentRepository.GetByIdAsync(request.Id);
            if (payment == null)
            {
                return ApiResponse<PaymentDto>.ErrorResult("Payment not found");
            }

            var paymentDto = MapToDto(payment);
            return ApiResponse<PaymentDto>.SuccessResult(paymentDto);
        }
        catch (Exception ex)
        {
            return ApiResponse<PaymentDto>.ErrorResult($"Error retrieving payment: {ex.Message}");
        }
    }

    private PaymentDto MapToDto(Domain.Entities.Payment payment)
    {
        return new PaymentDto
        {
            Id = payment.Id,
            PaymentNumber = payment.PaymentNumber,
            OrderId = payment.OrderId,
            UserId = payment.UserId,
            Amount = payment.Amount.Amount,
            Currency = payment.Amount.Currency,
            Status = payment.Status,
            PaymentMethod = new PaymentMethodDto
            {
                Type = payment.PaymentMethod.Type,
                Last4 = payment.PaymentMethod.Last4,
                Brand = payment.PaymentMethod.Brand,
                ExpiryMonth = payment.PaymentMethod.ExpiryMonth,
                ExpiryYear = payment.PaymentMethod.ExpiryYear
            },
            Gateway = payment.Gateway,
            GatewayTransactionId = payment.GatewayTransactionId,
            GatewayPaymentId = payment.GatewayPaymentId,
            Details = new PaymentDetailsDto
            {
                ProcessorName = payment.Details.ProcessorName,
                ProcessorData = payment.Details.ProcessorData
            },
            Attempts = payment.Attempts.Select(a => new PaymentAttemptDto
            {
                Status = a.Status,
                Message = a.Message,
                ErrorCode = a.ErrorCode,
                AttemptedAt = a.AttemptedAt
            }).ToList(),
            Refunds = payment.Refunds.Select(r => new RefundDto
            {
                Id = r.Id,
                PaymentId = r.PaymentId,
                Amount = r.Amount.Amount,
                Currency = r.Amount.Currency,
                Reason = r.Reason,
                Type = r.Type,
                Status = r.Status,
                GatewayRefundId = r.GatewayRefundId,
                CreatedAt = r.CreatedAt,
                ProcessedAt = r.ProcessedAt
            }).ToList(),
            ProcessedAt = payment.ProcessedAt,
            ExpiresAt = payment.ExpiresAt,
            FailureReason = payment.FailureReason,
            Metadata = payment.Metadata,
            CreatedAt = payment.CreatedAt,
            UpdatedAt = payment.UpdatedAt
        };
    }
}
