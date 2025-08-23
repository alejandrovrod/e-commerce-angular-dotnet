namespace ECommerce.User.Domain.Enums;

public enum UserRole
{
    Customer = 1,
    Admin = 2,
    SuperAdmin = 3
}

public enum UserStatus
{
    PendingVerification = 1,
    Active = 2,
    Inactive = 3,
    Suspended = 4
}

public enum Gender
{
    Male = 1,
    Female = 2,
    Other = 3,
    PreferNotToSay = 4
}

public enum AddressType
{
    Billing = 1,
    Shipping = 2,
    Both = 3
}


