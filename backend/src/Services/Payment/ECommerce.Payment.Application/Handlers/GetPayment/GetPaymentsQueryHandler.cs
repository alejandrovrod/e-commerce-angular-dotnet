using MediatR;
using ECommerce.BuildingBlocks.Common.Models;
using ECommerce.Payment.Application.DTOs;
using ECommerce.Payment.Application.Queries.GetPayment;
using ECommerce.Payment.Application.Interfaces;
using ECommerce.Payment.Domain.Enums;

namespace ECommerce.Payment.Application.Handlers.GetPayment;

public class GetPaymentsQueryHandler : IRequestHandler<GetPaymentsQuery, ApiResponse<List<PaymentDto>>>
{
    private readonly IPaymentRepository _paymentRepository;

    public GetPaymentsQueryHandler(IPaymentRepository paymentRepository)
    {
        _paymentRepository = paymentRepository;
    }

    public async Task<ApiResponse<List<PaymentDto>>> Handle(GetPaymentsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            List<Domain.Entities.Payment> payments;

            // Apply filters
            if (request.OrderId.HasValue)
            {
                var payment = await _paymentRepository.GetByOrderIdAsync(request.OrderId.Value);
                payments = payment != null ? new List<Domain.Entities.Payment> { payment } : new List<Domain.Entities.Payment>();
            }
            else if (request.UserId.HasValue)
            {
                payments = await _paymentRepository.GetByUserIdAsync(request.UserId.Value);
            }
            else if (request.Status.HasValue)
            {
                payments = await _paymentRepository.GetByStatusAsync(request.Status.Value);
            }
            else if (request.Gateway.HasValue)
            {
                payments = await _paymentRepository.GetByGatewayAsync(request.Gateway.Value);
            }
            else
            {
                payments = await _paymentRepository.GetAllAsync();
            }

            // Apply date filters
            if (request.FromDate.HasValue || request.ToDate.HasValue)
            {
                payments = payments.Where(p => 
                    (!request.FromDate.HasValue || p.CreatedAt >= request.FromDate.Value) &&
                    (!request.ToDate.HasValue || p.CreatedAt <= request.ToDate.Value)
                ).ToList();
            }

            // Apply pagination
            var totalCount = payments.Count;
            var paginatedPayments = payments
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            var paymentDtos = paginatedPayments.Select(MapToDto).ToList();
            return ApiResponse<List<PaymentDto>>.SuccessResult(paymentDtos);
        }
        catch (Exception ex)
        {
            return ApiResponse<List<PaymentDto>>.ErrorResult($"Error retrieving payments: {ex.Message}");
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
