namespace ECommerce.Product.Domain.Enums;

public enum ProductStatus
{
    Draft = 1,
    Active = 2,
    Inactive = 3,
    OutOfStock = 4,
    Discontinued = 5
}

public enum ProductType
{
    Physical = 1,
    Digital = 2,
    Service = 3,
    Subscription = 4
}

public enum ReviewStatus
{
    Pending = 1,
    Approved = 2,
    Rejected = 3
}

public enum SortBy
{
    Name = 1,
    Price = 2,
    CreatedDate = 3,
    Rating = 4,
    Sales = 5,
    Views = 6
}

public enum SortDirection
{
    Ascending = 1,
    Descending = 2
}





