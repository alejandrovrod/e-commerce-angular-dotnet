using MediatR;
using ECommerce.BuildingBlocks.Common.Models;
using ECommerce.Payment.Application.DTOs;
using ECommerce.Payment.Application.Commands.UpdatePaymentStatus;
using ECommerce.Payment.Application.Interfaces;
using ECommerce.Payment.Domain.Entities;
using ECommerce.Payment.Domain.Enums;

namespace ECommerce.Payment.Application.Handlers.UpdatePaymentStatus;

public class UpdatePaymentStatusCommandHandler : IRequestHandler<UpdatePaymentStatusCommand, ApiResponse<PaymentDto>>
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdatePaymentStatusCommandHandler(IPaymentRepository paymentRepository, IUnitOfWork unitOfWork)
    {
        _paymentRepository = paymentRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<PaymentDto>> Handle(UpdatePaymentStatusCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var payment = await _paymentRepository.GetByIdAsync(request.Id);
            if (payment == null)
            {
                return ApiResponse<PaymentDto>.ErrorResult("Payment not found");
            }

            // Update payment status based on the new status
            switch (request.Status)
            {
                case PaymentStatus.Processing:
                    payment.StartProcessing(request.GatewayPaymentId);
                    break;
                case PaymentStatus.Completed:
                    if (string.IsNullOrEmpty(request.GatewayTransactionId))
                    {
                        return ApiResponse<PaymentDto>.ErrorResult("Gateway transaction ID is required when completing a payment");
                    }
                    payment.Complete(request.GatewayTransactionId, request.Metadata);
                    break;
                case PaymentStatus.Failed:
                    payment.Fail(request.Reason ?? "Payment failed");
                    break;
                case PaymentStatus.Cancelled:
                    payment.Cancel(request.Reason ?? "Payment cancelled");
                    break;
                case PaymentStatus.Expired:
                    payment.ExpirePayment();
                    break;
                default:
                    // For other statuses, we need to handle them differently since there's no direct method
                    // We'll update the status manually and add an attempt
                    if (request.Status != payment.Status)
                    {
                        payment.GetType().GetProperty("Status")?.SetValue(payment, request.Status);
                        payment.GetType().GetMethod("AddAttempt", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.Invoke(payment, new object[] { PaymentAttemptStatus.Processing, $"Status updated to {request.Status}" });
                        payment.GetType().GetProperty("UpdatedAt")?.SetValue(payment, DateTime.UtcNow);
                    }
                    break;
            }

            // Update gateway info if provided
            if (!string.IsNullOrEmpty(request.GatewayTransactionId) || !string.IsNullOrEmpty(request.GatewayPaymentId))
            {
                payment.UpdateGatewayInfo(request.GatewayPaymentId, request.GatewayTransactionId);
            }

            // Add metadata if provided
            if (request.Metadata != null)
            {
                foreach (var kvp in request.Metadata)
                {
                    payment.AddMetadata(kvp.Key, kvp.Value);
                }
            }

            var updatedPayment = await _paymentRepository.UpdateAsync(payment);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var paymentDto = MapToDto(updatedPayment);
            return ApiResponse<PaymentDto>.SuccessResult(paymentDto);
        }
        catch (Exception ex)
        {
            return ApiResponse<PaymentDto>.ErrorResult($"Error updating payment status: {ex.Message}");
        }
    }

    private PaymentDto MapToDto(ECommerce.Payment.Domain.Entities.Payment payment)
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
