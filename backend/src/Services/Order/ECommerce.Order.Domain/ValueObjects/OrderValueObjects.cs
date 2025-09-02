using ECommerce.Order.Domain.Enums;

namespace ECommerce.Order.Domain.ValueObjects;

public record Address(
    string Street,
    string City,
    string State,
    string PostalCode,
    string Country,
    string? AddressLine2 = null)
{
    public string FullAddress => $"{Street}{(AddressLine2 != null ? $", {AddressLine2}" : "")}, {City}, {State} {PostalCode}, {Country}";
}

public record OrderPricing(
    decimal Subtotal,
    decimal Tax,
    decimal ShippingCost,
    decimal DiscountAmount,
    decimal Total)
{
    public OrderPricing ApplyDiscount(decimal discountAmount)
    {
        var newTotal = Math.Max(0, Total - discountAmount);
        return this with { DiscountAmount = discountAmount, Total = newTotal };
    }

    public OrderPricing UpdateShipping(decimal shippingCost)
    {
        var newTotal = Subtotal + Tax + shippingCost - DiscountAmount;
        return this with { ShippingCost = shippingCost, Total = newTotal };
    }
}

public record PaymentInfo(
    string PaymentMethod,
    string? PaymentId = null,
    string? TransactionId = null);

public record OrderStatusHistory(
    OrderStatus Status,
    string Reason,
    DateTime Timestamp);

public class OrderItem
{
    public Guid ProductId { get; private set; }
    public string ProductName { get; private set; } = default!;
    public string? ProductSku { get; private set; }
    public decimal UnitPrice { get; private set; }
    public int Quantity { get; private set; }
    public decimal TotalPrice => UnitPrice * Quantity;
    public decimal? Weight { get; private set; }

    private OrderItem() { } // For EF Core

    public static OrderItem Create(Guid productId, string productName, decimal unitPrice, int quantity, string? productSku = null, decimal? weight = null)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than 0");
        
        if (unitPrice < 0)
            throw new ArgumentException("Unit price cannot be negative");

        return new OrderItem
        {
            ProductId = productId,
            ProductName = productName,
            ProductSku = productSku,
            UnitPrice = unitPrice,
            Quantity = quantity,
            Weight = weight
        };
    }

    public void UpdateQuantity(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than 0");
        
        Quantity = quantity;
    }

    public void UpdatePrice(decimal unitPrice)
    {
        if (unitPrice < 0)
            throw new ArgumentException("Unit price cannot be negative");
        
        UnitPrice = unitPrice;
    }
}










