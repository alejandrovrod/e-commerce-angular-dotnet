namespace ECommerce.Payment.Domain.Enums;

public enum PaymentStatus
{
    Pending = 1,
    Processing = 2,
    Completed = 3,
    Failed = 4,
    Cancelled = 5,
    Expired = 6,
    Refunded = 7
}

public enum PaymentGateway
{
    Stripe = 1,
    PayPal = 2,
    Paystack = 3,
    Square = 4
}

public enum PaymentAttemptStatus
{
    Processing = 1,
    Succeeded = 2,
    Failed = 3,
    Cancelled = 4,
    Expired = 5
}

public enum RefundType
{
    Full = 1,
    Partial = 2
}

public enum RefundStatus
{
    Pending = 1,
    Processing = 2,
    Completed = 3,
    Failed = 4,
    Cancelled = 5
}








