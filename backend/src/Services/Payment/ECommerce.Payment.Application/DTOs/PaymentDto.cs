using ECommerce.Payment.Domain.Enums;

namespace ECommerce.Payment.Application.DTOs;

public class PaymentDto
{
    public Guid Id { get; set; }
    public string PaymentNumber { get; set; } = string.Empty;
    public Guid OrderId { get; set; }
    public Guid UserId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
    public PaymentStatus Status { get; set; }
    public PaymentMethodDto PaymentMethod { get; set; } = new();
    public PaymentGateway Gateway { get; set; }
    public string? GatewayTransactionId { get; set; }
    public string? GatewayPaymentId { get; set; }
    public PaymentDetailsDto Details { get; set; } = new();
    public List<PaymentAttemptDto> Attempts { get; set; } = new();
    public List<RefundDto> Refunds { get; set; } = new();
    public DateTime? ProcessedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public string? FailureReason { get; set; }
    public Dictionary<string, object>? Metadata { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class PaymentMethodDto
{
    public string Type { get; set; } = string.Empty;
    public string? Last4 { get; set; }
    public string? Brand { get; set; }
    public string? ExpiryMonth { get; set; }
    public string? ExpiryYear { get; set; }
}

public class PaymentDetailsDto
{
    public string ProcessorName { get; set; } = string.Empty;
    public Dictionary<string, string> ProcessorData { get; set; } = new();
}

public class PaymentAttemptDto
{
    public PaymentAttemptStatus Status { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? ErrorCode { get; set; }
    public DateTime AttemptedAt { get; set; }
}

public class RefundDto
{
    public Guid Id { get; set; }
    public Guid PaymentId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
    public string Reason { get; set; } = string.Empty;
    public RefundType Type { get; set; }
    public RefundStatus Status { get; set; }
    public string? GatewayRefundId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ProcessedAt { get; set; }
}

public class CreatePaymentDto
{
    public Guid OrderId { get; set; }
    public Guid UserId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
    public PaymentMethodDto PaymentMethod { get; set; } = new();
    public PaymentGateway Gateway { get; set; }
}

public class UpdatePaymentStatusDto
{
    public PaymentStatus Status { get; set; }
    public string? Reason { get; set; }
    public string? GatewayTransactionId { get; set; }
    public string? GatewayPaymentId { get; set; }
    public Dictionary<string, object>? Metadata { get; set; }
}

public class CreateRefundDto
{
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
    public string Reason { get; set; } = string.Empty;
    public RefundType Type { get; set; } = RefundType.Full;
}

public class PaymentFilterDto
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
