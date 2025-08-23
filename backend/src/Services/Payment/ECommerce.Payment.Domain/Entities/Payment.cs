using ECommerce.BuildingBlocks.Common.Models;
using ECommerce.Payment.Domain.ValueObjects;
using ECommerce.Payment.Domain.Enums;

namespace ECommerce.Payment.Domain.Entities;

public class Payment : BaseAuditableEntity
{
    public string PaymentNumber { get; private set; } = default!;
    public Guid OrderId { get; private set; }
    public Guid UserId { get; private set; }
    public Money Amount { get; private set; } = default!;
    public PaymentStatus Status { get; private set; } = PaymentStatus.Pending;
    public PaymentMethod PaymentMethod { get; private set; } = default!;
    public PaymentGateway Gateway { get; private set; }
    public string? GatewayTransactionId { get; private set; }
    public string? GatewayPaymentId { get; private set; }
    public PaymentDetails Details { get; private set; } = default!;
    public List<PaymentAttempt> Attempts { get; private set; } = new();
    public List<Refund> Refunds { get; private set; } = new();
    public DateTime? ProcessedAt { get; private set; }
    public DateTime? ExpiresAt { get; private set; }
    public string? FailureReason { get; private set; }
    public new Dictionary<string, object>? Metadata { get; private set; }

    private Payment() { } // For EF Core

    public static Payment Create(
        Guid orderId,
        Guid userId,
        Money amount,
        PaymentMethod paymentMethod,
        PaymentGateway gateway)
    {
        var payment = new Payment
        {
            PaymentNumber = GeneratePaymentNumber(),
            OrderId = orderId,
            UserId = userId,
            Amount = amount,
            PaymentMethod = paymentMethod,
            Gateway = gateway,
            Status = PaymentStatus.Pending,
            ExpiresAt = DateTime.UtcNow.AddHours(1), // Payment expires in 1 hour
            Details = PaymentDetails.Create(paymentMethod, gateway)
        };

        return payment;
    }

    public void StartProcessing(string? gatewayPaymentId = null)
    {
        if (Status != PaymentStatus.Pending)
            throw new InvalidOperationException("Only pending payments can start processing");

        Status = PaymentStatus.Processing;
        GatewayPaymentId = gatewayPaymentId;
        AddAttempt(PaymentAttemptStatus.Processing, "Payment processing started");
        UpdatedAt = DateTime.UtcNow;
    }

    public void Complete(string gatewayTransactionId, Dictionary<string, object>? metadata = null)
    {
        if (Status != PaymentStatus.Processing)
            throw new InvalidOperationException("Only processing payments can be completed");

        Status = PaymentStatus.Completed;
        GatewayTransactionId = gatewayTransactionId;
        ProcessedAt = DateTime.UtcNow;
        Metadata = metadata;
        AddAttempt(PaymentAttemptStatus.Succeeded, "Payment completed successfully");
        UpdatedAt = DateTime.UtcNow;
    }

    public void Fail(string reason, string? errorCode = null)
    {
        Status = PaymentStatus.Failed;
        FailureReason = reason;
        AddAttempt(PaymentAttemptStatus.Failed, reason, errorCode);
        UpdatedAt = DateTime.UtcNow;
    }

    public void Cancel(string reason = "Payment cancelled by user")
    {
        if (Status == PaymentStatus.Completed)
            throw new InvalidOperationException("Cannot cancel completed payments");

        Status = PaymentStatus.Cancelled;
        FailureReason = reason;
        AddAttempt(PaymentAttemptStatus.Cancelled, reason);
        UpdatedAt = DateTime.UtcNow;
    }

    public Refund CreateRefund(Money amount, string reason, RefundType type = RefundType.Full)
    {
        if (Status != PaymentStatus.Completed)
            throw new InvalidOperationException("Only completed payments can be refunded");

        var totalRefunded = Refunds.Where(r => r.Status == RefundStatus.Completed).Sum(r => r.Amount.Amount);
        if (totalRefunded + amount.Amount > Amount.Amount)
            throw new InvalidOperationException("Refund amount exceeds available amount");

        var refund = Refund.Create(Id, amount, reason, type);
        Refunds.Add(refund);
        UpdatedAt = DateTime.UtcNow;

        return refund;
    }

    public void ExpirePayment()
    {
        if (Status == PaymentStatus.Pending && ExpiresAt.HasValue && DateTime.UtcNow > ExpiresAt.Value)
        {
            Status = PaymentStatus.Expired;
            FailureReason = "Payment expired";
            AddAttempt(PaymentAttemptStatus.Expired, "Payment expired");
            UpdatedAt = DateTime.UtcNow;
        }
    }

    public void UpdateGatewayInfo(string? paymentId, string? transactionId)
    {
        GatewayPaymentId = paymentId;
        GatewayTransactionId = transactionId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddMetadata(string key, object value)
    {
        Metadata ??= new Dictionary<string, object>();
        Metadata[key] = value;
        UpdatedAt = DateTime.UtcNow;
    }

    private void AddAttempt(PaymentAttemptStatus status, string message, string? errorCode = null)
    {
        Attempts.Add(new PaymentAttempt(status, message, errorCode, DateTime.UtcNow));
    }

    private static string GeneratePaymentNumber()
    {
        return $"PAY-{DateTime.UtcNow:yyyyMMdd}-{Random.Shared.Next(100000, 999999)}";
    }

    public bool IsExpired() => ExpiresAt.HasValue && DateTime.UtcNow > ExpiresAt.Value;
    
    public bool CanBeRefunded() => Status == PaymentStatus.Completed;
    
    public decimal GetTotalRefunded() => Refunds.Where(r => r.Status == RefundStatus.Completed).Sum(r => r.Amount.Amount);
    
    public decimal GetRefundableAmount() => Amount.Amount - GetTotalRefunded();
    
    public bool IsFullyRefunded() => GetTotalRefunded() >= Amount.Amount;
}
