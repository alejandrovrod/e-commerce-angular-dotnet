using System.Text.Json.Serialization;

namespace ECommerce.BuildingBlocks.EventBus.Events;

public abstract record IntegrationEvent
{
    [JsonPropertyName("id")]
    public Guid Id { get; init; } = Guid.NewGuid();

    [JsonPropertyName("timestamp")]
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;

    [JsonPropertyName("eventType")]
    public string EventType { get; init; }

    [JsonPropertyName("version")]
    public string Version { get; init; } = "1.0";

    [JsonPropertyName("source")]
    public string? Source { get; init; }

    [JsonPropertyName("correlationId")]
    public string? CorrelationId { get; init; }

    protected IntegrationEvent()
    {
        EventType = GetType().Name;
    }
}

// User Events
public record UserRegisteredEvent(
    Guid UserId,
    string Email,
    string FirstName,
    string LastName,
    DateTime RegistrationDate
) : IntegrationEvent;

public record UserProfileUpdatedEvent(
    Guid UserId,
    string Email,
    string FirstName,
    string LastName
) : IntegrationEvent;

// Order Events
public record OrderCreatedEvent(
    Guid OrderId,
    Guid UserId,
    decimal TotalAmount,
    List<OrderItemData> Items,
    string Status
) : IntegrationEvent;

public record OrderStatusChangedEvent(
    Guid OrderId,
    string PreviousStatus,
    string NewStatus,
    DateTime StatusChangeDate,
    string? Reason
) : IntegrationEvent;

public record OrderPaymentProcessedEvent(
    Guid OrderId,
    Guid PaymentId,
    decimal Amount,
    string PaymentStatus,
    string PaymentMethod
) : IntegrationEvent;

// Product Events
public record ProductCreatedEvent(
    Guid ProductId,
    string Name,
    string Sku,
    decimal Price,
    int Stock
) : IntegrationEvent;

public record ProductUpdatedEvent(
    Guid ProductId,
    string Name,
    decimal Price,
    int Stock
) : IntegrationEvent;

public record ProductStockUpdatedEvent(
    Guid ProductId,
    int PreviousStock,
    int NewStock,
    string Reason
) : IntegrationEvent;

// Payment Events
public record PaymentInitiatedEvent(
    Guid PaymentId,
    Guid OrderId,
    decimal Amount,
    string Currency,
    string PaymentMethod
) : IntegrationEvent;

public record PaymentCompletedEvent(
    Guid PaymentId,
    Guid OrderId,
    decimal Amount,
    string PaymentMethod,
    string TransactionId,
    DateTime CompletedAt
) : IntegrationEvent;

public record PaymentFailedEvent(
    Guid PaymentId,
    Guid OrderId,
    decimal Amount,
    string Reason,
    string ErrorCode
) : IntegrationEvent;

// Notification Events
public record SendEmailEvent(
    string To,
    string Subject,
    string Body,
    string? Template = null,
    Dictionary<string, object>? TemplateData = null
) : IntegrationEvent;

public record SendSmsEvent(
    string PhoneNumber,
    string Message,
    string? Template = null
) : IntegrationEvent;

// Supporting Data Models
public record OrderItemData(
    Guid ProductId,
    string ProductName,
    decimal Price,
    int Quantity,
    decimal Total
);







