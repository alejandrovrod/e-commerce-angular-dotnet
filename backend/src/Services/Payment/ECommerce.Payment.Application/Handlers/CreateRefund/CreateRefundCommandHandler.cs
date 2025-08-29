using MediatR;
using ECommerce.BuildingBlocks.Common.Models;
using ECommerce.Payment.Application.DTOs;
using ECommerce.Payment.Application.Commands.CreateRefund;
using ECommerce.Payment.Application.Interfaces;
using ECommerce.Payment.Domain.Entities;
using ECommerce.Payment.Domain.ValueObjects;
using ECommerce.Payment.Domain.Enums;

namespace ECommerce.Payment.Application.Handlers.CreateRefund;

public class CreateRefundCommandHandler : IRequestHandler<CreateRefundCommand, ApiResponse<RefundDto>>
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateRefundCommandHandler(IPaymentRepository paymentRepository, IUnitOfWork unitOfWork)
    {
        _paymentRepository = paymentRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<RefundDto>> Handle(CreateRefundCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var payment = await _paymentRepository.GetByIdAsync(request.PaymentId);
            if (payment == null)
            {
                return ApiResponse<RefundDto>.ErrorResult("Payment not found");
            }

            if (!payment.CanBeRefunded())
            {
                return ApiResponse<RefundDto>.ErrorResult("Payment cannot be refunded");
            }

            if (request.Amount > payment.GetRefundableAmount())
            {
                return ApiResponse<RefundDto>.ErrorResult("Refund amount exceeds refundable amount");
            }

            // Create Money value object for refund
            var refundAmount = Money.Create(request.Amount, request.Currency);

            // Create refund using the payment entity
            var refund = payment.CreateRefund(refundAmount, request.Reason, request.Type);

            // Save payment (which now includes the refund)
            var updatedPayment = await _paymentRepository.UpdateAsync(payment);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Find the created refund in the updated payment
            var createdRefund = updatedPayment.Refunds.FirstOrDefault(r => 
                r.Amount.Amount == request.Amount && 
                r.Reason == request.Reason && 
                r.Type == request.Type);

            if (createdRefund == null)
            {
                return ApiResponse<RefundDto>.ErrorResult("Failed to create refund");
            }

            var refundDto = MapToDto(createdRefund);
            return ApiResponse<RefundDto>.SuccessResult(refundDto);
        }
        catch (Exception ex)
        {
            return ApiResponse<RefundDto>.ErrorResult($"Error creating refund: {ex.Message}");
        }
    }

    private RefundDto MapToDto(Refund refund)
    {
        return new RefundDto
        {
            Id = refund.Id,
            PaymentId = refund.PaymentId,
            Amount = refund.Amount.Amount,
            Currency = refund.Amount.Currency,
            Reason = refund.Reason,
            Type = refund.Type,
            Status = refund.Status,
            GatewayRefundId = refund.GatewayRefundId,
            CreatedAt = refund.CreatedAt,
            ProcessedAt = refund.ProcessedAt
        };
    }
}
