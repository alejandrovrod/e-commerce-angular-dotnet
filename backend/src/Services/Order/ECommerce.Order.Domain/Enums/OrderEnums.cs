namespace ECommerce.Order.Domain.Enums;

public enum OrderStatus
{
    Pending = 1,
    Confirmed = 2,
    Processing = 3,
    Shipped = 4,
    Delivered = 5,
    Cancelled = 6,
    Refunded = 7
}

public enum PaymentStatus
{
    Pending = 1,
    Authorized = 2,
    Captured = 3,
    Failed = 4,
    Cancelled = 5,
    Refunded = 6
}

public enum ShippingMethod
{
    Standard = 1,
    Express = 2,
    Overnight = 3,
    Pickup = 4
}


