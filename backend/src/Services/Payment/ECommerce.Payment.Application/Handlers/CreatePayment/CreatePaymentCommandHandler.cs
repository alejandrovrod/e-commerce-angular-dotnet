using MediatR;
using ECommerce.BuildingBlocks.Common.Models;
using ECommerce.Payment.Application.DTOs;
using ECommerce.Payment.Application.Commands.CreatePayment;
using ECommerce.Payment.Application.Interfaces;
using ECommerce.Payment.Domain.Entities;
using ECommerce.Payment.Domain.ValueObjects;
using ECommerce.Payment.Domain.Enums;

namespace ECommerce.Payment.Application.Handlers.CreatePayment;

public class CreatePaymentCommandHandler : IRequestHandler<CreatePaymentCommand, ApiResponse<PaymentDto>>
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreatePaymentCommandHandler(IPaymentRepository paymentRepository, IUnitOfWork unitOfWork)
    {
        _paymentRepository = paymentRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<PaymentDto>> Handle(CreatePaymentCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // Create Money value object
            var money = Money.Create(request.Amount, request.Currency);

            // Create PaymentMethod value object
            var paymentMethod = new PaymentMethod(
                request.PaymentMethod.Type,
                request.PaymentMethod.Last4,
                request.PaymentMethod.Brand,
                request.PaymentMethod.ExpiryMonth,
                request.PaymentMethod.ExpiryYear
            );

            // Create Payment entity
            var payment = ECommerce.Payment.Domain.Entities.Payment.Create(
                request.OrderId,
                request.UserId,
                money,
                paymentMethod,
                request.Gateway
            );

            // Save payment
            var createdPayment = await _paymentRepository.AddAsync(payment);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var paymentDto = MapToDto(createdPayment);

            return ApiResponse<PaymentDto>.SuccessResult(paymentDto);
        }
        catch (Exception ex)
        {
            return ApiResponse<PaymentDto>.ErrorResult($"Error creating payment: {ex.Message}");
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
