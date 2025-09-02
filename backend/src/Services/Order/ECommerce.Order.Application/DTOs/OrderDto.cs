using ECommerce.Order.Domain.Enums;

namespace ECommerce.Order.Application.DTOs;

public class OrderDto
{
    public Guid Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public OrderStatus Status { get; set; }
    public List<OrderItemDto> Items { get; set; } = new();
    public AddressDto ShippingAddress { get; set; } = new();
    public AddressDto? BillingAddress { get; set; }
    public OrderPricingDto Pricing { get; set; } = new();
    public string? CouponCode { get; set; }
    public Guid? ShippingMethodId { get; set; }
    public PaymentInfoDto? PaymentInfo { get; set; }
    public List<OrderStatusHistoryDto> StatusHistory { get; set; } = new();
    public string? Notes { get; set; }
    public DateTime? ShippedAt { get; set; }
    public DateTime? DeliveredAt { get; set; }
    public string? TrackingNumber { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class OrderItemDto
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string? ProductSku { get; set; }
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal TotalPrice { get; set; }
    public decimal? Weight { get; set; }
}

public class AddressDto
{
    public string Street { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string? AddressLine2 { get; set; }
    public string FullAddress { get; set; } = string.Empty;
}

public class OrderPricingDto
{
    public decimal Subtotal { get; set; }
    public decimal Tax { get; set; }
    public decimal ShippingCost { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal Total { get; set; }
}

public class PaymentInfoDto
{
    public string PaymentMethod { get; set; } = string.Empty;
    public string? PaymentId { get; set; }
    public string? TransactionId { get; set; }
}

public class OrderStatusHistoryDto
{
    public OrderStatus Status { get; set; }
    public string Reason { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}



