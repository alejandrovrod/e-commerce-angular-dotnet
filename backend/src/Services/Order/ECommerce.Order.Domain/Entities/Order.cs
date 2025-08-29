using ECommerce.BuildingBlocks.Common.Models;
using ECommerce.Order.Domain.ValueObjects;
using ECommerce.Order.Domain.Enums;

namespace ECommerce.Order.Domain.Entities;

public class Order : BaseAuditableEntity
{
    public string OrderNumber { get; private set; } = default!;
    public Guid UserId { get; private set; }
    public OrderStatus Status { get; private set; } = OrderStatus.Pending;
    public List<OrderItem> Items { get; private set; } = new();
    public Address ShippingAddress { get; private set; } = default!;
    public Address? BillingAddress { get; private set; }
    public OrderPricing Pricing { get; private set; } = default!;
    public string? CouponCode { get; private set; }
    public Guid? ShippingMethodId { get; private set; }
    public PaymentInfo? PaymentInfo { get; private set; }
    public List<OrderStatusHistory> StatusHistory { get; private set; } = new();
    public string? Notes { get; private set; }
    public DateTime? ShippedAt { get; private set; }
    public DateTime? DeliveredAt { get; private set; }
    public string? TrackingNumber { get; private set; }

    private Order() { } // For EF Core

    public static Order Create(
        Guid userId,
        List<OrderItem> items,
        Address shippingAddress,
        Address? billingAddress = null)
    {
        var order = new Order
        {
            OrderNumber = GenerateOrderNumber(),
            UserId = userId,
            Items = items,
            ShippingAddress = shippingAddress,
            BillingAddress = billingAddress ?? shippingAddress,
            Status = OrderStatus.Pending
        };

        order.CalculatePricing();
        order.AddStatusHistory(OrderStatus.Pending, "Order created");

        return order;
    }

    public void AddItem(Guid productId, string productName, decimal price, int quantity, string? productSku = null)
    {
        var existingItem = Items.FirstOrDefault(i => i.ProductId == productId);
        if (existingItem != null)
        {
            existingItem.UpdateQuantity(existingItem.Quantity + quantity);
        }
        else
        {
            Items.Add(OrderItem.Create(productId, productName, price, quantity, productSku));
        }
        
        CalculatePricing();
        UpdatedAt = DateTime.UtcNow;
    }

    public void RemoveItem(Guid productId)
    {
        Items.RemoveAll(i => i.ProductId == productId);
        CalculatePricing();
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateItemQuantity(Guid productId, int quantity)
    {
        var item = Items.FirstOrDefault(i => i.ProductId == productId);
        if (item != null)
        {
            if (quantity <= 0)
            {
                RemoveItem(productId);
            }
            else
            {
                item.UpdateQuantity(quantity);
                CalculatePricing();
                UpdatedAt = DateTime.UtcNow;
            }
        }
    }

    public void ApplyCoupon(string couponCode, decimal discountAmount)
    {
        CouponCode = couponCode;
        Pricing = Pricing.ApplyDiscount(discountAmount);
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetShippingMethod(Guid shippingMethodId, decimal shippingCost)
    {
        ShippingMethodId = shippingMethodId;
        Pricing = Pricing.UpdateShipping(shippingCost);
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetPaymentInfo(string paymentMethod, string? paymentId = null, string? transactionId = null)
    {
        PaymentInfo = new PaymentInfo(paymentMethod, paymentId, transactionId);
        UpdatedAt = DateTime.UtcNow;
    }

    public void Confirm()
    {
        if (Status != OrderStatus.Pending)
            throw new InvalidOperationException("Only pending orders can be confirmed");

        UpdateStatus(OrderStatus.Confirmed, "Order confirmed");
    }

    public void Process()
    {
        if (Status != OrderStatus.Confirmed)
            throw new InvalidOperationException("Only confirmed orders can be processed");

        UpdateStatus(OrderStatus.Processing, "Order is being processed");
    }

    public void Ship(string? trackingNumber = null)
    {
        if (Status != OrderStatus.Processing)
            throw new InvalidOperationException("Only processing orders can be shipped");

        ShippedAt = DateTime.UtcNow;
        TrackingNumber = trackingNumber;
        UpdateStatus(OrderStatus.Shipped, $"Order shipped{(trackingNumber != null ? $" with tracking number: {trackingNumber}" : "")}");
    }

    public void Deliver()
    {
        if (Status != OrderStatus.Shipped)
            throw new InvalidOperationException("Only shipped orders can be delivered");

        DeliveredAt = DateTime.UtcNow;
        UpdateStatus(OrderStatus.Delivered, "Order delivered");
    }

    public void Cancel(string reason)
    {
        if (Status == OrderStatus.Delivered || Status == OrderStatus.Cancelled)
            throw new InvalidOperationException("Cannot cancel delivered or already cancelled orders");

        UpdateStatus(OrderStatus.Cancelled, $"Order cancelled: {reason}");
    }

    public void Refund(string reason)
    {
        if (Status != OrderStatus.Delivered)
            throw new InvalidOperationException("Only delivered orders can be refunded");

        UpdateStatus(OrderStatus.Refunded, $"Order refunded: {reason}");
    }

    public void AddNotes(string notes)
    {
        Notes = notes;
        UpdatedAt = DateTime.UtcNow;
    }

    private void UpdateStatus(OrderStatus newStatus, string reason)
    {
        var previousStatus = Status;
        Status = newStatus;
        AddStatusHistory(newStatus, reason);
        UpdatedAt = DateTime.UtcNow;
    }

    private void AddStatusHistory(OrderStatus status, string reason)
    {
        StatusHistory.Add(new OrderStatusHistory(status, reason, DateTime.UtcNow));
    }

    private void CalculatePricing()
    {
        var subtotal = Items.Sum(i => i.TotalPrice);
        var tax = CalculateTax(subtotal);
        var shipping = Pricing?.ShippingCost ?? 0;
        var discount = Pricing?.DiscountAmount ?? 0;
        var total = subtotal + tax + shipping - discount;

        Pricing = new OrderPricing(subtotal, tax, shipping, discount, total);
    }

    private decimal CalculateTax(decimal subtotal)
    {
        // Simple tax calculation - in real world, this would be more complex
        return subtotal * 0.08m; // 8% tax
    }

    private static string GenerateOrderNumber()
    {
        return $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Random.Shared.Next(1000, 9999)}";
    }

    public bool CanBeCancelled() => Status is OrderStatus.Pending or OrderStatus.Confirmed or OrderStatus.Processing;
    
    public bool CanBeShipped() => Status == OrderStatus.Processing;
    
    public bool CanBeDelivered() => Status == OrderStatus.Shipped;
    
    public bool IsCompleted() => Status is OrderStatus.Delivered or OrderStatus.Cancelled or OrderStatus.Refunded;
    
    public int GetTotalItems() => Items.Sum(i => i.Quantity);
    
    public decimal GetTotalWeight() => Items.Sum(i => i.Weight ?? 0);
}








