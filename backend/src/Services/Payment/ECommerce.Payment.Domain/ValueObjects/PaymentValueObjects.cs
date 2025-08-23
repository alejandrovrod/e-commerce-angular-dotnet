using ECommerce.Payment.Domain.Enums;

namespace ECommerce.Payment.Domain.ValueObjects;

public record Money(decimal Amount, string Currency = "USD")
{
    public static Money Zero => new(0);
    public static Money Create(decimal amount, string currency = "USD") => new(amount, currency);
    
    public Money Add(Money other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationException("Cannot add money with different currencies");
        return new Money(Amount + other.Amount, Currency);
    }
    
    public Money Subtract(Money other)
    {
        if (Currency != other.Currency)
            throw new InvalidOperationException("Cannot subtract money with different currencies");
        return new Money(Amount - other.Amount, Currency);
    }
}

public record PaymentMethod(
    string Type, // "card", "paypal", "bank_transfer"
    string? Last4 = null,
    string? Brand = null,
    string? ExpiryMonth = null,
    string? ExpiryYear = null);

public record PaymentDetails(
    string ProcessorName,
    Dictionary<string, string> ProcessorData)
{
    public static PaymentDetails Create(PaymentMethod method, PaymentGateway gateway)
    {
        return gateway switch
        {
            PaymentGateway.Stripe => new PaymentDetails("Stripe", new Dictionary<string, string>
            {
                ["payment_method_type"] = method.Type,
                ["last4"] = method.Last4 ?? "",
                ["brand"] = method.Brand ?? ""
            }),
            PaymentGateway.PayPal => new PaymentDetails("PayPal", new Dictionary<string, string>
            {
                ["payment_method_type"] = method.Type
            }),
            _ => throw new ArgumentException($"Unsupported gateway: {gateway}")
        };
    }
}

public record PaymentAttempt(
    PaymentAttemptStatus Status,
    string Message,
    string? ErrorCode,
    DateTime AttemptedAt);

public class Refund
{
    public Guid Id { get; private set; }
    public Guid PaymentId { get; private set; }
    public Money Amount { get; private set; } = default!;
    public string Reason { get; private set; } = default!;
    public RefundType Type { get; private set; }
    public RefundStatus Status { get; private set; } = RefundStatus.Pending;
    public string? GatewayRefundId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? ProcessedAt { get; private set; }

    private Refund() { } // For EF Core

    public static Refund Create(Guid paymentId, Money amount, string reason, RefundType type)
    {
        return new Refund
        {
            Id = Guid.NewGuid(),
            PaymentId = paymentId,
            Amount = amount,
            Reason = reason,
            Type = type,
            Status = RefundStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Complete(string gatewayRefundId)
    {
        Status = RefundStatus.Completed;
        GatewayRefundId = gatewayRefundId;
        ProcessedAt = DateTime.UtcNow;
    }

    public void Fail(string reason)
    {
        Status = RefundStatus.Failed;
        Reason = reason;
    }
}



